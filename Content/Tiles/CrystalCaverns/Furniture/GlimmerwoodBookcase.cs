using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodBookcase : ModTile
    {
        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupBookcase(this, new Color(123, 123, 123), ModContent.ItemType<GlimmerwoodBookcaseItem>());
        }
    }
}