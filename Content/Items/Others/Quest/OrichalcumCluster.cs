using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class OrichalcumCluster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orichalcum Cluster");
            Tooltip.SetDefault("'The Rock Collector might like this...'");
        }
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.maxStack = 1;
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.Quest;
        }
    }
}