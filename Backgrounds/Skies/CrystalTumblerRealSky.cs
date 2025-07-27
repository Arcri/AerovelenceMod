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

namespace AerovelenceMod.Backgrounds.Skies
{
    public class CrystalTumblerRealSky : CustomSky
    {
        private bool skyActive;
        private float opacity;

        public override void Deactivate(params object[] args)
        {
            skyActive = false;
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override bool IsActive()
        {
            return skyActive || opacity > 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            skyActive = true;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (skyActive && opacity < 1f)
            {
                opacity += 0.01f;
            }
            else if (!skyActive && opacity > 0f)
            {
                opacity -= 0.005f;
            }
        }

        public override float GetCloudAlpha()
        {
            return (1f - opacity) * 0.9f + 0.1f;
        }
    }
}