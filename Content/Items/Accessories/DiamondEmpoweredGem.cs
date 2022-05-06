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
			Item.accessory = true;
            Item.width = 26;
            Item.height = 26;
            Item.value = 60000;
            Item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetDamage(DamageClass.Melee) += 0.1f;
			player.GetDamage(DamageClass.Summon) += 0.1f;
			player.GetDamage(DamageClass.Magic) += 0.1f;
			player.GetDamage(DamageClass.Ranged) += 0.1f;
			player.GetDamage(DamageClass.Throwing) += 0.1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddIngredient(ItemID.Diamond, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}