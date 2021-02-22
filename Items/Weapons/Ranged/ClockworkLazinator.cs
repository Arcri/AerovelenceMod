using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
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
			item.crit = 9;
			item.damage = 52;
			item.ranged = true;
			item.width = 74;
			item.height = 34;
			item.useAnimation = 12;
			item.useTime = 2;
			item.reuseDelay = 14;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.knockBack = 8;
			item.value = 10000;
			item.rare = ItemRarityID.Blue;
			item.autoReuse = true;
			item.shoot = ProjectileID.MiniRetinaLaser;
			item.shootSpeed = 15f;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}



		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(3));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			return true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
			tooltips.Add(line);

			line = new TooltipLine(mod, "Clockwork Lazinator", "Legendary item")
			{
				overrideColor = new Color(255, 241, 000)
			};
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.mod == "Terraria" && line2.Name == "ItemName")
				{
					line2.overrideColor = new Color(255, 132, 000);
				}
			}
			tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
		}
	}
}