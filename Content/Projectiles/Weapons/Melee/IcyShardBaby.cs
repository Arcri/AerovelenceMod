using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Melee
{
    public class IcyShardBaby : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.aiStyle = 0;
            projectile.width = 10;
            projectile.height = 22;
            projectile.alpha = 0;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.melee = true;
            projectile.timeLeft = 30;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = new Vector2(texture.Width / 2, projectile.height / 2);
            Color color = new Color(130, 130, 150, 0);
            for (int i = 0; i < 360; i += 60)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(1.5f, 2.5f), 0).RotatedBy(MathHelper.ToRadians(i * 2.5f));
                Main.spriteBatch.Draw(texture, projectile.Center + circular - Main.screenPosition, null, color * ((255f - projectile.alpha) / 255f), projectile.rotation, origin, projectile.scale * 0.8f, SpriteEffects.None, 0.0f) ;
            }
            //color = projectile.GetAlpha(Color.White);
            //Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0.0f);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 50, 0.75f, 0.1f);
            for (int k = 0; k < 2; k++)
            {
                float decrease = 2;
                for (int i = 8; i > 0; i--)
                {
                    Vector2 outwards = new Vector2(0, 1 * (k * 2 - 1)).RotatedBy(MathHelper.ToRadians(i * 12) + projectile.rotation);
                    for (float j = 0; j <= 1; j += 0.2f)
                    {
                        Vector2 spawnAt = projectile.Center;
                        Dust dust = Dust.NewDustDirect(spawnAt - new Vector2(5), 0, 0, ModContent.DustType<WispDust>());
                        dust.velocity = outwards * decrease;
                        dust.noGravity = true;
                        dust.scale *= 0.1f;
                        dust.scale += 1f;
                    }
                    decrease -= 0.25f;
                }
            }
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi / 2;
        }
    }
}