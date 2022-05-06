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
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
			player.moveSpeed += 0.03f;
        }
    }
}