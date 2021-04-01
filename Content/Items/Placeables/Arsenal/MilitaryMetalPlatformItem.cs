using AerovelenceMod.Content.Tiles.Arsenal;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Arsenal
{
    public class MilitaryMetalPlatformItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Military Metal Platform");
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
            item.value = Item.sellPrice(0, 0, 0, 0);
            item.consumable = true;
            item.createTile = ModContent.TileType<MilitaryMetalPlatform>();
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<MilitaryMetalItem>(), 1);
            modRecipe.SetResult(this, 2);
            modRecipe.AddRecipe();
        }
    }
}
