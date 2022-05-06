using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class FlareShark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flareshark");
            Tooltip.SetDefault("33% chance to not consume ammo\nShoots flares alongside bullets");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item41;
            Item.crit = 4;
            Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 8f;
        }
        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() >= .33f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            {
                {
                    type = Main.rand.Next(new int[] { type, ProjectileID.Bullet, ProjectileID.Flare });
                    return true;
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.Minishark, 1)
                .AddIngredient(ItemID.FlareGun, 1)
                .AddIngredient(ItemID.IllegalGunParts, 1)
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}