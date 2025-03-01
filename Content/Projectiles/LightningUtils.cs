using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Projectiles
{
    public static class LightningUtility
    {
        /// <summary>
        /// Data structure holding all key fields for the lightning system.
        /// </summary>
        public class LightningData
        {
            public int MaxSegments = 12;
            public float BranchChance = 1f;
            public int MaxBranches = 2;

            public Vector2[] SegmentPositions;
            public float[] SegmentOffsets;
            public List<Branch> Branches;
            public Vector2 TargetPosition;
            public float Alpha = 1f;
            public bool Initialized;
            public float DistanceToTarget;

            public LightningStyle Style = LightningStyle.Default;

            public float DisplacementIntensity = 1.0f;
            public float NoiseFrequency = 0.5f;

            public Projectile Projectile;
            public NPC Npc;

            public LightningData(Projectile projectile, LightningStyle style = LightningStyle.Default)
            {
                Projectile = projectile;
                Style = style;

                switch (Style)
                {
                    case LightningStyle.Jagged:
                        DisplacementIntensity = 0.5f;
                        NoiseFrequency = 10f;
                        break;
                    case LightningStyle.Smooth:
                        DisplacementIntensity = 0.5f;
                        NoiseFrequency = 0.3f;
                        break;
                    case LightningStyle.Chaotic:
                        DisplacementIntensity = 3.5f;
                        NoiseFrequency = 1.8f;
                        break;
                    default:
                        DisplacementIntensity = 1.0f;
                        NoiseFrequency = 0.5f;
                        break;
                }
            }

            public LightningData(NPC npc, LightningStyle style = LightningStyle.Default)
            {
                Npc = npc;
                Style = style;

                switch (Style)
                {
                    case LightningStyle.Jagged:
                        DisplacementIntensity = 0.5f;
                        NoiseFrequency = 10f;
                        break;
                    case LightningStyle.Smooth:
                        DisplacementIntensity = 0.5f;
                        NoiseFrequency = 0.3f;
                        break;
                    case LightningStyle.Chaotic:
                        DisplacementIntensity = 3.5f;
                        NoiseFrequency = 1.8f;
                        break;
                    default:
                        DisplacementIntensity = 1.0f;
                        NoiseFrequency = 0.5f;
                        break;
                }
            }
        }


        /// <summary>
        /// A single lightning branch.
        /// </summary>
        public class Branch
        {
            public Vector2[] Positions { get; set; }
            public float[] Offsets { get; set; }
            public float Alpha { get; set; }
            public int LifeTime { get; set; }
        }

        public enum LightningStyle
        {
            Default,
            Jagged,
            Smooth,
            Chaotic
        }

        /// <summary>
        /// Initializes the main lightning bolt based on the projectile's position and velocity.
        /// </summary>
        public static void InitializeProjectiles(LightningData data)
        {
            data.SegmentPositions = new Vector2[data.MaxSegments];
            data.SegmentOffsets = new float[data.MaxSegments];
            data.Branches = [];

            Vector2 direction = data.TargetPosition - data.Projectile.Center;
            data.DistanceToTarget = direction.Length();
            float segmentLength = data.DistanceToTarget / (data.MaxSegments - 1);
            direction.Normalize();

            for (int i = 0; i < data.MaxSegments; i++)
            {
                data.SegmentPositions[i] = data.Projectile.Center + direction * (segmentLength * i);
                data.SegmentOffsets[i] = 0f;
            }
            SoundEngine.PlaySound(SoundID.NPCHit53 with { Volume = 0.5f, Pitch = 0.3f });
        }

        /// <summary>
        /// Initializes the main lightning bolt based on the NPC's position and velocity.
        /// </summary>
        public static void InitializeNPCs(LightningData data)
        {
            data.SegmentPositions = new Vector2[data.MaxSegments];
            data.SegmentOffsets = new float[data.MaxSegments];
            data.Branches = [];

            Vector2 direction = data.TargetPosition - data.Npc.Center;
            data.DistanceToTarget = direction.Length();
            float segmentLength = data.DistanceToTarget / (data.MaxSegments - 1);
            direction.Normalize();

            for (int i = 0; i < data.MaxSegments; i++)
            {
                data.SegmentPositions[i] = data.Npc.Center + direction * (segmentLength * i);
                data.SegmentOffsets[i] = 0f;
            }
            SoundEngine.PlaySound(SoundID.NPCHit53 with { Volume = 0.5f, Pitch = 0.3f });
        }

        public static void InitializeBetweenPoints(LightningData data, Vector2 startPos, Vector2 endPos, LightningStyle style = LightningStyle.Default)
        {
            data.Style = style;
            data.Branches = [];

            data.SegmentPositions = new Vector2[data.MaxSegments];
            data.SegmentOffsets = new float[data.MaxSegments];

            Vector2 direction = endPos - startPos;
            data.DistanceToTarget = direction.Length();

            float segmentLength = data.DistanceToTarget / (data.MaxSegments - 1);
            direction.Normalize();

            for (int i = 0; i < data.MaxSegments; i++)
            {
                data.SegmentPositions[i] = startPos + direction * (segmentLength * i);
                data.SegmentOffsets[i] = 0f;
            }

            data.Initialized = true;
            SoundEngine.PlaySound(SoundID.NPCHit53 with { Volume = 0.5f, Pitch = 0.3f });
        }

        /// <summary>
        /// Updates the main segments of the bolt with noise and random displacement. Also calls branch creation logic.
        /// </summary>
        public static void UpdateSegments(LightningData data)
        {
            float time = Main.GameUpdateCount;
            float globalIntensity = (float)(Math.Sign(Math.Sin(time * 0.1f)) * 0.2f + Math.Sign(Math.Cos(time * 0.15f)) * 0.1f + 0.3f);

            for (int i = 1; i < data.MaxSegments - 1; i++)
            {
                float centerEmphasis = (float)Math.Exp(-(Math.Pow(i - data.MaxSegments / 2f, 2) / (2 * Math.Pow(data.MaxSegments / 4f, 2)))) * 0.7f;

                float noiseFactor = data.NoiseFrequency * (Main.rand.NextFloat() - 0.5f) * 2f;
                float roughnessMultiplier = 1.5f;
                float noise = (
                    Math.Sign(Math.Sin(time * 0.8f + i * noiseFactor)) * 1.2f
                    + Math.Sign(Math.Cos(time * 0.5f + i * 0.7f)) * 1.0f
                    + (Math.Sin(time * 1.2f + i * 0.2f) > 0 ? 1 : -1) * globalIntensity * 1.8f
                ) * centerEmphasis * roughnessMultiplier;


                float finalAmplitude = Math.Min(5f, data.DistanceToTarget * 0.06f);
                data.SegmentOffsets[i] = noise * finalAmplitude * data.DisplacementIntensity;

                Vector2 normal = (data.SegmentPositions[i + 1] - data.SegmentPositions[i - 1])
                    .RotatedBy(MathHelper.PiOver2)
                    .SafeNormalize(Vector2.Zero);

                float tangentOffset = Math.Sign(Math.Sin(time * 0.6f + i * 0.8f)) * 0.7f * centerEmphasis;
                float suddenMultiplier = Main.rand.NextBool(20) ? 1.5f : 1f;

                data.SegmentPositions[i] += (normal * data.SegmentOffsets[i] + tangentOffset * Vector2.UnitX) * suddenMultiplier;
            }

            if (Main.rand.NextBool(6))
            {
                int segment = Main.rand.Next(data.MaxSegments / 4, (data.MaxSegments * 3) / 4);
                float displacementAmount = Main.rand.NextFloat(-4f, 4f);
                Vector2 normal = (data.SegmentPositions[segment + 1] - data.SegmentPositions[segment - 1])
                    .RotatedBy(MathHelper.PiOver2)
                    .SafeNormalize(Vector2.Zero);

                data.SegmentPositions[segment] += normal * displacementAmount;
            }

            if (Main.rand.NextFloat() < data.BranchChance && data.Branches.Count < data.MaxBranches)
                CreateBranch(data);
        }


        /// <summary>
        /// Creates a new branch from somewhere in the main bolt.
        /// </summary>
        private static void CreateBranch(LightningData data)
        {
            int startSegment = Main.rand.Next(1, data.MaxSegments - 2);
            int branchSegments = Main.rand.Next(3, 6);

            Branch branch = new()
            {
                Positions = new Vector2[branchSegments],
                Offsets = new float[branchSegments],
                Alpha = 0.7f,
                LifeTime = Main.rand.Next(10, 20)
            };

            Vector2 branchDirection = (
                data.SegmentPositions[startSegment + 1] - data.SegmentPositions[startSegment]
            ).RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f));
            branchDirection.Normalize();

            for (int i = 0; i < branchSegments; i++)
            {
                branch.Positions[i] = data.SegmentPositions[startSegment] + branchDirection * (i * 8);
                branch.Offsets[i] = 0f;
            }

            data.Branches.Add(branch);
        }

        /// <summary>
        /// Updates all branches by applying noise and decrementing lifetime.
        /// </summary>
        public static void UpdateBranches(LightningData data)
        {
            for (int i = data.Branches.Count - 1; i >= 0; i--)
            {
                Branch branch = data.Branches[i];
                branch.LifeTime--;
                if (branch.LifeTime <= 0)
                {
                    data.Branches.RemoveAt(i);
                    continue;
                }
                branch.Alpha *= 0.95f;
                float time = Main.GameUpdateCount;
                for (int j = 1; j < branch.Positions.Length - 1; j++)
                {
                    float noise = (float)(
                        Math.Sin(time * 0.7f + j * 0.3f) * 1.5f
                        + Math.Cos(time * 0.4f + j * 0.6f) * 1.0f
                    );
                    branch.Offsets[j] = noise;
                    Vector2 normal = (branch.Positions[j + 1] - branch.Positions[j - 1])
                        .RotatedBy(MathHelper.PiOver2)
                        .SafeNormalize(Vector2.Zero);
                    branch.Positions[j] += normal * (branch.Offsets[j] - branch.Offsets[j]);
                }
            }
        }

        /// <summary>
        /// Spawns dust around main segments and branches
        /// </summary>
        public static void SpawnDust(LightningData data)
        {
            PixellationSystem.QueuePixelationAction(() =>
            {
                //Main segments dust
                for (int i = 0; i < 0.2; i++)
                {
                    Vector2 randomSegment = data.SegmentPositions[Main.rand.Next(0, data.MaxSegments)];
                    Vector2 dir = (data.SegmentPositions[data.MaxSegments - 1] - data.SegmentPositions[0])
                        .SafeNormalize(Vector2.Zero);

                    Color dustColor = Color.Lerp(
                        new Color(0, 236, 255),
                        new Color(0, 255, 191),
                        Main.rand.NextFloat()
                    );

                    Dust a = Dust.NewDustPerfect(
                        (randomSegment + dir * 2f) / 2,
                        ModContent.DustType<GlowStrong>(),
                        dir.RotatedByRandom(0.5f) * Main.rand.NextFloat(1f, 3f),
                        0,
                        newColor: dustColor,
                        Scale: Main.rand.NextFloat(0.5f, 2f)
                    );
                    a.alpha = 2;
                }

                //Branch dust
                foreach (Branch branch in data.Branches)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int i = 0; i < branch.Positions.Length - 1; i++)
                        {
                            Vector2 dustPos = Vector2.Lerp(
                                branch.Positions[i],
                                branch.Positions[i + 1],
                                Main.rand.NextFloat()
                            );

                            Color dustColor = Color.Lerp(
                                Color.Aqua,
                                Color.LightBlue,
                                Main.rand.NextFloat()
                            );

                            Dust dust = Dust.NewDustPerfect(
                                dustPos,
                                DustID.Electric,
                                Vector2.Zero,
                                0,
                                dustColor * branch.Alpha,
                                Main.rand.NextFloat(0.6f, 0.9f) * branch.Alpha
                            );
                            dust.noGravity = true;
                            dust.fadeIn = 0f;
                        }
                    }
                }
            }, PixellationSystem.RenderType.Additive);
        }

        /// <summary>
        /// Draws the main segments and branches of the bolt, including glow, distortion, and impact effects.
        /// Called in Projectile's PreDraw.
        /// </summary>
        public static void DrawLightning(LightningData data, SpriteBatch spriteBatch)
        {
            if (data.SegmentPositions == null)
                return;

            PixellationSystem.QueuePixelationAction(() =>
            {
                Texture2D lineTexture = TextureAssets.MagicPixel.Value;
                Rectangle sourceRect = new(0, 0, 1, 1);

                float spawnProgress = 1f - (data.Projectile.timeLeft / 30f);
                float flashIntensity = (float)Math.Pow(1f - spawnProgress, 2);
                float energyPulse = (float)Math.Sin(Main.GameUpdateCount * 0.2f) * 0.3f + 0.7f;

                //Lightning segments
                for (int i = 0; i < data.MaxSegments - 1; i++)
                {
                    Vector2 start = (data.SegmentPositions[i] - Main.screenPosition) / 2;
                    Vector2 end = (data.SegmentPositions[i + 1] - Main.screenPosition) / 2;
                    Vector2 direction = end - start;
                    float distance = direction.Length();
                    float rotation = direction.ToRotation();

                    //Flash pass
                    if (flashIntensity > 0)
                    {
                        spriteBatch.Draw(
                            lineTexture,
                            start,
                            sourceRect,
                            Color.Aqua * flashIntensity,
                            rotation,
                            new Vector2(0, 0.5f),
                            new Vector2(distance, 3f),
                            SpriteEffects.None,
                            0
                        );
                    }

                    //Core beam
                    spriteBatch.Draw(
                        lineTexture,
                        start,
                        sourceRect,
                        Color.Yellow * data.Alpha * energyPulse,
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 1f),
                        SpriteEffects.None,
                        0
                    );

                    //Middle glow
                    spriteBatch.Draw(
                        lineTexture,
                        start,
                        sourceRect,
                        new Color(150, 220, 255) * (data.Alpha * 0.5f * energyPulse),
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 2f),
                        SpriteEffects.None,
                        0
                    );

                    //Outer glow
                    spriteBatch.Draw(
                        lineTexture,
                        start,
                        sourceRect,
                        new Color(100, 180, 255) * (data.Alpha * 0.3f * energyPulse),
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 3f),
                        SpriteEffects.None,
                        0
                    );

                    //Distortion
                    float distortionOffset = (float)Math.Sin(Main.GameUpdateCount * 0.8f + i * 0.5f);
                    spriteBatch.Draw(
                        lineTexture,
                        start + new Vector2(0, distortionOffset),
                        sourceRect,
                        new Color(200, 230, 255) * (data.Alpha * 0.2f),
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 1.5f),
                        SpriteEffects.None,
                        0
                    );
                }

                //Draw glow trail slices
                Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/GlowTrailSlice").Value;
                for (int i = 0; i < data.MaxSegments - 1; i++)
                {
                    Vector2 start = (data.SegmentPositions[i] - Main.screenPosition) / 2;
                    Vector2 end = (data.SegmentPositions[i + 1] - Main.screenPosition) / 2;
                    Vector2 direction = end - start;
                    float distance = direction.Length();
                    float rotation = direction.ToRotation();

                    float glowWidth = 0.4f * (1f + (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.1f);
                    Color glowColor = new Color(150, 220, 255) * (data.Alpha * 0.2f);

                    for (int g = 0; g < 2; g++)
                    {
                        float offsetAngle = g * MathHelper.PiOver2;
                        Vector2 offset = new(
                            (float)Math.Cos(offsetAngle + Main.GameUpdateCount * 0.05f) * 0.5f,
                            (float)Math.Sin(offsetAngle + Main.GameUpdateCount * 0.05f) * 0.5f
                        );

                        spriteBatch.Draw(
                            glowTexture,
                            start + offset,
                            null,
                            glowColor * (1f - g * 0.3f),
                            rotation,
                            new Vector2(0, glowTexture.Height / 2f),
                            new Vector2(distance / (glowTexture.Width / 1f), glowWidth * (1f - g * 0.2f)),
                            SpriteEffects.None,
                            0
                        );
                    }
                }

                //Draw the tiny impact points at the ends
                void DrawImpactPoint(Vector2 position, float size)
                {
                    position = (position - Main.screenPosition) / 2;
                    float time = Main.GameUpdateCount * 0.1f;
                    float pulseSize = 1f + (float)Math.Sin(time) * 0.2f;

                    Texture2D starTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/CrispStarPMA").Value;

                    // rotating pixels
                    for (int i = 0; i < 4; i++)
                    {
                        float angle = i * MathHelper.PiOver2 + time;
                        Vector2 offset = new Vector2(
                            (float)Math.Cos(angle),
                            (float)Math.Sin(angle)
                        ) * size * pulseSize;

                        spriteBatch.Draw(
                            lineTexture,
                            position + offset,
                            sourceRect,
                            new Color(150, 220, 255) * (data.Alpha * 0.5f),
                            angle,
                            new Vector2(0.5f),
                            new Vector2(size * 0.25f, 1f),
                            SpriteEffects.None,
                            0
                        );
                    }

                    //First star
                    Color color1 = Color.Lerp(
                        new Color(0, 236, 255),
                        Color.White,
                        0.5f + (float)Math.Sin(time) * 0.2f
                    );
                    spriteBatch.Draw(
                        starTexture,
                        position,
                        null,
                        color1 * data.Alpha,
                        time * 0.5f,
                        starTexture.Size() / 2f,
                        0.2f * pulseSize,
                        SpriteEffects.None,
                        0
                    );

                    //Second star
                    Color color2 = Color.Lerp(
                        new Color(0, 255, 191),
                        Color.White,
                        0.3f + (float)Math.Sin(time * 1.5f) * 0.2f
                    );
                    spriteBatch.Draw(
                        starTexture,
                        position,
                        null,
                        color2 * data.Alpha,
                        -time * 0.7f,
                        starTexture.Size() / 2f,
                        0.125f * pulseSize,
                        SpriteEffects.None,
                        0
                    );
                }

                DrawImpactPoint(data.SegmentPositions[0], 4f);
                DrawImpactPoint(data.SegmentPositions[data.MaxSegments - 1], 4f);

                //Draw branches
                foreach (Branch branch in data.Branches)
                {
                    float branchEnergy = (float)Math.Sin(Main.GameUpdateCount * 0.3f) * 0.2f + 0.8f;

                    for (int i = 0; i < branch.Positions.Length - 1; i++)
                    {
                        Vector2 start = (branch.Positions[i] - Main.screenPosition) / 2;
                        Vector2 end = (branch.Positions[i + 1] - Main.screenPosition) / 2;
                        Vector2 direction = end - start;
                        float distance = direction.Length();
                        float rotation = direction.ToRotation();

                        //core
                        spriteBatch.Draw(
                            lineTexture,
                            start,
                            sourceRect,
                            Color.White * branch.Alpha * branchEnergy,
                            rotation,
                            new Vector2(0, 0.5f),
                            new Vector2(distance, 0.5f),
                            SpriteEffects.None,
                            0
                        );

                        //glow
                        spriteBatch.Draw(
                            lineTexture,
                            start,
                            sourceRect,
                            new Color(150, 220, 255) * (branch.Alpha * 0.3f * branchEnergy),
                            rotation,
                            new Vector2(0, 0.5f),
                            new Vector2(distance, 1f),
                            SpriteEffects.None,
                            0
                        );
                    }
                }

            }, PixellationSystem.RenderType.Additive);
        }
    }
}