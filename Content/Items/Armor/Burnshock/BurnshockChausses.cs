using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Burnshock
{
    [AutoloadEquip(EquipType.Legs)]
    public class BurnshockChausses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burnshock Chausses");
            Tooltip.SetDefault("4% increased critical strike chance\n11% increased movement speed");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.11f;
            player.magicCrit += 4;
            player.meleeCrit += 4;
            player.rangedCrit += 4;
        }

        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<BurnshockBar>(), 12);
            modRecipe.AddIngredient(ItemID.CrystalShard, 10);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}