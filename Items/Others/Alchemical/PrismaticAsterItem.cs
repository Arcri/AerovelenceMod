using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Others.Alchemical
{
    public class PrismaticAsterItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Aster");
        }


        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 22;
            item.value = 1000;
            item.rare = ItemRarityID.Orange;

            item.maxStack = 999;
        }
    }
}