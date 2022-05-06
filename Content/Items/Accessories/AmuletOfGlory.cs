using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
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
			Item.accessory = true;
            Item.width = 26;
            Item.height = 22;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(9, 2, false);
        }
    }
}