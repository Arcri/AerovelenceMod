using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories.Boss
{
    [AutoloadEquip(EquipType.Shield)]
    public class EnergyShield : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 38;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Expert;
            Item.accessory = true;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EnergyShieldPlayer>().EnergyShieldEquipped = true;
        }

    }

    public class EnergyShieldPlayer : ModPlayer
    {
        // These indicate what direction is what in the timer arrays used
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public int DashCooldown = 50; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public int DashDuration = 35; // Duration of the dash afterimage effect in frames

        // We can only dash if our vel is over the threshold
        public float VelocityThreshold = 10f;

        // The direction the player has double tapped.  Defaults to -1 for no dash double tap
        public int DashDir = -1;

        public bool EnergyShieldEquipped;
        public int DashDelay = 0; // frames remaining till we can dash again
        public int DashTimer = 0; // frames remaining in the dash

        public int TimeSinceFirstDash = 0; // frames remaining in the dash
        public int TimeSinceSecondDash = 0; // frames remaining in the dash
        public override void ResetEffects()
        {
            EnergyShieldEquipped = false;

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
            bool canUseSecondDash = (CanUseDash() && DashDir != -1 && DashDelay != 0 && (TimeSinceFirstDash > 10 && TimeSinceSecondDash > 60)); //10 60

            //Main.NewText(DashDelay);

            // if the player can use a dash, has double tapped in a direction, and both dashes are not currently on cooldown
            if (canUseFirstDash || canUseSecondDash)
            {
                bool firstDash = canUseFirstDash;

                switch (DashDir)
                {
                    // Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
                    case DashLeft when Player.velocity.X > -VelocityThreshold:
                    case DashRight when Player.velocity.X < VelocityThreshold:
                        {
                            int dash = Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<EnergyShieldDash>(), 0, 0, Player.whoAmI);
                            (Main.projectile[dash].ModProjectile as EnergyShieldDash).dashDirection = (DashDir == DashRight ? 0f : 3.14f);
                            (Main.projectile[dash].ModProjectile as EnergyShieldDash).isPink = !canUseFirstDash;

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
            }

            if (DashDelay > 0)
                DashDelay--;

            if (DashTimer > 0)
                DashTimer--;

            TimeSinceFirstDash++;
            TimeSinceSecondDash++;
        }

        private bool CanUseDash()
        {
            return EnergyShieldEquipped
                && !Player.setSolar // player isn't wearing solar armor
                && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
        }



        #region AfterImage

        public int AfterImageTrailCount = 0;
        public bool isPink = false;
        public bool dashActive = false;
        public override void DrawPlayer(Camera camera)
        {

            if (!dashActive)
                return;

            Vector2 playerPosition = Player.position + new Vector2(0f, Player.gfxOffY);

            //Border
            for (int i = 0; i < 4; i++)
            {
                float dist = 3f; //4

                Vector2 offset = new Vector2(dist, 0f).RotatedBy((MathHelper.TwoPi / 4f) * i);
                Vector2 offsetDrawPos = playerPosition + offset.RotatedBy(Main.timeForVisualEffects * 0.1f);

                CustomShadowColor = isPink ? Color.DeepPink : Color.SkyBlue;

                //if (i == 0)
                //    CustomShadowColor = Color.DeepSkyBlue;
                //if (i == 1)
                //    CustomShadowColor = Color.DeepSkyBlue;
                //if (i == 2)
                //    CustomShadowColor = Color.DeepPink;
                //if (i == 3)
                //    CustomShadowColor = Color.DeepPink;

                CustomShadowColor = CustomShadowColor with { A = 0 };

                Main.PlayerRenderer.DrawPlayer(camera, Player, offsetDrawPos, Player.fullRotation, Player.fullRotationOrigin, 0.15f);
            }
            CustomShadowColor = Color.White;

            int totalShadows = Math.Min(Player.availableAdvancedShadowsCount, AfterImageTrailCount);

            totalShadows = Math.Clamp(totalShadows, 0, 100);

            int skip = 1;
            for (int i = totalShadows - totalShadows % skip; i > 0; i -= skip)
            {
                EntityShadowInfo advancedShadow = Player.GetAdvancedShadow(i);
                float shadow = Utils.Remap((float)i / totalShadows, 0, 1, 0.15f, 0.5f, clamped: true);

                CustomShadowColor = (isPink ? Color.Lerp(Color.DeepPink, Color.SkyBlue, shadow) : Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.5f)) with { A = 0 };

                Main.PlayerRenderer.DrawPlayer(camera, Player, advancedShadow.Position, advancedShadow.Rotation, advancedShadow.Origin, shadow, 1f);
            }
            CustomShadowColor = Color.White; // Reset CustomShadowColor so it doesn't affect normal drawing or other clones.
        }

        public Color CustomShadowColor = Color.White;
        public override void TransformDrawData(ref PlayerDrawSet drawInfo)
        {
            // Check to only affect specific clones
            if (CustomShadowColor == Color.White)
            {
                return;
            }

            // Tint the clones with CustomShadowColor by modifying the draw color of every DrawData in drawInfo.DrawDataCache.
            for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
            {
                DrawData value = drawInfo.DrawDataCache[i];
                // Multiply the colors to tint it rather than assign it directly since DrawData.color will likely already have some non-white color.
                value.color = value.color.MultiplyRGBA(CustomShadowColor);
                drawInfo.DrawDataCache[i] = value;
            }
        }
        #endregion
    }


    public class EnergyShieldDash : ModProjectile
    {
        //Spawn Projectile when player presses dash inputs
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

        public float dashDirection = 0f;

        public bool isPink = false;

        Vector2 startingVel = Vector2.Zero;

        float easingProgress = 0f;
        int timer = 0;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            //isPink = false;

            if (timer == 0)
            {
                Projectile.velocity.Y = player.velocity.Y * 0.25f;

                startingVel = new Vector2(25f, 0f).RotatedBy(dashDirection); //25

                float totalDashVol = 0.65f;
                float pitch = isPink ? 0.2f : 0f;

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/Thunder/EnergyDash1") with { Volume = 0.25f * totalDashVol, Pitch = pitch, PitchVariance = 0.07f };
                SoundEngine.PlaySound(style, player.Center);

                SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/GloogaSlide") with { Volume = 0.1f * totalDashVol, Pitch = 0.35f + pitch, PitchVariance = 0.05f,  MaxInstances = -1 }, player.Center);
            }

            Projectile.velocity.X = Vector2.Lerp(startingVel, startingVel.SafeNormalize(Vector2.UnitX), Easings.easeOutQuad(easingProgress)).X;
            easingProgress = Math.Clamp(easingProgress + 0.03f, 0f, 1f); //.04

            if (Easings.easeOutQuad(easingProgress) <= 0.8f)
            {
                if (Easings.easeOutQuad(easingProgress) <= 0.35f) //.35
                {
                    player.GetModPlayer<EnergyShieldPlayer>().dashActive = true;
                    player.GetModPlayer<EnergyShieldPlayer>().AfterImageTrailCount = timer;
                    player.GetModPlayer<EnergyShieldPlayer>().isPink = isPink;
                }
                else if (Easings.easeOutQuad(easingProgress) <= 0.5)
                {
                    int AITC = player.GetModPlayer<EnergyShieldPlayer>().AfterImageTrailCount;
                    player.GetModPlayer<EnergyShieldPlayer>().AfterImageTrailCount = Math.Clamp(AITC - 1, 0, 100);
                }
                else
                {
                    player.GetModPlayer<EnergyShieldPlayer>().dashActive = false;
                    player.GetModPlayer<EnergyShieldPlayer>().AfterImageTrailCount = 0;
                }


                player.velocity.X = Projectile.velocity.X;
                Projectile.velocity += new Vector2(0, Main.player[Projectile.owner].gravity * 0.25f);


                Color dustCol = isPink ? Color.Lerp(Color.DeepPink, Color.HotPink, 0.35f) : Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.35f);

                //if (timer % 4 != 0)
                //    dustCol = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.35f);

                if (Easings.easeOutQuad(easingProgress) <= 0.65f)
                {
                    Dust p = Dust.NewDustPerfect(player.Center + new Vector2(-player.velocity.X * 0.75f, Main.rand.NextFloat(-20f, 20f)), ModContent.DustType<WindLine>(),
                        player.velocity.SafeNormalize(Vector2.UnitX) * -6f, newColor: dustCol, Scale: 4f);

                    WindLineBehavior wlb = new WindLineBehavior(VelFadePower: 0.95f, TimeToStartShrink: 0, ShrinkYScalePower: 0.7f, 0.3f, 0.55f, true); //0.7 yfade
                    wlb.drawWhiteCore = true;
                    p.customData = wlb;
                }

                //Spawn Circle Pulse if it is first frame (after 
                if (timer == 0)
                {
                    Dust d2 = Dust.NewDustPerfect(player.Center, ModContent.DustType<CirclePulse>(), player.velocity.SafeNormalize(Vector2.UnitX) * 2f,
                        newColor: dustCol * 1f);
                    d2.scale = 0.1f;
                    CirclePulseBehavior b2 = new CirclePulseBehavior(0.5f, true, 1, 0.25f, 0.5f);
                    b2.drawLayer = RenderLayer.UnderProjectiles;
                    d2.customData = b2;
                }
            }
            else
            {
                Projectile.active = false;
            }

            timer++;
        }

    }
}
