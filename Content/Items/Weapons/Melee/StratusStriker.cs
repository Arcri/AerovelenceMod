using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

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
            Item.UseSound = SoundID.Item1;
            Item.crit = 8;
            Item.damage = 74;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 52; 
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 0, 40, 20);
            Item.rare = ItemRarityID.Cyan;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StratusProjectile>();
            Item.shootSpeed = 18f;
        }
    }

    public class StratusProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Razor");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 7;
            Projectile.timeLeft = 200;
            Projectile.alpha = 100;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 vector = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 position = Projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.spriteBatch.Draw((Texture2D)TextureAssets.Projectile[Projectile.type], position, null, color, Projectile.rotation, vector, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Projectiles/Weapons/Magic/StormRazorProjectile_Glow");
            Main.EntitySpriteDraw(
                texture,
                new Vector2
                (
                    Projectile.Center.Y - Main.screenPosition.X,
                    Projectile.Center.X - Main.screenPosition.Y
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                Projectile.rotation,
                texture.Size(),
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }
        public override void AI()
        {
            Projectile.scale *= 1.002f;
            Projectile.rotation += 100;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 63, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.White, 1);
            Main.dust[dust].velocity /= 1.2f;
            Main.dust[dust].noGravity = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 offset = new Vector2(0, 0);
            SoundEngine.PlaySound(SoundID.Item10);

            for (float i = 0; i < 360; i += 0.5f)
            {
                float ang = (float)(i * Math.PI) / 180;
                float x = (float)(Math.Cos(ang) * 15) + Projectile.Center.X;
                float y = (float)(Math.Sin(ang) * 15) + Projectile.Center.Y;
                Vector2 vel = Vector2.Normalize(new Vector2(x - Projectile.Center.X, y - Projectile.Center.Y)) * 7;
                int dustIndex = Dust.NewDust(new Vector2(x - 3, y - 3), 6, 6, 63, vel.X, vel.Y);
                Main.dust[dustIndex].noGravity = true;
            }
            return false;
        }
    }
}