using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class SkySpawnStaffProfile
    {
        private const float TileSize = 16f;

        private const int LunarFlareProjectileCount = 3;
        private const int BlizzardProjectileCount = 4;
        private const int FastBlizzardProjectileCount = 6;
        private const int MeteorProjectileCount = 1;

        private const float LunarSpawnHeight = 720f;
        private const float BlizzardSpawnHeight = 720f;

        // Meteor Staff is special: it spawns 37.5 tiles above the player.
        // For Neuro, use Neuro's position instead of the player's position.
        private const float MeteorSpawnHeight = 37.5f * TileSize;

        private const float LunarHalfWidth = 4f * TileSize;
        private const float BlizzardHalfWidth = 8f * TileSize;

        private const float LunarTargetRandomness = 8f;
        private const float BlizzardTargetRandomness = 120f;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   (
                       weapon.type == ItemID.LunarFlareBook ||
                       weapon.type == ItemID.BlizzardStaff ||
                       weapon.type == ItemID.MeteorStaff
                   );
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            switch (weapon.type)
            {
                case ItemID.LunarFlareBook:
                    return CreateLunarFlareShots(
                        weapon,
                        basePosition,
                        targetPosition
                    );

                case ItemID.BlizzardStaff:
                    return CreateBlizzardStaffShots(
                        weapon,
                        basePosition,
                        targetPosition
                    );

                case ItemID.MeteorStaff:
                    return CreateMeteorStaffShots(
                        weapon,
                        basePosition,
                        baseVelocity,
                        targetPosition
                    );

                default:
                    return null;
            }
        }

        private static NeuroWeaponShot[] CreateLunarFlareShots(
            Item weapon,
            Vector2 neuroPosition,
            Vector2 targetPosition
        )
        {
            NeuroWeaponShot[] shots =
                new NeuroWeaponShot[LunarFlareProjectileCount];

            for (int i = 0; i < shots.Length; i++)
            {
                Vector2 spawnPosition = new Vector2(
                    targetPosition.X + Main.rand.NextFloat(
                        -LunarHalfWidth,
                        LunarHalfWidth
                    ),
                    neuroPosition.Y - LunarSpawnHeight
                );

                Vector2 aimedTarget = targetPosition + Main.rand.NextVector2Circular(
                    LunarTargetRandomness,
                    LunarTargetRandomness
                );

                Vector2 velocity = CreateVelocityToward(
                    spawnPosition,
                    aimedTarget,
                    weapon.shootSpeed
                );

                shots[i] = new NeuroWeaponShot(
                    weapon.shoot,
                    spawnPosition,
                    velocity
                );
            }

            return shots;
        }

        private static NeuroWeaponShot[] CreateBlizzardStaffShots(
            Item weapon,
            Vector2 neuroPosition,
            Vector2 targetPosition
        )
        {
            int projectileCount = GetBlizzardProjectileCount(weapon);

            NeuroWeaponShot[] shots =
                new NeuroWeaponShot[projectileCount];

            for (int i = 0; i < shots.Length; i++)
            {
                Vector2 spawnPosition = new Vector2(
                    targetPosition.X + Main.rand.NextFloat(
                        -BlizzardHalfWidth,
                        BlizzardHalfWidth
                    ),
                    neuroPosition.Y - BlizzardSpawnHeight
                );

                Vector2 aimedTarget = targetPosition + Main.rand.NextVector2Circular(
                    BlizzardTargetRandomness,
                    BlizzardTargetRandomness
                );

                Vector2 velocity = CreateVelocityToward(
                    spawnPosition,
                    aimedTarget,
                    weapon.shootSpeed
                );

                shots[i] = new NeuroWeaponShot(
                    weapon.shoot,
                    spawnPosition,
                    velocity
                );
            }

            return shots;
        }

        private static NeuroWeaponShot[] CreateMeteorStaffShots(
            Item weapon,
            Vector2 neuroPosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            NeuroWeaponShot[] shots =
                new NeuroWeaponShot[MeteorProjectileCount];

            Vector2 spawnPosition = new Vector2(
                targetPosition.X,
                neuroPosition.Y - MeteorSpawnHeight
            );

            float speed = weapon.shootSpeed;

            if (speed <= 0f)
            {
                speed = baseVelocity.Length();

                if (speed <= 0f)
                {
                    speed = 16f;
                }
            }

            Vector2 velocity = CreateVelocityToward(
                spawnPosition,
                targetPosition,
                speed
            );

            shots[0] = new NeuroWeaponShot(
                weapon.shoot,
                spawnPosition,
                velocity
            );

            return shots;
        }

        private static int GetBlizzardProjectileCount(Item weapon)
        {
            // Mythical and other speed-boosting reforges can lower use time /
            // use animation enough that vanilla effectively produces more
            // icicles per use. This catches that case without hardcoding a
            // prefix name.
            if (weapon.useTime <= 8 || weapon.useAnimation <= 8)
            {
                return FastBlizzardProjectileCount;
            }

            return BlizzardProjectileCount;
        }

        private static Vector2 CreateVelocityToward(
            Vector2 start,
            Vector2 target,
            float speed
        )
        {
            Vector2 direction = target - start;

            if (direction.LengthSquared() <= 0.01f)
            {
                return Vector2.UnitY * speed;
            }

            direction.Normalize();
            return direction * speed;
        }
    }
}