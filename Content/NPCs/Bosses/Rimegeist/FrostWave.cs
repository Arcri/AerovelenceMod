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

namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class FrostWave : ModProjectile
    {
        public int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Wave");
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        Vector2 scale = Vector2.Zero;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;


            if (timer < 45)
            {
                scale = new Vector2(1, (Projectile.velocity.Length() * 0.05f)) * 0.2f;
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
                if (scale.X <= 0 && scale.Y <= 0)
                    Projectile.active = false;
            }

            timer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Wave = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Rimegeist/37-export").Value;

            //Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            //myEffect.Parameters["uColor"].SetValue(Color.SkyBlue.ToVector3() * 2f);
            //myEffect.Parameters["uTime"].SetValue(2);
            //myEffect.Parameters["uOpacity"].SetValue(0.4f); //0.8
            //myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition, Wave.Frame(1, 1, 0, 0), Color.SkyBlue, Projectile.rotation, Wave.Size() / 2, scale, SpriteEffects.None, 0); ;
            Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition, Wave.Frame(1, 1, 0, 0), Color.SkyBlue, Projectile.rotation, Wave.Size() / 2, scale, SpriteEffects.None, 0); ;
            Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition - (Projectile.velocity * 0.35f), Wave.Frame(1, 1, 0, 0), Color.SkyBlue, Projectile.rotation, Wave.Size() / 2, new Vector2(scale.X * 1.15f, scale.Y * 0.55f), SpriteEffects.None, 0); ;
            Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition - (Projectile.velocity * 0.35f), Wave.Frame(1, 1, 0, 0), Color.SkyBlue, Projectile.rotation, Wave.Size() / 2, new Vector2(scale.X * 0.85f, scale.Y * 0.55f), SpriteEffects.None, 0); ;

            //Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition, Wave.Frame(1, 1, 0, 0), Color.SkyBlue, Projectile.rotation + 0.2f, Wave.Size() / 2, new Vector2(1, (Projectile.velocity.Length() * 0.2f)), SpriteEffects.None, 0); ;

            //Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition, Wave.Frame(1, 1, 0, 0), Color.SkyBlue, Projectile.rotation + 0.15f, Wave.Size() / 2, new Vector2(1.2f, 1), SpriteEffects.None, 0); ;
            //Main.spriteBatch.Draw(Wave, Projectile.Center - Main.screenPosition, Wave.Frame(1, 1, 0, 0), Color.SkyBlue, Projectile.rotation - 0.15f, Wave.Size() / 2, new Vector2(1.2f, 1), SpriteEffects.None, 0); ;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

    }
} 