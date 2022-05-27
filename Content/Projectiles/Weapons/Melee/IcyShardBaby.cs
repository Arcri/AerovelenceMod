using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace AerovelenceMod.Content.Projectiles.Weapons.Melee
{
    public class IcyShardBaby : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.width = 10;
            Projectile.height = 22;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 30;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
		public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)TextureAssets.Projectile[Projectile.type];
            Vector2 origin = new Vector2(texture.Width / 2, Projectile.height / 2);
            Color color = new Color(130, 130, 150, 0);
            for (int i = 0; i < 360; i += 60)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(1.5f, 2.5f), 0).RotatedBy(MathHelper.ToRadians(i * 2.5f));
                Main.EntitySpriteDraw(texture, Projectile.Center + circular - Main.screenPosition, null, color * ((255f - Projectile.alpha) / 255f), Projectile.rotation, origin, Projectile.scale * 0.8f, SpriteEffects.None, 0);
            }
            //color = projectile.GetAlpha(Color.White);
            //Main.EntitySpriteDraw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0.0f);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
            for (int k = 0; k < 2; k++)
            {
                float decrease = 2;
                for (int i = 8; i > 0; i--)
                {
                    Vector2 outwards = new Vector2(0, 1 * (k * 2 - 1)).RotatedBy(MathHelper.ToRadians(i * 12) + Projectile.rotation);
                    for (float j = 0; j <= 1; j += 0.2f)
                    {
                        Vector2 spawnAt = Projectile.Center;
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi / 2;
        }
    }
}
