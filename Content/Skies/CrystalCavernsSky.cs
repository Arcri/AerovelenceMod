using AerovelenceMod.Content.Biomes;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Skies
{
    public class CrystalCavernsSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        const float increment = 0.01f;
        private bool deactivating = false;

        public override void Activate(Vector2 position, params object[] args)
        {
            deactivating = false;
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            deactivating = true;
            intensity -= increment;
            if (intensity <= 0f)
            {
                intensity = 0f;
                isActive = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            spriteBatch.Draw(AerovelenceMod.Instance.Assets.Request<Texture2D>("Content/Skies/CrystalCavernsSky", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * (intensity));
        }

        public override bool IsActive()
        {
            return isActive;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive && !deactivating)
            {
                intensity += increment;
                /*if (intensity > 1f)
                {
                    intensity = MathHelper.Lerp(intensity, 1, 0.2f); //1f;
                }*/
            }
            else
            {
                Deactivate();
            }

            intensity = Math.Clamp(intensity, 0, 0.3f);
        }

        public override Color OnTileColor(Color color)
        {
            float amount = intensity * 1.25f;
            float redMod = amount;
            float greenMod = amount * 1.25f;
            float blueMod = amount * 0.875f;
            return color.MultiplyRGB(new Color(1f - redMod, 1f - greenMod, 1f - blueMod));
        }
    }
}
