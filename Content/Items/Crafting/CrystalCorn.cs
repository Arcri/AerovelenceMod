using AerovelenceMod.Common.Utilities;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Crafting
{
    public class CrystalCorn : ModItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.width = 26;
            Item.height = 22;
            Item.value = 10;
            Item.rare = ItemRarities.BasicMaterials;
        }
    }
}
