using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class TheInfinity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Infinity");
            Tooltip.SetDefault("Infinite ammo at the price of mana\n'NEVER RELOAD'");
        }
        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.mana = 5;
            Item.height = 32;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0.2f;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 8f;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override bool ConsumeAmmo(Player player)
        {
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddIngredient(ItemID.Diamond, 5)
                .AddIngredient(ItemID.IllegalGunParts, 1)
                .AddIngredient(ItemID.FlintlockPistol, 1)
                .AddTile(TileID.Hellforge)
                .Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(3, -2);
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