using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class CrystalShard : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.aiStyle = 1;
			Projectile.width = 10;
			Projectile.height = 22;
			Projectile.alpha =  0;
			Projectile.damage = 6;
			Projectile.friendly = true;
            Projectile.maxPenetrate = 3;
			Projectile.hostile = false;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{
			Projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.White, 1);
			Main.dust[dust1].velocity /= 4f;
		}
	}
}