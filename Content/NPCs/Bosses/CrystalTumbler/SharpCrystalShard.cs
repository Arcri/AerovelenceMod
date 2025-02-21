using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using System.Linq;
using AerovelenceMod.Content.Projectiles;
using System.Collections.Generic;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class SharpCrystalShard : TrailProjBase
    {
        private int timer;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 0;
        }

        float alpha = 1;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3() * 0.5f * alpha);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            timer++;
            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/LintyTrail").Value;
            trailColor = Color.MediumAquamarine * alpha;
            trailTime = timer * 0.05f;
            trailPointLimit = 10;
            trailWidth = 10;
            trailMaxLength = 120;
            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center + Projectile.velocity;
            TrailLogic();
        }


        public List<float> previousRotations;
        public List<Vector2> previousPositions;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projectileTexture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/SharpCrystalShard");

            if (timer != 0)
            {
                TrailDrawing();
            }

            #region after image
            if (previousRotations != null && previousPositions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    Color col = Color.Azure * Easings.easeOutCirc(progress);

                    float scale = 2.05f;

                    Main.EntitySpriteDraw(projectileTexture, previousPositions[i] - Main.screenPosition, null, col with { A = 0 } * progress * 0.9f,
                        previousRotations[i], projectileTexture.Size() / 2f, scale, SpriteEffects.None);
                }
            }
            #endregion

            for (int i = 0; i < 8; i++)
            {
                Color col2 = i == 0 ? Color.SkyBlue with { A = 0 } : Color.DeepSkyBlue with { A = 0 };

                Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, col2 * 1f, Projectile.rotation, projectileTexture.Size() / 2f, 1.1f, SpriteEffects.None, 0f);
            }
            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, projectileTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.25f, Projectile.rotation, projectileTexture.Size() / 2, 1.1f, SpriteEffects.None, 0f);

            return false;
        }

        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, trailWidth, num) * 0.5f;
        }
    }

    public class HomingSharpCrystalShard : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.alpha = 0;
            Projectile.damage = 6;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        private int dustSpawnTimer = 0;
        public int timer = 0;
        public float curveAmount = 1f;

        public List<float> previousRotations;
        public List<Vector2> previousPositions;

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/HomingSharpCrystalShard");

            #region after image
            if (previousRotations != null && previousPositions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    Color col = Color.Azure * Easings.easeOutCirc(progress);

                    float scale = 2.05f;

                    Main.EntitySpriteDraw(projectileTexture, previousPositions[i] - Main.screenPosition + new Vector2(0, 4), null, col with { A = 0 } * progress * 0.9f,
                        previousRotations[i], projectileTexture.Size() / 2f, scale, SpriteEffects.None);
                }
            }
            #endregion

            for (int i = 0; i < 8; i++)
            {
                Color col = i == 0 ? Color.SkyBlue with { A = 0 } : Color.DeepSkyBlue with { A = 0 };

                Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f) + new Vector2(0, 4), null, col * 1f, Projectile.rotation, projectileTexture.Size() / 2f, 1.1f, SpriteEffects.None, 0f);
            }
            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition + new Vector2(0, 4), null, lightColor, Projectile.rotation, projectileTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition + new Vector2(0, 4), null, Color.White with { A = 0 } * 0.25f, Projectile.rotation, projectileTexture.Size() / 2, 1.1f, SpriteEffects.None, 0f);


            return false;
        }


        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 0.9f);
            Projectile.rotation = Projectile.velocity.ToRotation();

            int targetIndex = -1;
            if (Main.player.Any(n => n.active && !n.dead))
            {
                targetIndex = Main.player.First(n => n.active && !n.dead).whoAmI;
            }
            Player player = Main.player[targetIndex];

            bool shouldHome = timer > 0f;
            float homeVal = MathHelper.Lerp(0f, 30f, Easings.easeInQuart(Math.Clamp(timer / 60f, 0f, 1f)));
            if (shouldHome)
            {
                Projectile.velocity = Projectile.velocity.ToRotation().AngleLerp((player.Center - Projectile.Center).ToRotation(), 0.01f).ToRotationVector2() * Projectile.velocity.Length();
                Projectile.timeLeft -= 1;

                float turnPower = homeVal;
                int turn2 = 30;

                Vector2 targetPos = player.Center;
                Vector2 toMouse = (targetPos - Projectile.Center).SafeNormalize(Vector2.UnitX) * turnPower;

                Projectile.velocity = (Projectile.velocity * (turn2 - 1) + toMouse) / turn2;
                if (Projectile.velocity.Length() > 15f)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 15f;
                }
                else if (Projectile.velocity.Length() < 15f)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 11f;
                }
            }

            if (timer < 0)
                Projectile.velocity *= 0.2f;

            dustSpawnTimer++;
            if (dustSpawnTimer >= 5)
            {
                Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(2f, 3.25f);

                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);

                dustSpawnTimer = 0;
            }

            timer++;
        }
    }
}