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
			Tooltip.SetDefault("Increases live regen by 10%");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 16;
            item.height = 26;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.lifeRegen += 5;
		}
    }
}