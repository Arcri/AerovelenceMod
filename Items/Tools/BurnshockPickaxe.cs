using AerovelenceMod.Items.Others.Crafting;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Tools
{
    public class BurnshockPickaxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnshock Pickaxe");
		}
        public override void SetDefaults()
        {
			item.crit = 4;
            item.damage = 33;
            item.melee = true;
            item.width = 50;
            item.height = 50;
            item.useTime = 20;
            item.useTurn = true;
            item.useAnimation = 20;
			item.pick = 200;
			item.UseSound = SoundID.Item1;
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