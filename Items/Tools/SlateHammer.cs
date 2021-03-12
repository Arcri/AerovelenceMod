using AerovelenceMod.Items.Placeable.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Tools
{
    public class SlateHammer : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slate Hammer");
		}
        public override void SetDefaults()
        {
			item.crit = 4;
            item.damage = 8;
            item.melee = true;
            item.width = 34;
            item.height = 34;
            item.useTurn = true;
            item.useTime = 28;
            item.useAnimation = 28;
			item.hammer = 35;
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
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}