using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.Ores
{
    public class PhanticOreBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
			mineResist = 2.5f;
			minPick = 55;
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
            drop = ModContent.ItemType<PhanticOre>();
			dustType = 10;
			soundType = SoundID.Tink;			
            AddMapEntry(new Color(255, 090, 090), Language.GetText("Phantic Ore"));
        }
    }
}