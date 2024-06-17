using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.SkillStrikes;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns
{
    public class AdamantitePulsar : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TitaniumRocketLauncher>();
            // DisplayName.SetDefault("Adamantite Pulsar");
            // Tooltip.SetDefault("Right-click to switch between 2 modes");
        }

        public override void SetDefaults()
        {
            Item.damage = 95;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 82;
            Item.height = 30;
            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AdamantitePulseShot>();
            Item.channel = true;
            Item.shootSpeed = 2f;
            Item.noUseGraphic = true;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-23, 4);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (mode == 0)
            {
                Item.noUseGraphic = true;
            }
            else
            {
                Item.useTime = 10;
                Item.useAnimation = 10 * 3;
                Item.noUseGraphic = true;
            }
        }

        //mode 0 = Single Shot
        //mode 1 = multi Shot
        int mode = 0;
        int currentShot = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.itemAnimationMax = 0;
                player.itemTime = 0;
                player.itemAnimation = 0;

                mode = mode == 0 ? 1 : 0;

                if (mode == 0)
                    CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, 2, 2), Color.Red, "Charge", false, true);
                else
                    CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, 2, 2), Color.Red, "Burst", false, true);

                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_149") with { Pitch = .35f, Volume = 0.45f, MaxInstances = 1 }; 
                SoundEngine.PlaySound(style, player.Center);

                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Research_3") with { Pitch = .15f, Volume = 0.45f, MaxInstances = 1 }; 
                SoundEngine.PlaySound(style3, player.Center);


                return false;
            }

            if (mode == 0)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<AdamantitePulsarHeldProj>(), damage, knockback, Main.myPlayer);
            }
            else if (mode == 1)
            {

                int b = Projectile.NewProjectile(null, position + velocity * (20), velocity * 2.25f, ModContent.ProjectileType<CirclePulse>(), 0, 0, Main.myPlayer);
                Main.projectile[b].rotation = velocity.ToRotation();
                if (Main.projectile[b].ModProjectile is CirclePulse pulseb)
                {
                    pulseb.color = new Color(255, 10, 10);
                    pulseb.size = 0.3f;
                }


                for (int i = 0; i < 5; i++)
                {
                    ColorSparkBehavior extraInfo = new ColorSparkBehavior();
                    Vector2 vel = velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.75f, 1.25f) * 8f;

                    Dust d = Dust.NewDustPerfect(position + velocity * 10, ModContent.DustType<ColorSpark>(), vel, 57 + Main.rand.Next(-5, 5), Color.Crimson, 0.2f + Main.rand.NextFloat(0.1f));
                    extraInfo.gravityIntensity = 0f;
                    d.fadeIn = Main.rand.NextFloat(0.5f, 1f);
                    d.customData = extraInfo;
                }

                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<AdamantitePulsarHeldBurst>(), 0, 0, Main.myPlayer);

                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 16;
                if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                {
                    position += muzzleOffset;
                }
                Projectile.NewProjectile(source, position, velocity * 4, ModContent.ProjectileType<AdamSmallShot>(), (int)(damage * 1.3f), knockback, Main.myPlayer);

                //lol
                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_92") with { Pitch = .80f, PitchVariance = 0.2f, Volume = 0.2f }; 
                SoundEngine.PlaySound(style, player.Center);
                SoundStyle style23 = new SoundStyle("Terraria/Sounds/Custom/dd2_sky_dragons_fury_shot_0") with { Pitch = .2f, PitchVariance = 0.1f, Volume = 0.4f };
                SoundEngine.PlaySound(style23, player.Center);
                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Research_2") with { Volume = .40f, Pitch = .8f, PitchVariance = 0.2f };
                SoundEngine.PlaySound(style3, player.Center);
                SoundStyle style4 = new SoundStyle("Terraria/Sounds/Research_3") with { Volume = .3f, Pitch = .55f, PitchVariance = 0.1f };
                SoundEngine.PlaySound(style4, player.Center);
                SoundStyle style5 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .05f, Pitch = 1f, PitchVariance = 0.25f }; 
                SoundEngine.PlaySound(style5, player.Center);

                currentShot++;
                if (currentShot == 3)
                {
                    delayTimer = 45;
                    currentShot = 0;
                }
            }

            return false;
        }
        public override void HoldItem(Player player)
        {
            //yeah reuseDelay exists but doing it this way is so item speed does not equal more shots 
            delayTimer--;
        }

        int delayTimer;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                return true;
            if (delayTimer > 0)
                return false;
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int itemID = ModContent.ItemType<AdamantitePulsar>();

            if (mode == 0)
            {
                TooltipLine modeDesc = new(Mod, "mode", "Charge - Hold to charge a piercing shot, accuracy increasing the longer you charge")
                {
                    
                    OverrideColor = Color.Red,
                };
                tooltips.Add(modeDesc);

                TooltipLine SSline = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Skill Strike by releasing with perfect timing [i:" + ItemID.FallenStar + "]")
                {
                    OverrideColor = Color.Gold,
                };
                tooltips.Add(SSline);
            }
            else if (mode == 1)
            {
                TooltipLine modeDesc = new(Mod, "mode", "Burst - Fires a burst of three bullets")
                {
                    OverrideColor = Color.Red,
                };
                tooltips.Add(modeDesc);

                TooltipLine SSline = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Third shot Skill Strikes if the other shots hit the same target [i:" + ItemID.FallenStar + "]")
                {
                    OverrideColor = Color.Gold,
                };
                tooltips.Add(SSline);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AdamantiteBar, 15).
                AddIngredient(ItemID.ChlorophyteBar, 4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    public class AdamantitePulsarHeldBurst : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        private bool firstFrame = false;

        private Vector2 currentDirection => Projectile.rotation.ToRotationVector2();

        Player owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.timeLeft = 999999;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() { return false; }

        public override bool? CanCutTiles() => false; 
        

        public override void AI()
        {
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            owner.heldProj = Projectile.whoAmI;

            if (owner.itemTime <= 1)
                Projectile.active = false;

            Projectile.Center = owner.Center;

            if (!firstFrame)
            {
                firstFrame = true;
                Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation();
            }

            if (Projectile.ai[0] < 5)
                offset = Math.Clamp(MathHelper.Lerp(offset, -5, 0.05f), 0, 10);
            else
                offset = Math.Clamp(MathHelper.Lerp(offset, 10, 0.2f), 0, 10);

            glowIntensity = Math.Clamp(MathHelper.Lerp(glowIntensity, -0.5f, 0.1f), 0, 1);

            Projectile.ai[0]++;
        }

        private float offset = 10;
        private float glowIntensity = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulsar").Value;
            Texture2D Glow = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulsarWhiteGlow").Value;


            Vector2 position = (owner.MountedCenter + (currentDirection * offset)) - Main.screenPosition;
            position.Y += owner.gfxOffY;
            position += new Vector2(8, 4 * owner.direction).RotatedBy(Projectile.rotation); //Extra Offset

            float rotation = currentDirection.ToRotation() + (owner.direction == 1 ? 0 : -MathF.PI);
            SpriteEffects SE = (owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            Vector2 origin = (Texture.Size() / 2) + new Vector2(-2 * owner.direction, 0); //Origin more at the trigger

            Color col = Color.Lerp(Color.White, Color.Red, 1 - glowIntensity);

            Main.spriteBatch.Draw(Texture, position, null, lightColor, rotation, origin, 1f, SE, 0.0f);
            Main.spriteBatch.Draw(Glow, position, null, col with { A = 0 } * glowIntensity * 1f, rotation, origin, 1f, SE, 0.0f);
            Main.spriteBatch.Draw(Glow, position, null, col with { A = 0 } * glowIntensity * 1f, rotation, origin, 1f, SE, 0.0f);

            return false;
        }
    }

    public class AdamantitePulsarHeldProj : ModProjectile
    {
        int timer = 0;
        public float offset = 10; 
        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpToStuff = 0;
        public bool hasReachedDestination = false;
        public float skillCritWindow = 10;

        Vector2 reticleLocation = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Adamantite Pulsar");
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.timeLeft = 999999;
            Projectile.width = Projectile.height = 20;
            Projectile.penetrate = -1;


            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

        bool hasLetGo = false;

        float reticleProgress = 0f;
        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Projectile.velocity = Vector2.Zero;
            Player.itemTime = 2; 
            Player.itemAnimation = 2;

            if (Projectile.owner == Main.myPlayer)
                reticleLocation = Main.MouseWorld + Player.velocity;

            if (Player.channel)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (Player.MountedCenter + Player.velocity)).ToRotation();
                    reticleLocation = (Main.MouseWorld);
                }
                direction = Angle.ToRotationVector2();

            } else
            {
                
                if (Projectile.timeLeft > 100)
                {
                    hasLetGo = true;
                    Projectile.timeLeft = 20;

                    if (reticleProgress == 1)
                        Projectile.timeLeft = 30;

                    //Shoot Proj
                    float spread = 15f * (1 - reticleProgress);
                    Vector2 adjustedVel = new Vector2(2, 0).RotatedBy(Angle).RotatedByRandom(MathHelper.ToRadians(spread));

                    Angle = adjustedVel.ToRotation();

                    //float velRot = Angle + (Main.rand.NextFloat(1 - reticleProgress, (1 - reticleProgress) * -1) * 0.5f);
                    //Vector2 vel = new Vector2(2, 0).RotatedBy(velRot);
                    int shot = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + adjustedVel * 10, adjustedVel * 1.5f, ModContent.ProjectileType<AdamantitePulseShot>(), Projectile.damage * 2, Projectile.knockBack, Main.myPlayer);

                    if (Main.projectile[shot].ModProjectile is AdamantitePulseShot aps)
                        aps.big = reticleProgress == 1;

                    #region pulses
                    float vel1 = reticleProgress == 1 ? 2.3f : 2.25f;
                    float vel2 = reticleProgress == 1 ? 2.8f : 2.75f;

                    int pulse1 = Projectile.NewProjectile(null, Projectile.Center + adjustedVel * 20, adjustedVel * vel1, ModContent.ProjectileType<CirclePulse>(), 0, 0, Main.myPlayer);
                    Main.projectile[pulse1].rotation = adjustedVel.ToRotation();
                    if (Main.projectile[pulse1].ModProjectile is CirclePulse funnyapple)
                    {
                        funnyapple.color = new Color(255, 10, 10);
                        funnyapple.size = (reticleProgress == 1 ? 0.65f : 0.55f);
                    }

                    int pulse2 = Projectile.NewProjectile(null, Projectile.Center + adjustedVel * 20, adjustedVel * vel2, ModContent.ProjectileType<CirclePulse>(), 0, 0, Main.myPlayer);
                    Main.projectile[pulse2].rotation = adjustedVel.ToRotation();
                    if (Main.projectile[pulse2].ModProjectile is CirclePulse funnyapple2)
                    {
                        funnyapple2.color = new Color(255, 10, 10);
                        funnyapple2.size = (reticleProgress == 1 ? 0.35f : 0.25f);
                    }
                    #endregion

                    for (int i = 0; i < 12 + (reticleProgress == 1 ? 4 : 0); i++)
                    {
                        float dustScale = 0.25f + Main.rand.NextFloat(0.15f) + (reticleProgress == 1 ? 0.1f : 0f);

                        ColorSparkBehavior extraInfo = new ColorSparkBehavior();
                        Vector2 vel = adjustedVel.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.75f, 1.25f) * 8f;

                        Dust d = Dust.NewDustPerfect(Projectile.Center + adjustedVel * 10, ModContent.DustType<ColorSpark>(), vel, 51 + Main.rand.Next(-5, 6), Color.Red, dustScale);
                        extraInfo.gravityIntensity = 0f;
                        d.fadeIn = Main.rand.NextFloat(0.5f, 1f);
                        d.customData = extraInfo;
                    }

                    if (skillCritWindow > 0 && reticleProgress == 1)
                    {
                        SkillStrikeUtil.setSkillStrike(Main.projectile[shot], 1.3f, 2);
                    }

                    SoundStyle style23 = new SoundStyle("Terraria/Sounds/Custom/dd2_sky_dragons_fury_shot_0") with { Pitch = .10f, PitchVariance = 0.4f, Volume = 0.4f };
                    SoundEngine.PlaySound(style23, Projectile.Center);


                    SoundStyle style32;
                    if (reticleProgress == 1)
                        style32 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_fire") with { Volume = 0.2f, Pitch = -0.33f, MaxInstances = -1, PitchVariance = 0.15f };
                    else
                        style32 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_fire") with { Volume = 0.2f, Pitch = 0f, MaxInstances = -1, PitchVariance = 0.1f };
                    SoundEngine.PlaySound(style32, Projectile.Center);

                    SoundStyle style3 = new SoundStyle("Terraria/Sounds/Research_3") with { Volume = .28f, Pitch = .6f, PitchVariance = 0.2f };
                    SoundEngine.PlaySound(style3, Projectile.Center);

                    offset = 2;

                    if (reticleProgress == 1)
                    {
                        SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .13f, Pitch = .15f, PitchVariance = 0.1f }; 
                        SoundEngine.PlaySound(style, Projectile.Center);

                        Player.GetModPlayer<AeroPlayer>().ScreenShakePower = 18;
                        Player.velocity += Angle.ToRotationVector2() * -5.5f;

                        offset = -7;
                    }

                    glowAmount = 1f;
                }

            }

            offset = Math.Clamp(MathHelper.Lerp(offset, 15f, 0.1f), -10, 10);

            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            direction = Angle.ToRotationVector2();
            Projectile.Center = Player.MountedCenter + (direction * offset);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);

            if (Player.channel)
                reticleProgress = Math.Clamp(reticleProgress + 0.02f, 0f, 1f);
            else if (hasLetGo && reticleProgress != 1)
                reticleAlpha = Math.Clamp(MathHelper.Lerp(reticleAlpha, -1, 0.05f), 0, 1f);

            if (reticleProgress == 1)
                skillCritWindow--;

            if (skillCritWindow > 0 && reticleProgress == 1)
                goldPulseAmount = 1;


            if (hasLetGo && Projectile.timeLeft < (reticleProgress == 1 ? 12 : 8))
            {
                gunOpacity = Math.Clamp(MathHelper.Lerp(gunOpacity, -0.65f, 0.06f), 0, 1);
                reticleAlpha = Math.Clamp(MathHelper.Lerp(reticleAlpha, -1f, 0.12f), 0, 1); 
            }

            goldPulseAmount = Math.Clamp(MathHelper.Lerp(goldPulseAmount, -0.5f, 0.04f), 0, 1);
            glowAmount = Math.Clamp(MathHelper.Lerp(glowAmount, -0.5f, 0.06f), 0, 1);

            timer++;
        }

        float glowAmount = 0f;
        float goldPulseAmount = 0f;
        float reticleAlpha = 1f;
        float gunOpacity = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D Glow = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulsarWhiteGlow").Value;

            #region Arc
            /*
            Texture2D Arc = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Medusa_Gray").Value;
            float opacity = Math.Clamp(reticleProgress * 1f, 0, 1) * 0.5f;
            Vector2 scale = new Vector2(2f, 4f - (3.2f * reticleProgress));
            Vector2 arcPos = Projectile.Center + Angle.ToRotationVector2() * 20f - Main.screenPosition;
            Vector2 arcOrigin = new Vector2(0f, Arc.Height / 2);

            Main.spriteBatch.Draw(Arc, arcPos, null, Color.Red with { A = 0 } * opacity, 
                direction.ToRotation(), arcOrigin, scale, SpriteEffects.None, 0.0f);

            Main.spriteBatch.Draw(Arc, arcPos, null, Color.Red with { A = 0 } * opacity * 0.75f, 
                direction.ToRotation(), arcOrigin, scale * 0.85f, SpriteEffects.None, 0.0f);

            Main.spriteBatch.Draw(Arc, arcPos, null, Color.White with { A = 0 } * opacity * 0.5f, 
                direction.ToRotation(), arcOrigin, scale * 0.7f, SpriteEffects.None, 0.0f);
            */
            #endregion


            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.Center - (0.5f * (direction * -17)) + new Vector2(0f, Player.gfxOffY) - Main.screenPosition).Floor();

            Vector2 newOffset = new Vector2(0, 3 * Player.direction).RotatedBy(Angle);

            SpriteEffects myEffect = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.spriteBatch.Draw(texture, position + newOffset, null, lightColor * gunOpacity, direction.ToRotation(), origin, Projectile.scale, myEffect, 0.0f);

            Color col1 = Color.Lerp(Color.White, Color.Gold, goldPulseAmount);

            Main.spriteBatch.Draw(Glow, position + newOffset, null, col1 with { A = 0 } * glowAmount * gunOpacity, direction.ToRotation(), origin, Projectile.scale, myEffect, 0.0f);
            Main.spriteBatch.Draw(Glow, position + newOffset, null, col1 with { A = 0 } * glowAmount * gunOpacity, direction.ToRotation(), origin, Projectile.scale, myEffect, 0.0f);

            Texture2D OuterL = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/RedOuterL").Value;
            Texture2D InnerL = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/WhiteInnerL").Value;

            float progress = Easings.easeInOutQuad(reticleProgress);
            float extraAngle = MathHelper.Lerp(MathF.PI * -0.25f, 2f * MathF.PI, progress);
            float opactity = MathHelper.Lerp(0f, 1f, Easings.easeInQuad(reticleProgress * 1.15f)) * reticleAlpha;
            float scale = reticleProgress * 0.9f;

            Color col = Color.Lerp(Color.Red, Color.Gold, goldPulseAmount);

            Main.spriteBatch.Draw(OuterL, reticleLocation - Main.screenPosition + new Vector2(0, 100 * (1 - reticleProgress) + 10).RotatedBy(Angle + extraAngle), null, Color.White with { A = 0 } * (opactity * 0.75f), Angle - MathHelper.PiOver4, OuterL.Size() / 2, scale, SpriteEffects.None, 0.0f); ;
            Main.spriteBatch.Draw(OuterL, reticleLocation - Main.screenPosition + new Vector2(0, -100 * (1 - reticleProgress) - 10).RotatedBy(Angle + extraAngle), null, Color.White with { A = 0 } * (opactity * 0.75f), Angle + MathHelper.PiOver4 + MathHelper.PiOver2, OuterL.Size() / 2, scale, SpriteEffects.None, 0.0f); ;

            Main.spriteBatch.Draw(InnerL, reticleLocation - Main.screenPosition + new Vector2(0, 100 * (1 - reticleProgress) + 10).RotatedBy(Angle + extraAngle), null, col * (opactity * 0.75f), Angle - MathHelper.PiOver4, OuterL.Size() / 2, scale, SpriteEffects.None, 0.0f); ;
            Main.spriteBatch.Draw(InnerL, reticleLocation - Main.screenPosition + new Vector2(0, -100 * (1 - reticleProgress) - 10).RotatedBy(Angle + extraAngle), null, col * (opactity * 0.75f), Angle + MathHelper.PiOver4 + MathHelper.PiOver2, OuterL.Size() / 2, scale, SpriteEffects.None, 0.0f); ;
            

            return false;
        }
    }
 
}