using System.Collections.Generic;

using NeuroCompanion.Neuro.Weapons.Firing;
using NeuroCompanion.Projectiles.Helpers;

namespace NeuroCompanion.Projectiles.Companion
{
    public partial class NeuroCompanionProjectile
    {
        private readonly List<PendingNeuroWeaponShot> PendingWeaponShots =
            new List<PendingNeuroWeaponShot>();

        private void QueuePendingWeaponShot(
            PendingNeuroWeaponShot pendingShot
        )
        {
            PendingWeaponShots.Add(pendingShot);
        }

        private void ProcessPendingWeaponShots()
        {
            if (PendingWeaponShots.Count <= 0)
            {
                return;
            }

            for (int i = PendingWeaponShots.Count - 1; i >= 0; i--)
            {
                PendingNeuroWeaponShot pendingShot = PendingWeaponShots[i];

                if (pendingShot.RemainingTicks > 0)
                {
                    PendingWeaponShots[i] =
                        pendingShot.WithRemainingTicks(
                            pendingShot.RemainingTicks - 1
                        );

                    continue;
                }

                NeuroProjectileSpawner.SpawnPendingWeaponShot(pendingShot);
                PendingWeaponShots.RemoveAt(i);
            }
        }
    }
}