using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Alchemical
{
    public class PrismaticAsterItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Aster");
        }


        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 34;
            Item.value = 1000;
            Item.rare = ItemRarityID.Orange;

            Item.maxStack = 999;
        }
    }
}