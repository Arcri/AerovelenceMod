using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodLamp : ModTile
    {
        private Asset<Texture2D> flameTexture;

        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupLamp(this,
                new Color(123, 123, 123),
                ModContent.ItemType<GlimmerwoodLampItem>(),
                1f,
                0.75f,
                1f,
                "AerovelenceMod/Content/Tiles/CrystalCaverns/Furniture/GlimmerwoodLamp_Flame",
                ref flameTexture);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CommonTileHelper.HandlePostDraw(flameTexture, i, j, spriteBatch, true);
        }
    }
}