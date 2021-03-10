using AerovelenceMod.Items.Others.Crafting;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class RedShade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Shade");
            Tooltip.SetDefault("Fires Skulls");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.mana = 5;
            item.damage = 18;
            item.magic = true;
            item.width = 36;
            item.height = 48;
            item.useTime = 10;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = ProjectileID.Skull;
            item.shootSpeed = 12f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 12);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}