using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.AmbrosiaMiningSet
{
    [AutoloadEquip(EquipType.Legs)]
    public class AmbrosiaMiningBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ambrosia Mining Boots");
            Tooltip.SetDefault("5% increased movment speed");
        }
        public override void SetDefaults()
        {
			Item.accessory = true;
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }
        public override void UpdateAccessory(Player player, bool isVisible)
        {
            player.moveSpeed += 0.05f;
        }
      /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AerovelenceMod:GoldBars", 12);
            recipe.AddIngredient(ModContent.ItemType<CavernCrystal>(), 5);
            recipe.AddIngredient(ItemID.Ruby, 2);
            recipe.AddIngredient(ItemID.Topaz, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
    }
}