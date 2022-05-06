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
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 13;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.15f;
            player.GetCritChance(DamageClass.Magic) += 5;
            player.GetCritChance(DamageClass.Melee) += 5;
            player.GetCritChance(DamageClass.Ranged) += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<CrystalShard>(), 10)
                .AddIngredient(ItemID.FallenStar, 8)
                .AddIngredient(ModContent.ItemType<RubyEmpoweredGem>(), 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}