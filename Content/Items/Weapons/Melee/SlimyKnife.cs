using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
	public class SlimyKnife : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slimy Knife");
		}
		public override void SetDefaults()
		{
			Item.crit = 4;
			Item.damage = 9;
			Item.DamageType = DamageClass.Melee;
			Item.width = 36;
			Item.height = 38;
			Item.useTime = 6;
			Item.useAnimation = 6;
			Item.UseSound = SoundID.Item1;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 4;
			Item.value = Item.sellPrice(0, 2, 50, 0);
			Item.rare = ItemRarityID.Purple;
			Item.autoReuse = true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(Mod, "Verbose:RemoveMe", "This is pretty wwwwwwwwoooooeeeeedfdoah");
			tooltips.Add(line);

			line = new TooltipLine(Mod, "SlimyKnife", "Legendary item")
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

        public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			Vector2 targetPosition = Main.MouseWorld - Main.LocalPlayer.Center;
			int direction = Math.Sign(targetPosition.X);

			player.ChangeDir(direction);

			player.direction = direction;
			player.itemRotation = (targetPosition * direction).ToRotation();
		}
	}
}