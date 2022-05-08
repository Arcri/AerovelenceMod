using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class CrystallineQuadshot : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystalline Quadshot");
            Tooltip.SetDefault("Fires rock and crystal shards");
        }
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 20;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.UseSound = SoundID.Item68;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<CrystalShard>();
            Item.shootSpeed = 3.5f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        float numberProjectiles = 2 + Main.rand.Next(2);
            float rotation = MathHelper.ToRadians(20);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(15));
                float scale = 1f - (Main.rand.NextFloat() * .3f);
                if (i == 1)
                {
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type, damage, knockback, player.whoAmI);
                    type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<RockShard>(), ModContent.ProjectileType<CrystalShard>() });
                }
                else
                {
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type, damage, knockback, player.whoAmI);
                }
            }
            return false;
        }
    }
}