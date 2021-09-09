using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Starglass










{
    [AutoloadEquip(EquipType.Legs)]
    public class StarglassGrieves : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starglass Grieves");
            Tooltip.SetDefault("4% increased critical strike chance\n11% increased movement speed");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 13;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.11f;
            player.magicCrit += 4;
            player.meleeCrit += 4;
            player.rangedCrit += 4;
        }
    }
}