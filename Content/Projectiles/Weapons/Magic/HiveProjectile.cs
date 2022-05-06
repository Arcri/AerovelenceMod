
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class HiveProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive Projectile");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 35;
           
            Projectile.aiStyle = -1;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;                       
        }

        int Timer = 0;
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.1f * Projectile.direction;
            Projectile.velocity.X *= 0.984f;
            Projectile.velocity.Y += 0.28f;

            Projectile.ai[0]++;
            if(Timer >= 15)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.Y - 16f, Main.rand.Next(-10, 9) * .25f, Main.rand.Next(-10, 5) * .25f, ProjectileID.Wasp, (int)(Projectile.damage * .5f), 0, Projectile.owner);
                Projectile.ai[0] = 0;
            }
        }

        public override void Kill(int timeLeft)
        { 
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.position);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.Y - 16f, Main.rand.Next(-10, 9) * .25f, Main.rand.Next(-10, 5) * .25f, ProjectileID.Wasp, (int)(Projectile.damage * .5f), 0, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.Y - 16f, Main.rand.Next(-2, 5) * .25f, Main.rand.Next(-2, 7) * .25f, ProjectileID.Wasp, (int)(Projectile.damage * .5f), 0, Projectile.owner);

            for(int i = 0; i < 12; ++i)
            { 
               float random = Main.rand.NextFloat(-6f, 6f);
               Dust dust = Dust.NewDustDirect(Projectile.position, 0 , 0, 153, random, random);
               dust.scale = 0.8f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Color color = Color.Lerp(Color.Red, Color.Pink, 0.5f + (float)Math.Sin(MathHelper.ToRadians(Projectile.frame)) / 2f) * 0.5f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle), color, Projectile.oldRot[i], rectangle.Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}
