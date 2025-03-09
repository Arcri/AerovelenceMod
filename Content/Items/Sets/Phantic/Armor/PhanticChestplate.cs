using AerovelenceMod.Common.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Sets.Phantic.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class PhanticChestplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarities.MidPHM;
            Item.defense = 5;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Melee) += 3;
			player.GetCritChance(DamageClass.Ranged) += 3;
			player.GetCritChance(DamageClass.Magic) += 3;
        }
        public override void AddRecipes()
        {

        }
    }
}