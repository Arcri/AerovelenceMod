using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class ClockworkLazinator : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Clockwork Lazinator");
			Tooltip.SetDefault("Handed down for millenia");
		}

		public override void SetDefaults()
		{
			Item.crit = 9;
			Item.damage = 52;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 74;
			Item.height = 34;
			Item.useAnimation = 12;
			Item.useTime = 2;
			Item.reuseDelay = 14;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 8;
			Item.value = 10000;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.MiniRetinaLaser;
			Item.shootSpeed = 15f;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}



		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 perturbedSpeed = new Vector2(speedX, velocity.Y).RotatedByRandom(MathHelper.ToRadians(3));
			speedX = perturbedSpeed.X;
			velocity.Y = perturbedSpeed.Y;
			return true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(Mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Clockwork Lazinator", "Artifact Weapon")
			{
				OverrideColor = new Color(255, 241, 000)
			};
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Mod == "Terraria" && line2.Name == "ItemName")
				{
					line2.OverrideColor = new Color(255, 132, 000);
				}
			}
			tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
		}
	}
}