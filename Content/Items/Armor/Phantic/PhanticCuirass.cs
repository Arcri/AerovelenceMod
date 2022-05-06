using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Phantic
{
    [AutoloadEquip(EquipType.Body)]
    public class PhanticCuirass : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantic Cuirass");
            Tooltip.SetDefault("5% increased damage reduction\n5% increased critical strike chance");
        } 			
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 8;
        }
        public override void UpdateEquip(Player player)
        {
			player.GetCritChance(DamageClass.Magic) += 5;
            player.GetCritChance(DamageClass.Ranged) += 5;
            player.GetCritChance(DamageClass.Melee) += 5;
            player.endurance += 0.05f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 17)
                .AddRecipeGroup("AerovelenceMod:EvilMaterials", 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}