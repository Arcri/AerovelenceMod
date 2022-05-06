using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class Skylight : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skylight");
		}

		public override void SetDefaults()
		{
			Item.damage = 95;
			Item.rare = ItemRarityID.Red;
			Item.width = 76;
			Item.height = 28;
			Item.useAnimation = 6;
			Item.useTime = 6;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 10f;
			Item.knockBack = 1.3f;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.shoot = ProjectileID.Bullet;
			Item.UseSound = SoundID.Item11;
			Item.useAmmo = AmmoID.Bullet;
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