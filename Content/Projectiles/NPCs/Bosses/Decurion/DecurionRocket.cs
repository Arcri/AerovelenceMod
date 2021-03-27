using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.NPCs.Bosses.Decurion
{
    public class DecurionRocket : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.Homing[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.width = 14;
			projectile.maxPenetrate = 1;
			projectile.height = 36;
			projectile.damage = 11;
			projectile.hostile = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.friendly = false;
			projectile.magic = true;
			projectile.timeLeft = 200;
		}

		public override void AI()
		{
			projectile.rotation = projectile.velocity.ToRotation();
			if (Math.Abs(projectile.velocity.X) >= 8f || Math.Abs(projectile.velocity.Y) >= 8f)
			{
				for (int num252 = 0; num252 < 2; num252++)
				{
					float num253 = 0f;
					float num254 = 0f;
					if (num252 == 1)
					{
						num253 = projectile.velocity.X * 0.5f;
						num254 = projectile.velocity.Y * 0.5f;
					}
					int num255 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num253, projectile.position.Y + 3f + num254) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 110, 0f, 0f, 100);
					Dust dust39 = Main.dust[num255];
					Dust dust2 = dust39;
					dust39 = Main.dust[num255];
					dust2 = dust39;
					dust2.velocity *= 0.2f;
					Main.dust[num255].noGravity = true;
					num255 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num253, projectile.position.Y + 3f + num254) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 110, 0f, 0f, 100, default(Color), 0.5f);
					Main.dust[num255].fadeIn = 1f + Main.rand.Next(5) * 0.1f;
					dust39 = Main.dust[num255];
					dust2 = dust39;
					dust2.velocity *= 0.05f;
				}
			}
			if (Math.Abs(projectile.velocity.X) < 15f && Math.Abs(projectile.velocity.Y) < 15f)
			{
				projectile.velocity *= 1.1f;
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
			Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 14);

			projectile.position.X = projectile.position.X + (projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (projectile.width / 2);
			projectile.width = 30;
			projectile.height = 30;
			projectile.position.X = projectile.position.X - (projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (projectile.width / 2);

			for (int i = 0; i < 35; i++)
			{
				int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 110, 0f, 0f, 100, default, 1f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 5f;
				dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 110, 0f, 0f, 100, default, 1f);
				Main.dust[dust].velocity *= 2f;
			}
		}
	}
}