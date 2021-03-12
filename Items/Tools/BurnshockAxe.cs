using AerovelenceMod.Items.Others.Crafting;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Tools
{
    public class BurnshockAxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnshock Axe");
		}
        public override void SetDefaults()
        {
			item.crit = 4;
            item.damage = 45;
            item.melee = true;
            item.width = 54;
            item.height = 48;
            item.useTime = 27;
            item.useAnimation = 27;
			item.axe = 20;
			item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BurnshockBar>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}