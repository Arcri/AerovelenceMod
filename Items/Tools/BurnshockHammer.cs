using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Tools
{
    public class BurnshockHammer : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnshock Hammer");
		}
        public override void SetDefaults()
        {
			item.crit = 5;
            item.damage = 35;
            item.melee = true;
            item.width = 52;
            item.height = 48;
            item.useTime = 25;
            item.useTurn = true;
            item.useAnimation = 25;
			item.hammer = 90;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 35, 0);
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