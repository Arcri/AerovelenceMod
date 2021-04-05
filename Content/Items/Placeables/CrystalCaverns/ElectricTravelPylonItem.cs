using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.CrystalCaverns
{
    public class ElectricTravelPylonItem : AerovelenceItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Travel Pylon");
            Tooltip.SetDefault("Shift-Right-Clicking the Pylon will open a Link, doing that again with a second Pylon will estabilish the Link, and you'll be able to become electricity and travel between them by Right-Clicking.");
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
            item.consumable = true;
            item.createTile = ModContent.TileType<ElectricTravelPylon>();
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<CrystalShard>(), 9);
            modRecipe.AddIngredient(RecipeGroupID.IronBar, 6);
            modRecipe.AddIngredient(ItemID.Wire, 12);
            modRecipe.AddTile(ModContent.TileType<CrystallineFabricator>());
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}
