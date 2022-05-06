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
			Item.maxStack = 999;
            Item.width = 26;
            Item.height = 22;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
        }
    }
}