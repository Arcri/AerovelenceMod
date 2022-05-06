using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Projectiles.NPCs.Bosses.Decurion
{
    public class DecurionRocket : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.Homing[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.maxPenetrate = 1;
			Projectile.height = 36;
			Projectile.damage = 11;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.friendly = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 200;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f)
			{
				for (int num252 = 0; num252 < 2; num252++)
				{
					float num253 = 0f;
					float num254 = 0f;
					if (num252 == 1)
					{
						num253 = Projectile.velocity.X * 0.5f;
						num254 = Projectile.velocity.Y * 0.5f;
					}
					int num255 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num253, Projectile.position.Y + 3f + num254) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 110, 0f, 0f, 100);
					Dust dust39 = Main.dust[num255];
					Dust dust2 = dust39;
					dust39 = Main.dust[num255];
					dust2 = dust39;
					dust2.velocity *= 0.2f;
					Main.dust[num255].noGravity = true;
					num255 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num253, Projectile.position.Y + 3f + num254) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 110, 0f, 0f, 100, default(Color), 0.5f);
					Main.dust[num255].fadeIn = 1f + Main.rand.Next(5) * 0.1f;
					dust39 = Main.dust[num255];
					dust2 = dust39;
					dust2.velocity *= 0.05f;
				}
			}
			if (Math.Abs(Projectile.velocity.X) < 15f && Math.Abs(Projectile.velocity.Y) < 15f)
			{
				Projectile.velocity *= 1.1f;
			}
		}
		public override bool? CanHitNPC(NPC target)
		{
			if (target.townNPC)
			{
				return false;
			}
			return base.CanHitNPC(target);
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, 14);

			Projectile.position.X = Projectile.position.X + (Projectile.width / 2);
			Projectile.position.Y = Projectile.position.Y + (Projectile.width / 2);
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
			Projectile.position.Y = Projectile.position.Y - (Projectile.width / 2);

			for (int i = 0; i < 35; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 110, 0f, 0f, 100, default, 1f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 5f;
				dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 110, 0f, 0f, 100, default, 1f);
				Main.dust[dust].velocity *= 2f;
			}
		}
	}
}