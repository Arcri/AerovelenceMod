using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Starglass
{
    [AutoloadEquip(EquipType.Body)]
    public class StarglassChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starglass Chestplate");
            Tooltip.SetDefault("4% increased damage\nImmunity to Electrified\n+15 max health");
        } 			
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 16;
        }
        public override void UpdateEquip(Player player)
        {
            player.rangedDamage += 0.04f;
            player.magicDamage += 0.04f;
            player.meleeDamage += 0.04f;
            player.statLifeMax2 += 15;
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