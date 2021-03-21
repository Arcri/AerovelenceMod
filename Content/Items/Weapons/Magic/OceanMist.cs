using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class OceanMist : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Ocean Mist");
            Tooltip.SetDefault("Shoots a jet of Water");
        }
        public override void SetDefaults()
        {
            item.crit = 2;
            item.damage = 12;
            item.magic = true;
            item.mana = 5;
            item.width = 40;
            item.height = 38;
            item.useTime = 18;
            item.useAnimation = 18;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = ProjectileID.WaterStream;
            item.shootSpeed = 13f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Seashell, 3);
            recipe.AddIngredient(ItemID.Starfish, 5);
            recipe.AddRecipeGroup("IronBar", 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}