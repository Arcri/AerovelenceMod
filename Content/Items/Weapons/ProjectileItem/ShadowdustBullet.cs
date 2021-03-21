using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.ProjectileItem
{
    public class ShadowdustBullet : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shadowdust Bullet");
		}
		public override void SetDefaults()
        {
            item.damage = 13;
            item.ranged = true;
            item.width = 14;
            item.height = 14;
            item.knockBack = 4;
            item.rare = ItemRarityID.Pink;
			item.maxStack = 999;
			item.consumable = true;
            item.shoot = mod.ProjectileType("ShadowdustBullet");
			item.shootSpeed = 5f;
            item.value = Item.sellPrice(0, 0, 1, 10);
            item.ammo = AmmoID.Bullet;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.MusketBall, 100);
            recipe.SetResult(this, 100);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddRecipe();
        }
    }
}