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
			Item.width = Item.height = 22;
			Item.rare = ItemRarityID.Blue;

			Item.mana = 30;
			Item.damage = 40;
			Item.knockBack = 0.5f;

			Item.useTime = Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.HoldUp;

			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.useTurn = false;

			Item.buffTime = 3600;
			Item.buffType = ModContent.BuffType<CharmingBushBuff>();

			Item.shootSpeed = 10f;
			Item.shoot = ModContent.ProjectileType<CharmingBushRedFlower>();

			Item.UseSound = SoundID.Item44;
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddRecipeGroup("Wood", 25)
				.AddIngredient(ItemID.Daybloom, 3)
				.AddIngredient(ItemID.Fireblossom, 3)
				.AddIngredient(ItemID.Waterleaf, 3)
				.AddTile(TileID.Anvils)
				.Register();
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
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

			player.AddBuff(Item.buffType, Item.buffTime);

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
