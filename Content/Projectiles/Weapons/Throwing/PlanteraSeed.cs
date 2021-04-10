using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Throwing
{
    public class PlanteraSeed : ModProjectile
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
        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<BlossomProjectile>(), projectile.damage * 2, projectile.knockBack, projectile.owner, 0f, 0f);
        }
    }
}
