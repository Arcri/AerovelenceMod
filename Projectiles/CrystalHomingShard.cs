using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Projectiles
{
	public class CrystalHomingShard : ModProjectile
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
			projectile.alpha = 255;
			projectile.damage = 15;
			projectile.hostile = false;
			projectile.timeLeft = 300;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.friendly = true;
			projectile.ranged = true;
		}

		public override void AI()
		{
			if (projectile.alpha > 30)
			{
				projectile.alpha -= 15;
				if (projectile.alpha < 30)
				{
					projectile.alpha = 30;
				}
			}
			if (projectile.localAI[0] == 0f)
			{
				AdjustMagnitude(ref projectile.velocity);
				projectile.localAI[0] = 1f;
			}
			Vector2 move = Vector2.Zero;
			float distance = 400f;
			bool target = false;
			for (int k = 0; k < 200; k++)
			{
				if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
				{
					Vector2 newMove = Main.npc[k].Center - projectile.Center;
					float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
					if (distanceTo < distance)
					{
						move = newMove;
						distance = distanceTo;
						target = true;
					}
				}
				projectile.rotation += projectile.velocity.X * 0.099f;
			}
			if (target)
			{
				AdjustMagnitude(ref move);
				projectile.velocity = (5 * projectile.velocity + move) / 6f;
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
			if (magnitude > 3f)
			{
				vector *= 5f / magnitude;
			}
		}
		public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			projectile.Kill();
        }
    }
}