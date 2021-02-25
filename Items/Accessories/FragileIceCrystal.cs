using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class FragileIceCrystal : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fragile Ice Crystal");
			Tooltip.SetDefault("Gain 15% inceased damage at the cost of your defense\nExpert");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 38;
            item.height = 38;
			item.value = Item.sellPrice(0, 5, 0, 0);
			item.rare = ItemRarityID.Expert;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                }
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statDefense -= 20;
            player.allDamage += 0.15f;
		}
    }
}