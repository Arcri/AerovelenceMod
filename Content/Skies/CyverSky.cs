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

        public override void OnLoad()
        {
            this._bgTexture = ModContent.Request<Texture2D>("Terraria/Images/Misc/StarDustSky/Background", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }


        public override void Update(GameTime gameTime)
        {

            const float increment = 0.01f;
            if (CheckActive())
            {
                intensity += increment;
                if (intensity > 1f)
                {
                    intensity = 1f;
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
                    return true;
                }
            }
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            /*
            if (maxDepth >= 3E+38f && minDepth < 3E+38f)
            {
                spriteBatch.Draw(AerovelenceMod.Instance.Assets.Request<Texture2D>("Content/Skies/piss1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * intensity * 0.30f);
                //spriteBatch.Draw(AerovelenceMod.Instance.Assets.Request<Texture2D>("Content/Skies/RuinedKingdomSky", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            }
            */
            /*
            this._bgTexture = ModContent.Request<Texture2D>("Terraria/Images/Misc/VortexSky/Background", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                spriteBatch.Draw(this._bgTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - (double)Main.screenPosition.Y - 1500.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), new Color(94, 255, 250, 240) * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * this.intensity));
                Vector2 value = new Vector2((float)(Main.screenWidth >> 1), (float)(Main.screenHeight >> 1));
                Vector2 value2 = 0.01f * (new Vector2((float)Main.maxTilesX * 8f, (float)Main.worldSurface / 2f) - Main.screenPosition);
            }
            */
            if (maxDepth >= 0 && minDepth < 0)
            {
                spriteBatch.Draw(AerovelenceMod.Instance.Assets.Request<Texture2D>("Content/Skies/Cyversky1080", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * intensity * 0.2f);
                //Texture2D tex = AerovelenceMod.Instance.Assets.Request<Texture2D>("Content/Skies/pisschar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                //spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, Color.White * intensity * 0.10f, MathHelper.ToRadians(timer), tex.Size() / 2 + new Vector2(100,100), SpriteEffects.None, 0);

            }

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

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

    }
}
