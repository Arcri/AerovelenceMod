using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    public class CavernGateway : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;

            CommonTileHelper.SetupMultiTile(this, 15, 15, [16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16]);

            AddMapEntry(new Color(200, 200, 200));
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }
    }

    public class CavernGatewayItem : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<CavernGateway>(); // Reference to the 15x15 tile
            Item.placeStyle = 0; // If you have multiple styles, specify the correct one here
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 5, 0, 0); // Set the value of the item
        }

    }
}