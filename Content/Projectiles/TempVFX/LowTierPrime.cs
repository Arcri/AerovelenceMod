using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.Players;
using Terraria.Map;
using ReLogic.Graphics;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns.Skylight;

namespace AerovelenceMod.Content.Projectiles.TempVFX
{
    public class LowTierPrime : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        float topPos = 0f;
        float botPos = 0f;

        Vector2 BottomVector2 = Vector2.Zero;
        Vector2 TopVector2 = Vector2.Zero;

        int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 102; //102
            Projectile.height = 102; //102
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5020; //480
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() { return false; }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);

            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            DialogHandler();

            Player myPlayer = Main.player[(int)Projectile.ai[0]];
            myPlayer.GetModPlayer<ScreenPlayer>().cutscene = true;
            myPlayer.GetModPlayer<ScreenPlayer>().ScreenGoalPos = Projectile.Center;

            Projectile.scale = 0.5f;

            topPos = Math.Clamp(MathHelper.Lerp(topPos, 58f, 0.02f), 18, 29); //140 (240)
            botPos = Math.Clamp(MathHelper.Lerp(botPos, 58f, 0.02f), 18, 29); //140 (240)

            BottomVector2 = Projectile.Center + new Vector2(0, -1 * botPos);
            TopVector2 = Projectile.Center + new Vector2(0, 1 * topPos);

            //textToDraw = CurrentLine;

            if (timer == timeToMoveOn - 30)
            {
                if (line == 1)
                {
                    Projectile.timeLeft = 20;
                }

                line++;

                textToDraw = "";
                timer = -1;
                lineIndex = 0;
            }

            if (Projectile.timeLeft <= 20)
            {
                myPlayer.GetModPlayer<ScreenPlayer>().lerpBackToPlayer = true;
                myPlayer.GetModPlayer<ScreenPlayer>().cutscene = false;
            }
            thunderVal = Math.Clamp(MathHelper.Lerp(thunderVal, -0.5f, 0.05f), 0f, 1f);
            timer++;
        }

        float thunderVal = 0f;

        String textToDraw = "";
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Bottom = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/PrimeHugeBottom").Value;
            Texture2D Top = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/PrimeHugeTop").Value;

            Main.spriteBatch.Draw(Bottom, BottomVector2 - Main.screenPosition, Bottom.Frame(1, 1, 0, 0), lightColor, Projectile.rotation, Bottom.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Top, TopVector2 - Main.screenPosition, Top.Frame(1, 1, 0, 0), lightColor, Projectile.rotation, Top.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);


            //Text
            DynamicSpriteFont myFont = FontAssets.DeathText.Value;
            Vector2 textPos = Projectile.Center + new Vector2(0, -90) - Main.screenPosition;
            Vector2 origin = (myFont.MeasureString(textToDraw) / 2f);

            Color textCol = Color.Lerp(Color.Red with { A = 0 }, Color.SkyBlue with { A = 0 }, thunderVal);

            Utils.DrawBorderStringFourWay(Main.spriteBatch, myFont, textToDraw, textPos.X, textPos.Y, Color.White, Color.Crimson * 0.85f, origin, 0.5f);
            //Utils.DrawBorderString(Main.spriteBatch, textToDraw, Projectile.Center + new Vector2(0, -100) - Main.screenPosition, Color.BlanchedAlmond, 1f, 0.5f, 0, 0);


            //Eye
            Texture2D eyeTex = Mod.Assets.Request<Texture2D>("Assets/TrailImages/PartiGlow").Value;

            Vector2 leftEyePos = TopVector2 + (new Vector2(-144, -310) / 4) * Projectile.scale; 
            Vector2 rightEyePos = TopVector2 + (new Vector2(144, -310) / 4) * Projectile.scale;


            Main.spriteBatch.Draw(eyeTex, leftEyePos - Main.screenPosition, eyeTex.Frame(1, 1, 0, 0), textCol * 0.6f, Projectile.rotation, eyeTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(eyeTex, rightEyePos - Main.screenPosition, eyeTex.Frame(1, 1, 0, 0), textCol * 0.6f, Projectile.rotation, eyeTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(eyeTex, leftEyePos - Main.screenPosition, eyeTex.Frame(1, 1, 0, 0), Color.White with { A = 0 } * 0.25f, Projectile.rotation, eyeTex.Size() / 2, Projectile.scale * 0.55f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(eyeTex, rightEyePos - Main.screenPosition, eyeTex.Frame(1, 1, 0, 0), Color.White with { A = 0 } * 0.25f, Projectile.rotation, eyeTex.Size() / 2, Projectile.scale * 0.55f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            return false;


        }

        int timeToMoveOn = 300;
        int lineIndex = 0;
        bool PreviousLetterVowel = false;
        int line = 1;

        String CurrentLine = "You should kill yourself--------- NOW!------*";
        String CurrentLineCentered = "You should kill yourself NOW!";

        public void DialogHandler()
        {
            switch (line)
            {
                
            }
            CurrentLine = "You should kill yourself--------- NOW!---------*-";
            
            if (timer % 3 == 0 && timer >= 50)
            {
                
                if (CurrentLine[lineIndex].ToString() == "*")
                {
                    Thunder();
                }

                bool isLetterVowel = CurrentLine[lineIndex].ToString().ToLower() == "a" || CurrentLine[lineIndex].ToString().ToLower() == "e" ||
                    CurrentLine[lineIndex].ToString().ToLower() == "i" || CurrentLine[lineIndex].ToString().ToLower() == "o" ||
                    CurrentLine[lineIndex].ToString().ToLower() == "u" || CurrentLine[lineIndex] == 'y';

                if (isLetterVowel && !PreviousLetterVowel)
                {
                    if (CurrentLine[lineIndex].ToString() == "O")
                    {
                        SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/p03dee") with { Pitch = -.4f, MaxInstances = -1, Volume = 1f };
                        SoundEngine.PlaySound(style, Projectile.Center);
                        SoundEngine.PlaySound(style, Projectile.Center);

                    }
                    else
                    {
                        SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/p03dee") with { Pitch = -.32f, PitchVariance = .44f, MaxInstances = -1, Volume = 2f };
                        SoundEngine.PlaySound(style, Projectile.Center);
                    }

                    PreviousLetterVowel = true;
                    topPos = 18;
                    botPos = 18;
                }
                else
                {
                    PreviousLetterVowel = false;
                }
                if (CurrentLine[lineIndex].ToString() != "-" && CurrentLine[lineIndex].ToString() != "*")
                {
                    textToDraw += CurrentLine[lineIndex];
                }
                lineIndex = Math.Clamp(lineIndex + 1, 0, CurrentLine.Length - 1);
                //check next index, if it is a vowel, play sound
                //increase the index
            }

        }

        public void Thunder()
        {
            Main.NewLightning();
            thunderVal = 1f;

            Player myPlayer = Main.player[Projectile.owner];
            //Lightning that hits you
            Vector2 outFromPlayer = new Vector2(0f, -1000f).RotatedByRandom(0.75f);
            Vector2 vel = outFromPlayer.SafeNormalize(Vector2.UnitX) * -20;
            int thunder = Projectile.NewProjectile(null, outFromPlayer + myPlayer.Center, vel, ModContent.ProjectileType<SkylightThunderStrike>(), 1000, 1, Main.myPlayer);
            Main.projectile[thunder].friendly = false;
            Main.projectile[thunder].hostile = true;


            //Other Lightning
            for (int i = -3; i < 4; i++)
            {
                Vector2 randomPos = myPlayer.Center + new Vector2((200 * i) + Main.rand.NextFloat(-80, 80), Main.rand.NextFloat(-900, -1400));
                Vector2 randomVel = new Vector2(0f, 25f).RotatedByRandom(0.25f) * Main.rand.NextFloat(0.7f, 1.55f);

                //int type = Main.rand.NextBool() ? ModContent.ProjectileType<SkylightThunderStrike>() : ModContent.ProjectileType<SkylightElectricShot>();

                //if (type == ModContent.ProjectileType<SkylightElectricShot>())
                    //randomVel *= 2f;
                int thunder2 = Projectile.NewProjectile(null, randomPos, randomVel, ModContent.ProjectileType<SkylightThunderStrike>(), 10, 1, Main.myPlayer);
            }
        }
    }
}