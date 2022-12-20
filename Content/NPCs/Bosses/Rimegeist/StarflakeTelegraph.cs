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
    public class StarflakeTelegraph : ModProjectile
    {
        public int timer = 0;
        public float initialRotation = 0f;
        float rotationBonus = 0f;
        public float numOfBarrages = 1f;
        public float numOfShots = 12f;
        public int tetheredNPC = 0; 

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starflake");
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.timeLeft = 400;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            //Lighting.AddLight(Projectile.Center, new Vector3(255, 255, 255) * 0.0005f);
            Projectile.Center = Main.npc[tetheredNPC].Center + new Vector2(-10,0);

            if (timer < 40)
            {
                initialRotation = initialRotation + 0.1f;
            }

            colorIntensity = MathHelper.Clamp((float)Math.Sin((float)timer / 20), 0, 1);

            if (timer == 60)
            {
                for (float i = 0f; i < 6.28f; i += 6.28f / numOfShots)
                {
                    //int pindexa = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(1, 0).RotatedBy(initialRotation + i) * 9f, ModContent.ProjectileType<IcySpike>(), Projectile.damage, 0);
                    int pindexb = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(1,0).RotatedBy(initialRotation + i) * 8f, ModContent.ProjectileType<WispSouls>(), Projectile.damage, 0);
                }
                Projectile.active = false;
                timer = -1;
            }

            Projectile.timeLeft++;
            timer++;
        }

        float teleScale = 1f;
        float drawAlpha = 1f;
        float colorIntensity = 2f;
        public override bool PreDraw(ref Color lightColor)
        {

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.White.ToVector3() * colorIntensity * 2);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.4f); //0.8
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            // : (
            for (float i = 0f; i < 6.28f; i += 6.28f / numOfShots)
            {
                Texture2D RayTex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/Medusa_Gray").Value;
                Main.spriteBatch.Draw(RayTex, Projectile.Center - Main.screenPosition + new Vector2(50 * teleScale, 0).RotatedBy(initialRotation + i), RayTex.Frame(1, 1, 0, 0), Color.Black * drawAlpha * 2f, initialRotation + i, RayTex.Size() / 2, teleScale, SpriteEffects.None, 0);;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            for (float i = 0f; i < 6.28f; i += 6.28f / numOfShots)
            {
                Texture2D RayTex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/Medusa_Gray").Value;
                Main.spriteBatch.Draw(RayTex, Projectile.Center - Main.screenPosition + new Vector2(50 * teleScale, 0).RotatedBy(initialRotation + i), RayTex.Frame(1, 1, 0, 0), Color.Black * drawAlpha * 1f, initialRotation + i, RayTex.Size() / 2, teleScale, SpriteEffects.None, 0); ;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

    }

    public class IceShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icy Spike");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public Rectangle frame = new Rectangle(1, 1, 0, 0);
        public override void SetDefaults()
        {
            frame = new Rectangle(0, 222 / 3 * Main.rand.Next(3), 24, 222 / 3);
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            //Projectile.velocity = new Vector2(-10, 0);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.SkyBlue) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], drawPos + new Vector2(-19,0).RotatedBy(Projectile.rotation + MathHelper.PiOver2), frame, color, Projectile.rotation, drawOrigin, Projectile.scale - (k * 0f), SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
} 