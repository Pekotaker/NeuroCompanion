using NeuroCompanion.Neuro;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Systems
{
    public class NeuroCompanionSystem : ModSystem
    {
        private const int ContextIntervalTicks = 5 * 60;

        private int contextTimer;

        public override void PostUpdateEverything()
        {
            if (Main.dedServ || Main.gameMenu)
            {
                return;
            }

            Player player = Main.LocalPlayer;

            ExecuteQueuedCommands(player);
            SendPeriodicContext(player);
        }

        public override void Unload()
        {
            NeuroClient.Instance.Stop();
        }

        private static void ExecuteQueuedCommands(Player player)
        {
            while (NeuroCommandQueue.TryDequeue(out QueuedNeuroCommand queuedCommand))
            {
                NeuroActionResult result =
                    NeuroActionExecutor.Execute(player, queuedCommand.Command);

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

            if (contextTimer < ContextIntervalTicks)
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