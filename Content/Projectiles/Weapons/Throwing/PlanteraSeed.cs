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
        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<BlossomProjectile>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 0f, 0f);
        }
    }
}
