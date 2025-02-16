/*
using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles
{
    public class AfflictionCrystal : ModTile
    {
        public override void SetStaticDefaults()
        {
			MineResist = 2.5f;
			MinPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CrystalDirt").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CrystalGrass").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CavernStone").Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
			AddMapEntry(new Color(061, 079, 110));
			DustType = 59;
			HitSound = SoundID.Tink;
            ItemDrop = ModContent.ItemType<Items.Placeables.Blocks.AfflictionCrystal>();

        }
    }
}
*/