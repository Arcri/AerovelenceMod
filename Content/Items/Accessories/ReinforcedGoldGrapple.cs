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
            Item.accessory = true;
            Item.width = 38;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AeroPlayer>().UpgradedHooks = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.GoldBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}