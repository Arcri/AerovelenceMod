using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class PoweredBattery : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Powered Battery");
			Tooltip.SetDefault("Has a chance to let out an electric explosion when taking damage");
		}
        public override void SetDefaults()
        {
			Item.accessory = true;
            Item.width = 30;
            Item.height = 14;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddBuff(BuffID.NightOwl,360,false);
		}
    }
}