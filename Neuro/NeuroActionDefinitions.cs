namespace NeuroCompanion.Neuro
{
    public static class NeuroActionDefinitions
    {
        public static object[] CreateActions()
        {
            return new object[]
            {
                new
                {
                    name = "recall_companion",
                    description = "Teleport Neuro's Terraria companion body back near the player. If the companion is not summoned, the action is skipped."
                },
                new
                {
                    name = "follow",
                    description = "Deactivate autoattack mode and make Neuro's companion follow the player. If the companion is not summoned, the action is skipped."
                },
                new
                {
                    name = "attack_once",
                    description = "Make Neuro's companion fire one Razorblade Typhoon attack. If a valid enemy is nearby, the attack targets that enemy. If no valid enemy is nearby, the attack fires toward the player's cursor. If the companion is not summoned, the action is skipped."
                },
                new
                {
                    name = "autoattack",
                    description = "Activate timed autoattack mode. During autoattack mode, Neuro's companion automatically fires Razorblade Typhoon attacks at nearby valid enemies. When the duration ends, the companion returns to follow mode. If the companion is not summoned, the action is skipped.",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            duration_seconds = new
                            {
                                type = "integer",
                                description = "How long autoattack mode should last, in seconds. Minimum 1 second. Maximum 180 seconds. If omitted, the default duration is 10 seconds.",
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
            };
        }
    }
}