using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Others.Quest
{
    public class SilverCluster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silver Cluster");
        }
        public override void SetDefaults()
        {
            item.value = Item.sellPrice(0, 0, 20, 0);
            item.maxStack = 1;
            item.width = 30;
            item.height = 28;
            item.rare = ItemRarityID.Quest;
        }
    }
}