#region Using directives

using System.Linq;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using AerovelenceMod.Content.Projectiles.Weapons.Summoning;
using AerovelenceMod.Content.Buffs;

#endregion

namespace AerovelenceMod.Content.Items.Weapons.Summoning
{
    public sealed class CharmingBush : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Summon a lovely carousel of flowers");
		}
		public override void SetDefaults()
		{
			item.width = item.height = 22;
			item.rare = ItemRarityID.Blue;

			item.mana = 30;
			item.damage = 25;
			item.knockBack = 0.5f;

			item.useTime = item.useAnimation = 25;
			item.useStyle = ItemUseStyleID.HoldingUp;

			item.summon = true;
			item.noMelee = true;
			item.useTurn = false;

			item.buffTime = 3600;
			item.buffType = ModContent.BuffType<CharmingBushBuff>();

			item.shootSpeed = 10f;
			item.shoot = ModContent.ProjectileType<CharmingBushRedFlower>();

			item.UseSound = SoundID.Item44;
		}
		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddRecipeGroup("Wood", 25);
			modRecipe.AddIngredient(ItemID.Daybloom, 3);
			modRecipe.AddIngredient(ItemID.Fireblossom, 3);
			modRecipe.AddIngredient(ItemID.Waterleaf, 3);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.SetResult(this);
			modRecipe.AddRecipe();
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (player.altFunctionUse == 2)
			{
				return (false);
			}

			int[] projectileTypes = { ModContent.ProjectileType<CharmingBushYellowFlower>(), type, ModContent.ProjectileType<CharmingBushGreenFlower>() };

			int projectileAmount =
				player.ownedProjectileCounts[projectileTypes[0]] +
				player.ownedProjectileCounts[projectileTypes[1]] +
				player.ownedProjectileCounts[projectileTypes[2]];

			player.AddBuff(item.buffType, item.buffTime);

			if (projectileAmount != player.maxMinions)
			{
				projectileAmount++;
			}

			Projectile.NewProjectile(player.Center, Vector2.Zero, projectileTypes[Main.rand.Next(projectileTypes.Length)], damage, knockBack, player.whoAmI, projectileAmount, projectileAmount);

			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && projectileTypes.Contains(Main.projectile[i].type))
				{
					Main.projectile[i].netUpdate = true;
					Main.projectile[i].ai[0] = projectileAmount;
				}
			}
			return (false);
		}
	}
}
