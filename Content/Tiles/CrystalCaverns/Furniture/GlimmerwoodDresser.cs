using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodDresser : ModTile
    {
        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupDresser(this, DustID.BlueCrystalShard, ModContent.ItemType<GlimmerwoodDresserItem>(), new Color(123, 123, 123));
        }

        public override bool RightClick(int i, int j)
        {
            return CommonTileHelper.HandleRightClick(this, i, j, Main.LocalPlayer, ModContent.ItemType<GlimmerwoodDresserItem>());
        }

        public override void MouseOver(int i, int j)
        {
            CommonTileHelper.HandleMouseOverNearAndFarSharedLogic(Main.LocalPlayer, i, j, ModContent.ItemType<GlimmerwoodDresserItem>());
        }

        public override void MouseOverFar(int i, int j)
        {
            Player player = Main.LocalPlayer;
            CommonTileHelper.HandleMouseOverNearAndFarSharedLogic(player, i, j, ModContent.ItemType<GlimmerwoodDresserItem>());
            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }
    }
}