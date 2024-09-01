using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Building;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class CavernRope : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileCut[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileRope[Type] = true;
            Main.tileFrameImportant[Type] = false;

            AddMapEntry(new Color(123, 123, 123));

            DustType = DustID.Dirt;
            HitSound = SoundID.Dig;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;
    }

    public class CavernRopeItem : ModItem
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
            Item.createTile = ModContent.TileType<CavernRope>();
            Item.rare = ItemRarityID.White;
            Item.value = 5;
            Item.tileBoost = 3;
        }
    }
}