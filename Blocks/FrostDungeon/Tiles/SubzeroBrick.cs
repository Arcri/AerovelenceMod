using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using AerovelenceMod.Items.Placeble.FrostDungeon;

namespace AerovelenceMod.Blocks.FrostDungeon.Tiles
{
    public class SubzeroBrick : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 200;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
			dustType = 59;
			soundType = 21;
			drop = ModContent.ItemType<SubzeroBrickItem>();
			
            AddMapEntry(new Color(076, 235, 228), Language.GetText("Subzero Brick")); // localized text for "Metal Bar"

        }
    }
}