using AerovelenceMod.Content.Items.Placeables.Walls;
using AerovelenceMod.Core;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Walls
{
    public class CavernDirtWall : ModWall
	{
		public override void SetStaticDefaults() => this.SimpleWall(ModContent.ItemType<CavernDirtWallItem>(), SoundID.Dig,
			DustID.Dirt, new Color(54, 87, 129), true);

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}