using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble
{
    public class CCRubble10 : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = false;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(051, 045, 159));
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            if (!tileBelow.active() || tileBelow.halfBrick() || tileBelow.topSlope())
            {
                WorldGen.KillTile(i, j);
            }

            return true;
        }
    }
}