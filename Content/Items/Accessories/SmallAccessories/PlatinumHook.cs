using Terraria;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Items.Accessories.SmallAccessories
{
    public class PlatinumHook : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarities.EarlyPHM;
            Item.accessory = true;
        }
    }
}