using System;
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
			item.rare = -12;
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