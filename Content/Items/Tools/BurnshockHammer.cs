using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools
{
    public class BurnshockHammer : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnshock Hammer");
		}
        public override void SetDefaults()
        {
			Item.crit = 5;
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.width = 52;
            Item.height = 48;
            Item.useTime = 25;
            Item.useTurn = true;
            Item.useAnimation = 25;
			Item.hammer = 90;
			Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 35, 0);
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