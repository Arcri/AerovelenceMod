using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class PhanticClaws : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantic Claws");
		}
        public override void SetDefaults()
        {
			item.crit = 4;
            item.damage = 9;
            item.melee = true;
            item.width = 20;
            item.height = 20;
            item.useTime = 5;
            item.useAnimation = 5;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 1, 0, 0);
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