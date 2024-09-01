using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using Terraria.ID;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodWorkbench : ModTile
    {
        public override void SetStaticDefaults()
        {
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            CommonTileHelper.SetupWorkbench(this, ModContent.ItemType<GlimmerwoodWorkbenchItem>());
        }
    }
}