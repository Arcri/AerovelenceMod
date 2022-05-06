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
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Electro-bolt");
            Tooltip.SetDefault("Shoots a bolt of electricity\nGets faster every time it hits a block");
        }
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 26;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 38;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 95, 50);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ElectricitySpark>();
            Item.shootSpeed = 5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.CopperBar, 3)
                .AddIngredient(ModContent.ItemType<LustrousCrystal>(), 1)
                .AddIngredient(ModContent.ItemType<CavernCrystal>(), 15)
                .AddRecipeGroup("IronBar", 5)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}