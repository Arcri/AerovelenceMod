using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        float numberProjectiles = 1 + Main.rand.Next(1);
            float rotation = MathHelper.ToRadians(15);
            position += Vector2.Normalize(velocity) * 2f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                if (i == 1)
                {
                    Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, 2f, player.whoAmI);
                    Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, 2f, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, 2f, player.whoAmI);
                }
            }
            return false;
        }
    }
}