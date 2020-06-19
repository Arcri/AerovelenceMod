using Microsoft.Xna.Framework;
using AerovelenceMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class MarshmallowCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marshmallow Cannon");
            Tooltip.SetDefault("Shoots a marshmallow");
        }
        public override void SetDefaults()
        {
            item.damage = 6;
            item.ranged = true;
            item.width = 20;
            item.height = 10;
            item.useTime = 25;
            item.useAnimation = 5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0.2f;
            item.value = Item.sellPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item11;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("MallowBullet");
            item.shootSpeed = 10f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("Wood", 15);
            recipe.AddRecipeGroup("IronBar", 4);
            recipe.AddIngredient(ItemID.Marshmallow, 5);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}