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
            item.accessory = true;
            item.width = 38;
            item.height = 32;
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.pickSpeed -= 0.15f;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.PlatinumBar, 5);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}