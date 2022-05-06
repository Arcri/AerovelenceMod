using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools
{
    public class PhanticAxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantic Axe");
		}
        public override void SetDefaults()
        {
			Item.crit = 4;
            Item.damage = 10;
            Item.DamageType = DamageClass.Melee;
            Item.width = 38;
            Item.height = 34;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useTurn = true;
            Item.axe = 14;
			Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}