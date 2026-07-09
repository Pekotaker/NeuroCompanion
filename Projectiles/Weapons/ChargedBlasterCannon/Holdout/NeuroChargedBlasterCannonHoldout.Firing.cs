using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Beam;
using NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Orb;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout
{
    public partial class NeuroChargedBlasterCannonHoldout
    {
        private void FireCurrentChargePhase()
        {
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }

            if (SingleShotOnly)
            {
                FireSingleOrbAndFinish();
                return;
            }

            if (ChargeTicks >= CycleResetTicks)
            {
                Projectile.Kill();
                return;
            }

            if (ChargeTicks < PhaseTwoStartTicks)
            {
                if (smallOrbsFired < SmallOrbCountBeforeHeavyPhase)
                {
                    rapidOrbTimer++;

                    if (rapidOrbTimer >= RapidOrbCooldownTicks)
                    {
                        rapidOrbTimer = 0;
                        smallOrbsFired++;

                        SpawnOrb(isHeavy: false);
                    }
                }

                return;
            }

            if (ChargeTicks < BeamStartTicks)
            {
                heavyHumTimer++;

                if (heavyHumTimer >= HeavyOrbHumIntervalTicks)
                {
                    heavyHumTimer = 0;
                    PlayHeavyOrbHumSound();
                }

                if (heavyOrbsFired < HeavyOrbCountBeforeBeamPhase)
                {
                    heavyOrbTimer++;

                    if (heavyOrbTimer >= HeavyOrbCooldownTicks)
                    {
                        heavyOrbTimer = 0;
                        heavyOrbsFired++;

                        SpawnOrb(isHeavy: true);
                    }
                }

                return;
            }

            if (!beamSpawned)
            {
                beamSpawned = true;
                PlayBeamStartSound();
                SpawnBeam();
            }
        }

        private void FireSingleOrbAndFinish()
        {
            if (ChargeTicks == 1f)
            {
                SpawnOrb(isHeavy: false);
            }

            if (ChargeTicks >= 2f)
            {
                Projectile.Kill();
            }
        }

        private void SpawnOrb(bool isHeavy)
        {
            Vector2 direction =
                SafeNormalize(Projectile.velocity, Vector2.UnitX);

            Vector2 spawnPosition =
                Projectile.Center + direction * MuzzleDistance;

            float speed =
                isHeavy
                    ? HeavyOrbSpeed
                    : SmallOrbSpeed;

            int damage =
                (int)(
                    Projectile.damage *
                    (
                        isHeavy
                            ? HeavyOrbDamageMultiplier
                            : SmallOrbDamageMultiplier
                    )
                );

            if (damage < 1)
            {
                damage = 1;
            }

            int projectileIndex =
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPosition,
                    direction * speed,
                    ModContent.ProjectileType<NeuroChargedBlasterOrb>(),
                    damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    isHeavy ? 1f : 0f
                );

            ApplyChildProjectileState(
                projectileIndex,
                useGenericOwnerDamage: true
            );

            if (isHeavy)
            {
                return;
            }

            if (!initialPewSoundPlayed)
            {
                PlayInitialPewSound();
            }
            else
            {
                PlaySmallOrbHumSound();
            }
        }

        private void SpawnBeam()
        {
            Vector2 direction =
                SafeNormalize(Projectile.velocity, Vector2.UnitX);

            int projectileIndex =
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center + direction * MuzzleDistance,
                    direction,
                    ModContent.ProjectileType<NeuroChargedBlasterBeam>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    Projectile.whoAmI
                );

            ApplyChildProjectileState(
                projectileIndex,
                useGenericOwnerDamage: false
            );
        }
    }
}