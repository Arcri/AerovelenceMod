using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Arsenal
{
    public class MilitaryMetalPlatformItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Military Metal Platform");
        }
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
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.consumable = true;
            //Item.createTile = ModContent.TileType<MilitaryMetalPlatform>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ModContent.ItemType<MilitaryMetalItem>(), 1)
                .Register();
        }
    }
}
