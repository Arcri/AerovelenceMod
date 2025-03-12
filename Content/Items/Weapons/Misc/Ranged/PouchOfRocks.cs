using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AerovelenceMod.Common.Systems;
using System.Collections.Generic;
using System;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
{
    public class PouchOfRocks : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("PouchOfMagnets", "Throws 2 magnetic stones which can collide and explode")
            .AddName(Language.Default, "Pouch of Magnets").AddTooltip(Language.Default, "Throws 2 magnetic stones which can collide and explode")
            .AddSkillStrike(Language.Default, "The explosions Skill Strike")

            .AddName(Language.Spanish, "Bolsa de Imanes").AddTooltip(Language.Spanish, "Lanza 2 piedras magnéticas que pueden chocar y explotar").AddSkillStrike(Language.Spanish, "Las explosiones realizan Golpes de Habilidad")
            .AddName(Language.French, "Pochette d'Aimants").AddTooltip(Language.French, "Lance 2 pierres magnétiques qui peuvent entrer en collision et exploser").AddSkillStrike(Language.French, "Les explosions déclenchent des Coups de Compétence")
            .AddName(Language.German, "Beutel mit Magneten").AddTooltip(Language.German, "Wirft 2 magnetische Steine, die kollidieren und explodieren können").AddSkillStrike(Language.German, "Die Explosionen führen Fähigkeitsschläge aus")
            .AddName(Language.Italian, "Sacca di Magneti").AddTooltip(Language.Italian, "Lancia 2 pietre magnetiche che possono collidere ed esplodere").AddSkillStrike(Language.Italian, "Le Esplosioni eseguono Colpi dell'Abilità")
            //.AddName(Language.Polish, "Worek z Magnesami").AddTooltip(Language.Polish, "Rzuca 2 magnetyczne kamienie, które mogą się zderzyć i eksplodować").AddSkillStrike(Language.Polish, "Eksplozje wykonują Ciosy Umiejętności")
            //.AddName(Language.PortugueseBrazil, "Bolsa de Ímãs").AddTooltip(Language.PortugueseBrazil, "Lança 2 pedras magnéticas que podem colidir e explodir").AddSkillStrike(Language.PortugueseBrazil, "As explosões realizam Golpes de Habilidade")
            .AddName(Language.Russian, "Мешочек с Магнитами").AddTooltip(Language.Russian, "Бросает 2 магнитных камня, которые могут столкнуться и взорваться").AddSkillStrike(Language.Russian, "Взрывы активируют Навык Удара");
            //.AddName(Language.ChineseTraditional, "磁石袋").AddTooltip(Language.ChineseTraditional, "投擲 2 顆磁石，可相撞並爆炸").AddSkillStrike(Language.ChineseTraditional, "爆炸觸發技能打擊")
            //.AddName(Language.ChineseSimplified, "磁石袋").AddTooltip(Language.ChineseSimplified, "投掷 2 颗磁石，可相撞并爆炸").AddSkillStrike(Language.ChineseSimplified, "爆炸触发技能打击");
        }

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarities.EarlyPHM;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<MagneticRock>();
            Item.shootSpeed = 7f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float spread = MathHelper.ToRadians(8);
            Vector2 vel1 = velocity.RotatedBy(-spread);
            Vector2 vel2 = velocity.RotatedBy(spread);
            int pairID = Main.rand.Next(1, int.MaxValue);
            Projectile.NewProjectile(source, position, vel1, type, damage, knockback, player.whoAmI, 0f, pairID);
            Projectile.NewProjectile(source, position, vel2, type, damage, knockback, player.whoAmI, 0f, pairID);
            return false;
        }
    }

    public class MagneticRock : ModProjectile
    {
        private const float CollisionDistance = 10f;
        private const float MagneticStrength = 0.5f;

        private bool lightningSpawned = false;

        private bool frameAssigned = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 26;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 15;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            if (!frameAssigned)
            {
                Projectile.frame = (Projectile.whoAmI % 2 == 0) ? 0 : 1;
                frameAssigned = true;
            }


            Projectile.rotation += 0.5f;
            Projectile.ai[0] += 1f;

            if (Projectile.ai[0] >= 15f)
            {
                Projectile.velocity.Y += 0.1f;
            }

            if (Projectile.ai[0] == 1f)
                Projectile.velocity *= Main.rand.NextFloat(0.9f, 1.1f);

            if (Projectile.ai[0] >= 40f)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile other = Main.projectile[i];
                    if (other.active && other.type == Projectile.type && other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner)
                    {
                        if (other.ai[1] != Projectile.ai[1])
                            continue;
                        if (other.ai[0] < 40f)
                            continue;

                        float distance = Vector2.Distance(Projectile.Center, other.Center);
                        Vector2 toOther = other.Center - Projectile.Center;
                        if (toOther != Vector2.Zero)
                        {
                            toOther.Normalize();
                            Projectile.velocity += toOther * MagneticStrength;
                            if (!lightningSpawned)
                            {
                                Vector2 zapOffset = other.Center - Projectile.Center;
                                int zapID = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, zapOffset, ModContent.ProjectileType<MagneticZap>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                                if (zapID >= 0 && zapID < Main.maxProjectiles)
                                {
                                    Main.projectile[zapID].localAI[0] = other.Center.X;
                                    Main.projectile[zapID].localAI[1] = other.Center.Y;
                                }
                                Projectile.tileCollide = false;
                                lightningSpawned = true;
                            }
                        }

                        if (distance < CollisionDistance)
                        {
                            TriggerCollisionEffect();
                            break;
                        }
                    }
                }
            }
        }



        private void TriggerCollisionEffect()
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MagneticRockExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.Kill();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (other.active && other.type == Projectile.type && other.owner == Projectile.owner && other.whoAmI != Projectile.whoAmI && other.ai[1] == Projectile.ai[1])
                    other.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.Center);
            for (float m = 0f; m < 5f; m += 0.5f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustID.Granite, new Vector2((float)Math.Sin(m) * 1.3f, (float)Math.Cos(m)) * 2.4f, 0, Color.Gray);
                dust.velocity *= Main.rand.NextFloat(0.4f, 1.3f);
                dust.noGravity = true;
                dust.scale = 1f;
            }
        }
    }

    public class MagneticZap : ModProjectile
    {
        private const int MAX_SEGMENTS = 12;
        private const float BRANCH_CHANCE = 1f;
        private const int MAX_BRANCHES = 2;
        private Vector2[] segmentPositions;
        private Vector2 targetPosition;
        private float[] segmentOffsets;
        private List<Branch> branches;
        private float alpha = 1f;
        private bool initialized;
        private float distanceToTarget;

        public override string Texture => "Terraria/Images/Projectile_0";


        private class Branch
        {
            public Vector2[] Positions { get; set; }
            public float[] Offsets { get; set; }
            public float Alpha { get; set; }
            public int LifeTime { get; set; }
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.light = 0.8f;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }

            UpdateSegments();
            UpdateBranches();

            PixellationSystem.QueuePixelationAction(() =>
            {
                for (int i = 0; i < 0.2; i++)
                {
                    Vector2 randomSegment = segmentPositions[Main.rand.Next(0, MAX_SEGMENTS)];
                    Vector2 dir = (segmentPositions[MAX_SEGMENTS - 1] - segmentPositions[0]).SafeNormalize(Vector2.Zero);

                    Color dustColor = Color.Lerp(
                        new Color(0, 236, 255),
                        new Color(0, 255, 191),
                        Main.rand.NextFloat()
                    );

                    Dust a = Dust.NewDustPerfect((randomSegment + dir * 2f) / 2,
                        ModContent.DustType<GlowStrong>(),
                        dir.RotatedByRandom(0.5f) * Main.rand.NextFloat(1f, 3f),
                        0, newColor: dustColor, Main.rand.NextFloat(0.5f, 2f));
                    a.alpha = 2;
                }

                foreach (Branch branch in branches)
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

            if (Projectile.timeLeft < 10)
            {
                alpha *= 0.7f;
            }
        }

        private void Initialize()
        {
            FindTargetPosition();

            segmentPositions = new Vector2[MAX_SEGMENTS];
            segmentOffsets = new float[MAX_SEGMENTS];
            branches = [];

            Vector2 direction = targetPosition - Projectile.Center;
            distanceToTarget = direction.Length();
            float segmentLength = distanceToTarget / (MAX_SEGMENTS - 1);
            direction.Normalize();

            for (int i = 0; i < MAX_SEGMENTS; i++)
            {
                segmentPositions[i] = Projectile.Center + direction * (segmentLength * i);
                segmentOffsets[i] = 0f;
            }
            SoundEngine.PlaySound(SoundID.Item79 with { Volume = 0.5f, Pitch = Main.rand.NextFloat(0.3f, 1) });
        }

        private void UpdateSegments()
        {
            float time = Main.GameUpdateCount;
            float globalIntensity = (float)(Math.Sign(Math.Sin(time * 0.1f)) * 0.2f + Math.Sign(Math.Cos(time * 0.15f)) * 0.1f + 0.3f);
            for (int i = 1; i < MAX_SEGMENTS - 1; i++)
            {
                float centerEmphasis = (float)Math.Exp(-(Math.Pow(i - MAX_SEGMENTS / 2f, 2) / (2 * Math.Pow(MAX_SEGMENTS / 4f, 2)))) * 0.7f;
                float noise = (float)(Math.Sign(Math.Sin(time * 0.8f + i * 0.5f)) * 1.2f + Math.Sign(Math.Cos(time * 0.5f + i * 0.7f)) * 1.0f + (Math.Sin(time * 1.2f + i * 0.2f) > 0 ? 1 : -1) * globalIntensity * 1.8f) * centerEmphasis;
                if (Main.rand.NextBool(30) && i > MAX_SEGMENTS / 4 && i < MAX_SEGMENTS * 3 / 4)
                {
                    noise += Main.rand.NextFloat(-1f, 1f) * centerEmphasis;
                    if (Main.rand.NextBool(2))
                        noise *= 1.5f;
                }
                float finalAmplitude = Math.Min(5f, distanceToTarget * 0.06f);
                segmentOffsets[i] = noise * finalAmplitude;
                Vector2 normal = (segmentPositions[i + 1] - segmentPositions[i - 1]).RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
                Vector2 tangent = (segmentPositions[i + 1] - segmentPositions[i - 1]).SafeNormalize(Vector2.Zero);
                float tangentOffset = Math.Sign(Math.Sin(time * 0.6f + i * 0.8f)) * 0.7f * centerEmphasis;
                float suddenMultiplier = Main.rand.NextBool(20) ? 1.5f : 1f;
                segmentPositions[i] += (normal * segmentOffsets[i] + tangent * tangentOffset) * suddenMultiplier;
            }
            if (Main.rand.NextBool(6))
            {
                int segment = Main.rand.Next(MAX_SEGMENTS / 4, (MAX_SEGMENTS * 3) / 4);
                float displacementAmount = Main.rand.NextFloat(-4f, 4f);
                Vector2 normal = (segmentPositions[segment + 1] - segmentPositions[segment - 1]).RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
                segmentPositions[segment] += normal * displacementAmount;
                if (Main.rand.NextBool(2))
                {
                    int adjacentSegment = segment + (Main.rand.NextBool() ? 1 : -1);
                    if (adjacentSegment > 0 && adjacentSegment < MAX_SEGMENTS - 1)
                        segmentPositions[adjacentSegment] += normal * displacementAmount * 0.7f;
                }
            }
            if (Main.rand.NextFloat() < BRANCH_CHANCE && branches.Count < MAX_BRANCHES)
                CreateBranch();
        }

        private void CreateBranch()
        {
            int startSegment = Main.rand.Next(1, MAX_SEGMENTS - 2);
            int branchSegments = Main.rand.Next(3, 6);
            Branch branch = new()
            {
                Positions = new Vector2[branchSegments],
                Offsets = new float[branchSegments],
                Alpha = 0.7f,
                LifeTime = Main.rand.Next(10, 20)
            };
            Vector2 branchDirection = (segmentPositions[startSegment + 1] - segmentPositions[startSegment]).RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f));
            branchDirection.Normalize();
            for (int i = 0; i < branchSegments; i++)
            {
                branch.Positions[i] = segmentPositions[startSegment] + branchDirection * (i * 8);
                branch.Offsets[i] = 0f;
            }
            branches.Add(branch);
        }

        private void UpdateBranches()
        {
            for (int i = branches.Count - 1; i >= 0; i--)
            {
                Branch branch = branches[i];
                branch.LifeTime--;
                if (branch.LifeTime <= 0)
                {
                    branches.RemoveAt(i);
                    continue;
                }
                branch.Alpha *= 0.95f;
                float time = Main.GameUpdateCount;
                for (int j = 1; j < branch.Positions.Length - 1; j++)
                {
                    float noise = (float)(Math.Sin(time * 0.7f + j * 0.3f) * 1.5f + Math.Cos(time * 0.4f + j * 0.6f) * 1.0f);
                    branch.Offsets[j] = noise;
                    Vector2 normal = (branch.Positions[j + 1] - branch.Positions[j - 1]).RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
                    branch.Positions[j] += normal * (branch.Offsets[j] - branch.Offsets[j]);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (segmentPositions == null) return false;
            PixellationSystem.QueuePixelationAction(() =>
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D lineTexture = TextureAssets.MagicPixel.Value;
                Rectangle sourceRect = new(0, 0, 1, 1);
                float spawnProgress = 1f - (Projectile.timeLeft / 30f);
                float flashIntensity = (float)Math.Pow(1f - spawnProgress, 2);
                float energyPulse = (float)Math.Sin(Main.GameUpdateCount * 0.2f) * 0.3f + 0.7f;
                for (int i = 0; i < MAX_SEGMENTS - 1; i++)
                {
                    Vector2 start = (segmentPositions[i] - Main.screenPosition) / 2;
                    Vector2 end = (segmentPositions[i + 1] - Main.screenPosition) / 2;
                    Vector2 direction = end - start;
                    float distance = direction.Length();
                    float rotation = direction.ToRotation();
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

                    //core beam
                    spriteBatch.Draw(
                        lineTexture,
                        start,
                        sourceRect,
                        Color.Yellow * alpha * energyPulse,
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 1f),
                        SpriteEffects.None,
                        0
                    );

                    //middle glow
                    spriteBatch.Draw(
                        lineTexture,
                        start,
                        sourceRect,
                        new Color(150, 220, 255) * (alpha * 0.5f * energyPulse),
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 2f),
                        SpriteEffects.None,
                        0
                    );

                    //outer glow
                    spriteBatch.Draw(
                        lineTexture,
                        start,
                        sourceRect,
                        new Color(100, 180, 255) * (alpha * 0.3f * energyPulse),
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 3f),
                        SpriteEffects.None,
                        0
                    );

                    //distortion
                    float distortionOffset = (float)Math.Sin(Main.GameUpdateCount * 0.8f + i * 0.5f);
                    spriteBatch.Draw(
                        lineTexture,
                        start + new Vector2(0, distortionOffset),
                        sourceRect,
                        new Color(200, 230, 255) * (alpha * 0.2f),
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 1.5f),
                        SpriteEffects.None,
                        0
                    );
                }

                Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/Clear/GlowTrailSlice").Value;
                for (int i = 0; i < MAX_SEGMENTS - 1; i++)
                {
                    Vector2 start = (segmentPositions[i] - Main.screenPosition) / 2;
                    Vector2 end = (segmentPositions[i + 1] - Main.screenPosition) / 2;
                    Vector2 direction = end - start;
                    float distance = direction.Length();
                    float rotation = direction.ToRotation();
                    float glowWidth = 0.4f * (1f + (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.1f);
                    Color glowColor = new Color(150, 220, 255) * (alpha * 0.2f);
                    for (int g = 0; g < 2; g++)
                    {
                        float offsetAngle = g * MathHelper.PiOver2;
                        Vector2 offset = new((float)Math.Cos(offsetAngle + Main.GameUpdateCount * 0.05f) * 0.5f, (float)Math.Sin(offsetAngle + Main.GameUpdateCount * 0.05f) * 0.5f);
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
                //tiny impact points
                void DrawImpactPoint(Vector2 position, float size)
                {
                    position = (position - Main.screenPosition) / 2;
                    float time = Main.GameUpdateCount * 0.1f;
                    float pulseSize = 1f + (float)Math.Sin(time) * 0.2f;

                    Texture2D starTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;

                    //rotating pixels
                    for (int i = 0; i < 4; i++)
                    {
                        float angle = i * MathHelper.PiOver2 + time;
                        Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * size * pulseSize;

                        spriteBatch.Draw(
                            lineTexture,
                            position + offset,
                            sourceRect,
                            new Color(150, 220, 255) * (alpha * 0.5f),
                            angle,
                            new Vector2(0.5f),
                            new Vector2(size * 0.25f, 1f),
                            SpriteEffects.None,
                            0
                        );
                    }

                    //first star
                    Color color1 = Color.Lerp(
                        new Color(0, 236, 255),
                        Color.White,
                        0.5f + (float)Math.Sin(time) * 0.2f
                    );
                    spriteBatch.Draw(
                        starTexture,
                        position,
                        null,
                        color1 * alpha,
                        time * 0.5f,
                        starTexture.Size() / 2f,
                        0.2f * pulseSize,
                        SpriteEffects.None,
                        0
                    );

                    //second star
                    Color color2 = Color.Lerp(
                        new Color(0, 255, 191),
                        Color.White,
                        0.3f + (float)Math.Sin(time * 1.5f) * 0.2f
                    );
                    spriteBatch.Draw(
                        starTexture,
                        position,
                        null,
                        color2 * alpha,
                        -time * 0.7f,
                        starTexture.Size() / 2f,
                        0.125f * pulseSize,
                        SpriteEffects.None,
                        0
                    );
                }

                DrawImpactPoint(segmentPositions[0], 4f);
                DrawImpactPoint(segmentPositions[MAX_SEGMENTS - 1], 4f);

                //draw branches
                foreach (Branch branch in branches)
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
            return false;
        }

        private void FindTargetPosition()
        {
            targetPosition = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
            if (targetPosition == Vector2.Zero)
            {
                targetPosition = Projectile.Center + Projectile.velocity;
            }
        }
    }

    public class MagneticRockExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;

        public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = 7;
        public float alphaPercent = 0;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage() => timer < 4;

        public override void AI()
        {
            if (timer == 0)
                Projectile.rotation = Main.rand.NextFloat(6.28f);

            alphaPercent = Math.Clamp(MathHelper.Lerp(alphaPercent, -0.2f, 0.08f), 0, 1);

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                if (Projectile.frame == 6)
                    Projectile.active = false;

                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }

            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * alphaPercent * 0.4f);

            SkillStrikeUtil.setSkillStrike(Projectile, 1.5f);
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Assets/Anim/BlueFlareDarkGlowPMA").Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 scale = new Vector2(1f, 1f);
            Color glowColor = Color.Aquamarine;
            glowColor.A = 0;
            Color whiteColor = Color.White;
            whiteColor.A = 0;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.Black * 0.4f, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, whiteColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}