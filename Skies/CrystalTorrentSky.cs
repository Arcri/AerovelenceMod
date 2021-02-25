using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace AerovelenceMod.Skies
{
    public class CrystalTorrentSky : CustomSky
    {
        private Random _random = new Random();
        private bool _isActive;

        public override void OnLoad()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override Color OnTileColor(Color inColor)
        {
            return new Color(077, 077, 091);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(077, 077, 091) * 0.24f);
            }
        }

        public override float GetCloudAlpha()
        {
            return 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            _isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            _isActive = false;
        }

        public override void Reset()
        {
            _isActive = false;
        }

        public override bool IsActive()
        {
            return _isActive;
        }
    }
}