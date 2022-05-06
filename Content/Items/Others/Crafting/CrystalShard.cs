using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Crafting
{
    public class CrystalShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Shard");
        }
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 2, 35);
            Item.maxStack = 999;
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.LightRed;
        }
    }
}