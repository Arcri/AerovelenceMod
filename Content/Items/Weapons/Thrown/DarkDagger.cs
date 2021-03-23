using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class DarkDagger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Secret Shadow Blade");
            Tooltip.SetDefault("Fires a spread of dark daggers around the player that fire towards the cursor");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item11;
            item.crit = 8;
            item.damage = 120;
            item.melee = true;
            item.width = 22;
            item.height = 38;
            item.useTime = 80;
            item.useAnimation = 80;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 5, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("UngodlyDaggerProjectile");
            item.shootSpeed = 12f;
        }
        float dynamicCounter = 0;
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 3 + (Main.expertMode ? 1 : 0); i++)
            {
                Vector2 toLocation = player.Center + new Vector2(Main.rand.NextFloat(100, 240), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360)));
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    damage = item.damage;
                    Projectile.NewProjectile(toLocation, Vector2.Zero, ModContent.ProjectileType<DarkDaggerProjectile>(), damage, 0, Main.myPlayer, player.whoAmI);
                }
                Vector2 toLocationVelo = toLocation - player.Center;
                Vector2 from = player.Center;
                for (int j = 0; j < 300; j++)
                {
                    Vector2 velo = toLocationVelo.SafeNormalize(Vector2.Zero);
                    from += velo * 12;
                    Vector2 circularLocation = new Vector2(10, 0).RotatedBy(MathHelper.ToRadians(j * 12 + dynamicCounter));

                    int dust = Dust.NewDust(from + new Vector2(-4, -4) + circularLocation, 0, 0, 164, 0, 0, 0, default, 1.25f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.1f;
                    Main.dust[dust].scale = 1.8f;

                    if ((from - toLocation).Length() < 24)
                    {
                        break;
                    }
                }
            }
            return false;
        }
    }

    public class DarkDaggerProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 44;
            projectile.timeLeft = 560;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.damage = 56;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool ShouldUpdatePosition()
        {
            return projectile.timeLeft <= 420;
        }
        public override void AI()
        {
            projectile.rotation = MathHelper.ToRadians(90) + projectile.velocity.ToRotation();
            if (projectile.timeLeft == 420)
            {
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 71, 0.75f);
                for (int i = 0; i < 360; i += 5)
                {
                    Vector2 circular = new Vector2(12, 0).RotatedBy(MathHelper.ToRadians(i));
                    Dust dust = Dust.NewDustDirect(projectile.Center - new Vector2(5) + circular, 0, 0, 164, 0, 0, projectile.alpha);
                    dust.velocity *= 0.15f;
                    dust.velocity += -projectile.velocity;
                    dust.scale = 2.75f;
                    dust.noGravity = true;
                }
            }
            if (projectile.timeLeft > 420)
            {
                Player player = Main.player[(int)projectile.ai[0]];
                if (player.active)
                {
                    Vector2 toPlayer = projectile.Center - Main.MouseWorld;
                    toPlayer = toPlayer.SafeNormalize(Vector2.Zero) * 12;
                    projectile.velocity = -toPlayer;
                }
            }
            else
            {
                projectile.hostile = false;
                int dust = Dust.NewDust(projectile.Center + new Vector2(-4, -4), 0, 0, 164, 0, 0, projectile.alpha, default, 1.25f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].scale *= 0.75f;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 1.8f / 255f, (255 - projectile.alpha) * 0.0f / 255f, (255 - projectile.alpha) * 0.0f / 255f);
            if (projectile.timeLeft <= 25)
                projectile.alpha += 10;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 3;
        }
    }
}