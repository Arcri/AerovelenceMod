using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class DepthLeviathan : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Depth Leviathan");
			Tooltip.SetDefault("'He might bite'");
		}


		public override void SetDefaults()
		{
			Item.crit = 9;
			Item.damage = 52;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 90;
			Item.height = 32;
			Item.useAnimation = 15;
			Item.useTime = 2;
			Item.reuseDelay = 14;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 8;
			Item.value = 10000;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item31;
			Item.shoot = AmmoID.Bullet;
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 15f;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-16, 0);
		}



		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(3));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			return true;
		}
	}
}