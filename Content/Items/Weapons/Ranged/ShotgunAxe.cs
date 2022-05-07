using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class ShotgunAxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shotgun-Axe");
			Tooltip.SetDefault("'First we sharpen the axe, then we chop the tree.'\n" +
							   "'My axe is plenty sharp, and a shotgun.'");
		}

		public override void SetDefaults()
		{
			Item.damage = 48;
			Item.rare = ItemRarityID.Yellow;
			Item.width = 78;
			Item.height = 32;
			Item.useAnimation = 35;
			Item.useTime = 35;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 7f;
			Item.knockBack = 6f;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item36;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.shoot = ProjectileID.Bullet;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.crit = 4;
				Item.damage = 73;
				Item.width = 78;
				Item.height = 32;
				Item.noMelee = false;
				Item.useTime = 60;
				Item.useAnimation = 60;
				Item.shoot = ProjectileID.None;
				Item.UseSound = SoundID.Item1;
				Item.useStyle = ItemUseStyleID.Swing;
				Item.knockBack = 4;
				Item.autoReuse = false;
			}
			else
			{
				Item.damage = 48;
				Item.rare = ItemRarityID.Yellow;
				Item.width = 78;
				Item.height = 32;
				Item.useAnimation = 35;
				Item.useTime = 35;
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.shootSpeed = 7f;
				Item.knockBack = 6f;
				Item.DamageType = DamageClass.Ranged;
				Item.autoReuse = true;
				Item.UseSound = SoundID.Item36;
				Item.noMelee = true;
				Item.shoot = ProjectileID.Bullet;
				Item.useAmmo = AmmoID.Bullet;
			}
			return base.CanUseItem(player);
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
		}
	}
}