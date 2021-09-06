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
            item.damage = 25;
            item.crit = 4;
            item.ranged = true;
            item.width = 44;
            item.height = 30;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 8f;
            item.useAmmo = AmmoID.Bullet;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AerovelenceMod:GoldBars", 12);
            recipe.AddIngredient(ItemID.SunplateBlock, 20);
            recipe.AddIngredient(ItemID.Cloud, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
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