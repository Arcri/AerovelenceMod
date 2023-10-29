using AerovelenceMod.Common.Utilities;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using System;

namespace AerovelenceMod.Content.Skies
{
    public class CyverSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private Texture2D _bgTexture;
        private int timer = 0;
        private float bonusIntensity;

        private Vector2[] bgLines = new Vector2[100]; //50
        private int[] xPos = new int[50];
        private int[] yPos = new int[50];

        private int CyverAttack = 0;
        private float rotation = 0;
        private float bigShotTimer = 0;
        private float bgLineBoost = 0;
        private float whiteStrength = 0f;
        private float lineBonusSpeed = 0f;
        private float bonusLines = 0f;

        bool runOnce = true;

        public override void OnLoad()
        {
            //this._bgTexture = ModContent.Request<Texture2D>("Terraria/Images/Misc/StarDustSky/Background", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }


        public override void Update(GameTime gameTime)
        {
            const float increment = 0.01f;
            if (CheckActive())
            {
                if (bigShotTimer > 0) //giga shot 
                {
                    bonusIntensity = MathHelper.Lerp(bonusIntensity, 0.6f, 0.2f); //0.3
                }
                else if (CyverAttack == -1) //spamming laser
                {
                    bonusIntensity = MathHelper.Lerp(bonusIntensity, 0.4f, 0.02f); //0.3
                }
                else
                {
                    bonusIntensity = MathHelper.Lerp(bonusIntensity, 0.2f, 0.2f); //0.2
                }

                intensity += increment;
                if (intensity > 1f && CyverAttack != -1)
                {
                    intensity = MathHelper.Lerp(intensity, 1, 0.2f); //1f;
                }
            }
            else
            {
                intensity -= increment;
                if (intensity <= 0f)
                {
                    intensity = 0f;
                    Deactivate();
                }
            }

            intensity = Math.Clamp(intensity, 0, 1);

            timer++;
        }

        public bool CheckActive()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<Cyvercry2>())
                {
                    rotation = Main.npc[i].rotation;
                    if (Main.npc[i].ModNPC is Cyvercry2 Cyver)
                    {
                        whichAttack(Cyver.getAttack());
                        bigShotTimer = Cyver.bigShotTimer;
                        bgLineBoost = Cyver.extraBoost;
                        whiteStrength = Cyver.whiteBackgroundPower;
                        lineBonusSpeed = Cyver.lineBonusSpeed;
                    }
                    return true;
                }
            }
            return false;
        }
        private float delay = 0;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {

            if (maxDepth >= 0 && minDepth < 0)
            {

                spriteBatch.Draw(AerovelenceMod.Instance.Assets.Request<Texture2D>("Content/Skies/Cyversky1080", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * (intensity) * (bonusIntensity));

                spriteBatch.Draw(AerovelenceMod.Instance.Assets.Request<Texture2D>("Content/Skies/WhiteSky", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White with { A = 0 } * whiteStrength);

                //Texture2D tex = AerovelenceMod.Instance.Assets.Request<Texture2D>("Content/Skies/pisschar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                //spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, Color.White * intensity * 0.10f, MathHelper.ToRadians(timer), tex.Size() / 2 + new Vector2(100,100), SpriteEffects.None, 0);

                if (CyverAttack == -1)
                {
                    if (runOnce)
                    {
                        for (int i = 0; i < 100; i++) 
                        {
                            bgLines[i].X = Main.rand.Next(Main.screenWidth);
                            bgLines[i].Y = Main.rand.NextBool() ? Main.rand.Next(0, (int)(Main.screenHeight / 4.5f)) : Main.rand.Next((int)((Main.screenHeight / 4.5f) * 3.5), Main.screenHeight);

                            //xPos[i] = Main.rand.Next(Main.screenWidth);
                            //yPos[i] = Main.rand.Next(Main.screenHeight);
                        }
                        runOnce = false;
                    }

                    for (int i = 0; i < 50; i++) 
                    {

                        if (i % 2 == 0)
                            bgLines[i] += new Vector2(2.5f + (i / 15), 0) * (1f + lineBonusSpeed);
                        else
                            bgLines[i] -= new Vector2(2.5f + (i / 15), 0) * (1f + lineBonusSpeed);

                        if (bgLines[i].X > (Main.screenWidth + 300))
                        {
                            bgLines[i].X = -200 + Main.rand.Next(-1000, 180);
                            bgLines[i].Y = Main.rand.NextBool() ? Main.rand.Next(0, (int)(Main.screenHeight / 4.5f)) : Main.rand.Next((int)((Main.screenHeight / 4.5f) * 3.5), Main.screenHeight);
                        }
                        else if (bgLines[i].X < -300)
                        {
                            bgLines[i].X = Main.screenWidth + 200 + Main.rand.Next(-180, 1000);
                            bgLines[i].Y = Main.rand.NextBool() ? Main.rand.Next(0, (int)(Main.screenHeight / 4.5f)) : Main.rand.Next((int)((Main.screenHeight / 4.5f) * 3.5), Main.screenHeight);
                        }

                        //if (bgLines[i].Y > Main.screenHeight || bgLines[i].Y < 0)
                        //{
                        //bgLines[i].Y = Main.rand.Next(Main.screenHeight - 500);
                        //}

                        //int width = Main.rand.Next(3, 100);
                        int width = Main.rand.Next(30, 60);
                        float width2 = Main.rand.NextFloat(0.25f, 1.75f);


                        /*
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                        Texture2D bgLineTex = AerovelenceMod.Instance.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/BGLine", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                        spriteBatch.Draw(bgLineTex, bgLines[i], null, Color.White * 0.75f, rotation, bgLineTex.Size() / 2, 1f, SpriteEffects.None, 0);

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
                        */
                        //0.2f alpha is old
                        //~spriteBatch.Draw(Terraria.GameContent.TextureAssets.BlackTile.Value, new Rectangle((int)bgLines[i].X - width / 2, (int)bgLines[i].Y, width, 1), Color.HotPink * bonusIntensity * intensity);

                        //spriteBatch.Draw(AerovelenceMod.Instance.Assets.Request<Texture2D>("Assets/TrailImages/RainbowRod", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                        //new Rectangle((int)bgLines[i].X - width / 2, (int)bgLines[i].Y, width, 10), Color.HotPink with { A = 0 } * bonusIntensity * intensity);

                        Color colToUse = Color.Lerp(Color.DeepSkyBlue, Color.DeepPink, bgLines[i].Y / Main.screenHeight);

                        spriteBatch.Draw(AerovelenceMod.Instance.Assets.Request<Texture2D>("Assets/TrailImages/Starlight", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            new Vector2(bgLines[i].X, bgLines[i].Y), null, colToUse with { A = 0 } * bonusIntensity * intensity * 2f, 0, new Vector2(36,36), new Vector2(width2, 0.10f), SpriteEffects.None, 0f );

                        spriteBatch.Draw(AerovelenceMod.Instance.Assets.Request<Texture2D>("Assets/TrailImages/Starlight", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            new Vector2(bgLines[i].X, bgLines[i].Y), null, Color.White with { A = 0 } * bonusIntensity * intensity, 0, new Vector2(36, 36), new Vector2(width2, 0.10f + (2f * bgLineBoost)) * 0.5f, SpriteEffects.None, 0f);
                    }
                }
                
                //

            }

            //Terraria.GameContent.TextureAssets.BlackTile.Value

        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive;// || intensity > 0f;
        }

        public void whichAttack(int input)
        {
            CyverAttack = input;
        }
    }
}
