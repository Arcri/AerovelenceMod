using AerovelenceMod.Content.Items.Accessories;
using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.SpiritCultist
{
    [AutoloadEquip(EquipType.Legs)]
    public class SpiritCultistLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Cultist Leggings");
            Tooltip.SetDefault("5% increased critical strike chance\n15% increased movement speed");
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Yellow;
            item.defense = 13;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.15f;
            player.magicCrit += 5;
            player.meleeCrit += 5;
            player.rangedCrit += 5;
        }

        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<CrystalShard>(), 10);
            modRecipe.AddIngredient(ItemID.FallenStar, 8);
            modRecipe.AddIngredient(ModContent.ItemType<RubyEmpoweredGem>(), 1);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}