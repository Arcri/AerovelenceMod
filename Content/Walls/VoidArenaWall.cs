using AerovelenceMod.Content.Items.Placeables.Walls;
using AerovelenceMod.Core;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Walls
{
    public class VoidArenaWall : ModWall
    {
        public override void SetDefaults() => this.SimpleWall(ModContent.ItemType<VoidArenaWallItem>(), SoundID.Dig,
            DustID.Dirt, new Color(54, 87, 129));

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}