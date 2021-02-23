using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.HM.Burnshock
{
    [AutoloadEquip(EquipType.Body)]
    public class BurnshockBodyArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burnshock Body Armor");
            Tooltip.SetDefault("4% increased damage\nImmunity to Electrified");
        } 			
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 20;
        }
        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.04f;
            player.buffImmune[BuffID.Electrified] = true;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<BurnshockBar>(), 15);
            modRecipe.AddIngredient(ItemID.CrystalShard, 10);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}