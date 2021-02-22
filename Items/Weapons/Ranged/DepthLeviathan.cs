using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
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
			item.crit = 9;
			item.damage = 52;
			item.ranged = true;
			item.width = 90;
			item.height = 32;
			item.useAnimation = 15;
			item.useTime = 2;
			item.reuseDelay = 14;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.knockBack = 8;
			item.value = 10000;
			item.rare = ItemRarityID.Blue;
			item.autoReuse = true;
			item.UseSound = SoundID.Item31;
			item.shoot = AmmoID.Bullet;
			item.useAmmo = AmmoID.Bullet;
			item.shootSpeed = 15f;
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