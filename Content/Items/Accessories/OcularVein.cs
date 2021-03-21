using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class OcularVein : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ocular Vein");
			Tooltip.SetDefault("Increases night vision slightly");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 30;
            item.height = 14;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddBuff(BuffID.NightOwl,360,false);
		}
    }
}