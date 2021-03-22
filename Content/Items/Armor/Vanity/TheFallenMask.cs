using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class TheFallenMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Fallen Mask");
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