using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Buffs;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class HomingWispPurple : ModProjectile
	{ 
		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.alpha = 255;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Magic;
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
			if (Projectile.ai[0] == 2)
			{
				if (target)
				{
					AdjustMagnitude(ref move);
					Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
					AdjustMagnitude(ref Projectile.velocity);
					if (Projectile.ai[0] == 2 && Projectile.ai[1] > 30f)
					{
						Projectile.ai[1] = 0;
						for (int i = 0; i < 360; i += 60)
						{
							Projectile.NewProjectile(Projectile.Center, new Vector2(20, 20).RotatedBy(MathHelper.ToRadians(i)), ProjectileType<WispLaser>(), Projectile.damage, 0, Main.myPlayer);
						}
					}
					Projectile.ai[1]++;
				}
			}
			if (Projectile.alpha <= 30)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<WispDustPurple>());
				Main.dust[dust].velocity *= 1f;
			}
		}

		private void AdjustMagnitude(ref Vector2 vector)
		{
			float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
			if (magnitude > 6f)
			{
				vector *= 11f / magnitude;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Projectile.ai[0] == 1)
			{
				Projectile.NewProjectileDirect(target.Center, Projectile.velocity, ProjectileType<LightBeam>(), 0, 0);
				if (Main.rand.NextBool())
				{
					target.AddBuff(BuffType<SoulFire>(), 300);
				}
			}

			if (Projectile.ai[0] == 2)
			{
				Projectile.NewProjectileDirect(target.Center, Projectile.velocity, ProjectileType<SpiralExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0);
			}
		}
	}
}