using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Core.Prim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class GaussianStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gaussian Star");
			ProjectileID.Sets.Homing[projectile.type] = true;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 46;
			projectile.height = 46;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.light = 1f;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
		}
		public override void AI()
		{
			if (projectile.direction == 1)
			{
				projectile.rotation += 0.15f;
			}
			else
			{
				projectile.rotation -= 0.15f;
			}
			if (projectile.ai[0] < 15)
			{
				projectile.ai[0]++;
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
						Vector2 newMove = Main.npc[i].Center - projectile.Center;
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
					projectile.velocity *= 2f;
					projectile.velocity = 2f * projectile.velocity + move;
					AdjustMagnitude(ref projectile.velocity);
				}
			}
		}
		private void AdjustMagnitude(ref Vector2 vector)
		{
			float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
			if (magnitude > 10f)
			{
				vector *= 10f / magnitude; 
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
				if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(projectile.Center, Main.npc[i].Center) < 85f)
				{
					int Direction = 0;
					if (projectile.position.X - Main.npc[i].position.X < 0)
						Direction = 1;
					else
						Direction = -1;
					Main.npc[i].StrikeNPC(projectile.damage, projectile.knockBack, Direction);
				}
			}
			for (double i = 0; i < 7.2; i += 0.2)
			{
				//if (Main.rand.NextFloat() <= 0.3f)
				//{
					Dust dust = Dust.NewDustPerfect(projectile.Center, DustID.Electric, new Vector2((float)Math.Sin(i) * 2.6f, (float)Math.Cos(i)) * 2.6f); //cos2.4
					Dust dust2 = Dust.NewDustPerfect(projectile.Center, DustID.Electric, new Vector2((float)Math.Sin(i) * 2.4f, (float)Math.Cos(i)).RotatedBy(Math.PI / 2) * 2.4f); //cos2.4

					dust.noGravity = true;
					dust2.noGravity = true;
					Main.PlaySound(SoundID.Item94, projectile.Center);
				//}
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, Main.projectileTexture[projectile.type].Height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length / 2);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}

			Texture2D texture2 = ModContent.GetTexture("AerovelenceMod/Assets/Glow");
			spriteBatch.Draw(texture2, projectile.Center - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Rectangle(0, 0, texture2.Width, texture2.Height), Color.CornflowerBlue * 0.8f, projectile.rotation, texture2.Size() / 2, 2f, SpriteEffects.None, 0f);

			spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height), lightColor, projectile.rotation, Main.projectileTexture[projectile.type].Size() / 2, 1f, SpriteEffects.None, 0f);


			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;

		}
	}
}