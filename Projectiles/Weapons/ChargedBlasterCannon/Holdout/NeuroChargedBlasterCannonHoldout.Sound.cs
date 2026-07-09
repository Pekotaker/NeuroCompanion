using Terraria;
using Terraria.Audio;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout
{
    public partial class NeuroChargedBlasterCannonHoldout
    {
        private void PlayInitialPewSound()
        {
            if (initialPewSoundPlayed)
            {
                return;
            }

            initialPewSoundPlayed = true;

            SoundEngine.PlaySound(
                InitialPewSound,
                Projectile.Center
            );
        }

        private void PlaySmallOrbHumSound()
        {
            SoundEngine.PlaySound(
                SmallOrbHumSound,
                Projectile.Center
            );
        }

        private void PlayHeavyOrbHumSound()
        {
            SoundEngine.PlaySound(
                HeavyOrbHumSound,
                Projectile.Center
            );
        }

        private void PlayBeamStartSound()
        {
            SoundEngine.PlaySound(
                BeamStartHumSound,
                Projectile.Center
            );
        }
    }
}