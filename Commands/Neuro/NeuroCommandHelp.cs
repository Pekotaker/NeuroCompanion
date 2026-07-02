using Terraria.ModLoader;

namespace NeuroCompanion.Commands.Neuro
{
    public static class NeuroCommandHelp
    {
        public static void ReplyWithHelp(CommandCaller caller)
        {
            caller.Reply("Neuro Companion commands:");
            caller.Reply("/neuro help");
            caller.Reply("/neuro status");
            caller.Reply("/neuro follow");
            caller.Reply("/neuro attack");
            caller.Reply("/neuro attack player");
            caller.Reply("/neuro autoattack [seconds]");
            caller.Reply("/neuro buff [buff name/id]");
            caller.Reply("/neuro debuff player [debuff name/id]");
            caller.Reply("/neuro debuff enemy [debuff name/id]");
            caller.Reply("/neuro weapon status");
            caller.Reply("/neuro weapon set");
            caller.Reply("/neuro weapon take");
            caller.Reply("/neuro weapon return");
        }
    }
}