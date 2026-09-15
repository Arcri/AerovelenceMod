using System;
using AerovelenceMod.Common;
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Corruption
{
    public class MimikosMirror : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Mimiko's Mirror", "Reflects magical light to banish evil\nHold to scatter five rays of demonite light\nThe rays periodically align, flash three times, then scatter again\nConsumes 10 mana per second while channeling")
                .AddName(Language.Spanish, "Espejo de Mimiko")
                .AddTooltip(Language.Spanish, "Refleja luz mágica para desterrar el mal\nMantén pulsado para dispersar cinco rayos de luz de demonita\nLos rayos se alinean periódicamente, destellan tres veces y vuelven a dispersarse\nConsume 10 de maná por segundo mientras se canaliza")
                .AddSkillStrike(Language.Default, "Strike enemies with the focused flashes")
                .AddSkillStrike(Language.Spanish, "Golpea a los enemigos con los destellos enfocados");
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 34;
            Item.height = 50;
            Item.damage = 16;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 1.5f;
            Item.mana = 4;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = Item.channel = true;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<MimikosMirrorHeld>();
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 40);
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 aim = velocity.SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(source, player.MountedCenter, aim, type, damage, knockback, player.whoAmI, aim.ToRotation());
            return false;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.DemoniteBar, 10)
            .AddIngredient(ItemID.PurificationPowder, 20).AddTile(TileID.Anvils).Register();
    }

    internal static class MimikoCycle
    {
        internal const int Duration = 150;
        internal static int Phase(int age) => age % Duration;
        internal static bool Focused(int age) => Phase(age) >= 108 && Phase(age) < 132;
        internal static bool Flash(int age) => Focused(age) && (Phase(age) - 108) % 8 < 2;
        internal static float Alignment(int age)
        {
            int phase = Phase(age);
            if (phase < 72) return 0f;
            if (phase < 108) return MathHelper.SmoothStep(0f, 1f, (phase - 72f) / 36f);
            if (phase < 132) return 1f;
            return 1f - MathHelper.SmoothStep(0f, 1f, (phase - 132f) / 18f);
        }
    }

    public class MimikosMirrorHeld : ModProjectile
    {
        private const int Rays = 5;
        private const int Points = 33;
        private readonly Vector2[][] paths = new Vector2[Rays][];
        private readonly float[] rayPulses = new float[Rays];
        private Vector2 face;
        private bool blocked;
        private bool exitPlayed;
        private int evaluatedAge;
        private int Age => evaluatedAge;
        private bool Retiring => Projectile.ai[2] != 0f;
        private float Opacity => Math.Min(1f, Age / 10f) * (Retiring ? Projectile.timeLeft / 18f : 1f);
        private float Alignment => MimikoCycle.Alignment(Age);
        private float Flash => MimikoCycle.Focused(Age) ? MathF.Pow(1f - ((MimikoCycle.Phase(Age) - 108) % 8) / 8f, 3f) : 0f;
        private static readonly Color Violet = new(145, 70, 255);
        private static readonly Color Pearl = new(230, 205, 255);
        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 420;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.netImportant = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => !Retiring && !blocked && Age > 8 && (!MimikoCycle.Focused(Age) || MimikoCycle.Flash(Age)) ? null : false;

        private void Retire()
        {
            if (Retiring) return;
            Projectile.ai[2] = 1f;
            Projectile.friendly = false;
            Projectile.timeLeft = 18;
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            evaluatedAge = (int)Projectile.ai[1];
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }
            if (player.CCed || player.noItems || player.HeldItem.type != ModContent.ItemType<MimikosMirror>()) Retire();
            if (Projectile.owner == Main.myPlayer && !Retiring)
            {
                if (!player.channel || (Age > 0 && Age % 24 == 0 && !player.CheckMana(player.HeldItem, 4, true)))
                    Retire();
                else
                {
                    Vector2 desired = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Projectile.ai[0].ToRotationVector2());
                    float turn = MathHelper.WrapAngle(desired.ToRotation() - Projectile.ai[0]);
                    Projectile.ai[0] = MathHelper.WrapAngle(Projectile.ai[0] + turn * 0.2f);
                    if (Age % 6 == 0 && Math.Abs(turn) > 0.005f) Projectile.netUpdate = true;
                }
            }
            Vector2 aim = Projectile.ai[0].ToRotationVector2();
            Projectile.rotation = Projectile.ai[0] + (aim.X < 0f ? MathHelper.Pi : 0f);
            if (!Retiring)
            {
                Projectile.timeLeft = 18;
                player.ChangeDir(aim.X >= 0f ? 1 : -1);
                player.heldProj = Projectile.whoAmI;
                player.SetDummyItemTime(2);
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.ai[0] - MathHelper.PiOver2);
                player.manaRegenDelay = Math.Max(player.manaRegenDelay, 60);
            }
            Vector2 handPosition = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.ai[0] - MathHelper.PiOver2) + new Vector2(0f, player.gfxOffY);
            Vector2 handOffset = aim.RotatedBy(player.direction < 0 ? -MathHelper.PiOver2 : MathHelper.PiOver2) * 3f - aim * 1.5f;
            Projectile.Center = handPosition + handOffset;
            Vector2 mirrorFaceDirection = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            face = Projectile.Center + mirrorFaceDirection * 14f + aim * 4f;
            blocked = !Collision.CanHitLine(player.MountedCenter, 1, 1, face, 1, 1) || Collision.SolidCollision(face - Vector2.One * 2f, 4, 4);
            BuildRays();
            if (!Main.dedServ && !blocked)
            {
                Color beamLight = Color.Lerp(Violet, Pearl, Alignment);
                for (int ray = 0; ray < Rays; ray++)
                {
                    Vector2[] path = paths[ray];
                    if (path == null) continue;
                    for (int i = 0; i < path.Length; i += 5)
                        Lighting.AddLight(path[i], beamLight.ToVector3() * (0.55f + Flash * 0.35f) * Opacity);
                }
            }
            Projectile.localNPCHitCooldown = MimikoCycle.Focused(Age) ? 7 : 16;
            var skill = Projectile.GetGlobalProjectile<SkillStrikeGProj>();
            skill.SkillStrike = false;
            if (!Retiring && MimikoCycle.Flash(Age))
                SkillStrikeUtil.setSkillStrike(Projectile, 1.5f, 100, 0.12f, 0.3f);
            if (!Retiring && MimikoCycle.Focused(Age) && (MimikoCycle.Phase(Age) - 108) % 8 == 0)
            {
                Array.Clear(Projectile.localNPCImmunity, 0, Projectile.localNPCImmunity.Length);
                SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.32f, Pitch = 0.25f + (MimikoCycle.Phase(Age) - 108) * 0.018f, MaxInstances = 3 }, face);
                EmitBurst(face, 8, 2f);
                if (Projectile.owner == Main.myPlayer) Projectile.netUpdate = true;
            }
            if (Age == 0 || (!Retiring && MimikoCycle.Phase(Age) == 72))
                SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.22f, Pitch = Alignment * 0.2f + 0.3f, MaxInstances = 2 }, face);
            if (!Main.dedServ)
            {
                Lighting.AddLight(face, Violet.ToVector3() * (0.2f + Alignment * 0.4f) * Opacity);
                if (!blocked && Age % 3 == 0)
                {
                    int ray = Main.rand.Next(Rays);
                    Vector2[] path = paths[ray];
                    if (path.Length > 4)
                    {
                        int index = Main.rand.Next(3, path.Length - 1);
                        Spark(path[index], (path[index] - path[index - 1]).SafeNormalize(aim) * Main.rand.NextFloat(0.6f, 2f), 0.15f + Flash * 0.1f);
                    }
                }
                if (Retiring && !exitPlayed)
                {
                    exitPlayed = true;
                    for (int i = 0; i < Rays; i++)
                        for (int j = 4; j < paths[i].Length; j += 9)
                            Spark(paths[i][j], Main.rand.NextVector2Circular(1.3f, 1.3f), 0.16f);
                }
            }
            if (!Retiring) Projectile.ai[1]++;
        }

        private void BuildRays()
        {
            for (int ray = 0; ray < Rays; ray++)
            {
                float pulse = 0.5f + 0.5f * MathF.Sin(Age * 0.7f + ray * 2.1f);
                rayPulses[ray] = pulse;
                float spread = ((ray - 2f) * 0.19f + MathF.Sin(Age * 0.07f + ray * 1.8f) * 0.08f) * (1f - Alignment);
                spread += (ray - 2f) * 0.0015f * Alignment;
                Vector2 direction = (Projectile.ai[0] + spread).ToRotationVector2();
                float length = MathHelper.Lerp(150f + pulse * 65f + ray % 2 * 20f, 285f, Alignment) * Math.Min(1f, Age / 10f);
                var points = new System.Collections.Generic.List<Vector2>(Points) { face };
                if (!blocked)
                    for (int i = 1; i < Points; i++)
                    {
                        float progress = i / (float)(Points - 1);
                        Vector2 next = face + direction * length * progress;
                        next += direction.RotatedBy(MathHelper.PiOver2) * MathF.Sin(progress * MathHelper.Pi) * MathF.Sin(Age * 0.075f + ray) * 5f * (1f - Alignment);
                        if (!Collision.CanHitLine(points[^1], 1, 1, next, 1, 1) || Collision.SolidCollision(next - Vector2.One, 2, 2)) break;
                        points.Add(next);
                    }
                paths[ray] = points.ToArray();
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Retiring || blocked) return false;
            for (int ray = 0; ray < Rays; ray++)
            {
                Vector2[] path = paths[ray];
                if (path == null) continue;
                for (int i = Math.Max(2, path.Length / 12); i < path.Length; i++)
                {
                    float collision = 0f;
                    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), path[i - 1], path[i], MimikoCycle.Focused(Age) ? 8f : 4f, ref collision)) return true;
                }
            }
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (MimikoCycle.Flash(Age)) modifiers.SourceDamage *= 1.2f;
            modifiers.HitDirectionOverride = target.Center.X >= Projectile.Center.X ? 1 : -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 point = Vector2.Clamp(face + Projectile.ai[0].ToRotationVector2() * 100f, target.Hitbox.TopLeft(), target.Hitbox.BottomRight());
            EmitBurst(point, MimikoCycle.Focused(Age) ? 7 : 3, MimikoCycle.Focused(Age) ? 2.5f : 1.3f);
        }
        public override void OnKill(int timeLeft) => EmitBurst(face, 6, 1.2f);

        private void GetVisualAnchor(out Vector2 center, out Vector2 facePosition, out float aimRotation)
        {
            Player player = Main.player[Projectile.owner];
            aimRotation = Projectile.ai[0];
            Vector2 aim = aimRotation.ToRotationVector2();
            int direction = aim.X >= 0f ? 1 : -1;
            float rotation = aimRotation + (direction < 0 ? MathHelper.Pi : 0f);
            Vector2 handPosition = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, aimRotation - MathHelper.PiOver2) + new Vector2(0f, player.gfxOffY);
            Vector2 handOffset = aim.RotatedBy(direction < 0 ? -MathHelper.PiOver2 : MathHelper.PiOver2) * 3f - aim * 1.5f;
            center = handPosition + handOffset;
            Vector2 mirrorFaceDirection = (rotation - MathHelper.PiOver2).ToRotationVector2();
            facePosition = center + mirrorFaceDirection * 14f + aim * 4f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GetVisualAnchor(out Vector2 drawCenter, out Vector2 drawFace, out float drawAimRotation);
            Vector2 drawAim = drawAimRotation.ToRotationVector2();
            float drawRotation = drawAimRotation + (drawAim.X < 0f ? MathHelper.Pi : 0f);
            Texture2D mirror = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 grip = new(mirror.Width * 0.5f, mirror.Height - 3f);
            SpriteEffects flip = drawAim.X < 0f ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(mirror, drawCenter - Main.screenPosition, null, lightColor * Opacity, drawRotation, grip, 1f, flip);
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64").Value;
            float flash = Flash;
            Color color = Color.Lerp(Violet, Pearl, Alignment);
            Texture2D bloom = CommonTextures.feather_circle128PMA.Value;
            Texture2D sigil = CommonTextures.RainbowRod.Value;
            float breathe = 1f + MathF.Sin(Age * 0.07f) * 0.1f;
            Vector2 sigilScale = new Vector2(0.38f, 0.23f) * breathe * (0.8f + Alignment * 0.3f + flash * 0.15f);
            float sigilRotation = drawAimRotation + MathHelper.PiOver2;
            Main.EntitySpriteDraw(bloom, drawFace - Main.screenPosition, null, Additive(color, Opacity * (0.3f + flash * 0.2f)), sigilRotation, bloom.Size() * 0.5f, sigilScale * 1.8f, SpriteEffects.None);
            Main.EntitySpriteDraw(sigil, drawFace - Main.screenPosition, null, Additive(Violet, Opacity * (0.4f + Alignment * 0.2f)), sigilRotation, sigil.Size() * 0.5f, sigilScale, SpriteEffects.None);
            Main.EntitySpriteDraw(sigil, drawFace - Main.screenPosition, null, Additive(Pearl, Opacity * (0.16f + flash * 0.35f)), sigilRotation, sigil.Size() * 0.5f, sigilScale * 0.78f, SpriteEffects.None);
            Main.EntitySpriteDraw(glow, drawFace - Main.screenPosition, null, Additive(color, Opacity * (0.2f + flash * 0.2f)), drawRotation, glow.Size() * 0.5f, 0.6f + flash * 0.2f, SpriteEffects.None);
            Main.EntitySpriteDraw(star, drawFace - Main.screenPosition, null, Additive(Pearl, Opacity * (0.3f + flash * 0.7f)), drawAimRotation, star.Size() * 0.5f, new Vector2(0.18f + flash * 0.22f, 0.08f + flash * 0.12f), SpriteEffects.None);
            if (!blocked)
            {
                Vector2[][] snapshot = new Vector2[Rays][];
                float[] widths = new float[Rays];
                float[] strengths = new float[Rays];
                float[] depths = new float[Rays];
                for (int i = 0; i < Rays; i++)
                {
                    if (paths[i] == null)
                    {
                        snapshot[i] = Array.Empty<Vector2>();
                    }
                    else
                    {
                        snapshot[i] = new Vector2[paths[i].Length];
                        for (int j = 0; j < paths[i].Length; j++)
                            snapshot[i][j] = paths[i][j] - face;
                    }
                    widths[i] = MathHelper.Lerp(10f + rayPulses[i] * 5f, 15f + flash * 7f, Alignment) * Opacity;
                    strengths[i] = Opacity * MathHelper.Lerp(0.58f + rayPulses[i] * 0.20f, 0.38f + flash * 0.62f, Alignment);
                    float orbitDepth = 0.5f + 0.5f * MathF.Sin(Age * 0.075f + i);
                    depths[i] = MathHelper.Lerp(0.28f + orbitDepth * 0.72f, 1f, Alignment);
                }
                float time = Age;
                float alignment = Alignment;
                float snapshotAim = Projectile.ai[0];
                Effect effect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaserVertexGradient", AssetRequestMode.ImmediateLoad).Value;
                Texture2D mask = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/Clear/GlowTrailClear", AssetRequestMode.ImmediateLoad).Value;
                ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.OverPlayers, () =>
                {
                    GetVisualAnchor(out _, out Vector2 liveFace, out float liveAim);
                    DrawRays(effect, mask, snapshot, widths, strengths, depths, time, alignment, liveFace, MathHelper.WrapAngle(liveAim - snapshotAim));
                });
            }
            return false;
        }

        private static Texture2D purpleGradient;
        private static Texture2D darkPurpleGradient;

        private static Texture2D PurpleGradient(bool dark)
        {
            Texture2D gradient = dark ? darkPurpleGradient : purpleGradient;
            if (gradient != null && !gradient.IsDisposed) return gradient;
            gradient = new Texture2D(Main.instance.GraphicsDevice, 256, 1);
            Color[] colors = new Color[256];
            Color purple = dark ? new Color(52, 18, 112) : new Color(135, 65, 255);
            Color pinkPurple = dark ? new Color(118, 38, 156) : new Color(235, 105, 255);
            Color palePurple = dark ? new Color(128, 78, 176) : new Color(220, 185, 255);
            for (int i = 0; i < colors.Length; i++)
            {
                float t = i / 255f;
                float wave = 0.5f + 0.5f * MathF.Sin(t * MathHelper.TwoPi * 2f);
                Color mixed = Color.Lerp(purple, pinkPurple, wave);
                colors[i] = Color.Lerp(mixed, palePurple, MathF.Pow(MathF.Sin(t * MathHelper.Pi), 6f) * 0.35f);
            }
            gradient.SetData(colors);
            if (dark) darkPurpleGradient = gradient;
            else purpleGradient = gradient;
            return gradient;
        }

        private static void DrawRays(Effect effect, Texture2D mask, Vector2[][] paths, float[] widths, float[] strengths, float[] depths, float time, float alignment, Vector2 anchor, float rotationOffset)
        {
            GraphicsDevice device = Main.instance.GraphicsDevice;
            BlendState oldBlend = device.BlendState;
            RasterizerState oldRasterizer = device.RasterizerState;
            device.BlendState = BlendState.Additive;
            device.RasterizerState = RasterizerState.CullNone;
            try
            {
                effect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                effect.Parameters["onTex"].SetValue(mask);
                effect.Parameters["satPower"].SetValue(0.8f);
                effect.Parameters["sampleTexture1"].SetValue(CommonTextures.ThinGlowLine.Value);
                effect.Parameters["sampleTexture2"].SetValue(CommonTextures.spark_06.Value);
                effect.Parameters["sampleTexture3"].SetValue(CommonTextures.Extra_196_Black.Value);
                effect.Parameters["sampleTexture4"].SetValue(CommonTextures.Trail5Loop.Value);
                effect.Parameters["grad1Speed"].SetValue(0.55f);
                effect.Parameters["grad2Speed"].SetValue(0.72f);
                effect.Parameters["grad3Speed"].SetValue(0.9f);
                effect.Parameters["grad4Speed"].SetValue(0.68f);
                effect.Parameters["gradientReps"].SetValue(0.9f);
                effect.Parameters["tex1reps"].SetValue(1.25f);
                effect.Parameters["tex2reps"].SetValue(0.375f);
                effect.Parameters["tex3reps"].SetValue(1.25f);
                effect.Parameters["tex4reps"].SetValue(0.3125f);
                for (int ray = 0; ray < paths.Length; ray++)
                {
                    Vector2[] relativePositions = paths[ray];
                    if (relativePositions.Length < 3 || widths[ray] < 0.05f) continue;
                    Vector2[] positions = new Vector2[relativePositions.Length];
                    for (int i = 0; i < relativePositions.Length; i++)
                        positions[i] = anchor + relativePositions[i].RotatedBy(rotationOffset);
                    float depth = MathHelper.Clamp(depths[ray], 0f, 1f);
                    bool behind = depth < 0.58f && alignment < 0.8f;
                    effect.Parameters["gradientTex"].SetValue(PurpleGradient(behind));
                    effect.Parameters["baseColor"].SetValue((behind ? new Color(105, 60, 170) : Pearl).ToVector3());
                    float[] rotations = new float[positions.Length];
                    for (int i = 0; i < rotations.Length; i++)
                        rotations[i] = (positions[Math.Min(i + 1, positions.Length - 1)] - positions[Math.Max(0, i - 1)]).ToRotation();
                    for (int layer = 0; layer < 3; layer++)
                    {
                        float width = widths[ray] * (layer == 0 ? 5.2f : layer == 1 ? 2.45f : 1.15f);
                        var strip = new VertexStrip();
                        strip.PrepareStrip(positions, rotations, _ => Color.White,
                            progress => width * MathHelper.SmoothStep(0f, 1f, Math.Min(1f, progress * 10f)) * MathF.Sqrt(Math.Max(0f, 1f - progress)),
                            -Main.screenPosition, includeBacksides: true);
                        float layerPower = layer == 0 ? 0.07f : layer == 1 ? 0.24f : 1.08f;
                        float focusCompensation = MathHelper.Lerp(1f, 0.74f, alignment);
                        float depthPower = behind ? MathHelper.Lerp(0.34f, 0.52f, depth / 0.58f) : MathHelper.Lerp(0.82f, 1f, depth);
                        float power = MathF.Sqrt(Math.Max(0f, strengths[ray] * layerPower * focusCompensation * depthPower));
                        effect.Parameters["tex1Mult"].SetValue(1.25f * power);
                        effect.Parameters["tex2Mult"].SetValue(1.5f * power);
                        effect.Parameters["tex3Mult"].SetValue(1.15f * power);
                        effect.Parameters["tex4Mult"].SetValue(2.5f * power);
                        effect.Parameters["totalMult"].SetValue(power);
                        effect.Parameters["uTime"].SetValue(-time * 0.025f - ray * 0.17f);
                        effect.CurrentTechnique.Passes["MainPS"].Apply();
                        strip.DrawTrail();
                    }
                }
            }
            finally
            {
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                device.BlendState = oldBlend;
                device.RasterizerState = oldRasterizer;
            }
        }

        private static Color Additive(Color color, float opacity) { color *= Math.Clamp(opacity, 0f, 1f); color.A = 0; return color; }
        private static void Spark(Vector2 position, Vector2 velocity, float scale)
        {
            if (Main.dedServ) return;
            Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<GlowPixelCross>(), velocity, 0, Pearl, scale);
            dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.15f, timeBeforeSlow: 8, preSlowPower: 0.94f,
                postSlowPower: 0.88f, velToBeginShrink: 1f, fadePower: 0.91f, shouldFadeColor: false);
        }
        private static void EmitBurst(Vector2 position, int count, float speed)
        {
            if (Main.dedServ) return;
            for (int i = 0; i < count; i++) Spark(position, Main.rand.NextVector2Circular(speed, speed), Main.rand.NextFloat(0.14f, 0.25f));
        }
    }
}