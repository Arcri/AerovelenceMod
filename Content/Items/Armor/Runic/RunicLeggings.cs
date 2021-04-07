using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Runic
{
    [AutoloadEquip(EquipType.Legs)]
    public class RunicLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Runic Leggings");
            Tooltip.SetDefault("Unfinished");
        }		
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
			player.moveSpeed += 0.03f;
        }
    }
}