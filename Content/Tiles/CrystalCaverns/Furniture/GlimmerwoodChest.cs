using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodChest : ModTile
    {
        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupChest(this, DustID.BlueCrystalShard, ModContent.ItemType<GlimmerwoodChestItem>(), new Color(123, 123, 123), "Crystal Chesrt");
        }

        public override bool RightClick(int i, int j)
        {
            return CommonTileHelper.HandleRightClick(this, i, j, Main.LocalPlayer, ItemID.GoldenKey);
        }

        public override void MouseOver(int i, int j)
        {
            CommonTileHelper.HandleMouseOver(this, i, j, ModContent.ItemType<GlimmerwoodChestItem>(), ItemID.GoldenKey);
        }
    }
}