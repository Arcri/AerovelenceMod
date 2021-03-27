using AerovelenceMod.Content.Items.Placeables.Arsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.Arsenal
{
    public class MilitaryMetal : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
			AddMapEntry(new Color(061, 079, 110));
			dustType = 59;
			soundType = SoundID.Tink;
            drop = ModContent.ItemType<MilitaryMetalItem>();

        }
    }
}