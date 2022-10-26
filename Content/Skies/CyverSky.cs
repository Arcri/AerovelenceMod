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
        private int[] xPos = new int[50];
        private int[] yPos = new int[50];

        private int CyverAttack = 0;

        public override void OnLoad()
        {
            this._bgTexture = ModContent.Request<Texture2D>("Terraria/Images/Misc/StarDustSky/Background", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }


        public override void Update(GameTime gameTime)
        {

            const float increment = 0.01f;
            if (CheckActive())
            {
                if (CyverAttack == -1)
                {
                    bonusIntensity = MathHelper.Lerp(bonusIntensity, 0.3f, 0.02f);
                }
                else
                {
                    bonusIntensity = MathHelper.Lerp(bonusIntensity, 0.2f, 0.2f);
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
                if (intensity < 0f)
                {
                    intensity = 0f;
                    Deactivate();
                }
            }
            timer++;
        }

        public bool CheckActive()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<Cyvercry2>())
                {
                    if (Main.npc[i].ModNPC is Cyvercry2 Cyver)
                        whichAttack(Cyver.getAttack());
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
                //Texture2D tex = AerovelenceMod.Instance.Assets.Request<Texture2D>("Content/Skies/pisschar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                //spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, Color.White * intensity * 0.10f, MathHelper.ToRadians(timer), tex.Size() / 2 + new Vector2(100,100), SpriteEffects.None, 0);

                //Code stolen from Fargo's Mutant Sky 
                if (CyverAttack == -1)
                {
                    if (--delay < 0)
                    {
                        delay = 600;
                        for (int i = 0; i < 50; i++) //update positions
                        {
                            xPos[i] = Main.rand.Next(Main.screenWidth);
                            yPos[i] = Main.rand.Next(Main.screenHeight);
                        }
                    }

                    

                    for (int i = 0; i < 50; i++) //static on screen
                    {
                        xPos[i] += 2;

                        if (xPos[i] > Main.screenWidth)
                            xPos[i] = 0;

                        int width = Main.rand.Next(3, 100);
                        spriteBatch.Draw(Terraria.GameContent.TextureAssets.BlackTile.Value, new Rectangle(xPos[i] - width / 2, yPos[i], width, 1),
                        Color.HotPink * 0.2f);
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
            return isActive || intensity > 0f;
        }

        public void whichAttack(int input)
        {
            CyverAttack = input;
        }
    }
}
