using AerovelenceMod.Items.Others.Crafting;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using AerovelenceMod.Items.Placeable.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.AmbrosiaMiningSet
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
			item.accessory = true;
            item.width = 22;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Blue;
        }
        public override void UpdateAccessory(Player player, bool isVisible)
        {
            player.moveSpeed += 0.05f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AerovelenceMod:GoldBars", 12);
            recipe.AddIngredient(ModContent.ItemType<CavernCrystalItem>(), 5);
            recipe.AddIngredient(ItemID.Ruby, 2);
            recipe.AddIngredient(ItemID.Topaz, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}