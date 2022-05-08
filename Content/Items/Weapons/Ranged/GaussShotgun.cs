using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class GaussShotgun : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gauss Shotgun");
			Tooltip.SetDefault("Fires a homing Gaussian Star");
		}

		public override void SetDefaults()
		{
			Item.damage = 21;
			Item.rare = ItemRarityID.Yellow;
			Item.width = 58;
			Item.height = 20;
			Item.useAnimation = 40;
			Item.useTime = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 7f;
			Item.knockBack = 6f;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.shoot = ProjectileID.Bullet;
			Item.UseSound = SoundID.Item92;
			Item.useAmmo = AmmoID.Bullet;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-8, 0);
		}


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        Projectile.NewProjectile(source, new Vector2(position.X, position.Y - 8), new Vector2(velocity.X, velocity.Y), ProjectileType<GaussianStar>(), 32, 5f, player.whoAmI);
			position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
			Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(15));
			velocity.X = perturbedSpeed.X;
			velocity.Y = perturbedSpeed.Y;
			for (int i = 0; i < 5; ++i)
			{
				Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
			}
			return true;
		}
	}
}