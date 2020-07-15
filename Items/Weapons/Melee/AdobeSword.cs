using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class AdobeSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adobe Sword");
        }
        public override void SetDefaults()
        {
            item.crit = 6;
            item.damage = 16;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 25;
            item.useAnimation = 25;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 0, 40, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = false;
        }
		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AdobeBrick>(), 16);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}