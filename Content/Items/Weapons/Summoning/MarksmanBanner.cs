#region Using directives

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
			item.width = item.height = 22;
			item.rare = ItemRarityID.Blue;
			item.value = Item.sellPrice(0, 0, 81, 20);

			item.crit = 4;
			item.mana = 12;
			item.damage = 25;
			item.knockBack = 3f;
			
			item.useTime = item.useAnimation = 25;
			item.useStyle = ItemUseStyleID.HoldingUp;
            
			item.summon = true;
			item.noMelee = true;
			item.useTurn = false;

			item.buffTime = 3600;
			item.buffType = ModContent.BuffType<HuntressBuff>();

			item.shootSpeed = 10f;
			item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Summoning.Huntress>();
			
			item.UseSound = SoundID.Item44;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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

			player.AddBuff(item.buffType, item.buffTime);
			Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI, summonType);

			return (false);
		}

		public override void UseStyle(Player player)
		 => player.itemLocation -= new Vector2(96 * player.direction, 18);
	}
}
