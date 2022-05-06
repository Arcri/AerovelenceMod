using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Crafting
{
    public class LustrousCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lustrous Crystal");
        }		
        public override void SetDefaults()
        {
			Item.maxStack = 999;
            Item.width = 16;
            Item.height = 22;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
        }
    }
}