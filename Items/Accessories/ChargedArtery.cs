using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class ChargedArtery : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Charged Artery");
			Tooltip.SetDefault("Life is increased by 50\nLife regen increased\n You run faster while this is equipped");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 32;
            item.height = 32;
            item.value = 60000;
            item.rare = 2;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.statLifeMax2 += 50;
			player.lifeRegen =+ 2;
			player.maxRunSpeed += 0.3f;
        }
    }
}