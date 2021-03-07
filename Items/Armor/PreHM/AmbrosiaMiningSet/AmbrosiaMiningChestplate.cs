using AerovelenceMod.Items.Others.Crafting;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using AerovelenceMod.Items.Placeable.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.AmbrosiaMiningSet
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
            item.value = 10;
            item.rare = ItemRarityID.Blue;
        }
        public override void UpdateEquip(Player player)
        {
            player.jumpSpeedBoost += 1.5f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AerovelenceMod:GoldBars", 15);
            recipe.AddIngredient(ModContent.ItemType<CavernCrystalItem>(), 10);
            recipe.AddIngredient(ItemID.Ruby, 3);
            recipe.AddIngredient(ItemID.Topaz, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}