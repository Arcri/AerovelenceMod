using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;

namespace AerovelenceMod.Items.Ores.PreHM.Phantic
{
    public class PhanticOreBlock : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 45;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
            drop = ModContent.ItemType<PhanticOreItem>();
			dustType = 10;
			soundType = SoundID.Tink;
			
			
            AddMapEntry(new Color(255, 090, 090), Language.GetText("Phantic Ore"));

        }
    }
}