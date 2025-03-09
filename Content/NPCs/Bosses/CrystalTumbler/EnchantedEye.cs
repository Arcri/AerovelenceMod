using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.ID;
using static System.Net.Mime.MediaTypeNames;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class EnchantedEye : ModProjectile
    {
        private int shootTimer;
        private Vector2 direction;

        private bool hasFired;

        private Texture2D bloomTexture;
        private float bloomOpacity = 0f;
        private float bloomScale = 1f;

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 104;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;

            bloomTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/EnchantedEye_Glow_Bloom").Value;

            hiltOffset = Vector2.Zero;
            crystalOffset = Vector2.Zero;
            hasFired = false;
        }

        private Vector2 hiltOffset;
        private Vector2 crystalOffset;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glowmaskTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/EnchantedEye_Glow").Value;
            Texture2D lineTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Medusa_Gray").Value;
            Vector2 direction = new((float)Math.Cos(Projectile.rotation - MathHelper.PiOver2), (float)Math.Sin(Projectile.rotation - MathHelper.PiOver2));
            Color fadeColor = Color.White * (1f - fadeOutTimer / 60f);
            DrawTelegraphLine(Main.spriteBatch, Projectile.Center, direction, lineTexture, Color.Blue, 0.1f, 100f);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + hiltOffset - Main.screenPosition, null, fadeColor, Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(glowmaskTexture, Projectile.Center + crystalOffset - Main.screenPosition, null, fadeColor, Projectile.rotation, new Vector2(glowmaskTexture.Width / 2, glowmaskTexture.Height / 2), Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            if (bloomOpacity > 0f)
            {
                Main.EntitySpriteDraw(bloomTexture, Projectile.Center - Main.screenPosition, null, Color.White * bloomOpacity, Projectile.rotation, new Vector2(bloomTexture.Width / 2, bloomTexture.Height / 2), 1, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(bloomTexture, Projectile.Center - Main.screenPosition, null, Color.White * bloomOpacity, Projectile.rotation, new Vector2(bloomTexture.Width / 2, bloomTexture.Height / 2), 1, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        private int fadeOutTimer = 0;
        public override void AI()
        {
            Projectile.alpha = Math.Max(Projectile.alpha - 5, 0);

            Vector2 targetCenter = new Vector2(Projectile.ai[1], Projectile.ai[2]);

            if (Projectile.ai[0] == 0f)
            {
                Vector2 startOffset = Projectile.Center - targetCenter;
                Projectile.localAI[1] = startOffset.Length();
                Projectile.localAI[2] = startOffset.ToRotation();
                Projectile.ai[0] = 1f;
            }
            float orbitSpeed = 0.005f;
            Projectile.localAI[2] += orbitSpeed;
            float radius = Projectile.localAI[1];
            float angle = Projectile.localAI[2];
            Vector2 newOffset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
            Projectile.Center = targetCenter + newOffset;
            Vector2 lookDirection = targetCenter - Projectile.Center;
            Projectile.rotation = lookDirection.ToRotation() + MathHelper.PiOver2;
            shootTimer++;
            if (shootTimer >= 100 && !hasFired)
            {
                shootTimer = 0;
                bloomOpacity = 1f;
                bloomScale = 1.5f;

                hiltOffset = -direction * 15f;
                crystalOffset = -direction * 5f;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, lookDirection.SafeNormalize(Vector2.Zero) * 10f,
                    ModContent.ProjectileType<SharpCrystalShard>(), 18, Projectile.knockBack, Projectile.owner);

                hasFired = true;
                fadeOutTimer = 0;
            }

            if (hasFired)
            {
                fadeOutTimer++;
                float fadeProgress = fadeOutTimer / 60f;
                Projectile.alpha = (int)MathHelper.Lerp(0, 255, fadeProgress);
                Projectile.Center += -direction * 2f;

                if (fadeOutTimer >= 60)
                {
                    Projectile.Kill();
                }
            }
            if (bloomOpacity > 0f)
            {
                bloomOpacity -= 0.05f;
                bloomScale += 0.02f;
            }
            hiltOffset *= 0.95f;
            crystalOffset *= 0.95f;
        }




        float lineExtraPower = 0f;

        private void DrawTelegraphLine(SpriteBatch spriteBatch, Vector2 start, Vector2 direction, Texture2D lineTexture, Color color, float opacity, float length)
        {
            lineExtraPower = Math.Clamp(MathHelper.Lerp(lineExtraPower, -0.25f, 0.1f), 0f, 1f);
            Vector2 lineScale = new Vector2(length / lineTexture.Width, 0.2f + (lineExtraPower * 0.1f)) * 1.5f;
            float rotation = direction.ToRotation();
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, color * opacity * 1.5f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 1.25f, SpriteEffects.None, 0f);
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, Color.Aqua * opacity * 1.6f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, color * opacity * 1.7f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 0.75f, SpriteEffects.None, 0f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(2.25f, 2.25f) * Main.rand.Next(1, 3);
                dustVel += Projectile.velocity * 0.3f;
                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.2f, 0.35f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, timeBeforeSlow: 5,
                    preSlowPower: 0.94f, postSlowPower: 0.90f, velToBeginShrink: 1f, fadePower: 0.92f, shouldFadeColor: false);
            }
            for (float i = 0; i < 6.28f; i += 0.3f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Granite, new Vector2(MathF.Sin(i) * 1.3f, MathF.Cos(i)) * 2.4f);
                dust.velocity *= Main.rand.NextFloat(0.8f, 1.3f);
                dust.noGravity = true;
            }
        }
    }

    public class DashingEnchantedEye : ModProjectile
    {
        private int shootTimer;
        private int shardSpawnTimer = 0;
        private int shardDirection = 1;
        private int shardLeftID;
        private int shardRightID;
        private bool shardsLaunched = false;
        private Vector2 direction;

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 104;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        float lineExtraPower = 0f;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glowmaskTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/EnchantedEye_Glow").Value;
            Texture2D lineTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Medusa_Gray").Value;

            Vector2 direction = new((float)Math.Cos(Projectile.rotation - MathHelper.PiOver2), (float)Math.Sin(Projectile.rotation - MathHelper.PiOver2));
            DrawTelegraphLine(Main.spriteBatch, Projectile.Center, direction, lineTexture, Color.Blue, 0.1f, 200f);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(glowmaskTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(glowmaskTexture.Width / 2, glowmaskTexture.Height / 2), Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        private void DrawTelegraphLine(SpriteBatch spriteBatch, Vector2 start, Vector2 direction, Texture2D lineTexture, Color color, float opacity, float length)
        {
            lineExtraPower = Math.Clamp(MathHelper.Lerp(lineExtraPower, -0.25f, 0.1f), 0f, 1f);
            Vector2 lineScale = new Vector2(length / lineTexture.Width, 0.2f + (lineExtraPower * 0.1f)) * 1.5f;
            float rotation = direction.ToRotation();
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, color * opacity * 1.5f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 1.25f, SpriteEffects.None, 0f);
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, Color.Aqua * opacity * 1.6f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, color * opacity * 1.7f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 0.75f, SpriteEffects.None, 0f);
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Vector2 playerPosition = new(Projectile.ai[1], Projectile.ai[2]);
                direction = Vector2.Normalize(playerPosition - Projectile.Center);
                Projectile.rotation = direction.ToRotation() + MathHelper.PiOver2;
                Projectile.ai[0] = 1f;
            }

            shardSpawnTimer++;
            if (shardSpawnTimer >= 20 && !shardsLaunched)
            {
                shardSpawnTimer = 0;
                shardDirection *= -1;
                Vector2 offset = new Vector2(Projectile.width / 2, 0).RotatedBy(Projectile.rotation);
                offset.Y += shardDirection * 5;
                Vector2 shardPositionLeft = Projectile.Center + offset;
                Vector2 shardPositionRight = Projectile.Center - offset;
                shardLeftID = Projectile.NewProjectile(Projectile.GetSource_FromAI(), shardPositionLeft, Vector2.Zero, ModContent.ProjectileType<SharpCrystalShard>(), 0, 0f, Main.myPlayer);
                shardRightID = Projectile.NewProjectile(Projectile.GetSource_FromAI(), shardPositionRight, Vector2.Zero, ModContent.ProjectileType<SharpCrystalShard>(), 0, 0f, Main.myPlayer);
                Main.projectile[shardLeftID].rotation = Projectile.rotation;
                Main.projectile[shardRightID].rotation = Projectile.rotation;
            }
            if (shootTimer < 100)
            {
                shootTimer++;
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                if (!shardsLaunched)
                {
                    shootTimer = 0;
                    Vector2 dashDirection = Projectile.velocity;
                    Projectile.velocity = dashDirection;
                    shardsLaunched = true;
                    Vector2 shardVelocity = dashDirection * 1.5f;
                    Main.projectile[shardLeftID].velocity = shardVelocity;
                    Main.projectile[shardRightID].velocity = shardVelocity;
                    Main.projectile[shardLeftID].damage = Projectile.damage;
                    Main.projectile[shardRightID].damage = Projectile.damage;
                    Main.projectile[shardLeftID].tileCollide = true;
                    Main.projectile[shardRightID].tileCollide = true;
                }
                if (shootTimer++ >= 200)
                {
                    shootTimer = 0;
                    Vector2 direction = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction * 10f, ModContent.ProjectileType<ElectricBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }
}