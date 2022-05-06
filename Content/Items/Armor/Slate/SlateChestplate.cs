using AerovelenceMod.Content.Items.Placeables.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Slate
{
    [AutoloadEquip(EquipType.Body)]
    public class SlateChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slate Chestplate");
            Tooltip.SetDefault("3% increased critical strike chance");
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 5;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Melee) += 3;
			player.GetCritChance(DamageClass.Ranged) += 3;
			player.GetCritChance(DamageClass.Magic) += 3;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<SlateOre>(), 65)
                .AddRecipeGroup("Wood", 25)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}