#region Using directives

using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
	public class IcicleBarrage : ModItem
	{
		private readonly int MaxProjectileHeight = 320;

		public override void SetStaticDefaults()
		{
			Item.staff[Item.type] = true;
		}
		public override void SetDefaults()
		{
			Item.width = 46;
			Item.height = 52;
			Item.rare = ItemRarityID.Purple;
			Item.value = Item.sellPrice(0, 10, 50, 0);

			Item.crit = 11;
			Item.mana = 20;
			Item.damage = 32;
			Item.knockBack = 6f;

			Item.useTime = Item.useAnimation = 65;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true;

			Item.shootSpeed = 15f;
			Item.shoot = ModContent.ProjectileType<WallOfIce>();

			Item.UseSound = SoundID.Item100;
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<FrostShard>(), 20)
				.AddIngredient(ItemID.SoulofLight, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        Vector2 velocity = new Vector2(speedX, speedY);

			while (Collision.CanHitLine(player.position, player.width, player.height, position, 1, 1))
			{
				position += velocity;
				if ((position - Main.MouseWorld).Length() < 20 + Math.Abs(speedX) + Math.Abs(speedY))
				{
					position = Main.MouseWorld;
					break;
				}
			}

			int tileX = (int)(position.X / 16);
			int tileY = (int)(position.Y / 16);

			int y = tileY;

			for (; tileY < Main.maxTilesY - 10 && tileY - y < 30 && !WorldGen.SolidTile(tileX, tileY) && !TileID.Sets.Platforms[Main.tile[tileX, tileY].TileType]; tileY++) ;

			if (!WorldGen.SolidTile(tileX, tileY) && !TileID.Sets.Platforms[Main.tile[tileX, tileY].TileType])
			{
				return (false);
			}

			float startTileY = tileY * 16;
			tileY = y;

			while (tileY > 10 && y - tileY < 30 && !WorldGen.SolidTile(tileX, tileY))
			{
				tileY--;
			}

			float projectileYPosition = tileY * 16 + 16;
			float projectileHeight = MathHelper.Clamp(startTileY - projectileYPosition, 16, MaxProjectileHeight);

			projectileYPosition = startTileY - projectileHeight;
			position.X = (int)(position.X / 16f) * 16;
			Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI, projectileYPosition, projectileHeight);

			return (false);
		}
	}
}
