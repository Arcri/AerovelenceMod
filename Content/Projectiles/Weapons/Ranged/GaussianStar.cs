using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Core.Prim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class GaussianStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gaussian Star");
			ProjectileID.Sets.Homing[Projectile.type] = true;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 46;
			Projectile.height = 46;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.light = 1f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
		}
		public override void AI()
		{
			if (Projectile.direction == 1)
			{
				Projectile.rotation += 0.15f;
			}
			else
			{
				Projectile.rotation -= 0.15f;
			}
			if (Projectile.ai[0] < 15)
			{
				Projectile.ai[0]++;
			}
			else
			{
				Vector2 move = Vector2.Zero;
				float distance = 250f;
				bool target = false;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && Main.npc[i].lifeMax > 5 && Main.npc[i].type != NPCID.TargetDummy)
					{
						Vector2 newMove = Main.npc[i].Center - Projectile.Center;
						float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
						if (distanceTo < distance)
						{
							move = newMove;
							distance = distanceTo;
							target = true;
						}
					}
				}
				if (target)
				{
					AdjustMagnitude(ref move);
					Projectile.velocity *= 2f;
					Projectile.velocity = 2f * Projectile.velocity + move;
					AdjustMagnitude(ref Projectile.velocity);
				}
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
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			AoE();
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			AoE();
			return true;
		}
		public void AoE() 
		{ 
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(Projectile.Center, Main.npc[i].Center) < 160f)
				{
					int Direction = 0;
					if (Projectile.position.X - Main.npc[i].position.X < 0)
						Direction = 1;
					else
						Direction = -1;
					Main.npc[i].StrikeNPC(Projectile.damage, Projectile.knockBack, Direction);
				}
			}
			for (double i = 0; i < 6.28; i += 0.2)
			{
				if (Main.rand.NextFloat() <= 0.3f)
				{
					Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, new Vector2((float)Math.Sin(i) * 2.6f, (float)Math.Cos(i)) * 2.4f);
					dust.noGravity = true;
					SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
				}
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[Projectile.type].Width * 0.5f, Main.projectileTexture[Projectile.type].Height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
				spriteBatch.Draw(Main.projectileTexture[Projectile.type], drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
	}
}