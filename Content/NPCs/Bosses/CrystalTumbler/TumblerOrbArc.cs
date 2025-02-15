using System;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerOrbArc : ModProjectile
    {
        private BaseTrailInfo trail1 = new BaseTrailInfo();
        private BaseTrailInfo trail2 = new BaseTrailInfo();
        private int timer;

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 88;
            Projectile.damage = 12;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 120 * (Projectile.extraUpdates + 1);
        }


        private int dustSpawnTimer = 0;

        public override void AI()
        {
            timer++;
            float lifeRatio = Projectile.timeLeft / 240f;
            int maxTrailLength1 = 600;
            int maxTrailPoints1 = 600;

            int maxTrailLength2 = 500;
            int maxTrailPoints2 = 500;
            trail1.trailMaxLength = (int)(maxTrailLength1 * lifeRatio);
            trail1.trailPointLimit = (int)(maxTrailPoints1 * lifeRatio);

            trail2.trailMaxLength = (int)(maxTrailLength2 * lifeRatio);
            trail2.trailPointLimit = (int)(maxTrailPoints2 * lifeRatio);

            // Setup trail1
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/RealLightning").Value;
            trail1.trailColor = Color.White * 1f;
            trail1.trailWidth = 60;
            trail1.timesToDraw = 1;
            trail1.pinch = true;
            trail1.pinchAmount = 0.55f;

            trail1.trailTime = timer * 0.01f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center;
            trail1.TrailLogic();

            // Setup trail2
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
            trail2.trailColor = Color.Wheat;
            trail2.trailWidth = 30;
            trail2.timesToDraw = 2;
            trail2.pinch = true;
            trail2.pinchAmount = 0.55f;

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/LoopingThunderGrad").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = timer * 0.01f;

            trail2.trailTime = timer * 0.02f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center;
            trail2.TrailLogic();

            dustSpawnTimer++;
            if (dustSpawnTimer >= 5)
            {
                Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(2f, 3.25f);
                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
                dustSpawnTimer = 0;
            }
            Projectile.frameCounter++;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.45f, 0.5f);
            if (Projectile.velocity == Vector2.Zero)
            {
                if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
                {
                    Projectile.frameCounter = 0;
                    bool flag34 = true;
                    for (int num874 = 1; num874 < Projectile.oldPos.Length; num874++)
                    {
                        if (Projectile.oldPos[num874] != Projectile.oldPos[0])
                        {
                            flag34 = false;
                        }
                    }
                    if (flag34)
                    {
                        Projectile.Kill();
                        return;
                    }
                }

                if (Main.rand.NextBool(Projectile.extraUpdates))
                {

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dustVel = Main.rand.NextVector2Circular(2.25f, 2.25f) * Main.rand.Next(1, 3);
                        dustVel += Projectile.velocity * 0.3f;

                        Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.2f, 0.35f));
                        gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, timeBeforeSlow: 5,
                            preSlowPower: 0.94f, postSlowPower: 0.90f, velToBeginShrink: 1f, fadePower: 0.92f, shouldFadeColor: false);
                    }

                }
            }
            else
            {
                if (Projectile.frameCounter < Projectile.extraUpdates * 2)
                {
                    return;
                }
                Projectile.frameCounter = 0;
                float num880 = Projectile.velocity.Length();
                UnifiedRandom unifiedRandom = new UnifiedRandom((int)Projectile.ai[1]);
                int num881 = 0;
                Vector2 spinningpoint14 = -Vector2.UnitY;

                while (true)
                {
                    int num882 = unifiedRandom.Next();
                    Projectile.ai[1] = num882;
                    num882 %= 100;
                    float f = (float)num882 / 100f * MathHelper.TwoPi;
                    Vector2 vector72 = f.ToRotationVector2();

                    if (vector72.Y > 0f)
                    {
                        vector72.Y *= -1f;
                    }

                    bool flag35 = false;
                    if (vector72.Y > -0.02f)
                    {
                        flag35 = true;
                    }

                    if (vector72.X * (Projectile.extraUpdates + 1) * 2f * num880 + Projectile.localAI[0] > 40f)
                    {
                        flag35 = true;
                    }

                    if (vector72.X * (Projectile.extraUpdates + 1) * 2f * num880 + Projectile.localAI[0] < -40f)
                    {
                        flag35 = true;
                    }

                    if (flag35)
                    {
                        if (num881++ >= 100)
                        {
                            Projectile.velocity = Vector2.Zero;
                            Projectile.localAI[1] = 1f;
                            break;
                        }
                        continue;
                    }

                    spinningpoint14 = vector72;
                    break;
                }

                if (Projectile.velocity != Vector2.Zero)
                {
                    Projectile.localAI[0] += spinningpoint14.X * (Projectile.extraUpdates + 1) * 2f * num880;
                    Projectile.velocity = spinningpoint14.RotatedBy(Projectile.ai[0] + MathHelper.PiOver2) * num880;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);
            return false;
        }
    }
}
