using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using AerovelenceMod.Items.Placeable.Ores;

namespace AerovelenceMod.Blocks.Ores
{
    public class SlateOreBlock : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 1f;
			minPick = 35;
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
            drop = ModContent.ItemType<SlateOreItem>();
			dustType = 4;
			soundType = SoundID.Tink;			
            AddMapEntry(new Color(108, 114, 116), Language.GetText("Slate Slab"));
        }
    }
}