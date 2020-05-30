using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class QueensStinger : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Queens Stinger");
			Tooltip.SetDefault("Bees have a 10% chance of spawning with every projectile");
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
			player.GetModPlayer<AeroPlayer>().QueensStinger = true;
            modPlayer.QueensStinger = true;
        }
    }
}