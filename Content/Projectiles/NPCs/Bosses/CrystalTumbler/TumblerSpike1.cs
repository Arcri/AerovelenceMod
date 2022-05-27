using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Projectiles.NPCs.Bosses.CrystalTumbler
{
	public class TumblerSpike1 : ModProjectile
	{
		int t;
		public override void SetDefaults()
		{
			Projectile.aiStyle = 1;
			Projectile.width = 44;
			Projectile.height = 44;
			Projectile.alpha =  0;
			Projectile.damage = 12;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{
			t++;
			if (Main.expertMode)
			{
				Projectile.damage = 16;
			}
			Projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.Black, 1);
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
