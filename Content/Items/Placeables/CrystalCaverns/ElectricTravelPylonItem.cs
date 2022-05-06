using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.CrystalCaverns
{
    public class ElectricTravelPylonItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Travel Pylon");
            Tooltip.SetDefault("Right-Click to become electricity and travel to the linked Pylon\nShift-Right-Click to open a link, and then do it again on another Pylon to estabilish it\nIf you hold Caps Lock while estabilishing a link it will automatically make it 2-way");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 48;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<ElectricTravelPylon>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<CavernCrystal>(), 9)
                .AddIngredient(RecipeGroupID.IronBar, 6)
                .AddIngredient(ItemID.Wire, 12)
                .AddTile(ModContent.TileType<CrystallineFabricator>())
                .Register();
        }
    }
}
