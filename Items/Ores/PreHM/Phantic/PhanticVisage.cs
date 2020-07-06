using AerovelenceMod.Items.Ores.PreHM.Frost;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Ores.PreHM.Phantic
{
	[AutoloadEquip(EquipType.Head)]
    public class PhanticVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantic Visage");
            Tooltip.SetDefault("10% increased magic damage\n7% increased magic critical strike chance\n10% less mana cost\n+40 max mana");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<PhanticCuisses>() && legs.type == ModContent.ItemType<PhanticCuirass>() && head.type == ModContent.ItemType<PhanticVisage>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "50% chance to inflict Frostburn on enemies";
            player.GetModPlayer<AeroPlayer>().FrostProjectile = true;
        } 	
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 6;
        }
		public override void UpdateEquip(Player player)
        {
            player.magicDamage *= 1.1f;
            player.magicCrit += 7;
            player.manaCost -= 0.1f;
            player.statManaMax2 += 40;
        }

        public override void AddRecipes()  //How to craft this gun
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<FrostShard>(), 8);
            modRecipe.AddIngredient(ItemID.IceBlock, 35);
            modRecipe.AddIngredient(ItemID.HellstoneBar, 8);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}