using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodSofa : ModTile
    {
        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupSofa(this, new Color(123, 123, 123), ModContent.ItemType<GlimmerwoodSofaItem>());
        }
    }
}