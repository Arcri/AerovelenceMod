using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Placeable.CrystalCaverns
{
    public class CavernCrystalItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cavern Crystal");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = Item.sellPrice(0, 0, 1, 50);
            item.createTile = mod.TileType("CavernCrystal"); //put your CustomBlock Tile name
            item.consumable = true;
        }
    }
}