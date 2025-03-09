using AerovelenceMod.Common.Utilities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Potions
{
    public class OnTheRocks : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarities.EarlyPHM;
        }
    }
}