
using Terraria;
using Terraria.Audio;

namespace AerovelenceMod.Common.Utilities
{
    public static class SoundUtils
    {
        //Safely handles updating a sound's position | (from example mod)
        public static bool BasicSoundUpdateCallback(Entity entity, ProjectileAudioTracker tracker, ActiveSound soundInstance)
        {
            // Update sound location according to projectile position
            soundInstance.Position = entity.position;
            // ProjectileAudioTracker is necessary to avoid rare situations where sounds can loop indefinitely. IsActiveAndInGame returns a value indicating if the sound should still be active.
            return tracker.IsActiveAndInGame();
        }
    }
}
