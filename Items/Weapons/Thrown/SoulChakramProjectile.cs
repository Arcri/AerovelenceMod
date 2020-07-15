using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
	public class SoulChakramProjectile : ModProjectile
	{
		int Counter = 2;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Chakram");
		}

		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 30;
			projectile.aiStyle = 3;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.magic = false;
			projectile.penetrate = 3;
			projectile.timeLeft = 600;
			projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			int num622 = Dust.NewDust(new Vector2(projectile.position.X - projectile.velocity.X, projectile.position.Y - projectile.velocity.Y), projectile.width, projectile.height, 191, 0f, 0f, 100, default, 2f);
			Main.dust[num622].noGravity = true;
			Main.dust[num622].scale = 0.5f;
			projectile.rotation += 0.1f;
		}
	}
}