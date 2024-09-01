using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodCandle : ModTile
    {
        private Asset<Texture2D> flameTexture;

        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupCandle(this, new Color(123, 123, 123), ModContent.ItemType<GlimmerwoodCandleItem>(), 0.9f, 0.9f, 0.9f, "AerovelenceMod/Content/Tiles/CrystalCaverns/Furniture/GlimmerwoodCandle_Flame", ref flameTexture);

            flameTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Furniture/GlimmerwoodTorch_Flame");
        }
    }
}