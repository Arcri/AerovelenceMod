using AerovelenceMod.Items.Others.Crafting;
using AerovelenceMod.Items.Placeable.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.Slate
{
    [AutoloadEquip(EquipType.Legs)]
    public class SlateLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slate Leggings");
            Tooltip.SetDefault("5% increased movment speed");
        }
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Blue;
            item.defense = 4;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.05f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SlateOreItem>(), 55);
            recipe.AddRecipeGroup("Wood", 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}