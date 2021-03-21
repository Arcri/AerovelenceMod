using AerovelenceMod.Content.Items.Placeables.Walls;
using AerovelenceMod.Core;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Walls
{
    public class LushGrowthWall: ModWall
    {
        public override void SetDefaults() => this.SimpleWall(ModContent.ItemType<LushGrowthWallItem>(), SoundID.Dig,
            DustID.Dirt, new Color(18, 78, 23));

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}