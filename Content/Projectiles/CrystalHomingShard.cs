using System;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Projectiles
{
	public class CrystalHomingShard : ModProjectile
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
			Projectile.alpha = 255;
			Projectile.damage = 15;
			Projectile.hostile = false;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
		}

		public override void AI()
		{
			if (Projectile.alpha > 30)
			{
				Projectile.alpha -= 15;
				if (Projectile.alpha < 30)
				{
					Projectile.alpha = 30;
				}
			}
			if (Projectile.localAI[0] == 0f)
			{
				AdjustMagnitude(ref Projectile.velocity);
				Projectile.localAI[0] = 1f;
			}
			Vector2 move = Vector2.Zero;
			float distance = 400f;
			bool target = false;
			for (int k = 0; k < 200; k++)
			{
				if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
				{
					Vector2 newMove = Main.npc[k].Center - Projectile.Center;
					float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
					if (distanceTo < distance)
					{
						move = newMove;
						distance = distanceTo;
						target = true;
					}
				}
				Projectile.rotation += Projectile.velocity.X * 0.099f;
			}
			if (target)
			{
				AdjustMagnitude(ref move);
				Projectile.velocity = (5 * Projectile.velocity + move) / 6f;
				AdjustMagnitude(ref Projectile.velocity);
			}
			if (Projectile.alpha <= 30)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<WispDust>());
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
			Projectile.Kill();
        }
    }
}