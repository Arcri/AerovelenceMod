using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class StratusStriker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stratus Striker");
            Tooltip.SetDefault("Fires a storm razor that can travel on blocks\nStorm razors also increase in size");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.crit = 8;
            item.damage = 74;
            item.melee = true;
            item.width = 50;
            item.height = 52; 
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5;
            item.value = Item.sellPrice(0, 0, 40, 20);
            item.rare = ItemRarityID.Cyan;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<StratusProjectile>();
            item.shootSpeed = 18f;
        }
    }

    public class StratusProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Razor");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 7;
            projectile.timeLeft = 200;
            projectile.alpha = 100;
        }
        public override bool PreDraw(SpriteBatch sb, Color lightColor)
        {
            Vector2 vector = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Vector2 position = projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / projectile.oldPos.Length);
                sb.Draw(Main.projectileTexture[projectile.type], position, null, color, projectile.rotation, vector, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("AerovelenceMod/Content/Projectiles/Weapons/Magic/StormRazorProjectile_Glow");
            spriteBatch.Draw(
                texture,
                new Vector2
                (
                    projectile.Center.Y - Main.screenPosition.X,
                    projectile.Center.X - Main.screenPosition.Y
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                projectile.rotation,
                texture.Size(),
                projectile.scale,
                SpriteEffects.None,
                0f
            );
        }
        public override void AI()
        {
            projectile.scale *= 1.002f;
            projectile.rotation += 100;
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 63, projectile.velocity.X, projectile.velocity.Y, 0, Color.White, 1);
            Main.dust[dust].velocity /= 1.2f;
            Main.dust[dust].noGravity = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 offset = new Vector2(0, 0);
            Main.PlaySound(SoundID.Item10);

            for (float i = 0; i < 360; i += 0.5f)
            {
                float ang = (float)(i * Math.PI) / 180;
                float x = (float)(Math.Cos(ang) * 15) + projectile.Center.X;
                float y = (float)(Math.Sin(ang) * 15) + projectile.Center.Y;
                Vector2 vel = Vector2.Normalize(new Vector2(x - projectile.Center.X, y - projectile.Center.Y)) * 7;
                int dustIndex = Dust.NewDust(new Vector2(x - 3, y - 3), 6, 6, 63, vel.X, vel.Y);
                Main.dust[dustIndex].noGravity = true;
            }
            return false;
        }
    }
}