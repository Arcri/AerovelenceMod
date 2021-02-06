using AerovelenceMod.Items.Placeable.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class SlateSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slate Sword");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 13;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 25;
            item.useAnimation = 25;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 20, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = false;
        }
		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SlateOreItem>(), 45);
            recipe.AddRecipeGroup("Wood", 15);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}