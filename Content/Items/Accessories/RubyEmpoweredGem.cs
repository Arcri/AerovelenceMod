using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class RubyEmpoweredGem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ruby Empowered Gem");
			Tooltip.SetDefault("Life regen increased");
		}
        public override void SetDefaults()
        {
			Item.accessory = true;
            Item.width = 24;
            Item.height = 18;
            Item.value = 60000;
            Item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.lifeRegen =+ 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddIngredient(ItemID.Ruby, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}