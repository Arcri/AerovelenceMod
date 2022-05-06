using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Runic
{
    [AutoloadEquip(EquipType.Body)]
    public class RunicGuard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Runic Guard");
            Tooltip.SetDefault("Unfinished item!");
        } 			
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
			player.GetCritChance(DamageClass.Magic) += 3;
            player.GetCritChance(DamageClass.Ranged) += 3;
            player.GetCritChance(DamageClass.Melee) += 3;
            player.GetKnockback(DamageClass.Summon).Base += 0.02f;
        }
    }
}