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
                    name = NeuroActionNames.AttackOnce,
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
                    name = NeuroActionNames.AutoAttack,
                    description = "Activate timed autoattack mode. During autoattack mode, Neuro's companion automatically fires her equipped magic weapon at nearby valid enemies. When the duration ends, the companion returns to follow mode. If the companion is not summoned or Neuro has no equipped weapon, the action is skipped.",
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
                    name = NeuroActionNames.WeaponStatus,
                    description = "Check what magic weapon Neuro currently has equipped."
                },
                new
                {
                    name = NeuroActionNames.EquipWeaponFromInventory,
                    description = "Let Neuro choose the strongest valid direct-fire magic weapon from the player's inventory. The selected weapon is moved into Neuro's weapon slot. If Neuro already has a weapon, the old weapon is automatically swapped back into the inventory slot the new weapon came from."
                },
                new
                {
                    name = NeuroActionNames.ReturnWeaponToPlayer,
                    description = "Return Neuro's equipped magic weapon to the player's inventory."
                },
                new
                {
                    name = NeuroActionNames.BuffPlayer,
                    description = "Apply 3 random Red Potion-style positive buffs to the player."
                },
                new
                {
                    name = NeuroActionNames.DebuffPlayer,
                    description = "Apply Red Potion-style debuffs to the player."
                },
                new
                {
                    name = NeuroActionNames.DebuffEnemy,
                    description = "Apply Red Potion-style debuffs to the nearest valid enemy."
                }
            };
        }
    }
}