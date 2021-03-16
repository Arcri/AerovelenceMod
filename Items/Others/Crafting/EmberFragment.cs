using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Others.Crafting
{
    public class EmberFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ember Fragment");
            Tooltip.SetDefault("The essense of hell");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 9));
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }
        public override void SetDefaults()
        {
            item.value = Item.sellPrice(0, 0, 10, 0);
            item.maxStack = 999;
            item.width = 18;
            item.height = 30;
            item.rare = ItemRarityID.LightRed;
        }
    }
}