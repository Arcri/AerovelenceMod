using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodChair : ModTile
    {
        public const int NextStyleHeight = 40;

        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupChair(this, DustID.BlueCrystalShard, ModContent.ItemType<GlimmerwoodChairItem>(), new Color(123, 123, 123));
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            CommonTileHelper.ModifySittingTargetInfo(i, j, ref info, NextStyleHeight);
        }

        public override bool RightClick(int i, int j)
        {
            CommonTileHelper.HandleChairRightClick(this, i, j);
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                return;
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<GlimmerwoodChairItem>();

            if (Main.tile[i, j].TileFrameX / 18 < 1)
            {
                player.cursorItemIconReversed = true;
            }
        }
    }
}