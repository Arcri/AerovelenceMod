using AerovelenceMod.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Item.UseSound = SoundID.Item1;
            Item.damage = 34;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 32;
            Item.useTime = Item.useAnimation = 36;
            
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 6, 10, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<SnowballProjectile>();
            Item.shootSpeed = 9f;
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
            Projectile.width = Projectile.height = 30;

            Projectile.CloneDefaults(ProjectileID.Shuriken);
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            for (int j = 0; j < 5; j++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + (10 * ((Projectile.rotation - 0.78f).ToRotationVector2())), 20);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.09f, 0.2f);
                dust.fadeIn = 1.5f;
            }
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            int damage = (int)(Projectile.damage);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<SnowballProjectileThree>(), damage, 0, Projectile.owner);

            if (Main.myPlayer == Projectile.owner)
            {
                damage = (int)(Projectile.damage * 0.5f);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 16f, 0, 5, ModContent.ProjectileType<SnowballProjectileTwo>(), damage, 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 16f, 5, 0, ModContent.ProjectileType<SnowballProjectileTwo>(), damage, 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 16f, 0, -5, ModContent.ProjectileType<SnowballProjectileTwo>(), damage, 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 16f, -5, 0, ModContent.ProjectileType<SnowballProjectileTwo>(), damage, 0, Projectile.owner);
            }

        }

        public class SnowballProjectileTwo : ModProjectile
        {
            public override string Texture => "Terraria/Items/Projectile_" + ProjectileID.None;
            public override void SetDefaults()
            {
                Projectile.width = Projectile.height = 8;

                Projectile.friendly = true;
                Projectile.DamageType = DamageClass.Ranged;

                Projectile.aiStyle = -1;

                Projectile.penetrate = -1;

                Projectile.timeLeft = 140;

                Projectile.tileCollide = false;

                Projectile.extraUpdates = 4;

            }
            public override void AI()
            {
                Projectile.velocity.X *= 1.01f;
                Projectile.velocity = Projectile.velocity.RotatedBy(System.Math.PI / 40);

                for (int j = 0; j < 10; j++)
                {
                    float x = Projectile.position.X - Projectile.velocity.X / 10f * (float)j;
                    float y = Projectile.position.Y - Projectile.velocity.Y / 10f * (float)j;
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
                Projectile.width = Projectile.height = 20;

                Projectile.aiStyle = -1;
                Projectile.friendly = true;
                Projectile.DamageType = DamageClass.Melee;// projectile.ignoreWater = true /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;

                Projectile.penetrate = -1;
                Projectile.timeLeft = 60;

                Projectile.tileCollide = false;
                Projectile.extraUpdates = 1;

                Projectile.alpha = 255;
            }
            public override void AI()
            {
                for (int i = 0; i < 2; ++i)
                {
                    float randomDust = Main.rand.NextFloat(-3, 3);
                    float randomDust2 = Main.rand.NextFloat(3, -3);
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 20, randomDust, randomDust2);
                }

            }
            public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
              => target.AddBuff(BuffID.Frostburn, 120);
        }
    }
}
