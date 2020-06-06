using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Projectiles
{
    public class MallowBullet : ModProjectile
    {
		public override void SetStaticDefaults()
		{
		DisplayName.SetDefault("Mallow Bullet");
		}
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 20;
            projectile.aiStyle = 2;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 65, projectile.oldVelocity.X * 0.6f, projectile.oldVelocity.Y * 0.1f);
			}
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 10);
        }
    }
}