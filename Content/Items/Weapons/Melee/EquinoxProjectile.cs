using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
	public class EquinoxProjectile : ModProjectile
	{
		private const string ChainTexturePath = "AerovelenceMod/Content/Items/Weapons/Melee/EquinoxProjectileChain";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Equinox");
		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Melee;
		}
		public override void AI()
		{
			var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 172, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100, default, 1.5f);
			dust.noGravity = true;
			dust.velocity /= 2f;

			var player = Main.player[Projectile.owner];
			if (player.dead)
			{
				Projectile.Kill();
				return;
			}
			player.itemAnimation = 10;
			player.itemTime = 10;
			int newDirection = Projectile.Center.X > player.Center.X ? 1 : -1;
			player.ChangeDir(newDirection);
			Projectile.direction = newDirection;

			var vectorToPlayer = player.MountedCenter - Projectile.Center;
			float currentChainLength = vectorToPlayer.Length();
			if (Projectile.ai[0] == 0f)
			{
				float maxChainLength = 160f;
				Projectile.tileCollide = true;

				if (currentChainLength > maxChainLength)
				{
					Projectile.ai[0] = 1f;
					Projectile.netUpdate = true;
				}
				else if (!player.channel)
				{
					if (Projectile.velocity.Y < 0f)
						Projectile.velocity.Y *= 0.9f;

					Projectile.velocity.Y += 1f;
					Projectile.velocity.X *= 0.9f;
				}
			}
			else if (Projectile.ai[0] == 1f)
			{
				float elasticFactorA = 14f / player.GetAttackSpeed(DamageClass.Melee);
				float elasticFactorB = 0.9f / player.GetAttackSpeed(DamageClass.Melee);
				float maxStretchLength = 300f;

				if (Projectile.ai[1] == 1f)
					Projectile.tileCollide = false;

				if (!player.channel || currentChainLength > maxStretchLength || !Projectile.tileCollide)
				{
					Projectile.ai[1] = 1f;

					if (Projectile.tileCollide)
						Projectile.netUpdate = true;

					Projectile.tileCollide = false;

					if (currentChainLength < 20f)
						Projectile.Kill();
				}

				if (!Projectile.tileCollide)
					elasticFactorB *= 2f;

				int restingChainLength = 60;

				if (currentChainLength > restingChainLength || !Projectile.tileCollide)
				{
					var elasticAcceleration = vectorToPlayer * elasticFactorA / currentChainLength - Projectile.velocity;
					elasticAcceleration *= elasticFactorB / elasticAcceleration.Length();
					Projectile.velocity *= 0.98f;
					Projectile.velocity += elasticAcceleration;
				}
				else
				{
					if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 6f)
					{
						Projectile.velocity.X *= 0.96f;
						Projectile.velocity.Y += 0.2f;
					}
					if (player.velocity.X == 0f)
						Projectile.velocity.X *= 0.96f;
				}
			}
			Projectile.rotation = (Projectile.Center - player.Center).ToRotation();
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			bool shouldMakeSound = false;

			if (oldVelocity.X != Projectile.velocity.X)
			{
				if (Math.Abs(oldVelocity.X) > 4f)
				{
					shouldMakeSound = true;
				}

				Projectile.position.X += Projectile.velocity.X;
				Projectile.velocity.X = -oldVelocity.X * 0.2f;
			}

			if (oldVelocity.Y != Projectile.velocity.Y)
			{
				if (Math.Abs(oldVelocity.Y) > 4f)
				{
					shouldMakeSound = true;
				}

				Projectile.position.Y += Projectile.velocity.Y;
				Projectile.velocity.Y = -oldVelocity.Y * 0.2f;
			}
			Projectile.ai[0] = 1f;

			if (shouldMakeSound)
			{
				Projectile.netUpdate = true;
				Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
				SoundEngine.PlaySound(SoundID.Dig, (int)Projectile.position.X, (int)Projectile.position.Y);
			}

			return false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			var player = Main.player[Projectile.owner];

			Vector2 mountedCenter = player.MountedCenter;
			Texture2D chainTexture = (Texture2D)ModContent.Request<Texture2D>(ChainTexturePath);

			var drawPosition = Projectile.Center;
			var remainingVectorToPlayer = mountedCenter - drawPosition;

			float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

			if (Projectile.alpha == 0)
			{
				int direction = -1;

				if (Projectile.Center.X < mountedCenter.X)
					direction = 1;

				player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
			}
			while (true)
			{
				float length = remainingVectorToPlayer.Length();
				if (length < 25f || float.IsNaN(length))
					break;
				drawPosition += remainingVectorToPlayer * 12 / length;
				remainingVectorToPlayer = mountedCenter - drawPosition;
				Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
				Main.EntitySpriteDraw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}

			return true;
		}
	}
}