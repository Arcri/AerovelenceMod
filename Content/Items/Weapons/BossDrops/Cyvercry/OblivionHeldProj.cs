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

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
    public class OblivionHeldProj : ModProjectile
    {
        public override bool ShouldUpdatePosition() => Projectile.ai[1] == 4;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Oblivion");
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
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
                    Main.npc[i].StrikeNPC(Projectile.damage, Projectile.knockBack, Direction);
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
            DisplayName.SetDefault("Cyver Pulse");
        }

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

            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.51f, 0.2f); //1.51
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

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
