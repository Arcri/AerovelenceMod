using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Msc
{
    [AutoloadEquip(EquipType.Head)]
    class JellyfishHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1, silver: 10, copper: 5);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;


        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BlueJellyfish)
                .AddRecipeGroup(RecipeGroupID.IronBar, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
















    }
}
