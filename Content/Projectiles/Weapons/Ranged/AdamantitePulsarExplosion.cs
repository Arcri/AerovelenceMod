using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class AdamantitePulsarExplosion : ModProjectile
    {

        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electromagnetic Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.damage = 50;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Projectile.ai[1] += 0.01f;
            Projectile.scale = Projectile.ai[1];
            if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }
            Projectile.alpha -= 63;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            Projectile.Damage();

            int dusts = 5;
            for (int i = 0; i < dusts; i++)
            {
                if (Main.rand.NextBool(3))
                {
                    float speed = 6f;
                    Vector2 velocity = new Vector2(0f, -speed * Main.rand.NextFloat(0.5f, 1.2f)).RotatedBy(MathHelper.ToRadians(360f / i * dusts + Main.rand.NextFloat(-50f, 50f)));
                    Dust dust1 = Dust.NewDustPerfect(Projectile.Center, 59, velocity, 150, default, 1.5f);
                    dust1.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int cooldown = 4;
            Projectile.localNPCImmunity[target.whoAmI] = 6;
            target.immune[Projectile.owner] = cooldown;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            {
                Main.instance.LoadProjectile(Projectile.type);
                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                Rectangle rectangle = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
                Color color = Projectile.GetAlpha(lightColor);

                if (!Projectile.hide)
                {
                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rectangle, color, Projectile.rotation, rectangle.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
                }
                return false;
            }
        }
    }
}