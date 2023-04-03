using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.SkillStrikes;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class LightningBorder : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;
        public override void SetStaticDefaults()
        {
        }

        public int enemiesHit = 0;
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 5000;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

        }
        public override void AI()
        {
            rot1 += 0.018f;
            rot2 -= 0.018f;
            rot3 += 0.04f;
            rot4 -= 0.04f;

            timer++;
            Projectile.velocity = Vector2.Zero;
        }
        float rot1 = 0;
        float rot2 = 0;
        float rot3 = 0;
        float rot4 = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D barrierTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_02");
            Texture2D barrierTex2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/bigCircle3");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.spriteBatch.Draw(barrierTex, Projectile.Center - Main.screenPosition, null, Color.HotPink * (float)(1f - (float)(Math.Sin(timer * 0.05f) * 0.25f)), rot1, barrierTex.Size() / 2, 3.33f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(barrierTex, Projectile.Center - Main.screenPosition, null, Color.HotPink * 0.8f, rot1, barrierTex.Size() / 2, 3.33f, SpriteEffects.None, 0f);
           // Main.spriteBatch.Draw(barrierTex, Projectile.Center - Main.screenPosition, null, Color.HotPink * 0.8f, rot1, barrierTex.Size() / 2, 3.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(barrierTex2, Projectile.Center - Main.screenPosition, null, Color.HotPink * 1.25f, rot1, barrierTex2.Size() / 2, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(barrierTex2, Projectile.Center - Main.screenPosition, null, Color.HotPink, rot1 * 2, barrierTex2.Size() / 2, 1.01f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(barrierTex2, Projectile.Center - Main.screenPosition, null, Color.HotPink, rot2, barrierTex2.Size() / 2, 1f, SpriteEffects.None, 0f);


            //spriteBatch.Draw(barrierTex, auraPosition - Main.screenPosition, null, Color.HotPink, 0f, barrierTex.Size() / 2, 3f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
    }
} 