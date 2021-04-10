#region Using directives

using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Utilities;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
    internal class CharmingBushFlower : ModProjectile
	{
		protected bool JustSpawned => projectile.localAI[0] == 0;

		protected virtual float ShootRange => 300;
		protected virtual int ShootCooldown => 60;

		private bool drawBehindOtherProjectiles = false;

		public override bool Autoload(ref string name)
			=> false;

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;

			projectile.minionSlots = 1f;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			AeroPlayer ap = owner.GetModPlayer<AeroPlayer>();

			if (JustSpawned)
			{
				for (int i = 0; i < 20; ++i)
				{
					Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.Leaves>(), 0, 0, 100);
					dust.velocity *= 1.5f;
				}

				projectile.localAI[0] = 1;
			}

			if (!owner.active)
			{
				projectile.active = false;
				return (false);
			}

			if (owner.dead)
			{
				ap.charmingBush = false;
			}
			if (ap.charmingBush)
			{
				projectile.timeLeft = 2;
			}

			FollowOwner(owner);

			if ((int)(++projectile.localAI[1]) % ShootCooldown == 0)
			{
				int target = -1;
				float distance = ShootRange;

				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].CanBeChasedBy(projectile))
					{
						float currentDistance = projectile.Distance(Main.npc[i].Center);

						if (currentDistance < distance)
						{
							target = i;
							distance = currentDistance;
						}
					}
				}

				if (target != -1)
				{
					if (Main.myPlayer == projectile.owner)
					{
						ShootAt(Main.npc[target]);
					}
				}
			}

			return (false);
		}

		public override bool CanDamage()
			=> false;

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
			// Make sure the projectiles draw behind the other projectiles when they're at the top of their rotation.
			if (drawBehindOtherProjectiles)
			{
				projectile.hide = true;
				drawCacheProjsBehindProjectiles.Add(index);
			}
			else
			{
				projectile.hide = false;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawProjectileCentered(spriteBatch, lightColor);

		public override bool PreDrawExtras(SpriteBatch spriteBatch)
		{
			Texture2D chain = ModContent.GetTexture(this.Texture + "_Chain");
			Rectangle frame = chain.Frame(1, 1, 0, 0);
			Vector2 chainOrigin = frame.Size() / 2;

			bool drawChain = true;
			float chainWidth = chain.Width * projectile.scale;

			Vector2 chainDirection = Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center);
			Vector2 chainPosition = projectile.Center + chainDirection * chainWidth / 3;
			float chainRotation = chainDirection.ToRotation();

			while (drawChain)
			{
				Color lightColor = Lighting.GetColor((int)chainPosition.X / 16, (int)chainPosition.Y / 16);
				float currentLength = (chainPosition - Main.player[projectile.owner].Center).Length();

				if (currentLength <= chainWidth + 4)
				{
					drawChain = false;
					frame.Width = (int)currentLength;
				}

				spriteBatch.Draw(chain, chainPosition - Main.screenPosition, frame, lightColor, chainRotation, chainOrigin, projectile.scale, SpriteEffects.None, 0f);

				chainPosition += chainDirection * chainWidth;
			}
			return (false);
		}

		private void FollowOwner(Player owner)
		{
			Vector2 targetPosition = owner.Center;

			this.drawBehindOtherProjectiles = false;

			float rotationOffset = 0;
			if (projectile.ai[0] != 0)
			{
				rotationOffset = MathHelper.TwoPi / projectile.ai[0] * projectile.ai[1];
			}

			float currentRotation = ((float)Main.time / 60 + rotationOffset) % MathHelper.TwoPi;

			float xOffset = (float)Math.Cos(currentRotation) * 60;
			float yOffset = (float)Math.Sin(currentRotation) * 40;
			if (yOffset > 0)
			{
				yOffset *= 0.6f;
				drawBehindOtherProjectiles = true;
			}

			targetPosition.X += owner.width / 2 + xOffset;
			targetPosition.Y -= 100 + yOffset;

			projectile.Center = Vector2.Lerp(projectile.Center, targetPosition, 0.2f);
			projectile.velocity *= 0.8f;
			projectile.direction = projectile.spriteDirection = owner.direction;

			projectile.rotation = (float)Math.Sin(projectile.localAI[1] / 15) * 0.2f;
		}

		protected virtual void ShootAt(NPC target) { }
	}
}
