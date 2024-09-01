using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using AerovelenceMod.Common.Utilities;
using Terraria.ID;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodTable : ModTile
    {
        public override void SetStaticDefaults()
        {
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            CommonTileHelper.SetupTable(this, ModContent.ItemType<GlimmerwoodTableItem>());
            AddMapEntry(new Color(123, 123, 123), Language.GetText("MapObject.Table"));
        }
    }
}