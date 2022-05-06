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
			Item.width = 24;
			Item.height = 32;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 20, 80);

			Item.crit = 4;
			Item.mana = 25;
			Item.damage = 23;
			Item.knockBack = 6;

			Item.useTime = Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true;

			Item.shootSpeed = 8;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Magic.CarbonCadence_Proj>();

			Item.UseSound = SoundID.Item9;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				return (player.ownedProjectileCounts[Item.shoot] > 0);
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
					if (p.active && p.owner == player.whoAmI && p.type == Item.shoot && p.ai[0] == 1)
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
