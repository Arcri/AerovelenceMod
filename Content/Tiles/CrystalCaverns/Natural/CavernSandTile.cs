using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    [LegacyName("CavernSand")]
    public class CavernSandTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            MineResist = 2.5f;
            Main.tileSolid[Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("CrystalGrass").Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("CavernCrystal").Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("CavernStone").Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("FieldStone").Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(90, 90, 120));
            DustType = 116;
            HitSound = SoundID.Dig;
            TileID.Sets.GeneralPlacementTiles[Type] = false;
        }
        public override bool CanExplode(int i, int j)
        {
            return true;
        }
    }

    public class CavernSandItem : ModItem
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
            Item.createTile = ModContent.TileType<CavernSandTile>();
            Item.rare = ItemRarityID.White;
            Item.value = 5;
        }
    }
}
