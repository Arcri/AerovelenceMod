using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Blocks.CrystalCaverns.Rubble
{
    public class LargeCrystal3 : ModTile
	{
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = false;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CoordinateHeights = new int[]
            { 16, 16, 16,
            };
            TileObjectData.newTile.CoordinateWidth = 24;
            TileObjectData.newTile.DrawYOffset = -8;
            AddMapEntry(new Color(051, 045, 159));
            TileObjectData.addTile(Type);
        }
    }
}