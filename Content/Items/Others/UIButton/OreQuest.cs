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
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.maxStack = 999;
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
        }
    }
}