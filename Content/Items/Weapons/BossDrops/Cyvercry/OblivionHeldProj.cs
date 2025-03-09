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

                Texture2D Trail = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Slash/pixelKennySlashTiny");
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

                Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/GlowStarPMA");
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
}
