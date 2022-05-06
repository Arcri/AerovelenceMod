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
            Item.damage = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 12;
            Item.knockBack = 4;
            Item.rare = ItemRarityID.Blue;
			Item.maxStack = 999;
			Item.consumable = true;
            Item.shoot = ModContent.ProjectileType<MallowBulletProj>();
            Item.shootSpeed = 10f;
            Item.value = Item.sellPrice(0, 0, 1, 10);
            Item.ammo = Item.type;
        }
        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddIngredient(ItemID.MusketBall, 50)
                .AddIngredient(ItemID.Marshmallow, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}