using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Walls.CrystalCaverns.Natural
{
    public class CavernStoneWallUnsafe : ModWall
    {
        public override void SetStaticDefaults()
        {
            this.SimpleWall(ModContent.ItemType<CavernStoneWallItem>(), SoundID.Dig,
            DustID.Dirt, new Color(60, 60, 80), false);
            WallID.Sets.Conversion.Stone[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}