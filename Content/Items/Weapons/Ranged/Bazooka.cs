using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Bazooka : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bazooka");
            Tooltip.SetDefault("Fires a large rocket that explodes");
        }
        public override void SetDefaults()
        {
            item.damage = 120;
            item.ranged = true;
            item.width = 62;
            item.height = 32;
            item.useTime = 5;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0.2f;
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.RocketIII;
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Rocket;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.ShroomiteBar, 10);
            modRecipe.AddIngredient(ItemID.RocketLauncher, 1);
            modRecipe.AddIngredient(ItemID.IllegalGunParts, 1);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this);
            modRecipe.AddRecipe();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, -2);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(3));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            Vector2 muzzleOffset = new Vector2(-7, -8);
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            return true;
        }
    }
}