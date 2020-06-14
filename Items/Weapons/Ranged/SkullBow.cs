using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class SkullBow : ModItem
    {
				public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Bow");
			Tooltip.SetDefault("Made up of the Dungeon's guardian");
		}
        public override void SetDefaults()
        {
			item.shootSpeed = 24f;
			item.crit = 6;
            item.damage = 15;
            item.ranged = true;
            item.width = 24;
            item.height = 42;
            item.useTime = 12;
            item.useAnimation = 12;
			item.UseSound = SoundID.Item5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 4;
			item.value = Item.sellPrice(0, 3, 80, 0);
			item.rare = ItemRarityID.Purple;
			item.shoot = ProjectileID.Bone;
			item.useAmmo = 154;
            item.autoReuse = true;
        }
		
    	public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
			tooltips.Add(line);

			line = new TooltipLine(mod, "Skull Bow", "Legendary item")
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

			// Here we will remove all tooltips whose title end with ':RemoveMe'
			// One like that is added at the start of this method
			tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
		}
	}
}