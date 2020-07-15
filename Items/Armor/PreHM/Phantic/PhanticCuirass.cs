using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.Phantic
{
    [AutoloadEquip(EquipType.Body)]
    public class PhanticCuirass : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantic Cuirass");
            Tooltip.SetDefault("4% increased damage\n3% increased critical strike chance");
        } 			
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 5;
        }
        public override void UpdateEquip(Player player)
        {
			player.magicCrit += 3;
            player.rangedCrit += 3;
            player.meleeCrit += 3;
            player.allDamage *= 1.14f;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 13);
            modRecipe.AddIngredient(ItemID.ShadowScale, 9);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}