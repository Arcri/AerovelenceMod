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
            Item.width = 28;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
    }
}