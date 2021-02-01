using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using AerovelenceMod.Items.Placeable.FrostDungeon;
using Terraria.ID;
using AerovelenceMod.Items.Placeable.Void;

namespace AerovelenceMod.Blocks.Void.Tiles
{
    public class VoidSilt : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 200;
            Main.tileMerge[Type][mod.TileType("VoidGrass")] = true;
            Main.tileMerge[Type][mod.TileType("VoidStone")] = true;
            Main.tileMerge[Type][mod.TileType("VoidDirt")] = true;
            Main.tileMerge[Type][mod.TileType("VoidstoneOreBlock")] = true;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
			dustType = 59;
			soundType = SoundID.Tink;
			drop = ModContent.ItemType<VoidSiltItem>();
			
            AddMapEntry(new Color(076, 235, 228)); // localized text for "Metal Bar"

        }
    }
}