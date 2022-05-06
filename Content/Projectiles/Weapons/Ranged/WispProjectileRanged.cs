
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class WispProjectileRanged : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 17;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            {
                // Vector2 drawOrigin = new Vector2((Texture2D)TextureAssets.Projectile[projectile.type].Width, (Texture2D)TextureAssets.Projectile[projectile.type].Height);
                Texture2D texture2D = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    float scale = Projectile.scale * (Projectile.oldPos.Length - k) / Projectile.oldPos.Length * .45f;
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + TextureAssets.Projectile[Projectile.type].Size() / 3f;
                    Color color = Projectile.GetAlpha(Color.Black) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                    Main.EntitySpriteDraw(texture2D, drawPos, null, color, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Size(), scale, SpriteEffects.None, 0);
                }

                return true;
            }
        }

        
        private bool spawned;
        public override void AI()
        {
            int radians = 16;
            int Timer = 0;

            Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / 20f, 0f, 1f);

            if (!spawned)
            {
                spawned = true;
                SoundEngine.PlaySound(SoundID.Item104);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 1.03f;

            if (Timer < 60)
                Projectile.velocity /= 1.03f;


            Projectile.ai[0] += 1;

            if (Projectile.ai[0] >= 8)
            {
                Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedByRandom(MathHelper.ToRadians(radians));
                if (radians >= 16)
                {
                    radians = -24;
                }
                if (radians <= -16)
                {
                    radians = 16;
                }
                float scale = 1f - (Main.rand.NextFloat() * .3f);
                perturbedSpeed = perturbedSpeed * scale;
                Projectile.velocity.Y = perturbedSpeed.Y;
                Projectile.velocity.X = perturbedSpeed.X;
                Projectile.ai[0] = 0;
            }
        }
    }
}
