using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Tools
{
    public class PhanticHammer : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantic Hammer");
		}
        public override void SetDefaults()
        {
			item.crit = 3;
            item.damage = 12;
            item.melee = true;
            item.width = 38;
            item.height = 38;
            item.useTime = 25;
            item.useAnimation = 25;
			item.hammer = 60;
            item.useTurn = true;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 35, 0);
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