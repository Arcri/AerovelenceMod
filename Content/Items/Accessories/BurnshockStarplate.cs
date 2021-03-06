using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class BurnshockStarplate : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnshock Starplate");
			Tooltip.SetDefault("15% increased damage\nMovement speed slightly increased\nStars and electricity fall down when hurt");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 24;
            item.height = 40;
            item.value = 60000;
            item.rare = ItemRarityID.Pink;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.starCloak = true;
			player.lifeRegen =+ 2;
			player.maxRunSpeed += 0.3f;
        }
    }
}