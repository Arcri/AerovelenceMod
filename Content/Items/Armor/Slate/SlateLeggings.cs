using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Slate
{
    [AutoloadEquip(EquipType.Legs)]
    public class SlateLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slate Leggings");
            // Tooltip.SetDefault("5% increased movement speed");
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.05f;
        }
        public override void AddRecipes()
        {
            /*
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<SlateOre>(), 55)
                .AddRecipeGroup("Wood", 20)
                .AddTile(TileID.Anvils)
                .Register();
            */
        }
    }
}