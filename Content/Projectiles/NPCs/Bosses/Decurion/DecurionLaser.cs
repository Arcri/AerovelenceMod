using System;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Projectiles.NPCs.Bosses.Decurion
{
	public class DecurionLaser : ModProjectile
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
			if (projectile.localAI[0] == 0f)
			{
				AdjustMagnitude(ref projectile.velocity);
				projectile.localAI[0] = 1f;
			}
			Vector2 move = Vector2.Zero;
			float distance = 1000f;
			bool target = false;
			for (int k = 0; k < Main.player.Length; k++)
			{
				Vector2 newMove = Main.player[k].Center - projectile.Center;
				float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
				if (distanceTo < distance)
				{
					move = newMove;
					distance = distanceTo;
					target = true;
				}
				projectile.rotation += projectile.velocity.X * 0.099f;
			}
			if (target)
			{
				AdjustMagnitude(ref move);
				projectile.velocity = (10 * projectile.velocity + move) / 11f;
				AdjustMagnitude(ref projectile.velocity);
			}
			if (projectile.alpha <= 30)
			{
				int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustType<WispDust>());
				Main.dust[dust].velocity *= 1f;
			}
		}

		private void AdjustMagnitude(ref Vector2 vector)
		{
			float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
			if (magnitude > 6f)
			{
				vector *= 6f / magnitude;
			}
		}
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			projectile.Kill();
        }
    }
}