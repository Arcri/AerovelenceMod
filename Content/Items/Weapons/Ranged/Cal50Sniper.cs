using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Cal50Sniper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("50 Cal Sniper");
            Tooltip.SetDefault("Fires 50 caliber rounds");
        }
        public override void SetDefaults()
        {
            Item.damage = 180;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.mana = 5;
            Item.height = 32;
            Item.useTime = 75;
            Item.useAnimation = 75;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0.2f;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 30f;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.SniperRifle, 1)
                .AddIngredient(ItemID.ShroomiteBar, 15)
                .AddIngredient(ItemID.IllegalGunParts, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, -2);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, velocity.Y).RotatedByRandom(MathHelper.ToRadians(3));
            speedX = perturbedSpeed.X;
            velocity.Y = perturbedSpeed.Y;
            Vector2 muzzleOffset = new Vector2(-7, -8);
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            return true;
        }
    }
}