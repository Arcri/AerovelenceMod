using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Runic
{
    [AutoloadEquip(EquipType.Head)]
    public class RunicMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Runic Mask");
            Tooltip.SetDefault("Unfinished");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<RunicGuard>() && legs.type == ModContent.ItemType<RunicLeggings>() && head.type == ModContent.ItemType<RunicMask>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "Summons a flying crab to protect you\nMovement speed in water heavily increased";
            if (player.wet)
            {
                player.moveSpeed += 0.15f;
            }
        } 	
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 1;
        }
		public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.06f;
            player.manaCost -= 0.08f;
            player.maxMinions += 1;
        }
    }
}