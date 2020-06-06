using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class LazX : ModItem
    {
				public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laz-X");
			Tooltip.SetDefault("Forged from the finest metals");
		}
        public override void SetDefaults()
        {
			item.crit = 11;
            item.damage = 464;
            item.magic = true;
            item.width = 60;
            item.height = 32; 
            item.useTime = 34;
            item.useAnimation = 34;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 13;
            item.value = 10000;
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ProjectileID.RubyBolt;
            item.shootSpeed = 32f;
        }
		
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-6, 0);
		}
		
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "I have an announcement to make. Shadow the hedgehog is a bitchass motherfucker. He pissed on my fucking wife.");
			tooltips.Add(line);

			line = new TooltipLine(mod, "Laz-X", "Legendary item")
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