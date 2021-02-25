using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
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
            item.damage = 211;
            item.magic = true;
            item.width = 70;
            item.height = 28; 
            item.useTime = 70;
            item.useAnimation = 70;
			item.UseSound = SoundID.Item68;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 13;
			item.value = Item.sellPrice(0, 10, 75, 0);
			item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ProjectileID.RubyBolt;
            item.shootSpeed = 40f;
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