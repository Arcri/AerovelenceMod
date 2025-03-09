using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Utilities.Generation.StructureStamper
{
    public class TheTile : ModTile
    {
        public override void SetStaticDefaults()
        {
			MineResist = 1f;
			MinPick = 10;
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
			AddMapEntry(new Color(213, 0, 255));
			DustType = 59;
			HitSound = SoundID.Dig;
        }
    }

    public class TheItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = false;
            Item.createTile = ModContent.TileType<TheTile>();
            Item.rare = ItemRarities.PostML;
            Item.value = 5;
            Item.tileBoost += 20;
        }
    }
}