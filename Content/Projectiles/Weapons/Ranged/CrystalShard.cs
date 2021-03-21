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
			projectile.aiStyle = 1;
			projectile.width = 10;
			projectile.height = 22;
			projectile.alpha =  0;
			projectile.damage = 6;
			projectile.friendly = true;
            projectile.maxPenetrate = 3;
			projectile.hostile = false;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
		}
		public override void AI()
		{
			projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Sparkle>(), projectile.velocity.X, projectile.velocity.Y, 0, Color.White, 1);
			Main.dust[dust1].velocity /= 4f;
		}
	}
}