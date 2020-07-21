using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class MirageCloak : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mirage Cloak");
			Tooltip.SetDefault("Has a chance to confuse enemies when hit\nCauses stars to fall out of the sky");
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
			player.rangedDamage += 0.1f;
            AeroPlayer modPlayer = (AeroPlayer)player.GetModPlayer(mod, "AeroPlayer");
            modPlayer.EmeraldEmpoweredGem = true;
        }
    }
}