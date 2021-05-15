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
            item.crit = 5;
            item.damage = 44;
            item.ranged = true;
            item.width = 20;
            item.height = 26;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 6, 10, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<SnowballProjectile>();
            item.shootSpeed = 5f;
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
            projectile.width = 20;
            projectile.height = 26;
            projectile.aiStyle = 16;
            projectile.friendly = projectile.ranged = true;
            
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 20, projectile.oldVelocity.X * 0.2f, projectile.oldVelocity.Y * 0.2f);
            }
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 10);

            int type = Main.rand.Next(new int[] { ProjectileID.SnowBallFriendly, ProjectileID.SnowBallFriendly });
            int num318 = Main.rand.Next(4, 8);
            for (int num319 = 0; num319 < num318; num319++)
            {
                Vector2 value16 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                value16.Normalize();
                value16 *= Main.rand.Next(280, 481) * 0.0111f;
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value16.X, value16.Y, type, projectile.damage, projectile.owner, 0, Main.rand.Next(-45, 1));
            }
            int damage = (int)(projectile.damage);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0,  ProjectileID.DD2ExplosiveTrapT2Explosion, damage, 0, projectile.owner);

            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 20, 0, 0);
        }
    }
}