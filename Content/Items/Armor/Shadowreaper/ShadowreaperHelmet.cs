using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Shadowreaper
{
    [AutoloadEquip(EquipType.Head)]
    public class ShadowreaperHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowreaper Helmet");
            Tooltip.SetDefault("50% increased damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<ShadowreaperChestplate>() && legs.type == ModContent.ItemType<ShadowreaperLeggings>() && head.type == ModContent.ItemType<ShadowreaperHelmet>();
		}
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "Potato";
			player.meleeDamage += 0.100f;
			player.moveSpeed += 500.75f;
			player.maxRunSpeed *= 5.0f;
		} 	
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 150;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.50f;
			player.rangedDamage += 0.50f;
			player.magicDamage += 0.50f;
        }
    }
}