using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class NerveEnding : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nerve Ending");
			Tooltip.SetDefault("Increases life regen by 10%");
		}
        public override void SetDefaults()
        {
			Item.accessory = true;
            Item.width = 16;
            Item.height = 26;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.lifeRegen += 5;
		}
    }
}