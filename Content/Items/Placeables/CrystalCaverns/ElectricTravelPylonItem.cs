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
            Tooltip.SetDefault("Right-Click to become electricity and travel to the linked Pylon\nShift-Right-Click to open a link, and then do it again on another Pylon to estabilish it\nIf you hold Caps Lock while estabilishing a link it will automatically make it 2-way");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 48;
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
