using NeuroCompanion.Neuro;
using Terraria;
using Terraria.ModLoader;

using NeuroCompanion.Configs;

namespace NeuroCompanion.Systems
{
    public class NeuroCompanionSystem : ModSystem
    {
        private const int TicksPerSecond = 60;

        private readonly NeuroContextEvents contextEvents = new();

        private int contextTimer;

        public override void PostUpdateEverything()
        {
            if (Main.dedServ || Main.gameMenu)
            {
                return;
            }

            Player player = Main.LocalPlayer;

            ExecuteQueuedCommands(player);
            contextEvents.CheckAndSendEvents(player);
            SendPeriodicContext(player);
        }

        public override void OnWorldUnload()
        {
            contextTimer = 0;
            contextEvents.Reset();
        }

        public override void Unload()
        {
            NeuroClient.Instance.Stop();
        }

        public override void OnWorldLoad()
        {
            contextTimer = 0;
            contextEvents.Reset();

            if (ModContent.GetInstance<NeuroCompanionConfig>().AutoConnectOnWorldLoad)
            {
                NeuroClient.Instance.Start();
            }
        }

        private static void ExecuteQueuedCommands(Player player)
        {
            while (NeuroCommandQueue.TryDequeue(out QueuedNeuroCommand queuedCommand))
            {
                NeuroActionResult result =
                    NeuroActionExecutor.Execute(player, queuedCommand.Command);

                NeuroRuntimeStatus.RecordExecutedCommand(
                    queuedCommand.ActionId,
                    queuedCommand.Command,
                    result
                );

                NeuroClient.Instance.SendActionResult(
                    queuedCommand.ActionId,
                    result
                );
            }
        }

        private void SendPeriodicContext(Player player)
        {
            if (!NeuroClient.Instance.IsConnected)
            {
                contextTimer = 0;
                return;
            }

            contextTimer++;

            int intervalSeconds =
                ModContent.GetInstance<NeuroCompanionConfig>().PeriodicContextIntervalSeconds;

            if (intervalSeconds < 1)
            {
                intervalSeconds = 1;
            }

            int intervalTicks = intervalSeconds * TicksPerSecond;

            if (contextTimer < intervalTicks)
            {
                return;
            }

            contextTimer = 0;

            string context = NeuroContextBuilder.Build(player);

            NeuroClient.Instance.SendContext(
                context,
                silent: true
            );
        }
    }
}