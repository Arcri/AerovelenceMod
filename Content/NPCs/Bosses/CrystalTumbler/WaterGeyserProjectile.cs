using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Projectiles;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class WaterGeyserProjectile : TrailProjBase
    {
        private Vector2 initialPosition;
        private float stopHeight = 500f;
        private int ascendTime = 120;
        private bool fadingOut = false;

        private Texture2D smokeTexture;
        private Texture2D slashTexture;

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        private float direction = 0f;
        private float curvePower = 1f;
        private int timer = 0;
        private float waveFrequency = 0.05f;
        private float waveAmplitude = 2f;
        private float fadeOutTimer = 60f;

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        public override void AI()
        {
            if (timer == 0)
            {
                initialPosition = Projectile.Center;
                smokeTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Smoke/smoke_01").Value;
                slashTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/pixelKennySlash").Value;
            }
            if (Projectile.position.Y <= initialPosition.Y - stopHeight || timer >= ascendTime)
            {
                fadingOut = true;
                Projectile.velocity.Y = 0f;
                Projectile.timeLeft = (int)fadeOutTimer;
            }
            else
            {
                Vector2 waveOffset = new((float)Math.Sin(timer * waveFrequency) * waveAmplitude, 0);
                Projectile.position += waveOffset.RotatedBy(Projectile.velocity.ToRotation());
            }

            float fadeFactor = (float)Projectile.timeLeft / fadeOutTimer;
            fadeFactor *= 1.1f;
            if (fadingOut)
            {   
                Main.NewText(fadeFactor);
                trail1.trailColor *= fadeFactor;
                trail2.trailColor *= fadeFactor;
                Main.NewText(fadeFactor);
                float slashScaleFactor = fadeFactor * 0.8f + (float)Math.Sin(timer * 0.05f) * 0.1f;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition;
                //Main.spriteBatch.Draw(slashTexture, drawPosition, null, Color.AliceBlue * fadeFactor, 0f, slashTexture.Size() / 2, slashScaleFactor, SpriteEffects.None, 0f);
            }
            ApplyWetDebuff();

            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value;
            trail1.trailPointLimit = (int)(1200 * fadeFactor);
            trail2.trailMaxLength = 1000 * fadeFactor;
            trail1.trailWidth = (int)(100 * fadeFactor);
            trail1.timesToDraw = 1;
            trail1.pinch = true;
            trail1.pinchAmount = 0.75f;

            trail1.trailTime = timer * 0.01f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center;
            trail1.TrailLogic();

            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value;
            trail2.trailPointLimit = (int)(1200 * fadeFactor);
            trail2.trailMaxLength = 1000 * fadeFactor;
            trail2.trailWidth = (int)(120 * fadeFactor);
            trail2.timesToDraw = 2;
            trail2.pinch = true;
            trail2.pinchAmount = 0.55f;

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/LoopingWaterGrad").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = timer * 0.01f;

            trail2.trailTime = timer * 0.02f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center;
            trail2.TrailLogic();

            timer++;
        }

        private void ApplyWetDebuff()
        {
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && player.Hitbox.Intersects(new Rectangle((int)initialPosition.X - Projectile.width / 2, (int)initialPosition.Y, Projectile.width, (int)stopHeight)))
                {
                    player.AddBuff(BuffID.Wet, 120);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var originalBlendState = Main.spriteBatch.GraphicsDevice.BlendState;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            DrawRadialSmokeEffect();
            DrawKennySlashEffect();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, originalBlendState, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);
            return false;
        }


        private void DrawRadialSmokeEffect()
        {
            if (Projectile.active)
            {
                int smokeCount = 5;
                float smokeScale = 0.2f;
                float rotationSpeed = 0.1f;
                float scaleVariation = 0.1f;

                for (int i = 0; i < smokeCount; i++)
                {
                    float rotation = MathHelper.ToRadians(360f / smokeCount * i + timer * rotationSpeed);
                    float scale = smokeScale + (float)Math.Sin(timer * 0.1f + i) * scaleVariation;
                    Vector2 offset = new Vector2(16f, 0).RotatedBy(rotation);
                    Vector2 drawPosition = initialPosition - Main.screenPosition + offset;

                    Main.spriteBatch.Draw(smokeTexture, drawPosition, null, Color.White * 0.6f, rotation, smokeTexture.Size() / 2, scale, SpriteEffects.None, 0f);
                }
            }
        }

        private void DrawKennySlashEffect()
        {
            if (Projectile.active)
            {
                float slashScale = 0.8f + (float)Math.Sin(timer * 0.05f) * 0.1f;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition;

                Main.spriteBatch.Draw(slashTexture, drawPosition, null, Color.AliceBlue * 0.8f, 0f, slashTexture.Size() / 2, slashScale, SpriteEffects.None, 0f);
            }
        }
    }
}