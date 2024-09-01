using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodPipeOrgan : ModTile
    {
        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupDecorativeMultiTile(
                this,
                "MapObject.PipeOrgan",
                new Color(123, 123, 123),
                3, 2,
                ModContent.ItemType<GlimmerwoodPipeOrganItem>()
            );
        }
    }
}