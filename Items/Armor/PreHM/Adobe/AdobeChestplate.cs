using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.Adobe
{
    [AutoloadEquip(EquipType.Body)]
    public class AdobeChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adobe Chestplate");
            Tooltip.SetDefault("5% increased critical damage");
        }
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 8;
        }
        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 4;
			player.rangedCrit += 4;
			player.magicCrit += 4;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AdobeBrick>(), 25);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}