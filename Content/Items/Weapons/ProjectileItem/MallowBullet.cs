using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.ProjectileItem
{
    public class MallowBullet : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mallow Bullet");
		}
		public override void SetDefaults()
        {
            item.damage = 6;
            item.ranged = true;
            item.width = 8;
            item.height = 12;
            item.knockBack = 4;
            item.rare = ItemRarityID.Blue;
			item.maxStack = 999;
			item.consumable = true;
            item.shoot = ModContent.ProjectileType<MallowBulletProj>();
            item.shootSpeed = 10f;
            item.value = Item.sellPrice(0, 0, 1, 10);
            item.ammo = item.type;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.MusketBall, 50);
            modRecipe.AddIngredient(ItemID.Marshmallow, 1);
            modRecipe.AddTile(TileID.WorkBenches);
            modRecipe.SetResult(this, 50);
            modRecipe.AddRecipe();
        }
    }
}