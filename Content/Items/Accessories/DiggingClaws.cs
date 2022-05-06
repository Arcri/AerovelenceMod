using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class DiggingClaws : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Digging Claws");
            Tooltip.SetDefault("Increases digging speed by 15%");
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 38;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.pickSpeed -= 0.15f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.PlatinumBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}