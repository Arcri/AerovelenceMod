﻿#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Buffs;

#endregion

namespace AerovelenceMod.Content.Items.Weapons.Summoning
{
	public sealed class MarksmanBanner : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Summons an stationary flower sentry to fight for you");
		}
		public override void SetDefaults()
		{
			Item.width = Item.height = 22;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 81, 20);

			Item.crit = 4;
			Item.mana = 12;
			Item.damage = 25;
			Item.knockBack = 3f;
			
			Item.useTime = Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.HoldUp;
            
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.useTurn = false;

			Item.buffTime = 3600;
			Item.buffType = ModContent.BuffType<HuntressBuff>();

			Item.shootSpeed = 10f;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Summoning.Huntress>();
			
			Item.UseSound = SoundID.Item44;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (player.altFunctionUse == 2)
			{
				return (false);
			}

			// Spawn new projectile with a random type.
			int summonType = Main.rand.Next(3);
			if (summonType == 0)
			{
				damage -= 9;
			}
			else if (summonType == 1)
			{
				damage += 7;
			}

			player.AddBuff(Item.buffType, Item.buffTime);
			Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI, summonType);

			return (false);
		}

		public override void UseStyle(Player player)
		 => player.itemLocation -= new Vector2(96 * player.direction, 18);
	}
}
