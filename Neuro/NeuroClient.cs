using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroCompanion.Neuro
{
    public sealed class NeuroClient
    {
        private const string GameName = "Terraria Neuro Companion";
        private const string RandyWebsocketUrl = "ws://localhost:8000";

        private readonly SemaphoreSlim sendLock = new(1, 1);

        private ClientWebSocket websocket;
        private CancellationTokenSource cancellationTokenSource;
        private Task receiveTask;

        public static NeuroClient Instance { get; } = new();

        public string LastStatus { get; private set; } = "Not connected.";

        public bool IsConnected =>
            websocket != null &&
            websocket.State == WebSocketState.Open;

        private NeuroClient()
        {
        }

        public void Start()
        {
            if (receiveTask != null && !receiveTask.IsCompleted)
            {
                LastStatus = "Neuro websocket client is already running.";
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            receiveTask = Task.Run(
                () => RunAsync(cancellationTokenSource.Token)
            );
        }

        public void Stop()
        {
            try
            {
                cancellationTokenSource?.Cancel();
                websocket?.Abort();
                websocket?.Dispose();
            }
            catch
            {
                // Ignore shutdown errors during mod unload.
            }

            LastStatus = "Disconnected.";
        }

        public void SendActionResult(
            string actionId,
            NeuroActionResult result
        )
        {
            if (string.IsNullOrWhiteSpace(actionId))
            {
                return;
            }

            _ = SendActionResultAsync(actionId, result);
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                websocket = new ClientWebSocket();

                LastStatus = $"Connecting to {RandyWebsocketUrl}...";

                await websocket.ConnectAsync(
                    new Uri(RandyWebsocketUrl),
                    cancellationToken
                );

                LastStatus = "Connected to Randy.";

                await SendStartupAsync(cancellationToken);
                await RegisterActionsAsync(cancellationToken);
                await SendContextAsync(
                    "Terraria Neuro Companion mod connected.",
                    silent: false,
                    cancellationToken
                );

                while (
                    websocket.State == WebSocketState.Open &&
                    !cancellationToken.IsCancellationRequested
                )
                {
                    string message = await ReceiveTextMessageAsync(cancellationToken);

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        continue;
                    }

                    await HandleIncomingMessageAsync(message, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                LastStatus = "Disconnected.";
            }
            catch (Exception exception)
            {
                LastStatus = $"Neuro websocket error: {exception.Message}";
            }
        }

        private async Task SendStartupAsync(CancellationToken cancellationToken)
        {
            await SendJsonAsync(
                new
                {
                    command = "startup",
                    game = GameName
                },
                cancellationToken
            );
        }

        private async Task RegisterActionsAsync(CancellationToken cancellationToken)
        {
            await SendJsonAsync(
                new
                {
                    command = "actions/register",
                    game = GameName,
                    data = new
                    {
                        actions = new object[]
                        {
                            new
                            {
                                name = "recall_companion",
                                description = "Recall Neuro's Terraria companion body back to the player."
                            },
                            new
                            {
                                name = "follow",
                                description = "Make Neuro stop attacking and follow the player."
                            },
                            new
                            {
                                name = "attack_once",
                                description = "Make Neuro fire one Razorblade Typhoon attack. If no enemy is nearby, she fires toward the cursor."
                            },
                            new
                            {
                                name = "autoattack",
                                description = "Make Neuro automatically attack nearby enemies for a limited duration.",
                                schema = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        duration_seconds = new
                                        {
                                            type = "integer",
                                            minimum = 1,
                                            maximum = 180
                                        }
                                    }
                                }
                            },
                            new
                            {
                                name = "buff_player",
                                description = "Apply 3 random Red Potion-style positive buffs to the player."
                            },
                            new
                            {
                                name = "debuff_player",
                                description = "Apply Red Potion-style debuffs to the player."
                            },
                            new
                            {
                                name = "debuff_enemy",
                                description = "Apply Red Potion-style debuffs to the nearest valid enemy."
                            }
                        }
                    }
                },
                cancellationToken
            );
        }

        public void SendContext(string message, bool silent = true)
        {
            if (!IsConnected)
            {
                return;
            }

            _ = SendContextAsync(
                message,
                silent,
                CancellationToken.None
            );
        }

        private async Task SendContextAsync(
            string message,
            bool silent,
            CancellationToken cancellationToken
        )
        {
            await SendJsonAsync(
                new
                {
                    command = "context",
                    game = GameName,
                    data = new
                    {
                        message,
                        silent
                    }
                },
                cancellationToken
            );
        }

        private async Task HandleIncomingMessageAsync(
            string message,
            CancellationToken cancellationToken
        )
        {
            JsonNode root;

            try
            {
                root = JsonNode.Parse(message);
            }
            catch
            {
                LastStatus = "Received invalid JSON from Randy.";
                return;
            }

            string command = root?["command"]?.GetValue<string>();

            if (command == "actions/reregister_all")
            {
                await RegisterActionsAsync(cancellationToken);
                LastStatus = "Randy requested action re-registration.";
                return;
            }

            if (command != "action")
            {
                LastStatus = $"Ignored Randy command: {command}";
                return;
            }

            HandleAction(root?["data"]);
        }

        private void HandleAction(JsonNode data)
        {
            string actionId = data?["id"]?.GetValue<string>();
            string actionName = data?["name"]?.GetValue<string>();

            if (string.IsNullOrWhiteSpace(actionId))
            {
                LastStatus = "Received action without id.";
                return;
            }

            if (!TryCreateCommandFromAction(data, out NeuroCommand command, out string error))
            {
                SendActionResult(
                    actionId,
                    NeuroActionResult.Fail(error)
                );

                LastStatus = error;
                return;
            }

            NeuroCommandQueue.Enqueue(
                new QueuedNeuroCommand(actionId, command)
            );

            LastStatus = $"Queued Neuro action: {actionName}";
        }

        private static bool TryCreateCommandFromAction(
            JsonNode data,
            out NeuroCommand command,
            out string error
        )
        {
            command = null;
            error = null;

            string actionName = data?["name"]?.GetValue<string>();

            switch (actionName)
            {
                case "recall_companion":
                    command = new NeuroCommand(NeuroCommandType.Recall);
                    return true;

                case "follow":
                    command = new NeuroCommand(NeuroCommandType.Follow);
                    return true;

                case "attack_once":
                    command = new NeuroCommand(NeuroCommandType.AttackOnce);
                    return true;

                case "autoattack":
                    command = new NeuroCommand(
                        NeuroCommandType.StartTimedAttack,
                        GetDurationSecondsFromActionData(data)
                    );
                    return true;

                case "buff_player":
                    command = new NeuroCommand(NeuroCommandType.BuffPlayer);
                    return true;

                case "debuff_player":
                    command = new NeuroCommand(NeuroCommandType.DebuffPlayer);
                    return true;

                case "debuff_enemy":
                    command = new NeuroCommand(NeuroCommandType.DebuffNearestEnemy);
                    return true;

                default:
                    error = $"Unknown Neuro action: {actionName}";
                    return false;
            }
        }

        private static int GetDurationSecondsFromActionData(JsonNode data)
        {
            const int defaultDurationSeconds = 10;

            string actionDataJson = data?["data"]?.GetValue<string>();

            if (string.IsNullOrWhiteSpace(actionDataJson))
            {
                return defaultDurationSeconds;
            }

            try
            {
                JsonNode actionData = JsonNode.Parse(actionDataJson);

                int? durationSeconds =
                    actionData?["duration_seconds"]?.GetValue<int>();

                return durationSeconds ?? defaultDurationSeconds;
            }
            catch
            {
                return defaultDurationSeconds;
            }
        }

        private async Task SendActionResultAsync(
            string actionId,
            NeuroActionResult result
        )
        {
            if (!IsConnected)
            {
                return;
            }

            await SendJsonAsync(
                new
                {
                    command = "action/result",
                    game = GameName,
                    data = new
                    {
                        id = actionId,
                        success = result.Success,
                        message = result.Message
                    }
                },
                CancellationToken.None
            );
        }

        private async Task SendJsonAsync(
            object payload,
            CancellationToken cancellationToken
        )
        {
            if (!IsConnected)
            {
                return;
            }

            string json = JsonSerializer.Serialize(payload);

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            await sendLock.WaitAsync(cancellationToken);

            try
            {
                await websocket.SendAsync(
                    bytes,
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken
                );
            }
            finally
            {
                sendLock.Release();
            }
        }

        private async Task<string> ReceiveTextMessageAsync(
            CancellationToken cancellationToken
        )
        {
            byte[] buffer = new byte[4096];

            using MemoryStream messageStream = new();

            while (true)
            {
                WebSocketReceiveResult result =
                    await websocket.ReceiveAsync(buffer, cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    return string.Empty;
                }

                messageStream.Write(buffer, 0, result.Count);

                if (result.EndOfMessage)
                {
                    break;
                }
            }

            return Encoding.UTF8.GetString(messageStream.ToArray());
        }
    }
}