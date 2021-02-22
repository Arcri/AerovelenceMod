using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Others.Crafting
{
    public class EmberFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ember Fragment");
        }
        public override void SetDefaults()
        {
            item.value = Item.sellPrice(0, 0, 2, 35);
            item.maxStack = 999;
            item.width = 22;
            item.height = 24;
            item.rare = ItemRarityID.LightRed;
        }
    }
}