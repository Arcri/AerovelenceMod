using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Shadowreaper
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
            Item.width = 30;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 160;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Melee) += 15;
			player.GetCritChance(DamageClass.Ranged) += 15;
			player.GetCritChance(DamageClass.Magic) += 15;
        }
    }
}