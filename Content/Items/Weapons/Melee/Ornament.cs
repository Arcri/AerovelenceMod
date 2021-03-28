using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class Ornament : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ornament");
        }
        public override void SetDefaults()
        {
            item.channel = true;		
            item.crit = 20;
            item.damage = 46;
            item.ranged = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 24;
            item.useAnimation = 24;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = ItemRarityID.Lime;
            item.autoReuse = false;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 2f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.rand.Next(2) == 0)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("OrnamentProjectileRed"), damage, knockBack, player.whoAmI);
            } else
            {
                Projectile.NewProjectile(position.X + 10, position.Y + 10, speedX + 4, speedY + 4, mod.ProjectileType("OrnamentProjectileGreen"), damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }

    public class OrnamentProjectileGreen : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 40;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 440f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 24f;
        }
        public override void AI()
        {
            if (Main.rand.Next(2) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }

            if (Main.rand.NextFloat() <= 0.03)
            {
                Projectile.NewProjectile(projectile.Center, new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(-2f, -4.5f)), ProjectileID.OrnamentFriendly, projectile.damage, projectile.knockBack, projectile.owner);
            }
        }

    }

    public class OrnamentProjectileRed : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 40;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 440f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 24f;
        }
        public override void AI()
        {
            if (Main.rand.Next(2) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (Main.rand.NextFloat() <= 0.03)
            {
                Projectile.NewProjectile(projectile.Center, new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(-2f, -4.5f)), ProjectileID.OrnamentFriendly, projectile.damage, projectile.knockBack, projectile.owner);
            }
        }
    }
}