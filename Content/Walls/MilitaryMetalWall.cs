using AerovelenceMod.Core;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Walls
{
    public class MilitaryMetalWall : ModWall
    {
        public override void SetDefaults() => this.SimpleWall(0, SoundID.Dig, DustID.Dirt, new Color(54, 87, 129));

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}