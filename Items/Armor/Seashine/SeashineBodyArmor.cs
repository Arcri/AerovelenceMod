using AerovelenceMod.Items.Ores.PreHM.Frost;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.Seashine
{
	[AutoloadEquip(EquipType.Body)]
    public class SeashineBodyArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seashine Body Armor");
            Tooltip.SetDefault("3% increased critical strike chance\n2% increased minion knockback");
        } 			
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 10;
        }

        public override void UpdateEquip(Player player)
        {
			player.magicCrit += 3;
            player.rangedCrit += 3;
            player.meleeCrit += 3;
            player.minionKB *= 1.2f;
        }

        public override void AddRecipes()
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