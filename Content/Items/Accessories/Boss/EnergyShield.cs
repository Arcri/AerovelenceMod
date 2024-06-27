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
using AerovelenceMod.Core;

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
		public const int DashRight = 2;
		public const int DashLeft = 3;

		public bool secondDash = false;

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

		public int TimeSinceFirstDash = 0; // frames remaining in the dash
		public int TimeSinceSecondDash = 0; // frames remaining in the dash
		public override void ResetEffects()
		{
			DashAccessoryEquipped = false;

			// ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
			// When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
			// If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
			if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
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
			bool canUseFirstDash = (CanUseDash() && DashDir != -1 && DashDelay == 0);
			bool canUseSecondDash = (CanUseDash() && DashDir != -1 && DashDelay != 0 && (TimeSinceFirstDash > 10 && TimeSinceSecondDash > 60));

			// if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
			if (canUseFirstDash || canUseSecondDash)
			{
				bool firstDash = canUseFirstDash;

				Vector2 newVelocity = Player.velocity;

				switch (DashDir)
				{
					// Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
					case DashLeft when Player.velocity.X > -DashVelocity:
					case DashRight when Player.velocity.X < DashVelocity:
						{
							int dash = Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<EnergyShieldDash>(), 0, 0, Player.whoAmI);
							(Main.projectile[dash].ModProjectile as EnergyShieldDash).dashDirection = (DashDir == DashRight ? 0f : 3.14f);
							
							if (firstDash)
								TimeSinceFirstDash = 0;
							else
								TimeSinceSecondDash = 0;
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
			{
				oldPositions.Add(Player.Center);
				//dash is active
				//This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
				//Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
				//Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect

				//Player.eocDash = DashTimer;
				//Player.armorEffectDrawShadowEOCShield = true;

				// count down frames remaining
				DashTimer--;
			}


			if (oldPositions.Count > 5 || (oldPositions.Count > 0 && DashTimer == 0))
				oldPositions.RemoveAt(0);

			if (DashTimer < 20 && oldPositions.Count > 0)
            {
				oldPositions.RemoveAt(0); 
				if (oldPositions.Count > 0)
					oldPositions.RemoveAt(0);
			}
				

			TimeSinceFirstDash++;
			TimeSinceSecondDash++;

		}

		private bool CanUseDash()
		{
			return DashAccessoryEquipped
				&& !Player.setSolar // player isn't wearing solar armor
				&& !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
		}


		//Drawing
		public List<Vector2> oldPositions = new List<Vector2>();
		public override void Load()
		{
			Terraria.On_Main.DrawPlayers_AfterProjectiles += Main_DrawPlayers_AfterProjectiles;
		}

		private void Main_DrawPlayers_AfterProjectiles(Terraria.On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
		{
			if (false)
			{
				Main.spriteBatch.Begin(default, blendState: BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player player = Main.player[i];
					if (player.active && !player.outOfRange && !player.dead && player.GetModPlayer<EnergyShieldPlayer>().oldPositions.Count > 0)
					{
						for (int x = player.GetModPlayer<EnergyShieldPlayer>().oldPositions.Count - 1; x > 0; x--)
						{

							//Drawing the after image would be messed up while zooming, so we have to do an absolute bullshit fucky wucky fix for it
							//This fix is slightly offset for some Zoom values (specifically scale I think), but it is perfect the for the two that matter the most: min and max
							//dm me (Linty) if you want to know what is happening (you dont) or if you have a similar zoom related problem

							float scale = 1f + ((1f - Main.GameZoomTarget) * 0.5f);

							float xZoomOffset = 40f * (Main.GameZoomTarget - 1);
							float yZoomOffset = -21f * (Main.GameZoomTarget - 1);

							Vector2 offset = new Vector2(-110 + xZoomOffset, -171 + yZoomOffset) * (1f + (1f - Main.GameZoomTarget) * 0.5f);

							// If you're curious how I got the specific values above, it was by manually testing values until it was right very fun very happy :) :) :) :)

							Main.spriteBatch.Draw(PlayerTarget.Target, player.GetModPlayer<EnergyShieldPlayer>().oldPositions[x] - Main.screenPosition + offset,
								PlayerTarget.getPlayerTargetSourceRectangle(player.whoAmI), Color.HotPink * ((x / 5f) * 2f), player.fullRotation, Vector2.Zero, scale, 0f, 0f);
						}
					}
				}

				Main.spriteBatch.End();
			}

			orig.Invoke(self);

		}
	}

	public class EnergyShieldDash : ModProjectile
    {

		//Spawn Projectile when player presses dash inputs
		//Attactch 
		public override void SetDefaults()
		{
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 100;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.tileCollide = false;
		}
		public override string Texture => "Terraria/Images/Projectile_0";

		public float dashDistance = 225f;
		public float dashDirection = 0f;
		public float dashMult;


		public Vector2 startingPoint = Vector2.Zero;
		public Vector2 endPoint = Vector2.Zero;
		public Vector2 startingVel = Vector2.Zero;
		public Vector2 endVel = Vector2.Zero;
		int timer = 0;
        public override void AI()
        {
			if (timer == 0)
            {
				startingPoint = Projectile.Center;
				Projectile.velocity.Y = Main.player[Projectile.owner].velocity.Y * 0.25f;
				endPoint = new Vector2(dashDistance, 0f).RotatedBy(dashDirection);

				startingVel = new Vector2(25, 0f).RotatedBy(dashDirection);

			}

			Projectile.velocity.X = Vector2.Lerp(startingVel, startingVel.SafeNormalize(Vector2.UnitX), Easings.easeOutQuad(easingProgress)).X;
			easingProgress = Math.Clamp(easingProgress + 0.04f, 0f, 1f);

			if (Easings.easeOutQuad(easingProgress) <= 0.8f)
            {
				Player player = Main.player[Projectile.owner];
				//player.armorEffectDrawShadow = true;
				//player.armorEffectDrawShadowEOCShield = true;

				Main.player[Projectile.owner].velocity.X = Projectile.velocity.X;
				Projectile.velocity += new Vector2(0, Main.player[Projectile.owner].gravity * 0.25f);
				//Main.player[Projectile.owner].Center = Projectile.Center;
			}
			timer++;
        }

		public float easingProgress = 0f;
        public float easingFunction(float progress)
        {
			
			return Easings.easeOutCirc(progress);
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Player myPlayer = Main.player[Projectile.owner];
			Texture2D Flash = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/muzzle_05");

			float rot = dashDirection == 0f ? -MathHelper.PiOver2 : MathHelper.PiOver2;
			float alpha = 1f - Easings.easeOutCirc(easingProgress);

			Vector2 vec2Scale = new Vector2(1f - ((1f - alpha) * 0.5f), 0.25f + (alpha * 0.5f)) * 0.5f;
			Vector2 origin = new Vector2(Flash.Width / 2f, 0f);

			Vector2 off = dashDirection == 0f ? new Vector2(-120f, 0f) : new Vector2(120f, 0f);

			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
			myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * alpha * 3f);
			myEffect.Parameters["uTime"].SetValue(2);
			myEffect.Parameters["uOpacity"].SetValue(0.4f); //0.6
			myEffect.Parameters["uSaturation"].SetValue(1.2f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(Flash, myPlayer.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f) + off, null, Color.HotPink * alpha, rot, origin, vec2Scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Flash, myPlayer.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f) + off, null, Color.HotPink * alpha, rot, origin, vec2Scale, SpriteEffects.FlipHorizontally, 0f);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
    }
}
