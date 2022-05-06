using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class TitaniumSuperCluster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titanium Cluster");
            Tooltip.SetDefault("'The Rock Collector might really like this...'");
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