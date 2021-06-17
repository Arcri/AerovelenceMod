using AerovelenceMod.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class Snowball : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Snowball");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.damage = 34;
            item.ranged = true;
            item.width = 30;
            item.height = 32;
            item.useTime = item.useAnimation = 36;
            
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.value = Item.sellPrice(0, 6, 10, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<SnowballProjectile>();
            item.shootSpeed = 9f;
        }
    }

    public class SnowballProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snowball blast");
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;

            projectile.CloneDefaults(ProjectileID.Shuriken);
            projectile.friendly = projectile.ranged = true;

            projectile.tileCollide = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            for (int j = 0; j < 5; j++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center + (10 * ((projectile.rotation - 0.78f).ToRotationVector2())), 20);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.09f, 0.2f);
                dust.fadeIn = 1.5f;
            }
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 10);

            int damage = (int)(projectile.damage);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<SnowballProjectileThree>(), damage, 0, projectile.owner);

            if (Main.myPlayer == projectile.owner)
            {
                damage = (int)(projectile.damage * 0.5f);

                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, 0, 5, ModContent.ProjectileType<SnowballProjectileTwo>(), damage, 0, projectile.owner);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, 5, 0, ModContent.ProjectileType<SnowballProjectileTwo>(), damage, 0, projectile.owner);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, 0, -5, ModContent.ProjectileType<SnowballProjectileTwo>(), damage, 0, projectile.owner);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, -5, 0, ModContent.ProjectileType<SnowballProjectileTwo>(), damage, 0, projectile.owner);
            }

        }

        public class SnowballProjectileTwo : ModProjectile
        {
            public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
            public override void SetDefaults()
            {
                projectile.width = projectile.height = 8;

                projectile.friendly = projectile.ranged = true;

                projectile.aiStyle = -1;

                projectile.penetrate = -1;

                projectile.timeLeft = 140;

                projectile.tileCollide = false;

                projectile.extraUpdates = 4;

            }
            public override void AI()
            {
                projectile.velocity.X *= 1.01f;
                projectile.velocity = projectile.velocity.RotatedBy(System.Math.PI / 40);

                for (int j = 0; j < 10; j++)
                {
                    float x = projectile.position.X - projectile.velocity.X / 10f * (float)j;
                    float y = projectile.position.Y - projectile.velocity.Y / 10f * (float)j;
                    Dust dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 20, 0, 0, 0, Color.Blue, 0.9f);
                    dust.position.X = x;
                    dust.position.Y = y;
                    dust.velocity *= 0f;
                    dust.noGravity = true;
                }
            }
        }
        public class SnowballProjectileThree : ModProjectile
        {
            public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
            public override void SetDefaults()
            {
                projectile.width = projectile.height = 20;

                projectile.aiStyle = -1;
                projectile.friendly = projectile.melee = projectile.ignoreWater = true;

                projectile.penetrate = -1;
                projectile.timeLeft = 60;

                projectile.tileCollide = false;
                projectile.extraUpdates = 1;

                projectile.alpha = 255;
            }
            public override void AI()
            {
                for (int i = 0; i < 2; ++i)
                {
                    float randomDust = Main.rand.NextFloat(-3, 3);
                    float randomDust2 = Main.rand.NextFloat(3, -3);
                    int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 20, randomDust, randomDust2);
                }

            }
            public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
              => target.AddBuff(BuffID.Frostburn, 120);
        }
    }
}