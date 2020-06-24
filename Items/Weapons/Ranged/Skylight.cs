using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Items.Weapons.Ranged
{
	public class Skylight : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skylight");
		}

		public override void SetDefaults()
		{
			item.damage = 95;
			item.rare = ItemRarityID.Red;
			item.width = 30;
			item.height = 30;
			item.useAnimation = 6;
			item.useTime = 6;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.shootSpeed = 10f;
			item.knockBack = 1.3f;
			item.ranged = true;
			item.autoReuse = true;
			item.noMelee = true;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.shoot = ProjectileID.Bullet;
			item.UseSound = SoundID.Item11;
			item.useAmmo = AmmoID.Bullet;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-8, 0);
		}


		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5)); //the angle in which the projectile can be fired
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			if (Main.rand.NextBool(5))
			{
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileType<SkylightProjectile>(), damage * 2, knockBack, player.whoAmI);
			}
			return true;
		}
	}
}