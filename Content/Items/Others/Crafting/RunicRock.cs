using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Crafting
{
    public class RunicRock : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Runic Rock");
        }		
        public override void SetDefaults()
        {
			item.maxStack = 999;
            item.width = 16;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Green;
        }
    }
}