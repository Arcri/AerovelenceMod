using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class GlassDeflector : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glass Deflector");
			Tooltip.SetDefault("Has a chance to reflect a projectile\n2 shards of space crystal float around you\nExpert");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 36;
            item.height = 44;
			item.value = Item.sellPrice(0, 5, 0, 0);
			item.rare = -12;
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
			player.statDefense -= 5;
			player.meleeDamage *= 1.15f;
			player.rangedDamage *= 1.15f;
			player.magicDamage *= 1.15f;
			player.minionDamage *= 1.15f;
		}
    }
}