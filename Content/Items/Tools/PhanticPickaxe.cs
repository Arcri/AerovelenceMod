using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools
{
    public class PhanticPickaxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantic Pickaxe");
		}
        public override void SetDefaults()
        {
			item.crit = 4;
            item.damage = 11;
            item.melee = true;
            item.width = 34;
            item.height = 34;
            item.useTime = 13;
            item.useAnimation = 13;
            item.useTurn = true;
			item.pick = 70;
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
            recipe.AddRecipeGroup("AerovelenceMod:EvilMaterials", 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}