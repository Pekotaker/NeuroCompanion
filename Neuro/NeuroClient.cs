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
                        actions = NeuroActionDefinitions.CreateActions()
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

            NeuroRuntimeStatus.RecordContextSent();
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
            NeuroRuntimeStatus.RecordReceivedAction(actionId, actionName);

            if (string.IsNullOrWhiteSpace(actionId))
            {
                LastStatus = "Received action without id.";
                return;
            }

            if (!NeuroActionParser.TryCreateCommandFromAction(data, out NeuroCommand command, out string error))
            {
                SendActionResult(
                    actionId,
                    NeuroActionResult.Fail(error)
                );

                LastStatus = error;
                return;
            }

            QueuedNeuroCommand queuedCommand =
                new QueuedNeuroCommand(actionId, command);

            NeuroCommandQueue.Enqueue(queuedCommand);

            NeuroRuntimeStatus.RecordQueuedCommand(actionId, command);

            LastStatus = $"Queued Neuro action: {actionName}";
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