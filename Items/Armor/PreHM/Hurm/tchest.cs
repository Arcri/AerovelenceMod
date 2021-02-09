using AerovelenceMod.Items.Others.Crafting;
using AerovelenceMod.Items.Placeable.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.Hurm
{
    [AutoloadEquip(EquipType.Body)]
    public class tchest : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hurm Ceeehoost");
            Tooltip.SetDefault("3% increased critical strike chance");
        }
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Blue;
            item.defense = 4;
        }
        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 3;
			player.rangedCrit += 3;
			player.magicCrit += 3;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SlateOreItem>(), 65);
            recipe.AddRecipeGroup("Wood", 25);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}