#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
	public sealed class CarbonCadence : ModItem
	{
        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Carbon Cadence");
			Tooltip.SetDefault("Casts explosive crystal mines that shatter into shards");
        }
        public override void SetDefaults()
		{
			item.width = 24;
			item.height = 32;
			item.rare = ItemRarityID.Blue;
			item.value = Item.sellPrice(0, 0, 20, 80);

			item.crit = 4;
			item.mana = 25;
			item.damage = 23;
			item.knockBack = 6;

			item.useTime = item.useAnimation = 32;
			item.useStyle = ItemUseStyleID.HoldingOut;

			item.magic = true;
			item.noMelee = true;
			item.autoReuse = true;

			item.shootSpeed = 8;
			item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Magic.CarbonCadence_Proj>();

			item.UseSound = SoundID.Item9;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				return (player.ownedProjectileCounts[item.shoot] > 0);
			}

			return (true);
		}

		public override bool AltFunctionUse(Player player)
			=> true;

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (player.altFunctionUse == 2)
			{
				for (int i = 0; i < Main.maxProjectiles; ++i)
				{
					Projectile p = Main.projectile[i];
					if (p.active && p.owner == player.whoAmI && p.type == item.shoot && p.ai[0] == 1)
					{
						p.ai[0] = (int)Projectiles.Weapons.Magic.CarbonCadence_Proj.AIState.Explosion;
						p.netUpdate = true;
					}
				}

				return (false);
			}
			return (true);
		}
	}
}
