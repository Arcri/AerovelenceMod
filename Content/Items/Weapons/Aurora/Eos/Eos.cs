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
using Terraria.Audio;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.Projectiles;
using System;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.Aurora.Eos
{
    public class Eos : ModItem
    {
        bool tick = false;

        int combo = 1;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Example Swing Sword");
            // Tooltip.SetDefault("Debug/Example Item");
        }
        public override void SetDefaults()
        {
            Item.knockBack = 2f;
            Item.crit = 14;
            Item.damage = 92;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Pink;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;

            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<EosSwing>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            tick = !tick;
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));

            //medium medium fast fast fast SPIN

            float shaFast = 120;
            float eaaFast = 0.065f;
            int ftssFast = 0;
            float taeFast = 0;
            float spFast = 0.05f;

            float shaMedium = 190;
            float eaaMedium = 0.015f;
            int ftssMedium = 3;
            float taeMedium = 0;
            float spMedium = 0.01f;

            float shaLong = 360 + 180;
            float eaaLong = 0.01f;
            int ftssLong = 10;
            float taeLong = 7;
            float spLong = 0.01f;

            Main.projectile[p].localNPCHitCooldown = -1;

            if (Main.projectile[p].ModProjectile is EosSwing swing)
            {
                
                if (combo == 1 || combo == 2)
                {
                    //taeMedium *= combo == 2 ? 0.5f : 1f;
                    //Main.projectile[p].localNPCHitCooldown = -1;

                    swing.medium = true;

                    swing.SwingHalfAngle = shaMedium;
                    swing.easingAdditionAmount = eaaMedium / Main.projectile[p].extraUpdates;
                    swing.frameToStartSwing = ftssMedium * Main.projectile[p].extraUpdates;
                    swing.timeAfterEnd = taeMedium * Main.projectile[p].extraUpdates;
                    swing.startingProgress = spMedium;
                }

                if (combo == 3 || combo == 4 || combo == 5)
                {

                    swing.fast = true;

                    swing.SwingHalfAngle = shaFast;
                    swing.easingAdditionAmount = eaaFast / Main.projectile[p].extraUpdates;
                    swing.frameToStartSwing = ftssFast * Main.projectile[p].extraUpdates;
                    swing.timeAfterEnd = taeFast * Main.projectile[p].extraUpdates;
                    swing.startingProgress = spFast;
                }

                if (combo == 6)
                {
                    Main.projectile[p].scale = 1.25f;
                    Main.projectile[p].localNPCHitCooldown = 60;

                    swing.spin = true;

                    swing.SwingHalfAngle = shaLong;
                    swing.easingAdditionAmount = eaaLong / Main.projectile[p].extraUpdates;
                    swing.frameToStartSwing = ftssLong * Main.projectile[p].extraUpdates;
                    swing.timeAfterEnd = taeLong * Main.projectile[p].extraUpdates;
                    swing.startingProgress = spLong;
                }

                combo++;

                if (combo >= 7)
                {
                    combo = 1;
                }

                //SwingHalfAngle = 190;
                //easingAdditionAmount = 0.015f / Projectile.extraUpdates;
                //frameToStartSwing = 3 * Projectile.extraUpdates;
                //timeAfterEnd = 6 * Projectile.extraUpdates;
                //startingProgress = 0.01f;
            }

            return false;
        }

    }

    public class EosPlayer : ModPlayer
    {
        public int bonusSlashCounter = 0;
    }

    public class EosSwing : BaseSwingSwordProj
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 10000;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 8;

            Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
        {
            if (justHitTime > -5) //-5 so the sword continues to swing a bit and won't get stuck on a bunch of npcs
                return false;

            if (spin)
                return getProgress(easingProgress) > 0.05f && getProgress(easingProgress) <= 0.95f;
            else 
                return getProgress(easingProgress) > 0.2f && getProgress(easingProgress) <= 0.85f;

        }

        public bool fast = false;
        public bool medium = false;
        public bool spin = false;
        public bool safety = false; //delete this variable if not used
        public bool shouldShootProj = false;

        bool shotProj = false;
        bool playedSound = false;
        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        public override void AI()
        {

            //Lighting
            if (getProgress(easingProgress) > 0.05f && getProgress(easingProgress) < 0.95f)
            {
                float colorScale = 0;
                float prog = getProgress(easingProgress);

                if (prog <= 0.5f) //0 - 0.5 --> 0 - 1
                    colorScale = prog * 2f;
                else //0.5 - 1 --> 1 -> 0 
                    colorScale = (1f - prog) * 2f;

                Lighting.AddLight(Projectile.Center, (getEosColor(prog) * colorScale).ToVector3());

            }


            if (timer == 0)
            {
                Projectile.spriteDirection = Main.MouseWorld.X > Main.player[Projectile.owner].MountedCenter.X ? 1 : -1;

                Player p = Main.player[Projectile.owner];

                if (!spin && p.GetModPlayer<EosPlayer>().bonusSlashCounter > 0)
                {
                    shouldShootProj = true;
                    p.GetModPlayer<EosPlayer>().bonusSlashCounter--;
                }

                previousRotations = new List<float>();
            }

            //SwingHalfAngle = 190;
            //easingAdditionAmount = 0.015f / Projectile.extraUpdates;
            offset = 50 * 1.5f * Projectile.scale;
            //frameToStartSwing = 3 * Projectile.extraUpdates;
            //timeAfterEnd = 6 * Projectile.extraUpdates;
            //startingProgress = 0.01f;
            
            
            StandardSwingUpdate();
            StandardHeldProjCode();

            if (justHitTime <= 0 && getProgress(easingProgress) > 0.1f)
                Trail();

            if (timer % 1 == 0 && justHitTime <= 0)
            {

                float rotationOffset;

                if (Projectile.ai[0] != 1)
                {
                    rotationOffset = MathHelper.PiOver4;
                }
                else
                {
                    rotationOffset = MathHelper.PiOver2 + MathHelper.PiOver4;
                }

                previousRotations.Add(currentAngle + rotationOffset);

                float listLength = spin ? 70 : 50;

                if (previousRotations.Count > listLength) //50
                {
                    previousRotations.RemoveAt(0);
                }
            }

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {

                float pitchBoost = !fast ? -0.2f : 0f;

                if (spin)
                {
                    SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/hero_tentacle_sword") with { PitchVariance = .24f, Volume = 0.7f, MaxInstances = 0 }; 
                    SoundEngine.PlaySound(style, Projectile.Center);
                }

                if (fast)
                {
                    SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/hero_butterfly_blade") with { Pitch = -.13f, PitchVariance = .24f, Volume = 0.3f, MaxInstances = 0 };
                    SoundEngine.PlaySound(style, Projectile.Center);
                }

                if (medium)
                {
                    SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/hero_tentacle_sword") with { PitchVariance = .24f, Volume = 0.2f, Pitch = 0.5f, MaxInstances = 0 };
                    SoundEngine.PlaySound(style, Projectile.Center);
                }

                SoundStyle style4 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_05") with { Pitch = -.5f + pitchBoost, PitchVariance = .5f, Volume = 0.12f };
                SoundEngine.PlaySound(style4, Projectile.Center);

                SoundStyle style5 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_line") with { Volume = .08f, Pitch = .38f, PitchVariance = .11f, MaxInstances = 0 };
                SoundEngine.PlaySound(style5, Projectile.Center);

                playedSound = true;
            }

            if (getProgress(easingProgress) >= 0.5f && !shotProj && !spin && shouldShootProj)
            {
                float speed = fast ? 17 : 17; //15 : 17 old

                float dirToMouse = (Main.MouseWorld - Main.player[Projectile.owner].Center).ToRotation();
                float newDir = currentAngle.AngleTowards(dirToMouse, 0.5f);
                //Vector2 newDir = currentAngle.ToRotationVector2().Rot

                Projectile.NewProjectile(null, Main.player[Projectile.owner].Center, newDir.ToRotationVector2() * speed, ModContent.ProjectileType<EosIDK>(), Projectile.damage, 0, Projectile.owner);

                shotProj = true;
            }

            //Dust
            if (timer % 2 == 0 && Main.rand.NextBool() && (getProgress(easingProgress) >= 0.2f && getProgress(easingProgress) <= 0.85f))
            {

                Dust.NewDustPerfect(Main.player[Projectile.owner].Center + currentAngle.ToRotationVector2() * Main.rand.NextFloat(20f, 120f), ModContent.DustType<GlowStrong>(),
                    currentAngle.ToRotationVector2().RotatedByRandom(0.1f).RotatedBy(MathHelper.PiOver2 * (Projectile.ai[0] > 0 ? 1 : -1)) * -Main.rand.NextFloat(2f, 5f),
                    0, newColor: getEosColor(Main.rand.NextFloat(0.0f, 1.0f)), 0.15f + Main.rand.NextFloat(-0.1f, 0.1f));
            }

        }

        public void Trail()
        {
            Vector2 gfxOffset = new Vector2(0, -Main.player[Projectile.owner].gfxOffY);

            float width = 0f;
            if (getProgress(easingProgress) <= 0.2f)
                width = getProgress(easingProgress) / 0.2f;
            else if (getProgress(easingProgress) >= 0.8f)
                width = 1f - ((getProgress(easingProgress) - 0.8f) / 0.2f);
            else
                width = 1f;

            if (width < 0.15)
                width = 0;

            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value;
            trail1.trailPointLimit = 800;
            trail1.trailWidth = (int)(50 * width * 1.5f);
            trail1.trailMaxLength = 300;
            trail1.timesToDraw = 1;
            trail1.relativeToPlayer = true;
            trail1.myPlayer = Main.player[Projectile.owner];
            trail1.pinch = true;
            trail1.pinchAmount = 0.8f;

            trail1.gradient = true;
            trail1.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/EosGrad").Value;
            trail1.shouldScrollColor = true;
            trail1.gradientTime = (float)Main.timeForVisualEffects * 0.03f;

            trail1.trailTime = (float)Main.timeForVisualEffects;
            trail1.trailRot = Projectile.rotation + MathHelper.PiOver4;

            float distance = spin ? 30 : 24;

            Vector2 relativePosition = (Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-0.93f) * distance) - Main.player[Projectile.owner].Center;
            trail1.trailPos = relativePosition - gfxOffset;
            trail1.TrailLogic();

            //Trail2
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/PixelTrail").Value;
            trail2.trailColor = Color.White * width;
            trail2.trailPointLimit = 800;
            trail2.trailWidth = (int)(8);
            trail2.trailMaxLength = 300;
            trail2.timesToDraw = 1;
            trail2.relativeToPlayer = true;
            trail2.myPlayer = Main.player[Projectile.owner];
            trail2.pinch = true;
            trail2.pinchAmount = 0.8f;

            trail2.trailTime = (float)Main.timeForVisualEffects;
            trail2.trailRot = Projectile.rotation + MathHelper.PiOver4;

            trail2.trailPos = relativePosition - gfxOffset;
            trail2.TrailLogic();
        }

        public List<float> previousRotations;

        public override bool PreDraw(ref Color lightColor)
        {
            trail1.trailTime = (float)Main.timeForVisualEffects * 0.04f;
            trail2.trailTime = (float)Main.timeForVisualEffects * 0.02f;


            Texture2D OuterGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Aurora/Eos/EosOuterGlow");


            Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Aurora/Eos/Eos");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Aurora/Eos/EosGlow");

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.ai[0] != 1)
            {
                origin = new Vector2(0, Blade.Height);
                rotationOffset = 0;
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Blade.Width, Blade.Height);
                rotationOffset = MathHelper.ToRadians(90f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle); // get position of hand

            //Sprite is 58x72 so -14y to "make it square", dont know about the x tbh
            Vector2 otherOffset = new Vector2(Projectile.spriteDirection > 0 ? 4 : 0, Projectile.spriteDirection > 0 ? -8 : -14).RotatedBy(currentAngle);

            Vector2 gfxOffset = new Vector2(0, -Main.player[Projectile.owner].gfxOffY);

            Main.spriteBatch.Draw(Blade, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), effects, 0f);
            Main.spriteBatch.Draw(Glow, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, Color.White, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), effects, 0f);



            if (getProgress(easingProgress) >= 0.00f && getProgress(easingProgress) <= 0.999f)
            {

                float alphaToUse = 0f;

                if (getProgress(easingProgress) <= 0.2f)
                    alphaToUse = getProgress(easingProgress) / 0.2f;
                else if (getProgress(easingProgress) >= 0.8f)
                    alphaToUse = 1 - ((getProgress(easingProgress) - 0.8f) / 0.2f);
                else
                    alphaToUse = 1f;
                for (int afterI = 0; afterI < previousRotations.Count; afterI++)
                {
                    float progress = (float)afterI / previousRotations.Count;

                    Color col = getEosColor(progress) with { A = 0 } * progress * 0.75f * alphaToUse;

                    if (justHitTime < 0)
                    {
                        Main.spriteBatch.Draw(OuterGlow, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, Color.Black * 0.25f * progress * alphaToUse,
                            previousRotations[afterI], origin, (Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f)) * 1f, effects, 0f);

                    }


                    Main.spriteBatch.Draw(OuterGlow, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, col,
                        previousRotations[afterI], origin, (Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f)) * 1f, effects, 0f);

                    Main.spriteBatch.Draw(OuterGlow, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, Color.White with { A = 0 } * alphaToUse * 0.5f, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), effects, 0f);

                }
            }

            if (false && getProgress(easingProgress) >= 0.4f && getProgress(easingProgress) <= 0.8f)
            {

                Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/GlowStarPMA");
                Color colToUse = Color.White * 1f;
                colToUse.A = 0;
                Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + new Vector2(35f * (1f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f)), 0).RotatedBy(currentAngle),
                    null, colToUse * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f),
                    Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Star.Size() / 2,
                    0.4f, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + new Vector2(35f * (1f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f)), 0).RotatedBy(currentAngle),
                    null, colToUse * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.7f),
                    Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Star.Size() / 2,
                    0.2f, SpriteEffects.None, 0f);
            }

            trail2.TrailDrawing(Main.spriteBatch);
            trail1.TrailDrawing(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

        public Color getEosColor(float progress)
        {
            Color myCol = Color.White;

            //Color purple = new Color(214, 143, 247);
            Color purple = new Color(184, 70, 241);
            //Color mediumBlue = new Color(97, 136, 186);
            Color mediumBlue = new Color(95, 103, 185);

            Color brightBlue = new Color(110, 234, 215);

            float purpleVal = spin ? 0.45f : 0.33f;

            if (progress < purpleVal)
            {
                myCol = Color.Lerp(purple, mediumBlue, progress * 3f);
            }
            else if (progress < 0.66f)
            {
                float fakeProgress = progress - 0.33f;
                myCol = Color.Lerp(mediumBlue, brightBlue, fakeProgress * 3f);

            }
            else
            {
                float fakeProgress = progress - 0.66f;
                myCol = Color.Lerp(brightBlue, mediumBlue, fakeProgress * 3f);

            }
            return myCol;
        }

        bool hasBashed = false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player p = Main.player[Projectile.owner];

            if (!hasBashed && p.timeSinceLastDashStarted < 30 && spin)
            {
                p.velocity.X = -p.velocity.X / 2;
                p.velocity.Y = -p.velocity.X / 2;
                p.GiveImmuneTimeForCollisionAttack(60);

                SoundStyle slash = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Impact_Sword_L_a") with { Pitch = 1f, PitchVariance = .1f, Volume = 0.4f };
                SoundEngine.PlaySound(slash, Projectile.Center);

                p.GetModPlayer<EosPlayer>().bonusSlashCounter = 10;

                Vector2 dir = (target.Center - p.Center).SafeNormalize(Vector2.UnitX);

                int flare = Projectile.NewProjectile(null, target.Center, dir * 1.75f, ModContent.ProjectileType<EosHitFlare>(), 0, 0, Projectile.owner);
                Main.projectile[flare].rotation = dir.ToRotation();
                Main.projectile[flare].scale = 1.35f;

                for (int i = 0; i < 6; i++)
                {
                    Dust a = Dust.NewDustPerfect(target.Center + dir * 2f, ModContent.DustType<MuraLineDust>(),
                        dir.RotatedByRandom(0.2f) * Main.rand.NextFloat(1f, 9f),
                        0, newColor: getEosColor(Main.rand.NextFloat(0.0f, 1.0f)));
                    a.alpha = 15;
                }

                for (int i = 0; i < 10; i++)
                {
                    Dust a = Dust.NewDustPerfect(target.Center + dir * 2f, ModContent.DustType<GlowStrong>(),
                        dir.RotatedByRandom(0.5f) * Main.rand.NextFloat(1f, 8f),
                        0, newColor: getEosColor(Main.rand.NextFloat(0.0f, 1.0f)), Main.rand.NextFloat(0.4f, 0.65f));
                    a.alpha = 2;
                }


                justHitTime = 50;
                Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 35;

                hasBashed = true;
            }

            if (medium)
            {
                if (justHitTime <= 0)
                    justHitTime = 60;
                Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 10;
            }
            else if (fast)
            {
                //justHitTime = 5;
                Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 4;
            }
            else if (spin)
            {
                if (Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower <= 0)
                    Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 15;

                if (justHitTime <= 0)
                    justHitTime = 20;

            }

            Vector2 randomPos = Main.rand.NextVector2CircularEdge(120, 120);

            Projectile.NewProjectile(null, target.Center + randomPos, randomPos.SafeNormalize(Vector2.UnitX) * -6f, ModContent.ProjectileType<EosSlash>(), Projectile.damage / 2, 0, Projectile.owner);

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(target.Center + randomPos, ModContent.DustType<GlowStrong>(), 
                    randomPos.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.2f) * -Main.rand.NextFloat(1f, 1.2f), 
                    0, newColor: getEosColor(Main.rand.NextFloat(0.0f, 1.0f)), 0.4f);
            }

        }

        public override float getProgress(float x)
        {
            float toReturn = 0f;
            #region easeExpo

            //pre 0.5
            if (x <= 0.5f)
            {
                toReturn = (float)(Math.Pow(2, (20 * x) - 10)) / 2;
            }
            else if (x > 0.5)
            {
                toReturn = (float)(2 - Math.Pow(2, (-20 * x) + 10)  ) / 2;
            }

            //post 0.5
            if (x == 0)
                toReturn = 0;
            if (x == 1)
                toReturn = 1;

            return toReturn;


            #endregion;

            //
            return Easings.easeOutCirc(x);


            //return base.getProgress(x);
        }
    }

    public class EosSlash : ModProjectile
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

            Projectile.extraUpdates = 4;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        int timer = 0;
        public bool rotDir = Main.rand.NextBool();
        float scale = 1f;
        public override void AI()
        {

            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTextureButBlack").Value;
            trail1.trailColor = Color.White * 0.4f;
            trail1.trailPointLimit = 300;
            trail1.trailWidth = (int)(10 * scale);
            trail1.trailMaxLength = 400;
            trail1.timesToDraw = 1;
            trail1.pinch = true;
            trail1.pinchAmount = 0.8f;
            trail1.shouldSmooth = true;

            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center;
            trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/GlowTrail").Value;
            trail2.trailPointLimit = 300;
            trail2.trailWidth = (int)(15 * scale);
            trail2.trailMaxLength = 400;
            trail2.timesToDraw = 1;
            trail2.pinch = true;
            trail2.pinchAmount = 0.8f;
            trail2.shouldSmooth = true;

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/EosGrad").Value;
            trail2.shouldScrollColor = true;

            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center;
            trail2.TrailLogic();

            Projectile.velocity = Projectile.velocity.RotatedBy(0.01f * (rotDir ? 1f : -1f));

            /*
            if (timer % 8 == 0 && Main.rand.NextBool(2))
            {
                int a = Dust.NewDust(Projectile.Center, 5, 5, ModContent.DustType<GlowStrong>(), Scale: Main.rand.NextFloat(0.1f, 0.15f));
                Main.dust[a].velocity *= 0.75f;
                Main.dust[a].velocity += Projectile.velocity * 0.2f;

                Main.dust[a].color = getEosColor(Main.rand.NextFloat(0.0f, 1.0f));
                Main.dust[a].alpha = 2;

            }
            */
            if (timer > 40)
            {
                scale = Math.Clamp(MathHelper.Lerp(scale, -0.5f, 0.07f), 0, 1);

                if (scale == 0)
                    Projectile.active = false;
            }

            Lighting.AddLight(Projectile.Center, (Color.Wheat * 0.5f * Math.Clamp(scale, 0, 1)).ToVector3());

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

        public Color getEosColor(float progress)
        {
            Color myCol = Color.White;

            Color purple = new Color(214, 143, 247);
            Color mediumBlue = new Color(97, 136, 186);
            Color brightBlue = new Color(110, 234, 215);


            if (progress < 0.33f)
            {
                myCol = Color.Lerp(purple, mediumBlue, progress * 3f);
            }
            else if (progress < 0.66f)
            {
                float fakeProgress = progress - 0.33f;
                myCol = Color.Lerp(mediumBlue, brightBlue, fakeProgress * 3f);

            }
            else
            {
                float fakeProgress = progress - 0.66f;
                myCol = Color.Lerp(brightBlue, mediumBlue, fakeProgress * 3f);

            }
            return myCol;
        }

    }

    public class EosIDK : ModProjectile
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

            Projectile.extraUpdates = 10;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        int timer = 0;
        public bool rotDir = Main.rand.NextBool();
        float scale = 1f;
        float srot = 0;
        public override void AI()
        {
            if (timer == 0)
                srot = Projectile.velocity.ToRotation();

            if (timer > 2)
            {
                int counter = 0;
                foreach (Vector2 v in trail1.trailPositions)
                {
                    if (counter % 2 == 0)
                    {
                        if (counter % 4 == 0)
                            Lighting.AddLight(v, (Color.LightBlue * 0.65f * Math.Clamp(scale, 0, 1)).ToVector3());
                        else
                            Lighting.AddLight(v, (Color.MediumPurple * 0.65f * Math.Clamp(scale, 0, 1)).ToVector3());
                    }
                    counter++;
                }

            }

            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTextureButBlack").Value;
            trail1.trailColor = Color.White * 0.8f;
            trail1.trailPointLimit = 300;
            trail1.trailWidth = (int)(30 * scale * 1);
            trail1.trailMaxLength = 400 * 2;
            trail1.timesToDraw = 1;
            trail1.pinch = true;
            trail1.pinchAmount = 0.5f;
            trail1.shouldSmooth = true;

            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center + Projectile.velocity;
            //trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail7").Value;
            trail2.trailPointLimit = 300;
            trail2.trailWidth = (int)(45 * scale * 0.5f);
            trail2.trailMaxLength = 400 * 2;
            trail2.timesToDraw = 2;
            trail2.pinch = true;
            trail2.pinchAmount = 0.5f;
            trail2.shouldSmooth = true;

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/EosGrad").Value;
            trail2.shouldScrollColor = true;

            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center + Projectile.velocity;
            //trail2.TrailLogic();

            if (timer < 30)
            {
                trail1.TrailLogic();
                trail2.TrailLogic();

                if (timer % 2 == 0 && Main.rand.NextBool())
                {
                    int a = Dust.NewDust(Projectile.Center, 5, 5, ModContent.DustType<GlowStrong>(), Scale: Main.rand.NextFloat(0.3f, 0.6f));
                    Main.dust[a].velocity *= 2;
                    Main.dust[a].velocity += Projectile.velocity * 0.2f;

                    Main.dust[a].color = getEosColor(Main.rand.NextFloat(0.0f, 1.0f));
                    Main.dust[a].alpha = 2;

                }

            }

            if (timer == 30)
            {
                Projectile.velocity = Vector2.Zero;
            }


            if (timer > 40)
            {
                scale = Math.Clamp(MathHelper.Lerp(scale, -0.5f, 0.02f), 0, 1);

                if (scale == 0)
                    Projectile.active = false;
            }
            else
            {
                scale = Math.Clamp(MathHelper.Lerp(scale, 1.25f, 0.02f), 0, 1);

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

            Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/TrailImages/RainbowRod").Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 scale2 = new Vector2(1f, 0.5f) * scale;
            Vector2 pos = Projectile.Center - Main.screenPosition - Projectile.velocity * 1f;

            Main.spriteBatch.Draw(Star, pos, null, Color.White, srot, Star.Size() / 2, scale2, SpriteEffects.None, 0f);


            float rot = (float)Main.timeForVisualEffects * 0.08f;
            Vector2 v = new Vector2(1.5f * (scale * 2), 0);


            Main.spriteBatch.Draw(Star, pos + v.RotatedBy(rot), null, Color.Aqua, srot, Star.Size() / 2, scale2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, pos + v.RotatedBy(rot + MathHelper.PiOver2), null, Color.LightBlue, srot, Star.Size() / 2, scale2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, pos + v.RotatedBy(rot + MathHelper.Pi), null, Color.MediumPurple, srot, Star.Size() / 2, scale2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, pos + v.RotatedBy(rot + -MathHelper.PiOver2), null, Color.Orange, srot, Star.Size() / 2, scale2, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }

        public Color getEosColor(float progress)
        {
            Color myCol = Color.White;

            Color purple = new Color(214, 143, 247);
            Color mediumBlue = new Color(97, 136, 186);
            Color brightBlue = new Color(110, 234, 215);


            if (progress < 0.33f)
            {
                myCol = Color.Lerp(purple, mediumBlue, progress * 3f);
            }
            else if (progress < 0.66f)
            {
                float fakeProgress = progress - 0.33f;
                myCol = Color.Lerp(mediumBlue, brightBlue, fakeProgress * 3f);

            }
            else
            {
                float fakeProgress = progress - 0.66f;
                myCol = Color.Lerp(brightBlue, mediumBlue, fakeProgress * 3f);

            }
            return myCol;
        }

    }

    public class EosHitFlare : ModProjectile
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
            Projectile.timeLeft = 100;
            Projectile.scale = 1f;

        }

        public override bool? CanDamage()
        {
            return false;
        }

        int timer = 0;
        public float scale = 1.5f;
        public float alpha = 1;
        bool starRotDir = Main.rand.NextBool();

        public override void AI()
        {
            Projectile.scale = Math.Clamp(MathHelper.Lerp(Projectile.scale, -0.5f, 0.08f), 0, 10);

            alpha *= 0.95f;

            if (Projectile.scale == 0)
                Projectile.active = false;
            //Projectile.rotation += 0.2f * (starRotDir ? 1 : -1);

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_1").Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 vec2Scale = new Vector2(0.5f, 1f) * Projectile.scale;

            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.Black * 0.2f * alpha, Projectile.rotation, Flare.Size() / 2, Projectile.scale * scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition + new Vector2(3, 0) + Main.rand.NextVector2Circular(3, 3), null, Color.Aqua * alpha, Projectile.rotation - MathHelper.PiOver2, Flare.Size() / 2, vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition + new Vector2(-3, 0) + Main.rand.NextVector2Circular(3, 3), null, Color.MediumPurple * alpha, Projectile.rotation - MathHelper.PiOver2, Flare.Size() / 2, vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition + new Vector2(0, 3) + Main.rand.NextVector2Circular(3, 3), null, Color.LightBlue * alpha, Projectile.rotation - MathHelper.PiOver2, Flare.Size() / 2, vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition + new Vector2(0, -3) + Main.rand.NextVector2Circular(3, 3), null, Color.Orange * alpha, Projectile.rotation - MathHelper.PiOver2, Flare.Size() / 2, vec2Scale, SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation - MathHelper.PiOver2, Flare.Size() / 2, vec2Scale, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }


    //Unused but pretty cool ill use it for the staff or something 
    public class EosDart : ModProjectile
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

            Projectile.extraUpdates = 6;
        }

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        int timer = 0;
        float alpha = 0f;
        Vector2 trailOffset = Vector2.Zero;

        public List<Vector2> previousPositions;
        public override void AI()
        {
            if (timer == 0)
            {
                previousPositions = new List<Vector2>();
                //Projectile.scale = 0.75f;
            }

            //NOPE TOO LAGGY CUZ EXTRA UPDATES
            /*
            var target = Projectile.FindTargetWithLineOfSight(240f);
            if (target != -1)
            {
                Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity,
                    Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * Projectile.velocity.Length(), 0.05f)) * Projectile.velocity.Length();
            }
            */

            if (timer % 2 == 0)
            {

                previousPositions.Add(Projectile.Center + Projectile.velocity);

                if (previousPositions.Count > 40)
                {
                    previousPositions.RemoveAt(0);
                }
            }


            #region Trail
            int trueTrailWidth = (int)(30 * alpha);

            if (trueTrailWidth < 3)
                trueTrailWidth = 0;

            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
            trail1.trailPointLimit = 300;
            trail1.trailWidth = trueTrailWidth;
            trail1.trailMaxLength = 500;
            trail1.timesToDraw = 2;
            trail1.shouldSmooth = false;
            trail1.pinch = true;

            trail1.gradient = true;
            trail1.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/EosGrad").Value;
            trail1.shouldScrollColor = true;

            float OffsetAmount = 35f * MathF.Sin((float)timer / 45 * Projectile.extraUpdates);
            Vector2 offsetPosition = new Vector2(0, OffsetAmount).RotatedBy(Projectile.velocity.ToRotation());

            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = offsetPosition + Projectile.Center + Projectile.velocity;//Projectile.Center + Projectile.velocity + Main.rand.NextVector2Circular(5,5);
            trail1.TrailLogic();


            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
            trail2.trailPointLimit = 300;
            trail2.trailWidth = trueTrailWidth;
            trail2.trailMaxLength = 500;
            trail2.timesToDraw = 2;
            trail2.shouldSmooth = false;
            trail2.pinch = true;

            float OffsetAmount2 = 35f * MathF.Sin((float)timer / 45 * Projectile.extraUpdates);
            Vector2 offsetPosition2 = new Vector2(0, -OffsetAmount2).RotatedBy(Projectile.velocity.ToRotation());

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/EosGrad").Value;
            trail2.shouldScrollColor = true;


            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = offsetPosition2 + Projectile.Center + Projectile.velocity;//Projectile.Center + Projectile.velocity + Main.rand.NextVector2Circular(5,5);
            trail2.TrailLogic();

            #endregion

            if (timer < 20 * Projectile.extraUpdates)
            {
                alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.5f, 0.02f), 0, 1);
            }

            if (timer > 15 * Projectile.extraUpdates)
            {
                if (timer < 30)
                {
                    alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.25f, 0.05f / Projectile.extraUpdates), 0, 1);
                    Projectile.scale -= 0.01f / Projectile.extraUpdates;
                    Projectile.scale = Math.Clamp(Projectile.scale, 0, 100);

                    Projectile.velocity *= 0.97f;

                }
                else
                {
                    alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.25f, 0.08f / Projectile.extraUpdates), 0, 1);
                    Projectile.scale -= 0.02f / Projectile.extraUpdates;
                    Projectile.scale = Math.Clamp(Projectile.scale, 0, 100);

                    Projectile.velocity *= 0.98f;

                }


            }

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            trail1.trailTime = (float)Main.timeForVisualEffects * 0.01f;
            trail1.trailTime = (float)Main.timeForVisualEffects * 0.03f;

            trail2.gradientTime = (float)Main.timeForVisualEffects * 0.02f;
            trail2.trailTime = (float)Main.timeForVisualEffects * 0.03f;

            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            Texture2D Swing = Mod.Assets.Request<Texture2D>("Assets/TrailImages/BusterGlow").Value;
            Texture2D SwingStandard = Mod.Assets.Request<Texture2D>("Assets/TrailImages/TestTex").Value;

            Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_4").Value;

            Vector2 scale = new Vector2(0.35f * alpha, 1f) * Projectile.scale;
            Vector2 scale2 = new Vector2(0.15f * alpha, 1f) * Projectile.scale;

            for (int afterI = 0; afterI < previousPositions.Count; afterI++)
            {
                //BLACK AT 100% OPACITY LOOKS REALLY COOL BUT DOESN'T WORK FOR THIS WEAPON
                float scaleMultiplier = (float)afterI / previousPositions.Count;


                Main.spriteBatch.Draw(Swing, previousPositions[afterI] - Main.screenPosition, null, Color.Black * alpha * 0.1f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, Swing.Size() / 2, scale * 1.5f * scaleMultiplier, SpriteEffects.None, 0f);

            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Swing, Projectile.Center - Main.screenPosition + -Projectile.velocity, null, Color.White * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, Swing.Size() / 2, scale2 * 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Swing, Projectile.Center - Main.screenPosition + -Projectile.velocity, null, Color.White * alpha * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, Swing.Size() / 2, scale2 * 2.4f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Swing, Projectile.Center - Main.screenPosition + -Projectile.velocity, null, Color.White * alpha * 0.2f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, Swing.Size() / 2, scale2 * 2.6f, SpriteEffects.None, 0f);


            for (int afterI = 0; afterI < previousPositions.Count; afterI++)
            {
                float scaleMultiplier = (float)afterI / previousPositions.Count;

                Main.spriteBatch.Draw(Swing, previousPositions[afterI] - Main.screenPosition, null, getEosColor((float)afterI / previousPositions.Count) * alpha * 0.6f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, SwingStandard.Size() / 2, scale * 1.7f * scaleMultiplier, SpriteEffects.None, 0f);


                Main.spriteBatch.Draw(Swing, previousPositions[afterI] - Main.screenPosition, null, getEosColor((float)afterI / previousPositions.Count) * alpha * 0.45f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, SwingStandard.Size() / 2, scale * 1.7f * scaleMultiplier, SpriteEffects.None, 0f);

            }


            Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Projectile.velocity * 2f, null, Color.MediumPurple * alpha * 0.7f, (float)Main.timeForVisualEffects * 0.03f + 1f, Star.Size() / 2, 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Projectile.velocity * 2f, null, Color.Aquamarine * alpha * 0.7f, (float)Main.timeForVisualEffects * -0.04f + 0.5f, Star.Size() / 2, 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Projectile.velocity * 2f, null, Color.SkyBlue * alpha * 0.7f, (float)Main.timeForVisualEffects * 0.05f, Star.Size() / 2, 0.8f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }


        public Color getEosColor(float progress)
        {
            Color myCol = Color.White;

            Color purple = new Color(214, 143, 247);
            Color mediumBlue = new Color(97, 136, 186);
            Color brightBlue = new Color(110, 234, 215);


            if (progress < 0.33f)
            {
                myCol = Color.Lerp(purple, mediumBlue, progress * 3f);
            }
            else if (progress < 0.66f)
            {
                float fakeProgress = progress - 0.33f;
                myCol = Color.Lerp(mediumBlue, brightBlue, fakeProgress * 3f);

            }
            else
            {
                float fakeProgress = progress - 0.66f;
                myCol = Color.Lerp(brightBlue, mediumBlue, fakeProgress * 3f);

            }
            return myCol;
        }

    }

}