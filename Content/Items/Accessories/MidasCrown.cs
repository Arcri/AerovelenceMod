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
			item.accessory = true;
            item.width = 26;
            item.height = 22;
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.jumpSpeedBoost += 2f;
            AeroPlayer modPlayer = (AeroPlayer)player.GetModPlayer(mod, "AeroPlayer");
            modPlayer.MidasCrown = true;
            player.AddBuff(75, 900, false);
        }
    }
}