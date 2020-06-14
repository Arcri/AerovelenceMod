
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;

namespace AerovelenceMod.NPCs.Bosses.Snowrium.Projectiles
{
	public class SnowriumFlailProjectile : ModProjectile
	{
		NPC npc1;


		private const string ChainTexturePath = "AerovelenceMod/NPCs/Bosses/Snowrium/Projectiles/SnowriumFlailProjectileChain";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Snowrium Flail");
		}

		public override void SetDefaults()
		{
			projectile.width = 28;
			projectile.height = 28;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.penetrate = -1;
			projectile.melee = true;
		}
		public override void AI()
		{

			var dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 172, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, default, 1.5f);
			dust.noGravity = true;
			dust.velocity /= 2f;
			for (int i = 0; i < 200; i++)
            {
				if (Main.npc[i].FullName == "Snowrium")
                {
					npc1 = Main.npc[i];
                }
            }
			var npc = npc1;
			npc1 = npc;
			if (!npc.active)
			{
				projectile.Kill();
				return;
			}
			int newDirection = projectile.Center.X > npc.Center.X ? 1 : -1;
			projectile.direction = newDirection;

			var vectorToPlayer = npc.Center - projectile.Center;
			float currentChainLength = vectorToPlayer.Length();
			if (projectile.ai[0] == 0f)
			{
				float maxChainLength = 600f;
				projectile.tileCollide = false;

				if (currentChainLength > maxChainLength)
				{
					projectile.ai[0] = 1f;
					projectile.netUpdate = true;
				}
			}
			else if (projectile.ai[0] == 1f)
			{
				float elasticFactorA = 10f;
				float elasticFactorB = 10f;
				float maxStretchLength = 600f;

				if (projectile.ai[1] == 1f)
					projectile.tileCollide = false;

				if (currentChainLength > maxStretchLength || !projectile.tileCollide)
				{
					projectile.ai[1] = 1f;

					if (projectile.tileCollide)
						projectile.netUpdate = true;

					projectile.tileCollide = false;

					if (currentChainLength < 20f)
						projectile.Kill();
				}

				if (!projectile.tileCollide)
					elasticFactorB *= 2f;

				int restingChainLength = 600;

				if (currentChainLength > restingChainLength || !projectile.tileCollide)
				{
					var elasticAcceleration = vectorToPlayer * elasticFactorA / currentChainLength - projectile.velocity;
					elasticAcceleration *= elasticFactorB / elasticAcceleration.Length();
					projectile.velocity *= 0.98f;
					projectile.velocity += elasticAcceleration;
				}
				else
				{
					if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 6f)
					{
						projectile.velocity.X *= 0.96f;
						projectile.velocity.Y += 0.2f;
					}
					if (npc.velocity.X == 0f)
						projectile.velocity.X *= 0.96f;
				}
			}
			projectile.rotation = vectorToPlayer.ToRotation() - projectile.velocity.X * 0.1f;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(1f, 1f, 1f, 1f);
		}



		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			bool shouldMakeSound = false;

			if (oldVelocity.X != projectile.velocity.X)
			{
				if (Math.Abs(oldVelocity.X) > 4f)
				{
					shouldMakeSound = true;
				}

				projectile.position.X += projectile.velocity.X;
				projectile.velocity.X = -oldVelocity.X * 0.2f;
			}

			if (oldVelocity.Y != projectile.velocity.Y)
			{
				if (Math.Abs(oldVelocity.Y) > 4f)
				{
					shouldMakeSound = true;
				}

				projectile.position.Y += projectile.velocity.Y;
				projectile.velocity.Y = -oldVelocity.Y * 0.2f;
			}
			projectile.ai[0] = 1f;

			if (shouldMakeSound)
			{
				projectile.netUpdate = true;
				Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
				Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
			}

			return false;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			var npc = npc1;

			Vector2 mountedCenter = npc.Center;
			Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

			var drawPosition = projectile.Center;
			var remainingVectorToNpc = mountedCenter - drawPosition;

			float rotation = remainingVectorToNpc.ToRotation() - MathHelper.PiOver2;

			if (projectile.alpha == 0)
			{
				int direction = -1;

				if (projectile.Center.X < mountedCenter.X)
					direction = 1;

				//player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
			}
			while (true)
			{
				float length = remainingVectorToNpc.Length();
				if (length < 25f || float.IsNaN(length))
					break;
				drawPosition += remainingVectorToNpc * 12 / length;
				remainingVectorToNpc = mountedCenter - drawPosition;
				Color color = new Color(1f, 1f, 1f, 1f);
				spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
			}

			return true;
		}
	}
}