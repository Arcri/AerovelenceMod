using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.AmbrosiaMiningSet
{
    [AutoloadEquip(EquipType.Body)]
    public class AmbrosiaMiningChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ambrosia Mining Chestplate");
            Tooltip.SetDefault("Increased jump height");
        }
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 18;
            item.value = Item.sellPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Blue;
        }
        public override void UpdateEquip(Player player)
        {
            player.jumpSpeedBoost += 1.5f;
        }
      /*  public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AerovelenceMod:GoldBars", 15);
            recipe.AddIngredient(ModContent.ItemType<CavernCrystal>(), 10);
            recipe.AddIngredient(ItemID.Ruby, 3);
            recipe.AddIngredient(ItemID.Topaz, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
    }
}