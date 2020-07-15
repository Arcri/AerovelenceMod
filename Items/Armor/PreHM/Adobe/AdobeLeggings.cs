using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.Adobe
{
    [AutoloadEquip(EquipType.Legs)]
    public class AdobeLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adobe Leggings");
            Tooltip.SetDefault("5% increased movment speed");
        }
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 22;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 6;
        }
        public override void UpdateAccessory(Player player, bool isVisible)
        {
			isVisible = true;
            player.moveSpeed += 0.05f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AdobeBrick>(), 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}