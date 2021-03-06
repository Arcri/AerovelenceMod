using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.UIButton
{
    public class OreQuest : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ore Quest");
        }
        public override void SetDefaults()
        {
            item.value = Item.sellPrice(0, 0, 0, 0);
            item.maxStack = 999;
            item.width = 30;
            item.height = 28;
            item.rare = ItemRarityID.Green;
        }
    }
}