
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class HiveProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive Projectile");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 35;
           
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = true;
        }

        int Timer = 0;
        public override void AI()
        {
            projectile.rotation += projectile.velocity.Length() * 0.1f * projectile.direction;
            projectile.velocity.X *= 0.984f;
            projectile.velocity.Y += 0.28f;

            Timer++;
            if(Timer >= 15)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-10, 9) * .25f, Main.rand.Next(-10, 5) * .25f, ProjectileID.Wasp, (int)(projectile.damage * .5f), 0, projectile.owner);
                Timer = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCDeath19, projectile.position);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-10, 9) * .25f, Main.rand.Next(-10, 5) * .25f, ProjectileID.Wasp, (int)(projectile.damage * .5f), 0, projectile.owner);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-2, 5) * .25f, Main.rand.Next(-2, 7) * .25f, ProjectileID.Wasp, (int)(projectile.damage * .5f), 0, projectile.owner);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Color color = Color.Lerp(Color.Red, Color.Pink, 0.5f + (float)Math.Sin(MathHelper.ToRadians(projectile.frame)) / 2f) * 0.5f;
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + projectile.Size / 2f - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle), color, projectile.oldRot[i], rectangle.Size() / 2f, 1f, projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}
