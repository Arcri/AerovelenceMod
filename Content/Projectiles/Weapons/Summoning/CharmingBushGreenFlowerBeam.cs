#region Using directives

using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class CharmingBushGreenFlowerBeam : ModProjectile
	{
		private readonly int AliveTime = 30;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.MinionShot[projectile.type] = true;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 18;

			projectile.alpha = 255;
			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Projectile parent = Main.projectile[(int)projectile.ai[0]];

			if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
			{
				projectile.velocity = -Vector2.UnitY;
			}

			if (parent.active && parent.type == ModContent.ProjectileType<CharmingBushGreenFlower>())
			{
				projectile.position = parent.Center - projectile.Size / 2f + new Vector2(0f, -parent.gfxOffY);
			}
			else
			{
				projectile.Kill();
				return (false);
			}

			if (++projectile.ai[1] >= AliveTime)
			{
				projectile.Kill();
				return (false);
			}			
			projectile.scale = 0.4f + (float)(Math.Abs(Math.Sin(projectile.ai[1] / 5)) * 0.3f);

			float projectileRotation = projectile.velocity.ToRotation();
			projectile.rotation = projectileRotation - MathHelper.PiOver2;
			projectile.velocity = projectileRotation.ToRotationVector2();

			float beamLength = 0f;
			float sampleCount = 2;
			Vector2 samplingPoint = projectile.Center;
			float[] samplePoints = new float[(int)sampleCount];

			Collision.LaserScan(samplingPoint, projectile.velocity, 0f, 2400f, samplePoints);
			for (int i = 0; i < samplePoints.Length; ++i)
			{
				beamLength += samplePoints[i];
			}
			beamLength /= sampleCount;
			float amount = 0.15f;

			projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], beamLength, amount);

			SpawnDust();

			DelegateMethods.v3_1 = new Vector3(0.4f, 0.85f, 0.9f);
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, DelegateMethods.CastLight);

			return (false);
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
			base.DrawBehind(index, drawCacheProjsBehindNPCsAndTiles, drawCacheProjsBehindNPCs, drawCacheProjsBehindProjectiles, drawCacheProjsOverWiresUI);
		}

		public override void CutTiles()
		{
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, DelegateMethods.CutTiles);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float collisionPoint = 0f;
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], 22f * projectile.scale, ref collisionPoint))
			{
				return true;
			}

			return (false);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.velocity == Vector2.Zero)
			{
				return (false);
			}
			Texture2D texture2D15 = Main.projectileTexture[projectile.type];
			float num175 = projectile.localAI[1];
			Color color37 = new Color(255, 255, 255, 0) * 0.9f;
			Rectangle rectangle14 = new Rectangle(0, 0, texture2D15.Width, 22);
			Vector2 value16 = new Vector2(0f, Main.player[projectile.owner].gfxOffY);

			spriteBatch.Draw(texture2D15, projectile.Center.Floor() - Main.screenPosition + value16, rectangle14, color37, projectile.rotation, rectangle14.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			num175 -= 33f * projectile.scale;
			Vector2 value17 = projectile.Center.Floor();
			value17 += projectile.velocity * projectile.scale * 10.5f;
			rectangle14 = new Rectangle(0, 25, texture2D15.Width, 28);
			if (num175 > 0f)
			{
				float num176 = 0f;
				while (num176 + 1f < num175)
				{
					if (num175 - num176 < rectangle14.Height)
					{
						rectangle14.Height = (int)(num175 - num176);
					}
					spriteBatch.Draw(texture2D15, value17 - Main.screenPosition + value16, rectangle14, color37, projectile.rotation, new Vector2(rectangle14.Width / 2, 0f), projectile.scale, SpriteEffects.None, 0f);
					num176 += rectangle14.Height * projectile.scale;
					value17 += projectile.velocity * rectangle14.Height * projectile.scale;
				}
			}

			rectangle14 = new Rectangle(0, 56, texture2D15.Width, 22);
			spriteBatch.Draw(texture2D15, value17 - Main.screenPosition + value16, rectangle14, color37, projectile.rotation, texture2D15.Frame().Top(), projectile.scale, SpriteEffects.None, 0f);
			return (false);
		}

		private void SpawnDust()
		{
			Vector2 dustPosition = projectile.Center + projectile.velocity * (projectile.localAI[1] - 8f);

			for (int i = 0; i < 2; ++i)
			{
				float dustRotation = projectile.velocity.ToRotation() + MathHelper.PiOver2 * (Main.rand.Next(2) == 1 ? -1f : 1f);
				float dustPositionModifier = Main.rand.NextFloat() * 0.8f + 1f;
				Vector2 dustVelocity = new Vector2((float)Math.Cos(dustRotation), (float)Math.Sin(dustRotation)) * dustPositionModifier;

				Dust dust = Dust.NewDustDirect(dustPosition, 0, 0, ModContent.DustType<Dusts.GreenPetal>(), dustVelocity.X, dustVelocity.Y);
				dust.scale = 1.2f;
				dust.noGravity = true;
			}
		}
	}
}
