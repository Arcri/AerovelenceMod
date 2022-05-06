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
			Item.accessory = true;
            Item.width = 22;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 155;
        }

        public override void UpdateAccessory(Player player, bool isVisible)
        {
			isVisible = true;
            player.moveSpeed += 500.75f;
        }
    }
}