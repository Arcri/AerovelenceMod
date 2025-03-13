using System;
using AerovelenceMod.Common.Utilities;
using System.Collections.Generic;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns.Skylight;
using AerovelenceMod.Content.Projectiles.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.GameContent;
using AerovelenceMod.Content.Items.Weapons.CrystalCaverns.ThunderLance;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerOrb : ModProjectile
    {

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 106;
            Projectile.height = 106;
            Projectile.aiStyle = 88;
            Projectile.damage = 15;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
        }
        private bool hasSpawnedVFX = false;

        public override void AI()
        {
            if (!hasSpawnedVFX)
            {
                int vfxIndex = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TumblerOrbVFX>(), 0, 0, Main.myPlayer);
                Main.projectile[vfxIndex].ai[0] = Projectile.whoAmI;
                hasSpawnedVFX = true;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 7 == 0)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            if (Projectile.localAI[1] == 0f) 
                Projectile.localAI[1] = 1f;

            if (Projectile.ai[0] < 180f)
            {
                Projectile.alpha -= 5;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

            }
            else
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    return;
                }
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] % 60f == 0f && Projectile.ai[0] < 180f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int[] targets = new int[10];
                Vector2[] targetPositions = new Vector2[5];
                int numberProjectiles = 0;
                float maxDistance = 2000f;
                for (int i = 0; i < 255; i++)
                {
                    Player player = Main.player[i];
                    if (!player.active || player.dead)
                    {
                        continue;
                    }
                    Vector2 playerCenter = player.Center;
                    float distanceToPlayer = Vector2.Distance(playerCenter, Projectile.Center);
                    if (distanceToPlayer < maxDistance && Collision.CanHit(Projectile.Center, 1, 1, playerCenter, 1, 1))
                    {
                        targets[numberProjectiles] = i;
                        targetPositions[numberProjectiles] = playerCenter;
                        numberProjectiles++;
                        if (numberProjectiles >= targetPositions.Length)
                        {
                            break;
                        }
                    }
                }

                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 directionToTarget = targetPositions[i] - Projectile.Center;
                    float ai = Main.rand.Next(100);
                    Vector2 velocity = Vector2.Normalize(directionToTarget.RotatedByRandom(MathHelper.ToRadians(45))) * 4f;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ProjectileType<TumblerOrbArc>(), 10, 0f, Main.myPlayer, directionToTarget.ToRotation(), ai);
                    SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/lightning_flash_01_trim") with { Volume = .75f, Pitch = 1.3f, PitchVariance = 0f, };
                    SoundEngine.PlaySound(stylea, Projectile.Center);
                }
            }

            Lighting.AddLight(Projectile.Center, 0.4f, 0.85f, 0.9f);
            if (Projectile.alpha < 150 && Projectile.ai[0] < 180f)
            {
                for (int i = 0; i < 2; i++)
                {
                    float offsetX = Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 dustPosition = new Vector2(-Projectile.width * 0.2f * Projectile.scale, 0f).RotatedBy(offsetX * MathHelper.TwoPi).RotatedBy(Projectile.velocity.ToRotation());
                    int dustIndex = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, DustID.Electric, -Projectile.velocity.X / 3f, -Projectile.velocity.Y / 3f, 150, Color.Transparent, 0.7f);
                    Main.dust[dustIndex].position = Projectile.Center + dustPosition;
                    Main.dust[dustIndex].velocity = Vector2.Normalize(Main.dust[dustIndex].position - Projectile.Center) * 2f;
                    Main.dust[dustIndex].noGravity = true;
                }

                for (int i = 0; i < 2; i++)
                {
                    float offsetY = Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 dustPosition = new Vector2(-Projectile.width * 6f * Projectile.scale, 0f).RotatedBy(offsetY * MathHelper.TwoPi).RotatedBy(Projectile.velocity.ToRotation());
                    int dustIndex = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, DustID.Electric, -Projectile.velocity.X / 3f, -Projectile.velocity.Y / 3f, 150, Color.Transparent, 0.7f);
                    Main.dust[dustIndex].velocity = Vector2.Zero;
                    Main.dust[dustIndex].position = Projectile.Center + dustPosition;
                    Main.dust[dustIndex].noGravity = true;
                }
            }
        }

        public List<float> previousRotations;
        public List<Vector2> previousPostions;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Orb = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/TumblerOrb").Value;
            int frameHeight = Orb.Height / Main.projFrames[Projectile.type];
            int frameY = Projectile.frame * frameHeight;
            Rectangle sourceRectangle = new(0, frameY, Orb.Width, frameHeight);
            for (int i = 0; i < 8; i++)
            {
                Color col = i == 0 ? Color.SkyBlue with { A = 0 } : Color.DeepSkyBlue with { A = 0 };
                Main.spriteBatch.Draw(Orb, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), sourceRectangle, col * 1f, Projectile.rotation, new Vector2(Orb.Width / 2, frameHeight / 2), Projectile.scale * 1.1f, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(Orb, Projectile.Center - Main.screenPosition, sourceRectangle, lightColor, Projectile.rotation, new Vector2(Orb.Width / 2, frameHeight / 2), Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Orb, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White with { A = 0 } * 0.25f, Projectile.rotation, new Vector2(Orb.Width / 2, frameHeight / 2), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }


    public class TumblerOrbVFX : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.timeLeft = 800;
            Projectile.scale = 1f;
        }

        public override bool? CanDamage() => false;

        private int timer = 0;
        private float scale = 0f;
        private float alpha = 1f;
        private float glowScale = 0f;
        private float initialScaleFactor = 0.25f;
        private bool parentDied = false;

        public override void AI()
        {
            Projectile parent = Main.projectile[(int)Projectile.ai[0]];
            if (!parent.active || parent.type != ModContent.ProjectileType<TumblerOrb>())
parentDied = true;
            if (!parentDied)
            {
                Projectile.Center = parent.Center;
                if (timer == 0)
                {
                    CreateDust();
                    CreateInitialVFX();
                }
                scale = MathHelper.Lerp(scale, initialScaleFactor * 4f, 0.2f);
                glowScale = MathHelper.Lerp(glowScale, 1f, 0.04f);
            }
            else
            {
                scale -= 0.035f;
                alpha -= 0.065f;
                if (alpha <= 0)
                {
                    Projectile.active = false;
                }
            }

            if (timer < 38)

                CreateContinualDust();
            

            Projectile.rotation += 0.12f;
            timer++;
        }
        private void CreateDust()
        {
            Color col = new(0, 155, 255);
            for (int i = 0; i < 45; i++)
            {
                Vector2 randomStart = Main.rand.NextVector2CircularEdge(10, 10);
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MuraLineDust>(),
                    randomStart * Main.rand.NextFloat(0.65f, 1.35f), newColor: col, Scale: 0.3f + Main.rand.NextFloat(0, 0.2f));
            }
        }

        private void CreateContinualDust()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 start = Main.rand.NextVector2CircularEdge(10, 10);
                Dust da = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MuraLineDust>(),
                    start * Main.rand.NextFloat(0.65f, 1.35f), newColor: Color.DodgerBlue, Scale: 0.3f + Main.rand.NextFloat(0, 0.2f));
                da.fadeIn = 10f;
                da.alpha = 13;
            }
        }

        private void CreateInitialVFX()
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 direction = new Vector2(1, 0).RotatedByRandom(6.28f);
                Vector2 ai1 = new Vector2((float)Math.Cos(direction.ToRotation()), (float)Math.Sin(direction.ToRotation())) * 10f;
                float ai2 = Main.rand.Next(100);
                int lightning = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity * 10, ai1.RotatedByRandom(6.28f) * 2.5f, ModContent.ProjectileType<LightningHitFX>(), 0, 0, Main.myPlayer, ai1.ToRotation(), ai2);
                Main.projectile[lightning].scale = 0.3f;
            }

            int afg = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DistortProj>(), 0, 0);
            Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);
            Main.projectile[afg].timeLeft = 10;

            if (Main.projectile[afg].ModProjectile is DistortProj distort)
            {
                distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                distort.implode = false;
                distort.scale = 0.6f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Flare/flare_4").Value;
            Texture2D Flare2 = Mod.Assets.Request<Texture2D>("Assets/Orbs/spiky_20fade").Value;
            Texture2D Flare3 = Mod.Assets.Request<Texture2D>("Assets/Flare/flare_1").Value;
            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/SofterBlueGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);
            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
            myEffect.Parameters["distortStrength"].SetValue(0.02f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue(timer * 0.015f);
            myEffect.Parameters["colorIntensity"].SetValue(alpha * 1.25f);

            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.Black * 0.5f * alpha, Projectile.rotation, Ball.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue * 0.3f * alpha, Projectile.rotation, Ball.Size() / 2, glowScale * 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.DodgerBlue * alpha, Projectile.rotation * 0.8f, Flare.Size() / 2, scale * 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.SkyBlue * alpha, Projectile.rotation * -0.8f, Flare.Size() / 2, scale * 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation * 0.8f, Flare.Size() / 2, scale * 0.35f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation * -0.8f, Flare.Size() / 2, scale * 0.35f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Ball.Size() / 2, scale * 0.45f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare3, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Flare3.Size() / 2, scale * 0.6f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare3, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * -1, Flare3.Size() / 2, scale * 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare2, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation + 1, Flare2.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare2, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * -1 + 1, Flare2.Size() / 2, scale * 0.7f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    public class SmallTumblerOrbVFX : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.aiStyle = 0;
            Projectile.penetrate = -1;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.timeLeft = 1000000000;
            Projectile.scale = 1f;
        }

        public override bool? CanDamage() => false;

        private int timer = 0;
        private float scale = 0f;
        private float alpha = 1f;
        private float glowScale = 0f;
        private float initialScaleFactor = 0.10f;

        public override void AI()
        {
            scale = MathHelper.Lerp(scale, initialScaleFactor * 4f, 0.2f);
            glowScale = MathHelper.Lerp(glowScale, 1f, 0.04f);

            if (timer < 38)
            {
                CreateContinualDust();
            }

            Projectile.rotation += 0.12f;
            timer++;
        }

        private void CreateContinualDust()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 start = Main.rand.NextVector2CircularEdge(10, 10);
                Dust da = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MuraLineDust>(),
                    start * Main.rand.NextFloat(0.65f, 1.35f), newColor: Color.DodgerBlue, Scale: 0.3f + Main.rand.NextFloat(0, 0.2f));
                da.fadeIn = 10f;
                da.alpha = 13;
            }
        }

        private void CreateInitialVFX()
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 direction = new Vector2(1, 0).RotatedByRandom(6.28f);
                Vector2 ai1 = new Vector2((float)Math.Cos(direction.ToRotation()), (float)Math.Sin(direction.ToRotation())) * 10f;
                float ai2 = Main.rand.Next(100);
                int lightning = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity * 10, ai1.RotatedByRandom(6.28f) * 2.5f, ModContent.ProjectileType<LightningHitFX>(), 0, 0, Main.myPlayer, ai1.ToRotation(), ai2);
                Main.projectile[lightning].scale = 0.3f;
            }

            int afg = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DistortProj>(), 0, 0);
            Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);
            Main.projectile[afg].timeLeft = 10;

            if (Main.projectile[afg].ModProjectile is DistortProj distort)
            {
                distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                distort.implode = false;
                distort.scale = 0.1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Flare/flare_4").Value;
            Texture2D Flare2 = Mod.Assets.Request<Texture2D>("Assets/Orbs/spiky_20fade").Value;
            Texture2D Flare3 = Mod.Assets.Request<Texture2D>("Assets/Flare/flare_1").Value;
            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/SofterBlueGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);
            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
            myEffect.Parameters["distortStrength"].SetValue(0.02f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue(timer * 0.015f);
            myEffect.Parameters["colorIntensity"].SetValue(alpha * 1.25f);

            // Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.Black * 0.5f * alpha, Projectile.rotation, Ball.Size() / 4, scale * 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue * 0.3f * alpha, Projectile.rotation, Ball.Size() / 4, glowScale * 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.DodgerBlue * alpha, Projectile.rotation * 0.8f, Flare.Size() / 2, scale * 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.SkyBlue * alpha, Projectile.rotation * -0.8f, Flare.Size() / 2, scale * 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation * 0.8f, Flare.Size() / 2, scale * 0.35f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation * -0.8f, Flare.Size() / 2, scale * 0.35f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);

            // Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Ball.Size() / 4, scale * 0.45f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare3, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Flare3.Size() / 2, scale * 0.6f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare3, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * -1, Flare3.Size() / 2, scale * 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare2, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation + 1, Flare2.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare2, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * -1 + 1, Flare2.Size() / 2, scale * 0.7f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    public class LightningLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public Vector2 endPoint;
        public float LaserRotation = 0;
        private Vector2 initialPosition;
        private NPC crystalTumbler;

        private bool isFadingOut = false;
        private float fadeOutRate = 0.05f;


        Vector2 storedCenter = Vector2.Zero;
        int timer = 0;

        Vector2 storedMousePos = Vector2.Zero;
        public float baseDamage = 0f;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 10;
            Projectile.scale = 1f;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

        }

        private NPC FindCrystalTumbler()
        {
            foreach (var npc in Main.npc)
            {
                if (npc.active && npc.type == ModContent.NPCType<CrystalTumbler>())
                {
                    return npc;
                }
            }
            return null;
        }

        private float laserLength;

        public override void AI()
        {
            if (initialPosition == Vector2.Zero)
            {
                initialPosition = Projectile.position;
                crystalTumbler = FindCrystalTumbler();
            }
            if (crystalTumbler != null)
            {
                var tumblerAI = (CrystalTumbler)crystalTumbler.ModNPC;
                if (!tumblerAI.zapBoss)
                {
                    isFadingOut = true;
                }
            }
            if (isFadingOut)
            {
                Projectile.alpha += (int)(fadeOutRate * 255);
                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
            {
                if (crystalTumbler != null && crystalTumbler.active)
                {
                    endPoint = crystalTumbler.Center;
                    LaserRotation = (endPoint - initialPosition).ToRotation();
                    laserLength = (endPoint - initialPosition).Length();
                }
                if (timer % 240 == 0 && timer != 0)
                {
                    for (int i = 90; i < laserLength; i += 6)
                    {
                        if (Main.rand.NextBool(4))
                        {
                            Color dustCol = Main.rand.NextBool() ? Color.Aquamarine : Color.Aqua;
                            Vector2 dustPosition = initialPosition + Vector2.UnitX.RotatedBy(LaserRotation) * i;
                            Dust.NewDustPerfect(dustPosition, ModContent.DustType<GlowStrong>(),
                                Main.rand.NextVector2CircularEdge(2f, 2f) + LaserRotation.ToRotationVector2() * 3f, newColor: dustCol, Scale: Main.rand.NextFloat(0.10f, 0.15f));
                        }
                    }
                }
                Projectile.position = initialPosition;
                starAlpha = MathHelper.Clamp(MathHelper.Lerp(starAlpha, 1.25f, 0.02f), 0f, 1f);
            }
            timer++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer > 0)
            {
                Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/CheapScroll", AssetRequestMode.ImmediateLoad).Value;

                #region Shader Params
                myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value);
                myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/RealLightning").Value);

                Color c1 = Color.Aqua * ((255 - Projectile.alpha) / 255f);
                Color c2 = Color.AliceBlue * ((255 - Projectile.alpha) / 255f);

                myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
                myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
                myEffect.Parameters["Color1Mult"].SetValue(1f);
                myEffect.Parameters["Color2Mult"].SetValue(1f);
                myEffect.Parameters["totalMult"].SetValue(1f);

                myEffect.Parameters["tex1reps"].SetValue(0.25f);
                myEffect.Parameters["tex2reps"].SetValue(0.25f);
                myEffect.Parameters["satPower"].SetValue(1f);
                myEffect.Parameters["time1Mult"].SetValue(1f);
                myEffect.Parameters["time2Mult"].SetValue(1f);
                myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.018f);
                #endregion

                Texture2D LaserTexture = Mod.Assets.Request<Texture2D>("Assets/GlowTrailMoreRes").Value;

                Vector2 origin2 = new(0, LaserTexture.Height / 2);

                float height = (100f);

                int width = (int)laserLength;

                var pos = Projectile.Center - Main.screenPosition;
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.7f));

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

                myEffect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(LaserTexture, target, null, Color.DeepPink * ((255 - Projectile.alpha) / 255f), LaserRotation, origin2, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                return false;
            }

            return false;
        }

        float starAlpha = 0f;
        public override void PostDraw(Color lightColor)
        {
            Vector2 laserEndPos = initialPosition + (Vector2.UnitX.RotatedBy(LaserRotation) * laserLength) - Main.screenPosition;
            Texture2D spotTex = Mod.Assets.Request<Texture2D>("Assets/Flare/CrispStarPMA").Value;
            Texture2D glowTex = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;
            Color adjustedColor = Color.Black * 0.5f * starAlpha * ((255 - Projectile.alpha) / 255f);

            Main.spriteBatch.Draw(glowTex, laserEndPos, glowTex.Frame(1, 1, 0, 0), adjustedColor, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), glowTex.Size() / 2, 0.1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, laserEndPos, spotTex.Frame(1, 1, 0, 0), adjustedColor, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 1.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, laserEndPos, spotTex.Frame(1, 1, 0, 0), adjustedColor, Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(glowTex, laserEndPos, glowTex.Frame(1, 1, 0, 0), Color.Blue * 0.3f * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), glowTex.Size() / 2, 0.2f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(spotTex, laserEndPos, spotTex.Frame(1, 1, 0, 0), Color.Blue * 2f * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 1.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, laserEndPos, spotTex.Frame(1, 1, 0, 0), Color.Aqua * 1.5f * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(spotTex, laserEndPos, spotTex.Frame(1, 1, 0, 0), Color.Blue * 2f * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, laserEndPos, spotTex.Frame(1, 1, 0, 0), Color.White * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.4f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}