using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class AmuletOfGlory : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Amulet Of Glory");
			Tooltip.SetDefault("Provides infinite spelunker");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 26;
            item.height = 22;
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(9, 2, false);
        }
    }
}