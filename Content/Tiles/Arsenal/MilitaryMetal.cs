using AerovelenceMod.Content.Items.Placeables.Arsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.Arsenal
{
    public class MilitaryMetal : ModTile
    {
        public override void SetStaticDefaults()
        {
			MineResist = 2.5f;
			MinPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
			AddMapEntry(new Color(061, 079, 110));
			DustType = 59;
			HitSound = SoundID.Tink;
            ItemDrop = ModContent.ItemType<MilitaryMetalItem>();

        }
    }
}
