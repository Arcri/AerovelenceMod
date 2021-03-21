using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class DiamondEmpoweredGem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Diamond Empowered Gem");
			Tooltip.SetDefault("All damage increased by 2%");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 26;
            item.height = 26;
            item.value = 60000;
            item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.meleeDamage += 0.1f;
			player.minionDamage += 0.1f;
			player.magicDamage += 0.1f;
			player.rangedDamage += 0.1f;
			player.thrownDamage += 0.1f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FallenStar, 3);
            recipe.AddIngredient(ItemID.Diamond, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}