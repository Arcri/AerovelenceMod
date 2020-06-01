using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Bruh
{
	[AutoloadEquip(EquipType.Body)]
    public class BruhChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bruh Chestplate");
            Tooltip.SetDefault("15% increased critical damage");
        }
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 18;
            item.value = 10;
            item.rare = 2;
            item.defense = 160;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 15;
			player.rangedCrit += 15;
			player.magicCrit += 15;
        }
    }
}