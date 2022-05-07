#region Using directives

using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class CharmingBushGreenFlowerBeam : ModProjectile
	{
		private readonly int AliveTime = 30;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.MinionShot[Projectile.type] = true;
		}
		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 18;

			Projectile.alpha = 255;
			Projectile.penetrate = -1;

			Projectile.friendly = true;
			Projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Projectile parent = Main.projectile[(int)Projectile.ai[0]];

			if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
			{
				Projectile.velocity = -Vector2.UnitY;
			}

			if (parent.active && parent.type == ModContent.ProjectileType<CharmingBushGreenFlower>())
			{
				Projectile.position = parent.Center - Projectile.Size / 2f + new Vector2(0f, -parent.gfxOffY);
			}
			else
			{
				Projectile.Kill();
				return (false);
			}

			if (++Projectile.ai[1] >= AliveTime)
			{
				Projectile.Kill();
				return (false);
			}			
			Projectile.scale = 0.4f + (float)(Math.Abs(Math.Sin(Projectile.ai[1] / 5)) * 0.3f);

			float projectileRotation = Projectile.velocity.ToRotation();
			Projectile.rotation = projectileRotation - MathHelper.PiOver2;
			Projectile.velocity = projectileRotation.ToRotationVector2();

			float beamLength = 0f;
			float sampleCount = 2;
			Vector2 samplingPoint = Projectile.Center;
			float[] samplePoints = new float[(int)sampleCount];

			Collision.LaserScan(samplingPoint, Projectile.velocity, 0f, 2400f, samplePoints);
			for (int i = 0; i < samplePoints.Length; ++i)
			{
				beamLength += samplePoints[i];
			}
			beamLength /= sampleCount;
			float amount = 0.15f;

			Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], beamLength, amount);

			SpawnDust();

			DelegateMethods.v3_1 = new Vector3(0.4f, 0.85f, 0.9f);
			Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);

			return (false);
		}

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

		public override void CutTiles()
		{
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float collisionPoint = 0f;
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref collisionPoint))
			{
				return true;
			}

			return (false);
		}

        public override bool PreDraw(ref Color lightColor)
        {
			if (Projectile.velocity == Vector2.Zero)
			{
				return (false);
			}
			Texture2D texture2D15 = (Texture2D)TextureAssets.Projectile[Projectile.type];
			float num175 = Projectile.localAI[1];
			Color color37 = new Color(255, 255, 255, 0) * 0.9f;
			Rectangle rectangle14 = new Rectangle(0, 0, texture2D15.Width, 22);
			Vector2 value16 = new Vector2(0f, Main.player[Projectile.owner].gfxOffY);

			Main.EntitySpriteDraw(texture2D15, Projectile.Center.Floor() - Main.screenPosition + value16, rectangle14, color37, Projectile.rotation, rectangle14.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
			num175 -= 33f * Projectile.scale;
			Vector2 value17 = Projectile.Center.Floor();
			value17 += Projectile.velocity * Projectile.scale * 10.5f;
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
					Main.EntitySpriteDraw(texture2D15, value17 - Main.screenPosition + value16, rectangle14, color37, Projectile.rotation, new Vector2(rectangle14.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
					num176 += rectangle14.Height * Projectile.scale;
					value17 += Projectile.velocity * rectangle14.Height * Projectile.scale;
				}
			}

			rectangle14 = new Rectangle(0, 56, texture2D15.Width, 22);
			Main.EntitySpriteDraw(texture2D15, value17 - Main.screenPosition + value16, rectangle14, color37, Projectile.rotation, texture2D15.Frame().Top(), Projectile.scale, SpriteEffects.None, 0);
			return (false);
		}

		private void SpawnDust()
		{
			Vector2 dustPosition = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 8f);

			for (int i = 0; i < 2; ++i)
			{
				float dustRotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * (Main.rand.Next(2) == 1 ? -1f : 1f);
				float dustPositionModifier = Main.rand.NextFloat() * 0.8f + 1f;
				Vector2 dustVelocity = new Vector2((float)Math.Cos(dustRotation), (float)Math.Sin(dustRotation)) * dustPositionModifier;

				Dust dust = Dust.NewDustDirect(dustPosition, 0, 0, ModContent.DustType<Dusts.GreenPetal>(), dustVelocity.X, dustVelocity.Y);
				dust.scale = 1.2f;
				dust.noGravity = true;
			}
		}
	}
}
