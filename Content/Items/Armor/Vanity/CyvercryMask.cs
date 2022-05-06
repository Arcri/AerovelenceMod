using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class CyvercryMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyvercry Mask");
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