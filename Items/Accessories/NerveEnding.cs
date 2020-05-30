using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class NerveEnding : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nerve Ending");
			Tooltip.SetDefault("Increases live regen by 5%");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 32;
            item.height = 32;
            item.value = 10000;
            item.rare = -12;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.lifeRegen += 5;
		}
    }
}