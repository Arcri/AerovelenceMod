using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.Ores
{
    public class StarglassOreBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
			MineResist = 2.5f;
			MinPick = 55;
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
            ItemDrop = ModContent.ItemType<StarglassCluster>();
			DustType = 10;
			HitSound = SoundID.Tink;			
            AddMapEntry(new Color(255, 090, 090), Language.GetText("Starglass Cluster"));
        }
    }
}
