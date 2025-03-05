using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using AerovelenceMod.Content.Projectiles;
using System;
using AerovelenceMod.Content.NPCs.CrystalCaverns;

namespace AerovelenceMod.Content.Projectiles
{
    public class LightningStrike : ModProjectile
    {
        public Vector2 TargetPosition;
        private bool struck = false;
        private float TelegraphTime;
        private LightningUtility.LightningData lightningData;

        private int DrawTimer = 0;

        private bool firstFrame = false;

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 500;
        }

        public override void AI()
        {
            if (!firstFrame)
            {
                TargetPosition = new Vector2(Projectile.ai[0], Projectile.ai[1]);
                TelegraphTime = Projectile.ai[2];

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), TargetPosition, Vector2.Zero, ModContent.ProjectileType<LightningTelegraphProjectile>(), 0, 0, Projectile.owner, TelegraphTime);

                firstFrame = true;
            }

            TelegraphTime--;
            DrawTimer++;
            if (TelegraphTime <= 0)
            {
                struck = true;
                Strike();
            }

            if (lightningData == null)
            {
                lightningData = new LightningUtility.LightningData(Projectile)
                {
                    MaxSegments = 12,
                    TargetPosition = TargetPosition
                };
                LightningUtility.InitializeBetweenPoints(lightningData, Projectile.Center, TargetPosition, LightningUtility.LightningStyle.Static);
            }
            else if (struck)
            {
                LightningUtility.UpdateSegments(lightningData);
                LightningUtility.UpdateBranches(lightningData);
                LightningUtility.SpawnDust(lightningData);
            }
        }


        private void Strike()
        {
            struck = true;
            SoundEngine.PlaySound(SoundID.Item14, TargetPosition);
            lightningData.Alpha = 0f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (lightningData == null)
                return false;

            if (struck)
            {
                LightningUtility.DrawLightning(lightningData, Main.spriteBatch);
            }
            return false;
        }

    }

    public static class LightningManager
    {
        /// <summary>
        /// Call this method with a start and end position to spawn a lightning strike.
        /// </summary>
        /// <param name="start">The starting position of the lightning (e.g. where the projectile spawns).</param>
        /// <param name="end">The target strike position.</param>
        /// <param name="damage">Damage to be dealt (if applicable).</param>
        /// <param name="knockBack">Knockback of the strike.</param>
        /// <param name="telegraphTime">How long it will take for the lightning to strike the telegraph.</param>
        public static void StrikeLightning(Vector2 start, Vector2 end, int damage = 50, float knockBack = 0f, float telegraphTime = 100)
        {
            //Main.NewText($"Spawning LightningStrike at {start} -> {end}, Telegraph Time: {telegraphTime}");

            int projectileIndex = Projectile.NewProjectile(null,
                                                            start,
                                                            Vector2.Zero,
                                                            ModContent.ProjectileType<LightningStrike>(),
                                                            damage,
                                                            knockBack,
                                                            Main.myPlayer,
                                                            end.X,
                                                            end.Y,
                                                            telegraphTime);

            if (projectileIndex < Main.maxProjectiles && Main.projectile[projectileIndex].active)
            {
                Projectile lightningProj = Main.projectile[projectileIndex];
                //Main.NewText($"Lightning Strike AI Values: {lightningProj.ai[0]}, {lightningProj.ai[1]}, {lightningProj.ai[2]}");
            }
        }



    }
    public class LightningTelegraphProjectile : ModProjectile
    {
        private float flare10Rotation = 0f;
        private float fadeAlpha = 1f;

        private float telegraphTime = 100;
        private bool firstFrame = false;
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.alpha = 0;
        }

        public override void OnKill(int timeLeft)
        {
            foreach (Player player in Main.player)
            {
                if (player != null && player.active)
                {
                    player.GetModPlayer<AeroPlayer>().ScreenShakePower = 15;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 sparkVelocity = Main.rand.NextVector2CircularEdge(0.5f, 1.3f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, sparkVelocity, ProjectileID.Spark, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            for (int i = 0; i < 10; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(2f, 2f);
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, dustVelocity.X, dustVelocity.Y, 100, default, 1.5f);
                Main.dust[dustIndex].noGravity = true;
            }

            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SapperGasCloud>(), 0, Projectile.knockBack, Projectile.owner);
            }

            for (int i = 0; i < 5; i++)
            {
                int goreType = Main.rand.Next(61, 64);
                Gore.NewGorePerfect(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X - 10, Projectile.Center.Y), new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-0.3f, 0.3f)), goreType);
            }


        }

        public override void AI()
        {
            if(!firstFrame)
            {
                telegraphTime = Projectile.ai[0];
                Projectile.timeLeft = (int)telegraphTime;
                firstFrame = true;
            }

            if (Main.rand.NextBool(3))
            {
                float speed = Main.rand.NextFloat(1f, 2.5f);
                Vector2 velocity = Main.rand.NextVector2Circular(speed, speed);
                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.BlueTorch, velocity.X, velocity.Y);
                dust.noGravity = true;
                dust.scale = 1.0f;
            }
            if (Projectile.timeLeft <= 10)
            {
                float progress = (float)Projectile.timeLeft / 10f;
                float sine = (float)Math.Sin(Main.GameUpdateCount * 0.5f);
                fadeAlpha = 0.6f + 0.4f * (0.5f + 0.5f * sine);
            }
            else
            {
                fadeAlpha = 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            var oldState = spriteBatch.GraphicsDevice.BlendState;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            DrawFlaresAndArcs(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        private void DrawFlaresAndArcs(SpriteBatch spriteBatch)
        {
            Vector2 center = Projectile.Center - Main.screenPosition;
            Texture2D flare3 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/flare_3").Value;
            float pulseSpeed = 0.4f;
            float scaleBase = 0.1f;
            float scaleRange = 0.2f;
            float scale = scaleBase + scaleRange * (0.5f + 0.5f * (float)Math.Sin(Main.GameUpdateCount * pulseSpeed));
            Color flare3Color = Color.Lerp(Color.Aqua, Color.White, 0.3f) * fadeAlpha;
            spriteBatch.Draw(
                flare3,
                center,
                null,
                flare3Color,
                0f,
                flare3.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );

            spriteBatch.Draw(flare3, center, null, Color.White, 0f, flare3.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            Texture2D flare10 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/flare_10").Value;
            if (Main.GameUpdateCount % 10 == 0)
            {
                flare10Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                DrawMiniArc(spriteBatch, center);
            }

            int mod10 = (int)(Main.GameUpdateCount % 10);
            float flash = 1f - Math.Abs(mod10 - 5) / 5f;
            float flare10Scale = 0.2f + 0.2f * flash;
            Color flare10Color = Color.Lerp(Color.Cyan, Color.White, 0.2f) * (0.5f + 0.5f * flash) * fadeAlpha;

            spriteBatch.Draw(flare10, center, null, flare10Color, flare10Rotation, flare10.Size() * 0.5f, flare10Scale, SpriteEffects.None, 0f);
        }

        private void DrawMiniArc(SpriteBatch spriteBatch, Vector2 center)
        {
            float angle = Main.rand.NextFloat(MathHelper.TwoPi);
            float distance = Main.rand.NextFloat(15f, 20f);
            Vector2 endPoint = center + angle.ToRotationVector2() * distance;

            Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;
            Vector2 direction = endPoint - center;
            float length = direction.Length();
            float rotation = direction.ToRotation();

            Color arcColor = Color.Cyan * 0.8f * fadeAlpha;

            // Slight thickness
            float thickness = 2f;

            spriteBatch.Draw(
                pixel,
                center,
                new Rectangle(0, 0, 1, 1),
                arcColor,
                rotation,
                new Vector2(0, 0.5f),
                new Vector2(length, thickness),
                SpriteEffects.None,
                0f
            );
        }
    }
}