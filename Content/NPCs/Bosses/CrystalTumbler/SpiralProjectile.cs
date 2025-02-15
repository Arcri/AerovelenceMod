using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class SpiralProjectile : ModProjectile
    {
        private Vector2 targetPosition;
        private bool isReturning = false;
        private float returnSpeed = 10f;
        private float orbitRadius = 200f;
        private float orbitSpeed = 0.1f;
        private float angle;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (!isReturning)
            {
                angle += orbitSpeed;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * orbitRadius;
                Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center + offset;
                if (Projectile.Distance(Main.npc[(int)Projectile.ai[0]].Center) > orbitRadius * 1.5f)
                    isReturning = true;
            }
            else
            {
                Vector2 direction = targetPosition - Projectile.Center;
                direction.Normalize();
                Projectile.velocity = direction * returnSpeed;
                Projectile.velocity *= 1.05f;
            }
            Projectile.rotation += 0.2f * Projectile.direction;
            targetPosition = Main.npc[(int)Projectile.ai[0]].Center;
        }
    }
}