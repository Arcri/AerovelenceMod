using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class Electrobolt : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Electro-bolt");
            Tooltip.SetDefault("Shoots a bolt of electricity\nGets faster every time it hits a block");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 26;
            item.magic = true;
            item.mana = 12;
            item.width = 38;
            item.height = 40;
            item.useTime = 15;
            item.useAnimation = 15;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 95, 50);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ElectricitySpark>();
            item.shootSpeed = 5f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CopperBar, 3);
            recipe.AddIngredient(ModContent.ItemType<LustrousCrystal>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CavernCrystal>(), 15);
            recipe.AddRecipeGroup("IronBar", 5);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}