using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class MidasCrown : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("King's Crown");
			Tooltip.SetDefault("Attacks make enemies drop more gold");
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
			player.jumpSpeedBoost += 2f;
            AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
            modPlayer.MidasCrown = true;
            player.AddBuff(75, 900, false);
        }
    }
}