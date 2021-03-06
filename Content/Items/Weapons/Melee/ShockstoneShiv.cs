using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class ShockstoneShiv : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electrified Shiv");
        }
        public override void SetDefaults()
        {
            item.useTurn = true;
            item.crit = 20;
            item.damage = 80;
            item.melee = true;
            item.width = 44;
            item.height = 44;
            item.useTime = 10;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.Stabbing;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 65, 20);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BurnshockBar>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}