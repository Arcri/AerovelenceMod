using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.Ores
{
    public class BurnshockOreBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
			MineResist = 1.0f;
			MinPick = 180;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileLighted[Type] = false;
            ItemDrop = ModContent.ItemType<BurnshockOre>();
            DustType = 59;
            SoundType = SoundID.Tink;
			
			
            AddMapEntry(new Color(255, 090, 090), Language.GetText("Burnshock Ore"));
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.6f;
            b = 0.9f;
        }
    }
}