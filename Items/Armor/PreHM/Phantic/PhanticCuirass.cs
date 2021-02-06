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
            Tooltip.SetDefault("5% increased damage reduction\n5% increased critical strike chance");
        } 			
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 8;
        }
        public override void UpdateEquip(Player player)
        {
			player.magicCrit += 5;
            player.rangedCrit += 5;
            player.meleeCrit += 5;
            player.endurance += 0.05f;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 17);
            modRecipe.AddRecipeGroup("AerovelenceMod:EvilMaterials", 15);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}