using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Event.BloodMoon
{
    public class BoomBugExplosion : ModProjectile
    {
        // Set this to your mod's empty texture.
        // Since I dont have any in my test mod, I'm just using a vanilla one and setting the proj alpha to 255.
        public override string Texture => "Terraria/Images/Item_" + ItemID.HermesBoots;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.width = Projectile.height = 80;

            Projectile.alpha = 255;
            Projectile.timeLeft = 5;

            Projectile.penetrate = -1;
        }
    }
}
