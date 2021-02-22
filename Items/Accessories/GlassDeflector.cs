using System;
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
			Tooltip.SetDefault("Gain 15% inceased damage at the cost of your defense\nExpert");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 36;
            item.height = 44;
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