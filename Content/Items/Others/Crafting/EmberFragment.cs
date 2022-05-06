using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Crafting
{
    public class EmberFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ember Fragment");
            Tooltip.SetDefault("The essense of hell");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 9));
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.maxStack = 999;
            Item.width = 18;
            Item.height = 30;
            Item.rare = ItemRarityID.LightRed;
        }
    }
}