using AerovelenceMod.Items.Accessories;
using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.HM.SpiritCultist
{
    [AutoloadEquip(EquipType.Body)]
    public class SpiritCultistCloak : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Cultist Cloak");
            Tooltip.SetDefault("5% increased damage\n+20 Max Life");
        } 			
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Yellow;
            item.defense = 19;
        }
        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.05f;
            player.statLifeMax2 += 20;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<CrystalShard>(), 15);
            modRecipe.AddIngredient(ItemID.FallenStar, 12);
            modRecipe.AddIngredient(ModContent.ItemType<DiamondEmpoweredGem>(), 1);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}