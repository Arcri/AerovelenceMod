using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class EatersTooth : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eater's Tooth");
			Tooltip.SetDefault("Melee damage increased slightly\nMelee speed increased slightly\nYou move slower when this item is equipped");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 14;
            item.height = 20;
            item.value = 60000;
            item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.meleeDamage += 0.10f;
			player.meleeSpeed =+ 1.2f;
			player.maxRunSpeed -= 0.3f;
        }
    }
}