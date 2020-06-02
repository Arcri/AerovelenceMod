using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class CthulhusWrath : ModItem
    {
		public override void SetStaticDefaults()
		{
			Item.staff[item.type] = true;
			DisplayName.SetDefault("Cthulhus Wrath");
			Tooltip.SetDefault("Unrivaled until you showed up. It's angered essence is trapped.");
		}
        public override void SetDefaults()
        {
			item.crit = 7;
            item.damage = 16;
            item.magic = true;
            item.width = 36;
            item.height = 36;
            item.useTime = 8;
            item.useAnimation = 8;
			item.UseSound = SoundID.Item20;
            item.useStyle = 5;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = 11;
            item.autoReuse = true;
            item.shoot = 85;
            item.shootSpeed = 12f;
		}
		
		
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "Why do I exist, dawg");
			tooltips.Add(line);

			line = new TooltipLine(mod, "Cthulhu's Wrath", "Legendary item")
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