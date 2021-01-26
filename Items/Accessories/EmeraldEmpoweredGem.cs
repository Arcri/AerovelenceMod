using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class EmeraldEmpoweredGem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Emerald Empowered Gem");
			Tooltip.SetDefault("Throwing and ranged weapons now cause Cursed Inferno");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 20;
            item.height = 20;
            item.value = 60000;
            item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.rangedDamage += 0.1f;
            AeroPlayer modPlayer = (AeroPlayer)player.GetModPlayer(mod, "AeroPlayer");
            modPlayer.EmeraldEmpoweredGem = true;
        }
    }
}