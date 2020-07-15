using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class StormEdge : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Storm Edge");
		}
        public override void SetDefaults()
        {	
			item.crit = 4;
            item.damage = 52;
            item.melee = true;
            item.width = 20;
            item.height = 30;
            item.useTime = 24;
            item.useAnimation = 24;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 2, 45, 0);
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("StormEdgeProjectile");
            item.UseSound = SoundID.Item1;
            item.shootSpeed = 13f;
        }


        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BurnshockBar>(), 6);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}