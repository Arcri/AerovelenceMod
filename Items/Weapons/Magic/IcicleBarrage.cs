#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Items.Weapons.Magic
{
	public class IcicleBarrage : ModItem
	{
		private readonly int MaxProjectileHeight = 160;

		public override void SetStaticDefaults()
		{
			Item.staff[item.type] = true;
		}
		public override void SetDefaults()
		{
			item.width = 46;
			item.height = 52;
			item.rare = ItemRarityID.Purple;
			item.value = Item.sellPrice(0, 10, 50, 0);

			item.crit = 11;
			item.mana = 20;
			item.damage = 82;
			item.knockBack = 6f;

			item.useTime = item.useAnimation = 65;
			item.useStyle = ItemUseStyleID.HoldingOut;

			item.magic = true;
			item.noMelee = true;
			item.autoReuse = true;

			item.shootSpeed = 15f;
			item.shoot = ModContent.ProjectileType<Projectiles.WallOfIce>();

			item.UseSound = SoundID.Item100;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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

			for (; tileY < Main.maxTilesY - 10 && tileY - y < 30 && !WorldGen.SolidTile(tileX, tileY) && !TileID.Sets.Platforms[Main.tile[tileX, tileY].type]; tileY++) ;

			if (!WorldGen.SolidTile(tileX, tileY) && !TileID.Sets.Platforms[Main.tile[tileX, tileY].type])
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
