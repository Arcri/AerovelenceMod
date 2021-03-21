using AerovelenceMod.Content.Items.Placeables.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Slate
{
    [AutoloadEquip(EquipType.Body)]
    public class SlateChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slate Chestplate");
            Tooltip.SetDefault("3% increased critical strike chance");
        }
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Blue;
            item.defense = 5;
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
            recipe.AddIngredient(ModContent.ItemType<SlateOre>(), 65);
            recipe.AddRecipeGroup("Wood", 25);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}