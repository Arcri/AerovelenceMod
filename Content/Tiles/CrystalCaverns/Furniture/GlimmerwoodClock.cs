using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodClock : ModTile
    {
        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupClock(this, DustID.BlueCrystalShard, new Color(123, 123, 123));
        }

        public override bool RightClick(int x, int y)
        {
            return CommonTileHelper.HandleClockRightClick(x, y);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}