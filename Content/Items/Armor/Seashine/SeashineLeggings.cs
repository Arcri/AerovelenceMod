using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Seashine
{
    [AutoloadEquip(EquipType.Legs)]
    public class SeashineLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seashine Leggings");
            Tooltip.SetDefault("3% increased movement speed");
        }		
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
			player.moveSpeed += 0.03f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.SandBlock, 20)
                .AddIngredient(ItemID.Seashell, 3)
                .AddIngredient(ItemID.Starfish, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}