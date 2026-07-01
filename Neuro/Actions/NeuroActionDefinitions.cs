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
                    name = NeuroActionNames.RecallCompanion,
                    description = "Teleport Neuro's Terraria companion body back near the player. If the companion is not summoned, the action is skipped."
                },
                new
                {
                    name = NeuroActionNames.Follow,
                    description = "Deactivate autoattack mode and make Neuro's companion follow the player. If the companion is not summoned, the action is skipped."
                },
                new
                {
                    name = NeuroActionNames.AttackOnce,
                    description = "Make Neuro's companion fire one shot from her equipped magic weapon. If a valid enemy is nearby, the attack targets that enemy. If no valid enemy is nearby, the attack fires toward the player's cursor. If the companion is not summoned or Neuro has no equipped weapon, the action is skipped."
                },
                new
                {
                    name = NeuroActionNames.AttackPlayer,
                    description = "Make Evil Neuro attack the player. If Neuro has a valid magic weapon equipped, she fires that weapon at the player. If Neuro has no valid weapon equipped, she fires a fallback evil bolt. If the companion is not summoned, the action is skipped."
                },
                new
                {
                    name = NeuroActionNames.AutoAttack,
                    description = "Activate timed autoattack mode. During autoattack mode, Neuro's companion automatically fires her equipped magic weapon at nearby valid enemies. When the duration ends, the companion returns to follow mode. The requested duration is clamped by the player's configured maximum autoattack duration. If the companion is not summoned or Neuro has no equipped weapon, the action is skipped.",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            duration_seconds = new
                            {
                                type = "integer",
                                minimum = 1,
                                maximum = 600
                            }
                        }
                    }
                },
                new
                {
                    name = NeuroActionNames.WeaponStatus,
                    description = "Check what magic weapon Neuro currently has equipped."
                },
                new
                {
                    name = NeuroActionNames.EquipWeaponFromInventory,
                    description = "Let Neuro choose the strongest valid magic weapon from the player's inventory. The selected weapon is moved into Neuro's weapon slot. If Neuro already has a weapon, the old weapon is automatically swapped back into the inventory slot the new weapon came from. The player's selected hotbar item is ignored."
                },
                new
                {
                    name = NeuroActionNames.ReturnWeaponToPlayer,
                    description = "Return Neuro's equipped magic weapon to the player's inventory."
                },
                new
                {
                    name = NeuroActionNames.BuffPlayer,
                    description = "Apply positive buffs to the player. If no buff is specified, Neuro applies up to 3 Red Potion-style positive buffs, prioritizing buffs the player does not already have. If buff is specified, it must be one of the allowed positive buff names or IDs from the current Terraria context.",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            buff = new
                            {
                                type = "string"
                            }
                        }
                    }
                },
                new
                {
                    name = NeuroActionNames.DebuffPlayer,
                    description = "Apply debuffs to the player. If no debuff is specified, Neuro applies Red Potion-style debuffs. If debuff is specified, it must be one of the allowed debuff names or IDs from the current Terraria context.",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            debuff = new
                            {
                                type = "string"
                            }
                        }
                    }
                },
                new
                {
                    name = NeuroActionNames.DebuffEnemy,
                    description = "Apply debuffs to the nearest valid enemy. If no debuff is specified, Neuro applies Red Potion-style debuffs. If debuff is specified, it must be one of the allowed debuff names or IDs from the current Terraria context.",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            debuff = new
                            {
                                type = "string"
                            }
                        }
                    }
                }
            };
        }
    }
}