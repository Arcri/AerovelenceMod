using AerovelenceMod.Content.Dusts;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Projectiles.NPCs.Bosses.CrystalTumbler
{
	public class TumblerSpike2 : ModProjectile
	{
		int t;
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			projectile.width = 44;
			projectile.height = 44;
			projectile.alpha = 0;
			projectile.damage = 12;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}
		public override void AI()
		{
			t++;
			if (Main.expertMode)
			{
				projectile.damage = 19;
			}
			projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustType<Sparkle>(), projectile.velocity.X, projectile.velocity.Y, 0, default, 1);
			Main.dust[dust1].velocity /= 2f;
			if (t > 100)
			{
				projectile.tileCollide = true;
			}
		}
	}
}