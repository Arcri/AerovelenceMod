using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Shadowreaper
{
	[AutoloadEquip(EquipType.Legs)]
    public class ShadowreaperLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowreaper Leggings");
            Tooltip.SetDefault("500% increased movement speed");
        }
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 22;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 155;
        }

        public override void UpdateAccessory(Player player, bool isVisible)
        {
			isVisible = true;
            player.moveSpeed += 500.75f;
        }
    }
}