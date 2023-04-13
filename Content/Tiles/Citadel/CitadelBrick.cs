using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.Citadel
{
    public class CitadelBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
			MineResist = 2.5f;
			MinPick = 180;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
			AddMapEntry(new Color(061, 079, 110));
			DustType = 59;
			HitSound = SoundID.Tink;
            ItemDrop = ModContent.ItemType<CitadelBrickItem>();

        }

        public class CitadelBrickItem : ModItem
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
                Item.createTile = ModContent.TileType<CitadelBrick>();
                Item.rare = ItemRarityID.White;
                Item.value = 5;
            }
        }
    }
}
