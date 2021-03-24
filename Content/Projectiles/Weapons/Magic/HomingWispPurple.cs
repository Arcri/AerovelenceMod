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
			projectile.width = 8;
			projectile.height = 8;
			projectile.alpha = 255;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.magic = true;
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
			if (projectile.ai[0] == 2)
			{
				if (target)
				{
					AdjustMagnitude(ref move);
					projectile.velocity = (10 * projectile.velocity + move) / 11f;
					AdjustMagnitude(ref projectile.velocity);
					if (projectile.ai[0] == 2 && projectile.ai[1] > 30f)
					{
						projectile.ai[1] = 0;
						for (int i = 0; i < 360; i += 60)
						{
							Projectile.NewProjectile(projectile.Center, new Vector2(20, 20).RotatedBy(MathHelper.ToRadians(i)), ProjectileType<WispLaser>(), projectile.damage, 0, Main.myPlayer);
						}
					}
					projectile.ai[1]++;
				}
			}
			if (projectile.alpha <= 30)
			{
				int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustType<WispDustPurple>());
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
			if (projectile.ai[0] == 1)
			{
				Projectile.NewProjectileDirect(target.Center, projectile.velocity, ProjectileType<LightBeam>(), 0, 0);
				if (Main.rand.NextBool())
				{
					target.AddBuff(BuffType<SoulFire>(), 300);
				}
			}

			if (projectile.ai[0] == 2)
			{
				Projectile.NewProjectileDirect(target.Center, projectile.velocity, ProjectileType<SpiralExplosion>(), projectile.damage, projectile.knockBack, projectile.owner, 0);
			}
		}
	}
}