using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.Ores
{
    public class SlateOreBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
			MineResist = 1f;
			MinPick = 35;
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
            ItemDrop = ModContent.ItemType<SlateOre>();
			DustType = 4;
			SoundType = SoundID.Tink;			
            AddMapEntry(new Color(108, 114, 116), Language.GetText("Slate Slab"));
        }
    }
}