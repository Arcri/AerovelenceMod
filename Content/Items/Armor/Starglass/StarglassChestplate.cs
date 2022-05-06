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
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 16;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.04f;
            player.GetDamage(DamageClass.Magic) += 0.04f;
            player.GetDamage(DamageClass.Melee) += 0.04f;
            player.statLifeMax2 += 15;
            player.buffImmune[BuffID.Electrified] = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<BurnshockBar>(), 15)
                .AddIngredient(ItemID.CrystalShard, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}