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
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 7;
        }
        public override void UpdateEquip(Player player)
        {
			player.moveSpeed += 0.07f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 15)
                .AddRecipeGroup("AerovelenceMod:EvilMaterials", 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}