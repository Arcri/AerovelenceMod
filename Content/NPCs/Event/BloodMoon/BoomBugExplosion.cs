using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Event.BloodMoon
{
    public class BoomBugExplosion : ModProjectile
    {
        // Set this to your mod's empty texture.
        // Since I dont have any in my test mod, I'm just using a vanilla one and setting the proj alpha to 255.
        public override string Texture => "Terraria/Item_" + ItemID.HermesBoots;

        public override void SetDefaults()
        {
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.width = projectile.height = 80;

            projectile.alpha = 255;
            projectile.timeLeft = 5;

            projectile.penetrate = -1;
        }
    }
}
