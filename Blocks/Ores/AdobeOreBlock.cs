using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using AerovelenceMod.Items.Tiles;

namespace AerovelenceMod.Blocks.Ores
{
    public class AdobeOreBlock : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 45;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
            drop = ModContent.ItemType<AdobeOreItem>();
			dustType = 10;
			soundType = SoundID.Tink;
			
			
            AddMapEntry(new Color(070, 036, 039), Language.GetText("Adobe Slab"));

        }
    }
}