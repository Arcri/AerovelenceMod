using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Alchemical.Potions
{
    public class CosmicBrew : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Brew");
        }		
        public override void SetDefaults()
        {
			item.maxStack = 999;
            item.width = 16;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Green;
        }
    }
}