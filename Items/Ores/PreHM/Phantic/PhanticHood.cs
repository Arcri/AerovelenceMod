using AerovelenceMod.Items.Ores.PreHM.Frost;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Ores.PreHM.Phantic
{
	[AutoloadEquip(EquipType.Head)]
    public class PhanticHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantic Hood");
            Tooltip.SetDefault("10% increased ranged damage\n8% increased ranged critical strike chance");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<PhanticCuirass>() && legs.type == ModContent.ItemType<PhanticCuisses>() && head.type == ModContent.ItemType<PhanticHood>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "50% chance to inflict Frostburn on enemies";
            player.GetModPlayer<AeroPlayer>().FrostProjectile = true;
        } 	
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(0, 1, 46, 34);
            item.rare = ItemRarityID.Orange;
            item.defense = 7;
        }
		public override void UpdateEquip(Player player)
        {
			player.rangedDamage *= 1.1f;
            player.rangedCrit += 8;
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