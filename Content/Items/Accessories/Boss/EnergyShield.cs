using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Accessories.Boss
{
    [AutoloadEquip(EquipType.Shield)] // Load the spritesheet you create as a shield for the player when it is equipped.
    public class EnergyShield : ModItem
    {
		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 38;
			Item.value = Item.buyPrice(0,4,0,0);
			Item.rare = ItemRarityID.Expert;
			Item.accessory = true;

		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<EnergyShieldPlayer>().DashAccessoryEquipped = true;
		}

	}

	public class EnergyShieldPlayer : ModPlayer
	{
		// These indicate what direction is what in the timer arrays used
		public const int DashDown = 0;
		public const int DashUp = 1;
		public const int DashRight = 2;
		public const int DashLeft = 3;

		public const int DashCooldown = 50; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
		public const int DashDuration = 35; // Duration of the dash afterimage effect in frames

		// The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
		public const float DashVelocity = 10f;

		// The direction the player has double tapped.  Defaults to -1 for no dash double tap
		public int DashDir = -1;

		// The fields related to the dash accessory
		public bool DashAccessoryEquipped;
		public int DashDelay = 0; // frames remaining till we can dash again
		public int DashTimer = 0; // frames remaining in the dash

		public override void ResetEffects()
		{
			DashAccessoryEquipped = false;

			// ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
			// When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
			// If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
			if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15)
			{
				DashDir = DashDown;
			}
			else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
			{
				DashDir = DashUp;
			}
			else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
			{
				DashDir = DashRight;
			}
			else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
			{
				DashDir = DashLeft;
			}
			else
			{
				DashDir = -1;
			}
		}

		// This is the perfect place to apply dash movement, it's after the vanilla movement code, and before the player's position is modified based on velocity.
		// If they double tapped this frame, they'll move fast this frame
		public override void PreUpdateMovement()
		{
			// if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
			if (CanUseDash() && DashDir != -1 && DashDelay == 0)
			{
				Vector2 newVelocity = Player.velocity;

				switch (DashDir)
				{
					// Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
					case DashUp when Player.velocity.Y > -DashVelocity:
					case DashDown when Player.velocity.Y < DashVelocity:
						{
							// Y-velocity is set here
							// If the direction requested was DashUp, then we adjust the velocity to make the dash appear "faster" due to gravity being immediately in effect
							// This adjustment is roughly 1.3x the intended dash velocity
							float dashDirection = DashDir == DashDown ? 1 : -1.3f;
							newVelocity.Y = dashDirection * DashVelocity;
							break;
						}
					case DashLeft when Player.velocity.X > -DashVelocity:
					case DashRight when Player.velocity.X < DashVelocity:
						{
							// X-velocity is set here
							float dashDirection = DashDir == DashRight ? 1 : -1;
							newVelocity.X = dashDirection * DashVelocity;
							break;
						}
					default:
						return; // not moving fast enough, so don't start our dash
				}

				// start our dash
				DashDelay = DashCooldown;
				DashTimer = DashDuration;
				Player.velocity = newVelocity;

				// Here you'd be able to set an effect that happens when the dash first activates
				// Some examples include:  the larger smoke effect from the Master Ninja Gear and Tabi
			}

			if (DashDelay > 0)
				DashDelay--;

			if (DashTimer > 0)
			{ // dash is active
			  // This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
			  // Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
			  // Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect
				Player.eocDash = DashTimer;
				Player.armorEffectDrawShadowEOCShield = true;

				// count down frames remaining
				DashTimer--;
			}
		}

		private bool CanUseDash()
		{
			return DashAccessoryEquipped
				&& !Player.setSolar // player isn't wearing solar armor
				&& !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
		}
	}

	public class EnergyShieldDash : ModProjectile
    {

		//Spawn Projectile when player presses dash inputs
		//Attactch 

		public override string Texture => "Terraria/Images/Projectile_0";

		public float dashDistance = 225f;
		public float dashDirection = 0f;

		public Vector2 startingPoint = Vector2.Zero;
		public Vector2 endPoint = Vector2.Zero;

		int timer = 0;
        public override void AI()
        {
			if (timer == 0)
            {
				startingPoint = Projectile.Center;
				endPoint = new Vector2(dashDistance, 0f).RotatedBy(dashDirection);
            }

			Projectile.Center = startingPoint + endPoint * easingFunction(easingProgress);
			easingProgress = Math.Clamp(easingProgress + 0.015f, 0f, 1f);

			if (easingFunction(easingProgress) <= 0.9f)
				Main.player[Projectile.owner].Center = Projectile.Center;

			timer++;
        }

		public float easingProgress = 0f;
        public float easingFunction(float progress)
        {
			float toReturn = 0;

			toReturn = 1f - (float)Math.Pow(1f - progress, 5f);

			return toReturn;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/Oblivion");
			Main.spriteBatch.Draw(Blade, Projectile.Center - Main.screenPosition, null, lightColor, 0f, Blade.Size() / 2, 1f, SpriteEffects.None, 0f);

			return false;
        }
    }
}
