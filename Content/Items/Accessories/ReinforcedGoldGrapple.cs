using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class ReinforcedGoldGrapple : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reinforced Gold Grapple");
            Tooltip.SetDefault("Makes gem hooks reach further\nGem hooks also provide a light source");
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
            player.GetModPlayer<AeroPlayer>().UpgradedHooks = true;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.GoldBar, 5);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}