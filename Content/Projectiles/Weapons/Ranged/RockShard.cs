using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
	public class RockShard : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.aiStyle = 1;
			Projectile.width = 14;
			Projectile.height = 18;
			Projectile.alpha =  0;
			Projectile.damage = 6;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{
			Projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.Blue, 1);
			Main.dust[dust1].velocity /= 2f;
		}
	}
}