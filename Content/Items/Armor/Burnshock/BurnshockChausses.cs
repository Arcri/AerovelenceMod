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
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.11f;
            player.GetCritChance(DamageClass.Magic) += 4;
            player.GetCritChance(DamageClass.Melee) += 4;
            player.GetCritChance(DamageClass.Ranged) += 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<BurnshockBar>(), 12)
                .AddIngredient(ItemID.CrystalShard, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}