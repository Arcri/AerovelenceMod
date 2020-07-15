using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Blocks.CrystalCaverns.Rubble
{
    public class CCRubble21 : ModTile
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
    }
}