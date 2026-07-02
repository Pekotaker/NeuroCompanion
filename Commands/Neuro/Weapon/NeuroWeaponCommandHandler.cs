using NeuroCompanion.Neuro;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Commands.Neuro.Weapon
{
    public static class NeuroWeaponCommandHandler
    {
        public static void Handle(
            CommandCaller caller,
            Player player,
            string[] args
        )
        {
            if (args.Length < 2)
            {
                ReplyWithHelp(caller);
                return;
            }

            string weaponCommand = args[1].ToLowerInvariant();

            NeuroActionResult result;

            switch (weaponCommand)
            {
                case "status":
                    caller.Reply(NeuroWeaponService.GetStatusText(player));
                    return;

                case "set":
                    result = NeuroWeaponService.SetFromSelectedItem(player);
                    caller.Reply(result.Message);
                    return;

                case "take":
                    result = NeuroWeaponService.TakeStrongestFromInventory(player);
                    caller.Reply(result.Message);
                    return;

                case "return":
                    result = NeuroWeaponService.ReturnWeaponToInventory(player);
                    caller.Reply(result.Message);
                    return;

                case "inspect":
                    caller.Reply(NeuroWeaponService.InspectSelectedItem(player));
                    return;

                default:
                    caller.Reply($"Unknown weapon command: {weaponCommand}");
                    ReplyWithHelp(caller);
                    return;
            }
        }

        public static void ReplyWithHelp(CommandCaller caller)
        {
            caller.Reply("Neuro weapon commands:");
            caller.Reply("/neuro weapon status");
            caller.Reply("/neuro weapon set");
            caller.Reply("/neuro weapon take");
            caller.Reply("/neuro weapon return");
            caller.Reply("/neuro weapon inspect");
        }
    }
}