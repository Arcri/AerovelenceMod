
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Throwing
{
    public class PlanteraGreenSeed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plantera Seed");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
        }

        private bool rotChanged = false;

        public override void AI()
        {
            if (!rotChanged)
            {
                Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2;
                rotChanged = true;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {

            target.AddBuff(BuffID.Poisoned, 180);
            if (Main.myPlayer == Projectile.owner)
            {
                damage = (int)(Projectile.damage * 0.5f);

                Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-10, 11) * .25f, Main.rand.Next(-10, -5) * .25f, ProjectileID.SporeGas, damage, 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-5, 12) * .25f, Main.rand.Next(-6, 10) * .25f, ProjectileID.SporeGas, damage, 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-2, 9) * .25f, Main.rand.Next(-3, 5) * .25f, ProjectileID.SporeGas, damage, 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-1, 8) * .25f, Main.rand.Next(-5, 10) * .25f, ProjectileID.SporeGas, damage, 0, Projectile.owner);
            }
        }
    }
}
