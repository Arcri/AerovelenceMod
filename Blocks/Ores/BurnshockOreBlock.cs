using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using AerovelenceMod.Items.Placeable.Ores;

namespace AerovelenceMod.Blocks.Ores
{
    public class BurnshockOreBlock : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 1.0f;
			minPick = 180;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileLighted[Type] = false;
            drop = ModContent.ItemType<BurnshockOreItem>();
            dustType = 59;
            soundType = SoundID.Tink;
			
			
            AddMapEntry(new Color(255, 090, 090), Language.GetText("Burnshock Ore"));
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.6f;
            b = 0.9f;
        }
    }
}