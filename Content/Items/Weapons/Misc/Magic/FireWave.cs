using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic
{
    public class FireWave : ModProjectile
    {
        public int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire Wave");
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        Vector2 scale = Vector2.Zero;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());

            if (timer % 2 == 0 && timer < 70)
            {
                int a = Dust.NewDust(Projectile.position, 20, 20, DustID.FlameBurst);
                Main.dust[a].velocity *= 0.25f;
                Main.dust[a].color = Color.Yellow;
                Main.dust[a].scale = 0.65f;
                Main.dust[a].noGravity = true;

            }



            if (timer < 45)
            {
                float yScale = Math.Clamp(timer * 0.01f, 0, 0.7f);
                scale = new Vector2(yScale, (Projectile.velocity.Length() * 0.075f)) * 0.15f;
                Projectile.velocity *= 1.083f;

            } 
            else if (timer < 65)
            {
                Projectile.velocity *= 0.92f;
            }
            else
            {
                Projectile.velocity *= 0.94f;

                scale = Vector2.Lerp(scale, new Vector2(-1,-1), 0.01f);
                if (scale.Y <= 0)
                {
                    Projectile.Kill();
                }
            }

            timer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Wave = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FireWave").Value;

            //Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            //myEffect.Parameters["uColor"].SetValue(Color.SkyBlue.ToVector3() * 2f);
            //myEffect.Parameters["uTime"].SetValue(2);
            //myEffect.Parameters["uOpacity"].SetValue(0.4f); //0.8
            //myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition, Wave.Frame(1, 1, 0, 0), Color.OrangeRed, Projectile.rotation, Wave.Size() / 2, scale * 4, SpriteEffects.None, 0); ;
            Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition, Wave.Frame(1, 1, 0, 0), Color.Orange, Projectile.rotation, Wave.Size() / 2, scale * 4, SpriteEffects.None, 0); ;
            Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition - (Projectile.velocity * 0.55f), Wave.Frame(1, 1, 0, 0), Color.OrangeRed, Projectile.rotation, Wave.Size() / 2, scale * 3.5f, SpriteEffects.None, 0); ;
            Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition - (Projectile.velocity * 0.55f), Wave.Frame(1, 1, 0, 0), Color.Red, Projectile.rotation, Wave.Size() / 2, scale * 3.5f, SpriteEffects.None, 0); ;

            //Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition, Wave.Frame(1, 1, 0, 0), Color.SkyBlue, Projectile.rotation + 0.2f, Wave.Size() / 2, new Vector2(1, (Projectile.velocity.Length() * 0.2f)), SpriteEffects.None, 0); ;

            //Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition, Wave.Frame(1, 1, 0, 0), Color.SkyBlue, Projectile.rotation + 0.15f, Wave.Size() / 2, new Vector2(1.2f, 1), SpriteEffects.None, 0); ;
            //Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition, Wave.Frame(1, 1, 0, 0), Color.SkyBlue, Projectile.rotation - 0.15f, Wave.Size() / 2, new Vector2(1.2f, 1), SpriteEffects.None, 0); ;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 5; i++)
            {

                int a = Dust.NewDust(Projectile.position, 20, 20, DustID.FlameBurst);
                Main.dust[a].velocity *= 0.55f;
                Main.dust[a].color = Color.Yellow;
                Main.dust[a].scale = 0.75f;
                Main.dust[a].noGravity = true;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 5; i++)
            {

                int a = Dust.NewDust(Projectile.position, 20, 20, DustID.FlameBurst);
                Main.dust[a].velocity *= 0.55f;
                Main.dust[a].color = Color.Yellow;
                Main.dust[a].scale = 0.75f;
                Main.dust[a].noGravity = true;
            }
            base.OnHitNPC(target, damage, knockback, crit);
        }

    }
} 