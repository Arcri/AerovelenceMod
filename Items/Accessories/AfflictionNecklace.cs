using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class AfflictionNecklace : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Affliction Necklace");
			Tooltip.SetDefault("Has a chance to inflict Lost Souls");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 30;
            item.height = 14;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddBuff(BuffID.NightOwl,360,false);
		}
    }
}