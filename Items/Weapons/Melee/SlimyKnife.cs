using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class SlimyKnife : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slimy Knife");
		}
		public override void SetDefaults()
		{
			item.crit = 4;
			item.damage = 9;
			item.melee = true;
			item.width = 36;
			item.height = 38;
			item.useTime = 6;
			item.useAnimation = 6;
			item.UseSound = SoundID.Item1;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.knockBack = 4;
			item.value = Item.sellPrice(0, 2, 50, 0);
			item.rare = ItemRarityID.Purple;
			item.autoReuse = true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "This is pretty wwwwwwwwoooooeeeeedfdoah");
			tooltips.Add(line);

			line = new TooltipLine(mod, "SlimyKnife", "Legendary item")
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