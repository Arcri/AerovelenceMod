using AerovelenceMod.Content.Items.Accessories;
using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.SpiritCultist
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
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 19;
        }
        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.05f;
            player.statLifeMax2 += 20;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<CrystalShard>(), 15)
                .AddIngredient(ItemID.FallenStar, 12)
                .AddIngredient(ModContent.ItemType<DiamondEmpoweredGem>(), 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}