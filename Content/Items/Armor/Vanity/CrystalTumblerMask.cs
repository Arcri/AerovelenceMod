using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class CrystalTumblerMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Tumbler Mask");
        }
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
            item.vanity = true;
        }
    }
}