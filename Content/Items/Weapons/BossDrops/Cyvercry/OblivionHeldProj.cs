using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged;
using ReLogic.Content;
using AerovelenceMod.Common.Globals.SkillStrikes;
using static Terraria.NPC;
using AerovelenceMod.Content.Projectiles;
using System.Collections.Generic;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
    public class OblivionHeldProj : ModProjectile
    {
        public override bool ShouldUpdatePosition() => Projectile.ai[1] == 4;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Oblivion");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; //WORKJS //WORKS !  1! ! !
        }
        int maxTime = 30;
        public override void SetDefaults()
        {
            maxTime = 32;
            Projectile.timeLeft = maxTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
        }

        bool firstFrame = true;
        float startingAng = 0;
        float endingAng = 0;
        float currentAng = 0;

        public override void AI()
        {
            //Matenince 
            Player player = Main.player[Projectile.owner];
            //This make the arm point to the sword but apparently isn't completely accurate so ill do that later
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            
            player.heldProj = Projectile.whoAmI;
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
            }
            
            //This is were we set the beggining and ending angle of the sword 
            if (firstFrame)
            {
                maxTime = 0;
                //No getting the mouse Direction via Main.mouse world did not work
                Vector2 mouseDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                
                //I don't know why this is called sus but this is the angle we will compare the mouseDir to 
                Vector2 sus1 = new Vector2(-10, 0).SafeNormalize(Vector2.UnitX);
                
                //Yes I did have to normalize it again (i don't fuckin know why but it is needed)
                Vector2 sus2 = mouseDir.SafeNormalize(Vector2.UnitX);

                startingAng = sus1.AngleTo(sus2) * 2; //psure the * 2 is from double normalization

                //we set Projectile.ai[0] in the slate set. This is so the sword alternates direction
                if (Projectile.ai[0] == 1)
                {
                    endingAng = startingAng + MathHelper.ToRadians(-120);
                    startingAng = startingAng - MathHelper.ToRadians(-120);
                }
                else
                {
                    endingAng = startingAng - MathHelper.ToRadians(-120);
                    startingAng = startingAng + MathHelper.ToRadians(-120);
                }

                currentAng = startingAng;
                //Main.NewText(MathHelper.ToDegrees(sus1.AngleTo(sus2) * 2));
                firstFrame = false;
            }
            
            //wait 2 frames to give it a little more oomf

            if (maxTime >= 0)
            {
                if (Projectile.ai[0] == 1)
                    currentAng = startingAng - MathHelper.ToRadians(getProgress());
                else
                    currentAng = startingAng + MathHelper.ToRadians(getProgress());

                angleTimer++;
            }

            /* old lerp
            if (maxTime > 10) //2
            {
                //the actual lerping
                float sussybaka = (maxTime * 0.005f);
                //currentAng = MathHelper.SmoothStep(currentAng, endingAng, 0.12f);

                currentAng = MathHelper.Lerp(currentAng, endingAng, sussybaka);
            }
            else
            {
                currentAng = MathHelper.Lerp(currentAng, endingAng, 0.01f);
            }
            */

            Projectile.rotation = currentAng + MathHelper.PiOver4;

            //normal held proj stuff
            Projectile.Center = (currentAng.ToRotationVector2() * 53) + player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = currentAng * player.direction;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (getProgress() > AngleSpread - 10)
                Projectile.active = false;

            maxTime++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0, Projectile.owner);

            int strikeCount = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(Projectile.Center, Main.npc[i].Center) < 85f && strikeCount < 5)
                {
                    int Direction = 0;
                    if (Projectile.position.X - Main.npc[i].position.X < 0)
                        Direction = 1;
                    else
                        Direction = -1;
                    strikeCount++;

                    HitInfo myHit = new HitInfo();
                    myHit.Damage = Projectile.damage;
                    myHit.Knockback = Projectile.knockBack;
                    myHit.HitDirection = Direction;
                    Main.npc[i].StrikeNPC(myHit);

                    //Main.npc[i].StrikeNPC(Projectile.damage, Projectile.knockBack, Direction);
                    //Main.npc[i].GetGlobalNPC<SkillStrikeNPC>().strikeCTRemove = true;
                }
            }
            SoundEngine.PlaySound(SoundID.Item94 with { Pitch = 0.4f, Volume = 0.75f, PitchVariance = 0.2f }, Projectile.Center);
            //SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/RockCollide") with { Volume = .3f, PitchVariance = 0.3f, Pitch = -0.3f }, Projectile.Center);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (getProgress() > 180 || getProgress() < 50)
                return false;

            //Vector2 diff = target.position - Main.player[Projectile.owner].Center;
            //Vector2 toProj = Projectile.Center - Main.player[Projectile.owner].Center;

            return base.CanHitNPC(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/SlateSet/SlateSlice1").Value;

            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/OblivionHeldProj_Glow").Value;
            Texture2D twirl = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_03").Value;

            Player player = Main.player[Projectile.owner];

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            //myEffect.Parameters["uColor"].SetValue(Color.WhiteSmoke.ToVector3() * 1f); 

            myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * 2f * (getStrength() / 120));
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.9f);
            myEffect.Parameters["uSaturation"].SetValue(0f);

            /*
            if (player.direction == -1)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
                myEffect.CurrentTechnique.Passes[0].Apply();
            }
            else
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            }
            */

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(twirl, player.Center - Main.screenPosition, null, Color.DeepPink * (getStrength() / 120) * 0.75f, Projectile.rotation + (Projectile.ai[0] != 1 ? MathHelper.PiOver2 : MathHelper.ToRadians(0)), twirl.Size() / 2, Projectile.scale * 0.45f, Projectile.ai[0] != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 0)
            {

                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
                //for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                //{
                    //float progress = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type] * i;
                    //Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + new Vector2(23,23), new Rectangle(0, 0, texture.Width, texture.Height), Color.HotPink * (1f - progress), Projectile.oldRot[i] + (Projectile.ai[0] == -1 ? 0 : MathHelper.PiOver2 * 3), new Rectangle(0, 0, texture.Width, texture.Height).Size() / 2f, Math.Max(Projectile.scale * (1f - progress), 0.1f), Projectile.ai[0] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
                //}
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
                
                //no stretch
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), texture.Size() / 2, Projectile.scale, Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
                Main.spriteBatch.Draw(glowMask, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), texture.Size() / 2, Projectile.scale, Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);


                //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), texture.Size() / 2, Projectile.scale + 0.15f * (getStrength() / 120), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
                //Main.spriteBatch.Draw(glowMask, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), texture.Size() / 2, Projectile.scale + 0.15f * (getStrength() / 120), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            }

            return false;
        }

        float angleTimer = 0;
        float AngleSpread = 240;
        public float getProgress()
        {
            float halfOfAngle = AngleSpread / 2;
            float stretch = 5; //Will last for about 3 times this

            //float valToReturn = (halfOfAngle * -1) * (float)Math.Cos(angleTimer / MathHelper.Clamp(15 - 5 * Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee), 1, 10)) + halfOfAngle;

            //float valTest = (240f) * (float)Math.Sqrt(1 - Math.Pow( (angleTimer / 30 * Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee)) - 1, 2)); //semicircle
            float valTest = (240f * -1) * (float)Math.Pow(2 , (angleTimer /  (14 - Math.Clamp((7 * Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee)), 0, 13)) ) * -1) + 240;

            //-60cos(x/10) + 60
            //The *60 and +60 come from it being half of 1/120

            return valTest;
        }

        public float getStrength()
        {
            float toReturn = 120f * (float)Math.Sin(angleTimer / 5);
            return MathHelper.Clamp(toReturn, 0, 120);
        }
    }

    public class PinkExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyver Pulse");
        }

        public float exploScale = 0.51f;
        public float numberOfDraws = 3f;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.scale = 0.2f;

            Projectile.timeLeft = 30;
            Projectile.tileCollide = false; //false;
            Projectile.width = 20;
            Projectile.height = 20;
        }

        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.28f);
                //SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
            }

            Projectile.scale = MathHelper.Lerp(Projectile.scale, exploScale, 0.2f); //1.51
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //texture = gaussExplosion
            //distort = noise
            //caustics = GlowLine1 (Flare)
            //gradient = DarnessDischarge

            Player Player = Main.player[Projectile.owner];
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/GaussExplosion").Value;

            //ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/Solsear_Glow").Value

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value;

            //myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/GlowLine1").Value);
            myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GaussianStar").Value);


            myEffect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);

            myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/PinkStarrrr").Value);
            //myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/Solsear_Glow").Value);
            myEffect.Parameters["uTime"].SetValue(timer * 0.08f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            int height1 = texture.Height;
            Vector2 origin1 = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);

            for (float y = 0; y < numberOfDraws; y++)
            {
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);

            }
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class Oblivion2ElectricBoogaloo : BaseSwingSwordProj
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
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

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 3; //0
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f) && justHitTime <= -1;
            return shouldDamage;
        }

        bool playedSound = false;
        public override void AI()
        {
            if (timer == 0)
                previousRotations = new List<float>();
            //Main.NewText(getProgress(easingProgress));


            //test high starting amount and long frame to start swing

            SwingHalfAngle = 160;
            easingAdditionAmount = 0.01f; //01
            offset = 55;
            frameToStartSwing = 2 * 3;
            timeAfterEnd = 5 * 3;

            StandardHeldProjCode();
            StandardSwingUpdate();

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {

                SoundEngine.PlaySound(SoundID.Item71 with { Pitch = -0.4f, PitchVariance = 0.15f, Volume = 0.67f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.8f, Pitch = 0.4f }, Projectile.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Sharp_M_a") with { Pitch = -.82f, PitchVariance = .16f, Volume = 0.10f };
                SoundEngine.PlaySound(style, Projectile.Center);
                playedSound = true;
            }

            if (timer % 2 == 0 && justHitTime <= 0)
            {
                previousRotations.Add(currentAngle);

                if (previousRotations.Count > 5)
                {
                    previousRotations.RemoveAt(0);
                }
            }

            //Dust
            /*
            if (timer % 1 == 0 && getProgress(easingProgress) > 0.2f && getProgress(easingProgress) < 0.85f && justHitTime <= 0)
            {
                //i rofl to hide the pain
                for (int i = 0; i < (Main.rand.NextBool() ? 1 : 0); i++)
                {
                    int a = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowPixelAlts>(), newColor: Color.HotPink, Scale: 0.4f);
                    Main.dust[a].velocity *= Main.rand.NextFloat(1f);
                    Main.dust[a].velocity += originalAngle.ToRotationVector2() * 3.5f;
                    //Main.dust[a].velocity += currentAngle.ToRotationVector2().RotatedBy(Projectile.ai[0] == 0 ? MathHelper.PiOver2 : -MathHelper.PiOver2) * 2f;

                }

            }
            */
            justHitTime--;
        }

        public List<float> previousRotations;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/Oblivion");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/OblivionHeldProj_Glow");

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.spriteDirection > 0)
            {
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
            }
            else
            {

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

            }

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle); // get position of hand

            //Sprite is 60x68 so -8y to "make it square", dont know about the x tbh
            Vector2 offset = new Vector2(Projectile.spriteDirection > 0 ? 0 : 0, -8).RotatedBy(currentAngle);


            if (getProgress(easingProgress) >= 0.1f && getProgress(easingProgress) <= 0.9f ) {

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D Trail = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/pixelKennySlashTiny");
                Vector2 pos = Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(5f + 10f * (float)Math.Sin(MathHelper.Pi * getProgress(easingProgress)), 0).RotatedBy(originalAngle);

                Main.spriteBatch.Draw(Trail, pos, Trail.Frame(1, 1, 0, 0), Color.DeepPink * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 1f), originalAngle + MathHelper.PiOver2, Trail.Size() / 2, 0.65f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 1.1f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Trail, pos, Trail.Frame(1, 1, 0, 0), Color.White * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.5f), originalAngle + MathHelper.PiOver2, Trail.Size() / 2, 0.65f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 1.1f), SpriteEffects.None, 0f);

                Vector2 otherOffset = new Vector2(-8, -8).RotatedBy(currentAngle) * (Projectile.ai[0] > 0 ? 0 : 1);

                Texture2D OuterGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/OblivionOuterGlow");
                Main.spriteBatch.Draw(OuterGlow, armPosition - Main.screenPosition + offset + otherOffset, null, Color.HotPink * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 1f), Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f) + 0.1f, effects, 0f);
                Main.spriteBatch.Draw(OuterGlow, armPosition - Main.screenPosition + offset + otherOffset, null, Color.HotPink * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 1f), Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.4f) + 0.1f, effects, 0f);
                //Main.spriteBatch.Draw(OuterGlow, Projectile.Center - Main.screenPosition, null, Color.HotPink * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 1f), Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), OuterGlow.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f) + 0.1f, Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

               

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                //Texture2D OuterGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/OblivionWhiteGlow");
                //Main.spriteBatch.Draw(OuterGlow, Projectile.Center - Main.screenPosition, null, Color.DeepPink * 0.5f, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), OuterGlow.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            }


            


            Main.spriteBatch.Draw(Blade, armPosition - Main.screenPosition + offset, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), effects, 0f);
            Main.spriteBatch.Draw(Glow, armPosition - Main.screenPosition + offset, null, Color.White, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), effects, 0f);

            /* This would work well for another weapon, but not this one
            if (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.7f)
            {

                Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/GlowStarPMA");
                Color colToUse = Color.HotPink;
                colToUse.A = 0;
                Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + new Vector2(35f * (1f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f)), 0).RotatedBy(currentAngle),
                    null, colToUse * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f),
                    Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Star.Size() / 2,
                    0.6f, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + new Vector2(35f * (1f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f)), 0).RotatedBy(currentAngle),
                    null, colToUse * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.7f),
                    Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Star.Size() / 2,
                    0.3f, SpriteEffects.None, 0f);
            }
            */
            return false;
        }

        public override void OnHitNPC(NPC target, HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item94 with { Pitch = 0.4f, Volume = 0.5f, PitchVariance = 0.4f }, target.Center);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0, Projectile.owner);

            int strikeCount = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(Projectile.Center, Main.npc[i].Center) < 85f && strikeCount < 5)
                {
                    int Direction = 0;
                    if (Projectile.position.X - Main.npc[i].position.X < 0)
                        Direction = 1;
                    else
                        Direction = -1;
                    strikeCount++;

                    HitInfo myHit = new HitInfo();
                    myHit.Damage = Projectile.damage;
                    myHit.HitDirection = Direction;
                    Main.npc[i].StrikeNPC(myHit);
                }
            }

            for (int i = 0; i < 8; i++)
            {

                Dust a = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowPixelAlts>(), newColor: Color.HotPink, Scale: 0.5f + Main.rand.NextFloat(-0.1f, 0.11f));
                a.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 0.5f;

                a.velocity += (target.Center - Main.player[Projectile.owner].Center).SafeNormalize(Vector2.UnitX);
                a.velocity = a.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
                a.velocity *= Main.rand.NextFloat(0f, 6f);
            }
            justHitTime = 24;
        }


        // Find the start and end of the sword and use a line collider to check for collision with enemies
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Main.player[Projectile.owner].MountedCenter;
            Vector2 end = start + currentAngle.ToRotationVector2() * ((Projectile.Size.Length() * 1.2f) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        public override float getProgress(float x) //From 0 to 1 and returns 0-1
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
                toReturn = (float)(2 - ((Math.Pow(2, (-20 * x) + 10)))) / 2;
            }

            //post 0.5
            if (x == 0)
                toReturn = 0;
            if (x == 1)
                toReturn = 1;

            return toReturn;


            #endregion;
        }
    }

    public class OblivionPulse : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.scale = 0.1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 35;
        }

        private int timer = 0;
        private Vector2 startingCenter = Vector2.Zero;

        public override bool? CanDamage()
        {
            if (Projectile.timeLeft < 20)
                return false;
            return true;
        }
        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.28f);
                startingCenter = Projectile.Center;
            }
            
            if (Projectile.scale < 1f)
                Projectile.scale =  Math.Clamp(MathHelper.Lerp(Projectile.scale, 3.51f, 0.09f), 0, 2f);
            else
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 2.51f, 0.09f);
            Projectile.width = (int)(200 * Projectile.scale);
            Projectile.height = (int)(200 * Projectile.scale);
            Projectile.Center = startingCenter;
            Projectile.velocity = Vector2.Zero;

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //texture = gaussExplosion
            //distort = noise
            //caustics = GlowLine1 (Flare)
            //gradient = DarnessDischarge

            Player Player = Main.player[Projectile.owner];
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/GaussExplosion").Value;

            //ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/Solsear_Glow").Value

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value;

            //myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/GlowLine1").Value);
            myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GaussianStar").Value);
            myEffect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);
            myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/tstar").Value);
            myEffect.Parameters["uTime"].SetValue(timer * 0.065f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            int height1 = texture.Height;
            Vector2 origin1 = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);

            for (float y = 0; y < 1; y++)
            {
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);

            }
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class DrawingTest : ModProjectile 
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
        }

        private int timer = 0;
        private Vector2 startingCenter = Vector2.Zero;

        public override void AI()
        {

            timer++;
        }

        float rotationVar = 0f;
        bool spriteEffectVar = false;
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
