using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodSink : ModTile
    {
        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupDecorativeMultiTile(
                this,
                "MapObject.Sink",
                new Color(123, 123, 123),
                2, 2,
                ModContent.ItemType<GlimmerwoodSinkItem>()
            );
        }
    }
}