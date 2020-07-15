using AerovelenceMod.Items.Others.Crafting;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Tools
{
    public class PhanticAxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantic Axe");
		}
        public override void SetDefaults()
        {
			item.crit = 4;
            item.damage = 10;
            item.melee = true;
            item.width = 38;
            item.height = 34;
            item.useTime = 20;
            item.useAnimation = 20;
			item.axe = 14;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}