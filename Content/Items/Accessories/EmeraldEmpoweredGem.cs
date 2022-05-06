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
			Item.accessory = true;
            Item.width = 20;
            Item.height = 20;
            Item.value = 60000;
            Item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetDamage(DamageClass.Ranged) += 0.1f;
            AeroPlayer modPlayer = (AeroPlayer)player.GetModPlayer(Mod, "AeroPlayer");
            modPlayer.EmeraldEmpoweredGem = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddIngredient(ItemID.Emerald, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}