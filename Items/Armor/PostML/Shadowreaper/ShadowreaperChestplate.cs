using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PostML.Shadowreaper
{
	[AutoloadEquip(EquipType.Body)]
    public class ShadowreaperChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowreaper Chestplate");
            Tooltip.SetDefault("15% increased critical damage");
        }
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
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