using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.DataStructures;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
    public class ESABall : ModProjectile
    {
        public int timer = 0;
        bool TrueChaseX = true;
        bool startSmaller = false;

        String mode = "";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("ESA Ball");
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 42;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.damage = 54;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        float quadrant = 0;
        
        //I should probably use an enum for this, but it is like 1 AM rn so I'm too lazy
        //I tried to code this whole thing not miserably but it didn't work for some reason
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.9f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.7f / 255f);
            Player player = Main.player[(int)Projectile.ai[0]];
            Projectile.timeLeft++;
            

            if (timer == 0)
            {
                quadrant = getQuadrant(player);

                if (TrueChaseX && (quadrant == 1 || quadrant == 4)) mode = "RightToLeft";
                if (TrueChaseX && (quadrant == 2 || quadrant == 3)) mode = "LeftToRight";

                if (!TrueChaseX && (quadrant == 2 || quadrant == 1)) mode = "UpToDown";
                if (!TrueChaseX && (quadrant == 3 || quadrant == 3)) mode = "DownToUp";

            }

            if (mode.Equals("LeftToRight"))
            {
                Projectile.velocity.X += 0.2f;

                if (Projectile.Center.X > player.Center.X)
                    Clear();
            } 
            else if (mode.Equals("RightToLeft"))
            {
                Projectile.velocity.X -= 0.2f;

                if (Projectile.Center.X < player.Center.X)
                    Clear();
            }
            else if (mode.Equals("UpToDown"))
            {
                Projectile.velocity.Y += 0.2f;

                if (Projectile.Center.Y > player.Center.Y)
                    Clear();
            }
            else if (mode.Equals("DownToUp"))
            {
                Projectile.velocity.Y -= 0.2f;
                if (Projectile.Center.Y < player.Center.Y)
                    Clear();
            }



            #region old
            /*
            if (TrueChaseX)
            {
                if (timer == 0)
                    startSmaller = (player.Center.X > Projectile.Center.X);

                if (player.Center.X > Projectile.Center.X)
                {
                    if (timer % 10 == 0)
                        CombatText.NewText(new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y, 1, 1), Color.White, "RIGHT");
                    Projectile.velocity.X += 0.06f;
                }
                else
                {
                    if (timer % 10 == 0)
                        CombatText.NewText(new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y, 1, 1), Color.White, "LEFT");
                    Projectile.velocity.X -= 0.06f;
                }
            }
            else
            {

                if (timer == 0)
                    startSmaller = (player.Center.Y > Projectile.Center.Y);

                if (player.Center.Y > Projectile.Center.Y)
                {
                    if (timer % 10 == 0)
                        CombatText.NewText(new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y, 1, 1), Color.White, "DOWN");
                    Projectile.velocity.Y += 0.06f;
                }
                else
                {
                    if (timer % 10 == 0)
                        CombatText.NewText(new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y, 1, 1), Color.White, "UP");
                    Projectile.velocity.Y -= 0.06f;
                }

            }

            bool leftToRight = (Projectile.Center.X > player.Center.X && startSmaller);
            bool rightToLeft = (Projectile.Center.X < player.Center.X && !startSmaller);
            if (leftToRight || rightToLeft)
            {
                startSmaller = !startSmaller;

                TrueChaseX = false;
                timer = -1;
                Projectile.velocity.X = 0;
            }

            bool upToDown = (Projectile.Center.Y > player.Center.Y && startSmaller);
            bool downToUp = (Projectile.Center.Y < player.Center.Y && !startSmaller);
            if (upToDown || downToUp)
            {
                startSmaller = !startSmaller;

                TrueChaseX = true;
                timer = -1;
                Projectile.velocity.Y = 0;
            }
            */
            #endregion

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }

            timer++;
        }

        public float getQuadrant(Player myPlayer)
        {
            bool LesserX = Projectile.Center.X < myPlayer.Center.X;
            bool LesserY = Projectile.Center.Y < myPlayer.Center.Y;

            if (!LesserX && LesserY) return 1;
            if (LesserX && LesserY)  return 2;
            if (LesserX && !LesserY) return 3;
            if (!LesserX && !LesserY) return 4;

            return -1;
        }

        public void Clear()
        {
            TrueChaseX = !TrueChaseX;
            Projectile.Center = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10f + Projectile.Center;
            Projectile.velocity = Vector2.Zero;
            timer = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            //Draw the Circlular Glow
            var Tex = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.HotPink * 0.5f, Projectile.rotation, Tex.Size() / 2, 1f, SpriteEffects.None, 0f);
            return true;
        }

    }

    public class DifferentExplodeBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Ball");
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 42;
            Projectile.timeLeft = 100;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
        }

        public override bool? CanDamage() { return false; }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public float rotOffset = 0f;
        public int numberOfLasers = 9;
        float lineScale = 1;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.9f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.7f / 255f);
            Projectile.rotation = 0;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            Projectile.velocity *= 0.92f;

            if (Projectile.ai[0] == 1)
            {
                rotOffset = Main.rand.NextFloat(6.28f);
            }

            Main.NewText(Projectile.ai[0]);

            Projectile.ai[0]++;
        }
        public override void Kill(int timeLeft)
        {
            var entitySource = Projectile.GetSource_FromAI();

            SoundEngine.PlaySound(SoundID.Item94 with { Pitch = 0.4f, Volume = 0.35f, PitchVariance = 0.2f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item91 with { Pitch = 0.4f }, Projectile.Center);
            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_explosive_trap_explode_1") with { PitchVariance = .16f, Volume = 0.8f, Pitch = 0.7f };
            SoundEngine.PlaySound(style, Projectile.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                //int numberOfLasers = 9;
                for (int i = 0; i < 360; i += 360 / numberOfLasers)
                {

                    int proj = Projectile.NewProjectile(entitySource, Projectile.Center, new Vector2(0.4f, 0).RotatedBy(MathHelper.ToRadians(i) + rotOffset), ModContent.ProjectileType<StretchLaser>(), Projectile.damage, 0, Main.myPlayer);
                    Main.projectile[proj].timeLeft = 400;
                    if (Main.projectile[proj].ModProjectile is StretchLaser laser)
                    {
                        laser.accelerateTime = 200;
                        laser.accelerateStrength = 1.025f; //1.025
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D circle2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_05");
            Texture2D Line = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/Medusa_Gray");


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.DeepPink * 0.7f, Projectile.rotation, circle2.Size() / 2, Projectile.scale * 0.3f, 0, 0f);

            int numberOfLasers = 9;
            for (int i = 0; i < 360; i += 360 / numberOfLasers)
            {
                Vector2 vec2Scale = new Vector2(lineScale, lineScale * 0.5f);

                Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.HotPink, MathHelper.ToRadians(i) + rotOffset, new Vector2(0, Line.Height / 2), vec2Scale, SpriteEffects.None, 0f);


            }


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }
    }

    public class DookieTelegraph : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1; 
            Projectile.timeLeft = 100;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() { return false; }

        public float yScale = 0.25f;
        public float overallScale = 5f;
        public float alpha = 1f;
        public float rot = 0f;

        float whiteAmount = 1f;

        int timer = 0;
        public override void AI()
        {
            if (timer == 0) rot = Projectile.rotation;

            if (timer > 15)
            {
                //alpha -= 0.03f;

                //if (alpha <= 0f)
                //Projectile.active = false;

                yScale = Math.Clamp(MathHelper.Lerp(yScale, -0.4f, 0.04f), 0, 1);

                if (yScale <= 0)
                    Projectile.active = false;
            }

            Projectile.velocity = Vector2.Zero;

            whiteAmount = Math.Clamp(MathHelper.Lerp(whiteAmount, -0.25f, 0.04f), 0, 1);

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Line = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/Medusa_Gray");

            Vector2 vec2Scale = new Vector2(overallScale, overallScale * yScale);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Normal
            Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.HotPink * alpha, rot, new Vector2(0, Line.Height / 2), vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.HotPink * alpha, rot + MathHelper.Pi, new Vector2(0, Line.Height / 2), vec2Scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.HotPink * alpha, rot, new Vector2(0, Line.Height / 2), vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.HotPink * alpha, rot + MathHelper.Pi, new Vector2(0, Line.Height / 2), vec2Scale, SpriteEffects.None, 0f);


            //White
            Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.White * whiteAmount, rot, new Vector2(0, Line.Height / 2), vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.White * whiteAmount, rot + MathHelper.Pi, new Vector2(0, Line.Height / 2), vec2Scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.White * whiteAmount, rot, new Vector2(0, Line.Height / 2), vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.White * whiteAmount, rot + MathHelper.Pi, new Vector2(0, Line.Height / 2), vec2Scale, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
} 