using AerovelenceMod.Items.Ores.PreHM.Frost;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.Frost
{
	[AutoloadEquip(EquipType.Head)]
    public class FrostGreathelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frozen Greathelm");
            Tooltip.SetDefault("10% increased melee damage\n8% increased melee swing speed\n8% increased melee critical strike chance");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<FrostMail>() && legs.type == ModContent.ItemType<FrostGreaves>() && head.type == ModContent.ItemType<FrostGreathelm>();
		}
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "50% chance to inflict Frostburn on enemies";
            player.GetModPlayer<AeroPlayer>().FrostMelee = true;

        } 	
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 9;
        }
		public override void UpdateEquip(Player player)
        {
            player.meleeDamage *= 1.1f;
            player.meleeSpeed *= 1.1f;
            player.meleeCrit += 10;
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