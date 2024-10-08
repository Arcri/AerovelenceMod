using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    public class ArenaCavernCrystal : ModTile
    {
        public override void SetStaticDefaults()
        {
            MineResist = 2.5f;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CrystalGrass").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CavernCrystal").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CavernStone").Type] = true;
          //  Main.tileMerge[Type][Mod.Find<ModTile>("FieldStone").Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(102, 108, 117));
            DustType = 116;
            HitSound = SoundID.Tink;
            //ItemDrop/* tModPorter Note: Removed. Tiles and walls will drop the item which places them automatically. Use RegisterItemDrop to alter the automatic drop if necessary. */ = ModContent.ItemType<CavernSandItem>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.6f;
            b = 0.9f;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }

    public class ArenaCavernCrystalItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<ArenaCavernCrystal>();
            Item.rare = ItemRarityID.White;
            Item.value = 5;
        }
    }
}
