using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class OceanMist : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Ocean Mist");
            Tooltip.SetDefault("Shoots a jet of Water");
        }
        public override void SetDefaults()
        {
            Item.crit = 2;
            Item.damage = 12;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 40;
            Item.height = 38;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WaterStream;
            Item.shootSpeed = 13f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.Seashell, 3)
                .AddIngredient(ItemID.Starfish, 5)
                .AddRecipeGroup("IronBar", 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}