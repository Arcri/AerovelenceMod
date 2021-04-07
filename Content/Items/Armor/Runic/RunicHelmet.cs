using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Runic
{
    [AutoloadEquip(EquipType.Head)]
    public class RunicHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Runic Helmet");
            Tooltip.SetDefault("Unfinished");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<RunicGuard>() && legs.type == ModContent.ItemType<RunicLeggings>() && head.type == ModContent.ItemType<RunicHelmet>();
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
            item.width = 22;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 1;
        }
		public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.06f;
            player.manaCost -= 0.08f;
            player.maxMinions += 1;
        }
    }
}