using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class RicochetPistol : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ricochet Pistol");
			Tooltip.SetDefault("'A Skygod's wish, fulfilled within'");
            Tooltip.SetDefault("Fires a cloud that bounces off of walls and enemies");
        }
		public override void SetDefaults()
		{
            Item.damage = 25;
            Item.crit = 4;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 44;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 8f;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddRecipeGroup("AerovelenceMod:GoldBars", 12)
                .AddIngredient(ItemID.SunplateBlock, 20)
                .AddIngredient(ItemID.Cloud, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(4, -2);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = ModContent.ProjectileType<BounceBullet>();

            Vector2 muzzleOffset = new Vector2(-7, -8);
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            return true;
        }
	}
}