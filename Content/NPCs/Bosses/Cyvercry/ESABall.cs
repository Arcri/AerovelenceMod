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
            DisplayName.SetDefault("ESA Ball");
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
} 