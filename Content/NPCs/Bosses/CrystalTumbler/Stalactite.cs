using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class Stalactite : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 48;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 1;
            Projectile.velocity = Vector2.UnitY * 10f;
        }
    }
}