using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class EmeraldEmpoweredGem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Emerald Empowered Gem");
			Tooltip.SetDefault("Throwing and ranged weapons now cause a short Cursed Inferno");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 20;
            item.height = 20;
            item.value = 60000;
            item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.rangedDamage += 0.1f;
            AeroPlayer modPlayer = (AeroPlayer)player.GetModPlayer(mod, "AeroPlayer");
            modPlayer.EmeraldEmpoweredGem = true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FallenStar, 3);
            recipe.AddIngredient(ItemID.Emerald, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}