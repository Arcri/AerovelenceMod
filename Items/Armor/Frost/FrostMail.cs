using AerovelenceMod.Items.Ores.PreHM.Frost;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.Frost
{
	[AutoloadEquip(EquipType.Body)]
    public class FrostMail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Plate Mail");
            Tooltip.SetDefault("5% increased critical strike chance\nImmunity to Frostburn and Frozen");
        } 			
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 10;
        }

        public override void UpdateEquip(Player player)
        {
			player.magicCrit += 5;
            player.rangedCrit += 5;
            player.meleeCrit += 5;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Frostburn] = true;
        }

        public override void AddRecipes()  //How to craft this gun
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<FrostShard>(), 15);
            modRecipe.AddIngredient(ItemID.IceBlock, 40);
            modRecipe.AddIngredient(ItemID.HellstoneBar, 12);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}