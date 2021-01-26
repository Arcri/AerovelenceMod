using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
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
            item.accessory = true;
            item.width = 38;
            item.height = 32;
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.jumpSpeedBoost -= 2f;
            player.gravity *= 1.3f;
            player.maxFallSpeed += 5;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddRecipeGroup("IronBar", 12);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}