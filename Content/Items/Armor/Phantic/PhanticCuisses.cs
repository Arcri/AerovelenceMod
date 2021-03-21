using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Phantic
{
    [AutoloadEquip(EquipType.Legs)]
    public class PhanticCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantic Cuisses");
            Tooltip.SetDefault("7% increased movement speed");
        }		
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 7;
        }
        public override void UpdateEquip(Player player)
        {
			player.moveSpeed += 0.07f;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 15);
            modRecipe.AddRecipeGroup("AerovelenceMod:EvilMaterials", 15);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}