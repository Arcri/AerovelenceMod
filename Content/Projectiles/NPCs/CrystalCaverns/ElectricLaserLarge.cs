using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.NPCs.CrystalCaverns
{
    public class ElectricLaserLarge : ModProjectile
    {
        public int i;
        public int counter = 0;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
        private readonly int oneHelixRevolutionInUpdateTicks = 30;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Laser");
        }
        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = false;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.extraUpdates = 2;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override bool PreAI()
        {
            i++;
            ++Projectile.localAI[0];
            float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
            float piFractionVelocity = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
            float ReversepiFraction = MathHelper.Pi + oneHelixRevolutionInUpdateTicks;
            Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * Projectile.height;
            Dust newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDustPosition.Y *= -1;
            newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDust.velocity *= 0f;
            Vector2 newDustPosition2 = new Vector2(0, (float)Math.Sin((Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * ReversepiFraction)) * Projectile.height;
            Dust newDust2 = Dust.NewDustPerfect(Projectile.Center + newDustPosition2.RotatedBy(Projectile.velocity.ToRotation()), 68);
            newDust2.noGravity = true;
            newDustPosition2.Y *= -1;
            newDust2 = Dust.NewDustPerfect(Projectile.Center + newDustPosition2.RotatedBy(Projectile.velocity.ToRotation()), 160);
            newDust2.noGravity = true;
            newDust2.velocity *= 0f;
            Projectile.rotation += Projectile.velocity.Length() * 0.1f * Projectile.direction;
            Vector2 Velocity2 = new Vector2(0, (float)Math.Sin(Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks * piFraction)) * Projectile.height;
            return (false);
        }
        public override void AI()
        {

            int num294 = Main.rand.Next(3, 7);
            for (int num295 = 0; num295 < num294; num295++)
            {
                counter++;
                if (counter >= 17)
                {
                    int num296 = Dust.NewDust(Projectile.Center - Projectile.velocity / 2f, 0, 0, DustID.Electric, 0f, 0f, 100, default, 2.1f);
                    Dust dust105 = Main.dust[num296];
                    Dust dust2 = dust105;
                    dust2.velocity *= 2f;
                    Main.dust[num296].noGravity = true;
                }
            }
            if (Projectile.ai[1] != 1f)
            {
                Projectile.ai[1] = 1f;
                Projectile.position += Projectile.velocity;
                Projectile.velocity = Projectile.velocity;
            }
        }
    }
}