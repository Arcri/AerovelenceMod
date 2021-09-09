using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Crafting
{
    public class BloodChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Chunk");
        }
        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.width = 26;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Green;
        }
    }
}