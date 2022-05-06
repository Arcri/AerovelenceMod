using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class IronChain : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iron Chain");
            Tooltip.SetDefault("Makes you heavier\nNot recommended without wings");
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
            player.jumpSpeedBoost -= 2f;
            player.gravity *= 1.3f;
            player.maxFallSpeed += 5;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddRecipeGroup("IronBar", 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}