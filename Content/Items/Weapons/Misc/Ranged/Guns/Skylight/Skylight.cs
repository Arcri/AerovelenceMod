using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns.Skylight
{
    public class Skylight : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 95;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 0;

            Item.width = 46;
            Item.height = 28;
            Item.useTime = 10;
            Item.useAnimation = 30;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;

            Item.shoot = ModContent.ProjectileType<SkylightElectricShot>();
            Item.shootSpeed = 2f;

            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<SkylightHeldProjectile>(), damage, knockback, Main.myPlayer);

            return false;
        }
        
    }

    public class SkylightHeldProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        int burstTimer = 0;
        float recoilTimer = 20;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Adamantite Pulsar");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 999999;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        Vector2 offset = Vector2.Zero;

        float recoilAngle = 0f;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (timer == 0)
                Projectile.velocity = player.DirectionTo(Main.MouseWorld);


            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (!player.channel)
            {
                Projectile.active = false;
                return; //dont want shite to still run
            }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.rotation += recoilAngle * -player.direction;

            //Projectile.velocity = player.DirectionTo(Main.MouseWorld);

            Projectile.timeLeft = 2;

            offset = Vector2.Lerp(offset, new Vector2(10, 0), 0.3f);    //new Vector2(10, 0);
            Projectile.Center = player.MountedCenter + offset.RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi);
            player.ChangeDir(Projectile.direction);


            if (burstTimer % 19 == 0 && burstTimer > 0 && burstTimer < 85)
            {
                Projectile.velocity = player.DirectionTo(Main.MouseWorld);
                Projectile.velocity = Vector2.Normalize(Projectile.velocity);

                Vector2 rotatedVelocity = Projectile.velocity.RotatedByRandom(-0.07f) * 25f;

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, rotatedVelocity, ModContent.ProjectileType<SkylightElectricShot>(), Projectile.damage, 0, player.whoAmI);

                Projectile.velocity = rotatedVelocity.SafeNormalize(Vector2.UnitX);


                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/lightning_flash_01") with { Pitch = 1f, PitchVariance = 0.2f, Volume = 0.4f };
                SoundEngine.PlaySound(style, Projectile.Center);

                for (int m = 0; m < 5; m++) // m < 8
                {

                    Color col = new Color(0, 155, 255);
                    //Color.DodgerBlue
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 25, ModContent.DustType<MuraLineDust>(),
                        Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.12f, 0.12f)) * Main.rand.NextFloat(1, 5) * 2, newColor: col, Scale: 0.3f + Main.rand.NextFloat(0, 0.2f));
                    d.alpha = 2; //Dust color will fade out
                }

                for (int m = 0; m < 6; m++) // m < 9
                {
                    float rotAdd = (Main.rand.NextBool() ? 0.5f : -0.5f) + Main.rand.NextFloat(-0.22f, 0.22f);

                    Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 25, ModContent.DustType<GlowStrong>(),
                        Projectile.velocity.RotatedBy(rotAdd) * Main.rand.NextFloat(2, 5) * 1.5f, newColor: Color.DodgerBlue, Scale: 0.15f + Main.rand.NextFloat(0, 0.2f));
                    d.alpha = 2; //Dust color will fade out
                }

                glowVal = 1;
                offset = new Vector2(2, 0);
                recoilTimer = 0;
            }

            if (burstTimer == 85 || burstTimer == 90 || burstTimer == 95 || burstTimer == 100) {

                Projectile.velocity = player.DirectionTo(Main.MouseWorld);
                Projectile.velocity = Vector2.Normalize(Projectile.velocity);

                Vector2 rotatedVelocity = Projectile.velocity.RotatedByRandom(-0.15f) * 30f;

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, rotatedVelocity, ModContent.ProjectileType<SkylightElectricShot>(), Projectile.damage, 0, player.whoAmI);

                Projectile.velocity = rotatedVelocity.SafeNormalize(Vector2.UnitX);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/lightning_flash_01") with { Pitch = 1f, PitchVariance = 0.2f, Volume = 0.4f };
                SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/LightningMothSFX_1") with { Pitch = 1f, MaxInstances = 2, Volume = 0.3f }; 
                SoundEngine.PlaySound(style2, Projectile.Center);
                SoundEngine.PlaySound(style2, Projectile.Center);

                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Item_106") with { Volume = .2f, Pitch = .82f, }; SoundEngine.PlaySound(style3, Projectile.Center);

                for (int m = 0; m < 6; m++) //6
                {

                    Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 25, ModContent.DustType<GlowStrong>(),
                        Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.12f, 0.12f)) * Main.rand.NextFloat(1, 5) * 2, newColor: Color.DodgerBlue, Scale: 0.3f + Main.rand.NextFloat(0, 0.2f));
                    d.alpha = 2; //Dust color will fade out
                }

                for (int m = 0; m < 4; m++) 
                {
                    float rotAdd = (Main.rand.NextBool() ? 0.5f : -0.5f) + Main.rand.NextFloat(-0.22f, 0.22f);

                    Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 25, ModContent.DustType<GlowStrong>(),
                        Projectile.velocity.RotatedBy(rotAdd) * Main.rand.NextFloat(2, 5) * 1.5f, newColor: Color.DodgerBlue, Scale: 0.15f + Main.rand.NextFloat(0, 0.2f));
                    d.alpha = 2; //Dust color will fade out
                }

                glowVal = 1;
                recoilTimer = 0;
                offset = new Vector2(2, 0);


                if (burstTimer == 100) burstTimer = -1;
            }

            glowVal = Math.Clamp(MathHelper.Lerp(glowVal, -0.5f, 0.04f), 0, 1);

            //Up
            if (recoilTimer < 6)
            {
                float easingProgress = Easings.easeOutQuint(recoilTimer / 3f);
                recoilAngle = MathHelper.Lerp(recoilAngle, 0.075f, easingProgress);
            }
            else if (recoilTimer < 20) //Down
            {
                float easingProgress = ((recoilTimer - 6f) / 10f);
                recoilAngle = MathHelper.Lerp(recoilAngle, 0f, easingProgress);
            }

            recoilTimer++;
            burstTimer++;
            timer++;
        }

        float glowVal = 0;
        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Gun = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/Skylight/Skylight");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/Skylight/SkylightWhite");

            Texture2D GlowFuzz = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/Skylight/SkylightWhiteGlow");


            SpriteEffects fx = Main.player[Projectile.owner].direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(Gun, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY), null, lightColor, Projectile.rotation - MathHelper.PiOver2, Gun.Size() / 2, Projectile.scale, fx, 0f);

            Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY), null, Color.SkyBlue with { A = 0 } * glowVal, Projectile.rotation - MathHelper.PiOver2, Glow.Size() / 2, Projectile.scale, fx, 0f);

            //Epic shader time that nobody will never notice but am keeping anyway wahoo
            if (glowVal > 0)
            {
                if (myEffect == null)
                    myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

                myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/MapOdd").Value);
                myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/ThunderGrad").Value);
                myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

                myEffect.Parameters["flowSpeed"].SetValue(0.3f);
                myEffect.Parameters["vignetteSize"].SetValue(0f);
                myEffect.Parameters["vignetteBlend"].SetValue(1f);
                myEffect.Parameters["distortStrength"].SetValue(0.02f);
                myEffect.Parameters["xOffset"].SetValue(0.3f);
                myEffect.Parameters["uTime"].SetValue(timer * 0.01f);
                myEffect.Parameters["colorIntensity"].SetValue(glowVal);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
                myEffect.CurrentTechnique.Passes[0].Apply();


                Main.spriteBatch.Draw(GlowFuzz, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY), null, Color.SkyBlue * glowVal, Projectile.rotation - MathHelper.PiOver2, GlowFuzz.Size() / 2, Projectile.scale * 1f, fx, 0f);

                Main.spriteBatch.Draw(GlowFuzz, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY), null, Color.SkyBlue * glowVal, Projectile.rotation - MathHelper.PiOver2, GlowFuzz.Size() / 2, Projectile.scale * 1f, fx, 0f);


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                //Reset twice because tmod 1.4.4 is broke
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            }

            return false;
        }
    }

    public class SkylightElectricShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 250;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        bool hasHit = false;

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        int timer = 0;

        float alpha = 0f;
        float scale = 0f;
        public override void AI()
        {
            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTextureButBlack").Value;
            trail1.trailColor = Color.White * 0.7f * alpha;
            trail1.trailPointLimit = 300;
            trail1.trailWidth = (int)(11f * scale);
            trail1.trailMaxLength = 600;
            trail1.timesToDraw = 2;
            trail1.pinch = false;
            trail1.pinchAmount = 0.1f;


            trail1.trailTime = timer * 0.02f;
            trail1.trailRot = Projectile.velocity.ToRotation();

            //trail2.trailPos = Projectile.Center; ////
            //trail2.TrailLogic();

            trail1.trailPos = Projectile.Center + Projectile.velocity; 
            trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5Loop").Value;
            trail2.trailColor = Color.DeepSkyBlue * alpha;
            trail2.trailPointLimit = 300;
            trail2.trailWidth = (int)(40 * scale);
            trail2.trailMaxLength = 600;
            trail2.timesToDraw = 2;
            trail2.pinch = false;
            trail2.pinchAmount = 0.4f;


            //trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/LoopingThunderGrad").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = timer * 0.03f;

            trail2.trailTime = timer * 0.05f;
            trail2.trailRot = Projectile.velocity.ToRotation();

            //trail2.trailPos = Projectile.Center; ////
            //trail2.TrailLogic(); ////

            trail2.trailPos = Projectile.Center + Projectile.velocity;
            trail2.TrailLogic();

            if (hasHit)
            {
                scale = Math.Clamp(MathHelper.Lerp(scale, -0.3f, 0.06f), 0f, 1f);
                alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.3f, 0.12f), 0f, 1f);

                Projectile.velocity *= 0.9f;
            }
            else
            {
                scale = Math.Clamp(MathHelper.Lerp(scale, 1.3f, 0.075f), 0f, 1f); //1.3, 0.075
                alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.35f, 0.2f), 0f, 1f); //
            }


            if ((timer % 4 == 0) && Main.rand.NextBool() && !hasHit && timer != 0)
            {
                int a = Dust.NewDust(Projectile.position, Projectile.width * 2, Projectile.height * 2, ModContent.DustType<GlowStrong>(), newColor: Color.DeepSkyBlue * 0.5f, Scale: Main.rand.NextFloat(0.25f, 0.5f));
                Main.dust[a].velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * -Main.rand.NextFloat(2, 5);


                //int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<MuraLineDust>(), newColor: Color.DeepSkyBlue);
                //Main.dust[d].velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * -Main.rand.NextFloat(2, 5);
                //Main.dust[d].alpha = 10;
            }

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;
            Texture2D DiamondGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/DiamondGlow");




            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -3, null, Color.DeepSkyBlue * alpha * 0.26f, Projectile.velocity.ToRotation() - MathHelper.PiOver2, Ball.Size() / 2, Projectile.scale * 0.25f * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -3, null, Color.SkyBlue * alpha * 0.35f, Projectile.velocity.ToRotation() - MathHelper.PiOver2, Ball.Size() / 2, Projectile.scale * 0.18f * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -3, null, Color.White * alpha * 0.2f, Projectile.velocity.ToRotation() - MathHelper.PiOver2, Ball.Size() / 2, Projectile.scale * 0.12f * scale, SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(DiamondGlow, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -3, null, Color.DeepSkyBlue * alpha, Projectile.velocity.ToRotation() - MathHelper.PiOver2, DiamondGlow.Size() / 2, new Vector2(Projectile.scale * 0.6f, Projectile.scale) * 0.4f * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(DiamondGlow, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -3, null, Color.SkyBlue * alpha, Projectile.velocity.ToRotation() - MathHelper.PiOver2, DiamondGlow.Size() / 2, new Vector2(Projectile.scale * 0.5f, Projectile.scale) * 0.3f * scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(DiamondGlow, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -3, null, Color.White * alpha, Projectile.velocity.ToRotation() - MathHelper.PiOver2, DiamondGlow.Size() / 2, new Vector2(Projectile.scale * 0.3f, Projectile.scale) * 0.35f * scale, SpriteEffects.None, 0f);



            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            Projectile.velocity *= 0.8f;

            Projectile.timeLeft = 60;
            hasHit = true;

            //Do dust stuff

            for (int m = 0; m < 3 + Main.rand.Next(0, 2); m++)
            {
                Color col = new Color(0, 155, 255);
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MuraLineDust>(),
                    Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(0.75f, 3.5f) * 0.17f, newColor: col);
                d.fadeIn = 10f;
                d.alpha = 13;
            }

            /*
            for (int i = 0; i < 5 + Main.rand.Next(0, 2); i++)
            {

                Dust a = Dust.NewDustPerfect(Projectile.Center , ModContent.DustType<GlowStrong>(),
                    Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-2, 2)) * Main.rand.NextFloat(1, 3) * 0.7f,
                    newColor: Color.DodgerBlue, Scale: Main.rand.NextFloat(0.2f, 0.25f));

                a.fadeIn = 2;
            }
            */

            SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = 0.15f, Pitch = 1f, MaxInstances = 3, PitchVariance = 0.2f };
            SoundEngine.PlaySound(style2, target.Center);
        }

        public override bool? CanDamage()
        {
            return !hasHit;
        }
    }

    public class SkylightThunderStrike : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;

            Projectile.extraUpdates = 2;
        }

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        int timer = 0;

        bool chase = false;

        int turnDir = 1;
        int turnTime = 10;

        Vector2 initialVel = Vector2.Zero;

        public override void AI()
        {
            if (timer == 0)
                initialVel = Projectile.velocity;

            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTextureButBlack").Value;
            trail1.trailColor = Color.White * 0.7f;
            trail1.trailPointLimit = 300;
            trail1.trailWidth = 22 * 2;
            trail1.trailMaxLength = 4000;
            trail1.timesToDraw = 2;
            trail1.pinch = false;


            trail1.trailTime = timer * 0.01f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center;
            trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5Loop").Value;
            trail2.trailColor = Color.DeepSkyBlue;
            trail2.trailPointLimit = 300;
            trail2.trailWidth = 80 * 2;
            trail2.trailMaxLength = 4000;
            trail2.timesToDraw = 2;
            trail2.pinch = false;

            //trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/LoopingThunderGrad").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = timer * 0.02f;

            trail2.trailTime = timer * 0.02f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center;
            trail2.TrailLogic();

            //Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 15;

            if (timer % turnTime == 0)
            {
                Projectile.velocity = initialVel.RotatedBy(Main.rand.NextFloat(0.2f) * turnDir);

                turnDir *= -1;
                turnTime = 10;
            }

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            trail1.trailTime = (float)Main.timeForVisualEffects * 0.01f;


            trail2.gradientTime = (float)Main.timeForVisualEffects * 0.02f;
            trail2.trailTime = (float)Main.timeForVisualEffects * 0.03f;

            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/bigCircle2").Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Ball.Size() / 2, Projectile.scale * 0.1f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }

    }

    public class SkylightVFX : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;

            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.timeLeft = 800;
            Projectile.scale = 1f;

        }

        public override bool? CanDamage()
        {
            return false;
        }

        int timer = 0;
        float scale = 0;
        float alpha = 1;
        public override void AI()
        {
            if (timer < 30)
            {
                float easingProgress = Easings.easeInOutSine((float)timer / 60f);


                scale = MathHelper.Lerp(scale, 0.65f, easingProgress);

            }
            else if (timer >= 30)
            {
                scale = scale + 0.02f;
                alpha -= 0.05f;
            }
   

            /*
            float ts = 0.4f;
            if (timer < 40)
                scale = Math.Clamp(MathHelper.Lerp(scale, 2f * ts, (float)(timer * 0.02f)), 0, 1.5f * ts);
            else
                scale = Math.Clamp(MathHelper.Lerp(scale, -0.5f, 0.05f), 0, 1.5f * ts);
            */
            Projectile.rotation += 0.12f;


            Projectile.timeLeft = 2;

            if (timer > 180)
                alpha -= 0.02f;

            if (alpha <= 0)
                Projectile.active = false;

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Orbs/impact_2fade2").Value;
            //flare 1


            Texture2D Flare2 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/ElectricRadialEffect").Value;

            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/GrayGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
            myEffect.Parameters["distortStrength"].SetValue(0.02f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            myEffect.Parameters["colorIntensity"].SetValue(alpha * 2);


            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.Black with { A = 0 }, Projectile.rotation, Flare.Size() / 2, scale * 1f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.Black with { A = 0 }, Projectile.rotation * -1, Flare.Size() / 2, scale * 1.25f, SpriteEffects.None, 0f);            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.Black with { A = 0 }, Projectile.rotation * -1, Flare.Size() / 2, scale * 1.25f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.Black * 0.15f, Projectile.rotation, Ball.Size() / 2, scale, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Ball.Size() / 2, scale * 0.25f, SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Flare.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Flare.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * -1, Flare.Size() / 2, scale * 0.7f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Flare2, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation + 1, Flare2.Size() / 2, scale * 1.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare2, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * -1 - 1, Flare2.Size() / 2, scale * 1.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}