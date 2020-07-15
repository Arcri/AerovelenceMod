using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.Phantic
{
    [AutoloadEquip(EquipType.Legs)]
    public class PhanticCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantic Cuisses");
            Tooltip.SetDefault("3% increased critical strike chance\n5% increased movement speed");
        }		
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 4;
        }
        public override void UpdateEquip(Player player)
        {
			player.moveSpeed *= 1.19f;
            player.magicCrit += 3;
            player.meleeCrit += 3;
            player.rangedCrit += 3;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 11);
            modRecipe.AddIngredient(ItemID.ShadowScale, 6);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}