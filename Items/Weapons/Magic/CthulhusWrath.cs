using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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
			Tooltip.SetDefault("It's angered essence is trapped.");
		}
        public override void SetDefaults()
        {
            item.crit = 7;
            item.damage = 16;
            item.magic = true;
			item.mana = 5;
            item.width = 36;
            item.height = 36;
            item.useTime = 8;
            item.useAnimation = 8;
            item.UseSound = SoundID.Item20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 6;
			item.value = Item.sellPrice(0, 5, 30, 0);
			item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ProjectileID.Flames;
            item.shootSpeed = 12f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }


		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.useStyle = ItemUseStyleID.HoldingOut;
				item.useTime = 70;
				item.useAnimation = 70;
				item.damage = 35;
				item.mana = 0;
			}
			else
			{
				item.useStyle = ItemUseStyleID.HoldingOut;
				item.useTime = 12;
				item.useAnimation = 12;
				item.damage = 16;
				item.mana = 5;
				item.shoot = ProjectileID.Flames;
				item.shootSpeed = 12f;
			}
			return base.CanUseItem(player);
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			if (player.altFunctionUse == 2)
					if (player.direction == 1)
					{
					player.velocity.X += 10;
					}
					else
					{
					player.velocity.X -= 10;
					}
			return true;
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