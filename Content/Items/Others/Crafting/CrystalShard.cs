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
            item.value = Item.sellPrice(0, 0, 2, 35);
            item.maxStack = 999;
            item.width = 30;
            item.height = 28;
            item.rare = ItemRarityID.LightRed;
        }
    }
}