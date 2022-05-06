using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class SolarWind : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Solar Wind");
		
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8f, 0f);
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item60;
            Item.crit = 8;
            Item.damage = 50;
            Item.reuseDelay = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 5;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SolarWindProjectile>();
            Item.shootSpeed = 16f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 1 + Main.rand.Next(1);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 2f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(3));
                if (i == 1)
                {
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type, damage, knockBack, player.whoAmI);
                    Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type, damage, knockBack, player.whoAmI);
                }
            }
            return false;
        }
    }
}