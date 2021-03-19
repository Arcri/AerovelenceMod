using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class SorcerersStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Sorcerer's Staff");
            Tooltip.SetDefault("Casts water bolts\n'What should we call this? It's a sorcerer's staff.\n'Hmm... Sorcerer's Staff?'");
        }
        public override void SetDefaults()
        {
            item.crit = 6;
            item.damage = 22;
            item.magic = true;
            item.mana = 10;
            item.width = 64;
            item.height = 64;
            item.useTime = 20;
            item.useAnimation = 20;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SorcerersStaffProjectile>();
            item.shootSpeed = 5f;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class SorcerersStaffProjectile : ModProjectile
    {
        public int i;
        public int counter = 0;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
        private readonly int oneHelixRevolutionInUpdateTicks = 30;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Sphere");
        }
        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.hostile = false;
            projectile.extraUpdates = 2;
            projectile.scale = 1f;
            projectile.timeLeft = 600;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override bool PreAI()
        {
            i++;
            ++projectile.localAI[0];
            float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
            float ReversepiFraction = MathHelper.Pi + oneHelixRevolutionInUpdateTicks;
            Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * projectile.height;
            Dust newDust = Dust.NewDustPerfect(projectile.Center + newDustPosition.RotatedBy(projectile.velocity.ToRotation()), 29);
            newDust.noGravity = true;
            newDustPosition.Y *= -1;
            newDust = Dust.NewDustPerfect(projectile.Center + newDustPosition.RotatedBy(projectile.velocity.ToRotation()), 29);
            newDust.noGravity = true;
            newDust.velocity *= 0f;
            Vector2 newDustPosition2 = new Vector2(0, (float)Math.Sin((projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * ReversepiFraction)) * projectile.height;
            Dust newDust2 = Dust.NewDustPerfect(projectile.Center + newDustPosition2.RotatedBy(projectile.velocity.ToRotation()), 29);
            newDust2.noGravity = true;
            newDustPosition2.Y *= -1;
            newDust2 = Dust.NewDustPerfect(projectile.Center + newDustPosition2.RotatedBy(projectile.velocity.ToRotation()), 29);
            newDust2.noGravity = true;
            newDust2.velocity *= 0f;
            projectile.rotation += projectile.velocity.Length() * 0.1f * projectile.direction;
            return (false);
        }
        public override void AI()
        {

            int num294 = Main.rand.Next(3, 7);
            for (int num295 = 0; num295 < num294; num295++)
            {
                counter++;
                if (counter >= 17)
                {
                    int num296 = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, DustID.Electric, 0f, 0f, 100, default, 2.1f);
                    Dust dust105 = Main.dust[num296];
                    Dust dust2 = dust105;
                    dust2.velocity *= 2f;
                    Main.dust[num296].noGravity = true;
                }
            }
            if (projectile.ai[1] != 1f)
            {
                projectile.ai[1] = 1f;
                projectile.position += projectile.velocity;
                projectile.velocity = projectile.velocity;
            }
        }
    }
}