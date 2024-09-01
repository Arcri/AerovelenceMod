using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodToilet : ModTile
    {
        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupToilet(this);
        }
    }
}