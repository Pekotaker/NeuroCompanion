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
            };
        }
    }
}