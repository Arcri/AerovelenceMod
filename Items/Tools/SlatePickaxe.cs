using AerovelenceMod.Items.Placeable.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Tools
{
    public class SlatePickaxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slate Pickaxe");
		}
        public override void SetDefaults()
        {
			item.crit = 4;
            item.damage = 6;
            item.melee = true;
            item.width = 36;
            item.height = 36;
            item.useTime = 16;
            item.useAnimation = 16;
			item.pick = 60;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 20, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
        }
		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SlateOreItem>(), 35);
            recipe.AddRecipeGroup("Wood", 15);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}