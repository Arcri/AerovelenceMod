using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class RubyEmpoweredGem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ruby Empowered Gem");
			Tooltip.SetDefault("Life regen increased");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 32;
            item.height = 32;
            item.value = 60000;
            item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.lifeRegen =+ 1;
        }
    }
}