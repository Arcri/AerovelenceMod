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
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
			player.moveSpeed += 0.03f;
        }

        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.SandBlock, 20);
            modRecipe.AddIngredient(ItemID.Seashell, 3);
            modRecipe.AddIngredient(ItemID.Starfish, 3);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}