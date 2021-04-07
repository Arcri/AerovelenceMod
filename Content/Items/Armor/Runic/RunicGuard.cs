using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Runic
{
    [AutoloadEquip(EquipType.Body)]
    public class RunicGuard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Runic Guard");
            Tooltip.SetDefault("Unfinished item!");
        } 			
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
			player.magicCrit += 3;
            player.rangedCrit += 3;
            player.meleeCrit += 3;
            player.minionKB += 0.02f;
        }
    }
}