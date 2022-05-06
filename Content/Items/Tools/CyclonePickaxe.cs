using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools
{
    public class CyclonePickaxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cyclone Pickaxe");
		}
        public override void SetDefaults()
        {
			Item.crit = 4;
            Item.damage = 33;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 20;
            Item.useAnimation = 20;
			Item.pick = 200;
			Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<BurnshockBar>(), 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}