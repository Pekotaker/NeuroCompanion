using NeuroCompanion.Neuro;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Systems
{
    public class NeuroCompanionSystem : ModSystem
    {
        public override void PostUpdateEverything()
        {
            if (Main.dedServ)
            {
                return;
            }

            Player player = Main.LocalPlayer;

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

        public override void Unload()
        {
            NeuroClient.Instance.Stop();
        }
    }
}