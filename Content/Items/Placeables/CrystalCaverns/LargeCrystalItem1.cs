using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.CrystalCaverns
{
    public class LargeCrystalItem1 : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("LargeCrystalItem1");

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = Item.sellPrice(0, 0, 0, 0);
            item.consumable = true;
            item.createTile = mod.TileType("LargeCrystal1");
        }

        public override void AddRecipes()
        {
            var modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<Glimmerwood>(), 3);
            modRecipe.AddIngredient(ModContent.ItemType<CavernCrystal>(), 1);
            modRecipe.AddTile(ModContent.TileType<CrystallineFabricator>());
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}
