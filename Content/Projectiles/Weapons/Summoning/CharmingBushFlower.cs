#region Using directives

using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Utilities;
using Terraria.ID;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
    internal class CharmingBushFlower : ModProjectile
	{
		protected bool JustSpawned => Projectile.localAI[0] == 0;

		protected virtual float ShootRange => 300;
		protected virtual int ShootCooldown => 60;

		private bool drawBehindOtherProjectiles = false;

		//public override bool Autoload(ref string name)
		//=> false;

		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.None;

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 22;

			Projectile.minionSlots = 1f;

			Projectile.minion = true;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[Projectile.owner];
			AeroPlayer ap = owner.GetModPlayer<AeroPlayer>();

			if (JustSpawned)
			{
				for (int i = 0; i < 20; ++i)
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Leaves>(), 0, 0, 100);
					dust.velocity *= 1.5f;
				}

				Projectile.localAI[0] = 1;
			}

			if (!owner.active)
			{
				Projectile.active = false;
				return (false);
			}

			if (owner.dead)
			{
				ap.charmingBush = false;
			}
			if (ap.charmingBush)
			{
				Projectile.timeLeft = 2;
			}

			FollowOwner(owner);

			if ((int)(++Projectile.localAI[1]) % ShootCooldown == 0)
			{
				int target = -1;
				float distance = ShootRange;

				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].CanBeChasedBy(Projectile))
					{
						float currentDistance = Projectile.Distance(Main.npc[i].Center);

						if (currentDistance < distance)
						{
							target = i;
							distance = currentDistance;
						}
					}
				}

				if (target != -1)
				{
					if (Main.myPlayer == Projectile.owner)
					{
						ShootAt(Main.npc[target]);
					}
				}
			}

			return (false);
		}

		public override bool? CanDamage()
			=> false;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			// Make sure the projectiles draw behind the other projectiles when they're at the top of their rotation.
			if (drawBehindOtherProjectiles)
			{
				Projectile.hide = true;
				behindProjectiles.Add(index);
			}
			else
			{
				Projectile.hide = false;
			}
		}

        public override bool PreDraw(ref Color lightColor)
        => this.DrawProjectileCentered(Main.spriteBatch, lightColor);

        public override bool PreDrawExtras()
        {
        Texture2D chain = (Texture2D)ModContent.Request<Texture2D>(Texture + "_Chain");
			Rectangle frame = chain.Frame(1, 1, 0, 0);
			Vector2 chainOrigin = frame.Size() / 2;

			bool drawChain = true;
			float chainWidth = chain.Width * Projectile.scale;

			Vector2 chainDirection = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center);
			Vector2 chainPosition = Projectile.Center + chainDirection * chainWidth / 3;
			float chainRotation = chainDirection.ToRotation();

			while (drawChain)
			{
				Color lightColor = Lighting.GetColor((int)chainPosition.X / 16, (int)chainPosition.Y / 16);
				float currentLength = (chainPosition - Main.player[Projectile.owner].Center).Length();

				if (currentLength <= chainWidth + 4)
				{
					drawChain = false;
					frame.Width = (int)currentLength;
				}

				Main.EntitySpriteDraw(chain, chainPosition - Main.screenPosition, frame, lightColor, chainRotation, chainOrigin, Projectile.scale, SpriteEffects.None, 0);

				chainPosition += chainDirection * chainWidth;
			}
			return (false);
		}

		private void FollowOwner(Player owner)
		{
			Vector2 targetPosition = owner.Center;

			this.drawBehindOtherProjectiles = false;

			float rotationOffset = 0;
			if (Projectile.ai[0] != 0)
			{
				rotationOffset = MathHelper.TwoPi / Projectile.ai[0] * Projectile.ai[1];
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

			Projectile.Center = Vector2.Lerp(Projectile.Center, targetPosition, 0.2f);
			Projectile.velocity *= 0.8f;
			Projectile.direction = Projectile.spriteDirection = owner.direction;

			Projectile.rotation = (float)Math.Sin(Projectile.localAI[1] / 15) * 0.2f;
		}

		protected virtual void ShootAt(NPC target) { }
	}
}
