using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using AerovelenceMod.Items.Placeable.Ores;

namespace AerovelenceMod.Blocks.Ores
{
    public class VoidstoneOreBlock : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 45;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
            Main.tileMerge[Type][mod.TileType("VoidGrass")] = true;
            Main.tileMerge[Type][mod.TileType("VoidSilt")] = true;
            Main.tileMerge[Type][mod.TileType("VoidStone")] = true;
            Main.tileMerge[Type][mod.TileType("VoidDirt")] = true;
            drop = ModContent.ItemType<AdobeOreItem>();
			dustType = 10;
			soundType = SoundID.Tink;
			
			
            AddMapEntry(new Color(070, 036, 039), Language.GetText("Voidstone"));

        }
    }
}