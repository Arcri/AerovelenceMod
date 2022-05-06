using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools
{
    public class BurnshockAxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnshock Axe");
		}
        public override void SetDefaults()
        {
			Item.crit = 4;
            Item.damage = 45;
            Item.DamageType = DamageClass.Melee;
            Item.width = 54;
            Item.height = 48;
            Item.useTime = 27;
            Item.useAnimation = 27;
			Item.axe = 20;
			Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
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
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}