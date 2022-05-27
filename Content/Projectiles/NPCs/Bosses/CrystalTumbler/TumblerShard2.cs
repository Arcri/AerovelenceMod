using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Projectiles.NPCs.Bosses.CrystalTumbler
{
	public class TumblerShard2 : ModProjectile
	{
		int t;
		public override void SetDefaults()
		{
			Projectile.aiStyle = 1;
			Projectile.width = 10;
			Projectile.height = 22;
			Projectile.alpha =  0;
			Projectile.damage = 6;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{
			t++;
			Projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.White, 1);
			Main.dust[dust1].velocity /= 2f;
			if (t > 100)
			{
				Projectile.tileCollide = true;
			}
		}
		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item, Projectile.Center);
		}
	}
}
