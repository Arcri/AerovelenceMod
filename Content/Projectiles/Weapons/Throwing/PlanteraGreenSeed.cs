
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
            projectile.width = 14;
            projectile.height = 22;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = true;
        }

        private bool rotChanged = false;

        public override void AI()
        {
            if (!rotChanged)
            {
                projectile.rotation = projectile.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2;
                rotChanged = true;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {

            target.AddBuff(BuffID.Poisoned, 180);
            if (Main.myPlayer == projectile.owner)
            {
                damage = (int)(projectile.damage * 0.5f);

                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-10, 11) * .25f, Main.rand.Next(-10, -5) * .25f, ProjectileID.SporeGas, damage, 0, projectile.owner);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-5, 12) * .25f, Main.rand.Next(-6, 10) * .25f, ProjectileID.SporeGas, damage, 0, projectile.owner);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-2, 9) * .25f, Main.rand.Next(-3, 5) * .25f, ProjectileID.SporeGas, damage, 0, projectile.owner);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-1, 8) * .25f, Main.rand.Next(-5, 10) * .25f, ProjectileID.SporeGas, damage, 0, projectile.owner);
            }
        }
    }
}
