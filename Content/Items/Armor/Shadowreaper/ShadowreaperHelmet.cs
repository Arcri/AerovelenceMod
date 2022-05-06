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
			player.GetDamage(DamageClass.Melee) += 0.100f;
			player.moveSpeed += 500.75f;
			player.maxRunSpeed *= 5.0f;
		} 	
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 150;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.50f;
			player.GetDamage(DamageClass.Ranged) += 0.50f;
			player.GetDamage(DamageClass.Magic) += 0.50f;
        }
    }
}