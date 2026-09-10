using System;
using System.IO;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.BossSummons;
using AerovelenceMod.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public static class TumblerVFX
    {
        public static void SpawnSpark(Vector2 position, Vector2 velocity, Color color, float scale = 0.2f)
        {
            if (Main.dedServ)
                return;
            Dust spark = Dust.NewDustPerfect(position, ModContent.DustType<ElectricSparkGlow>(), velocity, 0, color, scale);
            spark.customData = new ElectricSparkBehavior(FadeAlphaPower: 0.89f, FadeScalePower: 0.98f, FadeVelPower: 0.92f, TimeBetweenFrames: 3, KillEarlyTime: 24, Pixelize: true, UnderGlowPower: 1.4f, WhiteLayerPower: 0.8f);
        }

        public static Color PhaseColor(float phase)
        {
            return phase >= 1f ? new Color(255, 177, 45) : new Color(45, 226, 255);
        }

        public static Color Glow(Color color, float opacity = 1f)
        {
            color *= MathHelper.Clamp(opacity, 0f, 1f);
            color.A = 0;
            return color;
        }

        public static void BeginAdditive(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public static void EndAdditive(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public static void DrawElectricLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float opacity, int segments = 18, float seed = 0f, float width = 2f)
        {
            Vector2 fullDelta = end - start;
            if (fullDelta.LengthSquared() < 0.01f || opacity <= 0f)
                return;
            float time = (float)(Main.GameUpdateCount / 3) * 0.83f;
            Vector2 normal = fullDelta.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
            segments = Math.Clamp(segments, 2, 64);
            Vector2[] points = new Vector2[segments + 1];
            points[0] = start + Main.screenPosition;
            for (int i = 1; i <= segments; i++)
            {
                float progress = i / (float)segments;
                Vector2 current = Vector2.Lerp(start, end, progress);
                if (i < segments)
                {
                    float envelope = MathF.Sin(progress * MathHelper.Pi);
                    float wave = MathF.Sin(seed * 3.17f + i * 8.31f + time) + MathF.Sin(seed + i * 3.73f - time * 1.7f) * 0.45f;
                    current += normal * wave * 5f * envelope;
                }
                points[i] = current + Main.screenPosition;
                if (width > 1.5f && i > 2 && i < segments - 1 && i % 5 == 0)
                {
                    float branchSide = MathF.Sin(seed * 2.31f + i * 4.7f) >= 0f ? 1f : -1f;
                    Vector2 branch = (points[i] - points[i - 1]).SafeNormalize(Vector2.UnitX).RotatedBy(branchSide * 0.78f) * (12f + 5f * MathF.Abs(MathF.Sin(seed + i)));
                    TumblerLightningSystem.DrawPath(new[] { points[i], points[i] + branch * 0.5f + normal * 2f, points[i] + branch }, color, opacity * 0.55f, Math.Max(1f, width * 0.55f));
                }
            }
            TumblerLightningSystem.DrawPath(points, color, opacity, width);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width)
        {
            Vector2 delta = end - start;
            if (delta.LengthSquared() < 0.01f)
                return;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, start, new Rectangle(0, 0, 1, 1), color, delta.ToRotation(), new Vector2(0f, 0.5f), new Vector2(delta.Length(), Math.Max(0.5f, width)), SpriteEffects.None, 0f);
        }

        public static void DrawTelegraph(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float opacity, float spacing = 46f)
        {
            if (opacity <= 0f)
                return;
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            float distance = Vector2.Distance(start, end);
            int count = Math.Clamp((int)(distance / 28f), 4, 60);
            for (int i = 0; i < count; i++)
            {
                float progress = (i + 0.5f) / count;
                float fade = (1f - progress) * (1f - progress);
                Vector2 segmentStart = Vector2.Lerp(start, end, i / (float)count);
                Vector2 segmentEnd = Vector2.Lerp(start, end, (i + 1f) / count);
                DrawLine(spriteBatch, segmentStart, segmentEnd, Glow(color, opacity * fade * 0.14f), 4f);
                DrawLine(spriteBatch, segmentStart, segmentEnd, Glow(Color.Lerp(color, Color.White, 0.2f), opacity * fade * 0.82f), 1f);
            }
            float lockGlow = MathHelper.Clamp((opacity - 0.45f) / 0.4f, 0f, 1f);
            spriteBatch.Draw(star, start, null, Glow(Color.Lerp(color, Color.White, lockGlow * 0.65f), opacity), (end - start).ToRotation(), star.Size() * 0.5f, new Vector2(0.22f + lockGlow * 0.13f, 0.1f + lockGlow * 0.08f), SpriteEffects.None, 0f);
        }

        public static void DrawCorona(SpriteBatch spriteBatch, Vector2 center, float radius, Color color, float opacity, float seed = 0f, float width = 1.5f)
        {
            if (opacity <= 0f || radius <= 0f)
                return;
            float time = (float)(Main.GameUpdateCount / 3) * 0.65f;
            int segments = Math.Clamp((int)(radius * 0.8f), 12, 48);
            Vector2[] points = new Vector2[segments + 1];
            points[0] = center + Main.screenPosition + new Vector2(radius, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float angle = i / (float)segments * MathHelper.TwoPi;
                float jitter = MathF.Sin(i * 8.17f + seed + time) * 2.4f;
                Vector2 current = center + angle.ToRotationVector2() * (radius + (i == segments ? 0f : jitter));
                points[i] = current + Main.screenPosition;
            }
            TumblerLightningSystem.DrawPath(points, color, opacity * 0.8f, width);
        }

        public static void DrawCharge(SpriteBatch spriteBatch, Vector2 center, Color color, float progress, float radius, float rotation = 0f, float opacity = 1f)
        {
            if (radius <= 0f || opacity <= 0f)
                return;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64").Value;
            spriteBatch.Draw(glow, center, null, Glow(color, (0.12f + progress * 0.22f) * opacity), rotation, glow.Size() * 0.5f, radius * 3f / glow.Width, SpriteEffects.None, 0f);
            DrawCorona(spriteBatch, center, radius * (1.5f - progress * 0.5f), color, (0.2f + progress * 0.55f) * opacity, rotation);
            spriteBatch.Draw(star, center, null, Glow(color, (0.6f + progress * 0.4f) * opacity), rotation, star.Size() * 0.5f, radius * 2f / star.Width, SpriteEffects.None, 0f);
            spriteBatch.Draw(star, center, null, Glow(Color.White, (0.55f + progress * 0.45f) * opacity), -rotation, star.Size() * 0.5f, radius / star.Width, SpriteEffects.None, 0f);
            for (int i = 0; i < 3; i++)
            {
                float angle = rotation + i * MathHelper.TwoPi / 3f;
                Vector2 tip = center + angle.ToRotationVector2() * radius * (1.5f - progress * 0.5f);
                spriteBatch.Draw(star, tip, null, Glow(color, (0.45f + progress * 0.4f) * opacity), -angle, star.Size() * 0.5f, Math.Min(0.16f, radius / 75f), SpriteEffects.None, 0f);
            }
        }
    }

    public class TumblerSpark : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.timeLeft = 28;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.93f;
            Projectile.rotation += 0.2f;
            Lighting.AddLight(Projectile.Center, TumblerVFX.PhaseColor(Projectile.ai[0]).ToVector3() * 0.35f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = Projectile.timeLeft / 28f;
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            Color color = TumblerVFX.PhaseColor(Projectile.ai[0]);
            Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition, null, TumblerVFX.Glow(color, opacity), Projectile.rotation, star.Size() * 0.5f, 0.16f + opacity * 0.12f, SpriteEffects.None);
            return false;
        }
    }

    public class TumblerStar : ModProjectile
    {
        private int timer;
        private int orbitSlot;
        private bool initialized;
        private bool beamLocked;
        private float groundX;
        private readonly TumblerLightningVisual tether = new();

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1200;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(orbitSlot);
            writer.Write(initialized);
            writer.Write(beamLocked);
            writer.Write(groundX);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            orbitSlot = reader.ReadInt32();
            initialized = reader.ReadBoolean();
            beamLocked = reader.ReadBoolean();
            groundX = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 310;
        }

        public override bool ShouldUpdatePosition() => false;

        private bool TryGetOwner(out NPC owner)
        {
            int index = (int)Projectile.ai[1];
            owner = index >= 0 && index < Main.maxNPCs ? Main.npc[index] : null;
            return owner != null && owner.active && owner.type == ModContent.NPCType<CrystalTumbler>();
        }

        private bool BeamActive => beamLocked && timer >= 90 && timer < 235;

        private Vector2 BeamEndpoint(NPC owner) => new(groundX, ArenaData.Valid ? ArenaData.FloorY - 4f : owner.Bottom.Y);

        public override void AI()
        {
            if (!initialized)
            {
                float angle = Projectile.velocity.ToRotation();
                orbitSlot = (int)MathF.Round((angle + MathHelper.TwoPi) % MathHelper.TwoPi / MathHelper.TwoPi * 7f) % 7;
                initialized = true;
            }
            timer++;
            if (!TryGetOwner(out NPC owner))
            {
                Projectile.Kill();
                return;
            }
            float unfold = MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(timer / 42f, 0f, 1f));
            float radius = MathHelper.Lerp(48f, 170f, unfold);
            float orbitAngle = orbitSlot * MathHelper.TwoPi / 7f + timer * 0.024f;
            Vector2 orbitCenter = owner.Center - new Vector2(0f, 82f);
            if (ArenaData.Valid)
                orbitCenter.Y = Math.Min(orbitCenter.Y, ArenaData.FloorY - radius - 26f);
            Projectile.Center = orbitCenter + orbitAngle.ToRotationVector2() * radius;
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = orbitAngle;
            if (Projectile.ai[0] < 1f)
            {
                int fireTime = 163 + orbitSlot * 18;
                int lockTime = fireTime - 36;
                if (timer >= lockTime - 36 && timer <= lockTime && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
                    Projectile.ai[2] = (target.Center - Projectile.Center).ToRotation();
                    Projectile.netUpdate = timer % 8 == 0 || timer == lockTime;
                }
                if (timer == fireTime && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 direction = Projectile.ai[2].ToRotationVector2();
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction * 9f, ModContent.ProjectileType<ElectricBolt>(), Projectile.damage, 0f, Main.myPlayer);
                }
                Projectile.scale = MathHelper.Clamp((fireTime + 20f - timer) / 20f, 0f, 1f);
                if (timer >= fireTime + 20)
                    Projectile.Kill();
            }
            else
            {
                if (timer >= 20 && (beamLocked || Main.netMode != NetmodeID.MultiplayerClient))
                {
                    Player player = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
                    if (!beamLocked)
                    {
                        int side = player.Center.X >= owner.Center.X ? 1 : -1;
                        groundX = player.Center.X - side * 180f;
                        beamLocked = true;
                        Projectile.netUpdate = true;
                    }
                    else if (BeamActive)
                    {
                        groundX += MathHelper.Clamp(player.Center.X - groundX, -1.8f, 1.8f);
                        Projectile.netUpdate = Main.netMode != NetmodeID.MultiplayerClient && timer % 10 == 0;
                    }
                    if (ArenaData.Valid)
                        groundX = MathHelper.Clamp(groundX, ArenaData.InnerArenaBoundaryLeft.X + 35f, ArenaData.InnerArenaBoundaryRight.X - 35f);
                }
                if (beamLocked)
                    tether.Update(Projectile, Projectile.Center, BeamEndpoint(owner), 0.18f);
                Projectile.scale = MathHelper.Clamp((260f - timer) / 18f, 0f, 1f);
                if (timer == 90)
                    SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.55f, Pitch = 0.15f }, Projectile.Center);
                if (BeamActive && !Main.dedServ && timer % 4 == 0)
                    TumblerVFX.SpawnSpark(BeamEndpoint(owner), new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(-3f, -1f)), new Color(255, 235, 170), 0.23f);
                if (timer >= 260)
                    Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, (Projectile.ai[0] >= 1f ? Color.Gold : Color.DeepSkyBlue).ToVector3() * 0.65f);
        }

        public override bool? CanDamage() => Projectile.ai[0] >= 1f && BeamActive && TryGetOwner(out _);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!TryGetOwner(out NPC owner))
                return false;
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, BeamEndpoint(owner), 10f, ref point);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 45);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color = Projectile.ai[0] >= 1f ? new Color(255, 190, 45) : new Color(40, 225, 255);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            int fireTime = 163 + orbitSlot * 18;
            float charge = MathHelper.Clamp(Projectile.ai[0] >= 1f ? timer / 90f : (timer - fireTime + 72f) / 72f, 0f, 1f);
            TumblerVFX.DrawCharge(Main.spriteBatch, drawPosition, color, charge, Projectile.scale * (18f + charge * 5f), Projectile.rotation);
            if (Projectile.ai[0] >= 1f && TryGetOwner(out NPC owner) && beamLocked)
            {
                Vector2 end = BeamEndpoint(owner) - Main.screenPosition;
                if (timer >= 90 && timer < 250)
                {
                    float fade = MathHelper.Clamp((250f - timer) / 15f, 0f, 1f);
                    float flash = MathHelper.Clamp((102f - timer) / 12f, 0f, 1f);
                    Color beamColor = Color.Lerp(color, Color.White, flash * 0.8f);
                    tether.Draw(Main.spriteBatch, beamColor, fade, 2.6f);
                    TumblerVFX.DrawCharge(Main.spriteBatch, end, beamColor, 1f, 18f, timer * 0.08f, fade);
                }
                else if (timer < 90)
                {
                    float warning = MathHelper.Clamp((timer - 20f) / 70f, 0f, 1f);
                    TumblerVFX.DrawTelegraph(Main.spriteBatch, drawPosition, end, color, 0.2f + warning * 0.55f);
                    TumblerVFX.DrawCharge(Main.spriteBatch, end, color, warning, 14f, -timer * 0.03f);
                }
            }
            else if (timer >= fireTime - 72 && timer < fireTime)
                TumblerVFX.DrawTelegraph(Main.spriteBatch, drawPosition, drawPosition + Projectile.ai[2].ToRotationVector2() * 1000f, color, timer >= fireTime - 36 ? 0.85f : charge * 0.6f);
            return false;
        }
    }

    public class TumblerAimLine : ModProjectile
    {
        private int timer;

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1200;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.timeLeft = 48;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            timer++;
            if (timer < 22 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Player player = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
                Projectile.velocity = Vector2.Lerp(Projectile.velocity.SafeNormalize(Vector2.UnitX), (player.Center - Projectile.Center).SafeNormalize(Vector2.UnitX), 0.14f).SafeNormalize(Vector2.UnitX);
                Projectile.netUpdate = timer % 7 == 0;
            }
            if (timer == 22 && Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.netUpdate = true;
            if (timer == 36 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10.5f, ModContent.ProjectileType<ElectricBolt>(), Projectile.damage, 0f, Main.myPlayer, Projectile.ai[0]);
                SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.35f, Pitch = 0.2f }, Projectile.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = timer < 22 ? timer / 22f * 0.48f : timer < 36 ? 0.85f : MathHelper.Clamp((44f - timer) / 8f, 0f, 1f) * 0.35f;
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 end = start + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 1100f;
            Color color = TumblerVFX.PhaseColor(Projectile.ai[0]);
            TumblerVFX.DrawTelegraph(Main.spriteBatch, start, end, color, opacity);
            if (timer < 36)
                TumblerVFX.DrawCharge(Main.spriteBatch, start, color, timer / 36f, 15f, timer * 0.05f);
            return false;
        }
    }

    public class TumblerLightningBolt : ModProjectile
    {
        private int timer;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            Projectile.timeLeft = reader.ReadInt32();
        }
        private readonly TumblerLightningVisual lightning = new();

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            timer++;
            if (timer == (int)Projectile.ai[0])
            {
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.5f, Pitch = 0.15f }, Projectile.Center);
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 7; i++)
                        TumblerVFX.SpawnSpark(Projectile.Center + Projectile.velocity * ((i + 0.5f) / 7f), new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(-2f, 0.5f)), TumblerVFX.PhaseColor(Projectile.ai[1]), 0.23f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity, Vector2.Zero, ModContent.ProjectileType<TumblerAuraPulse>(), 0, 0f, Main.myPlayer, 54f, 20f, Projectile.ai[1]);
            }
            if (timer >= Projectile.ai[0])
            {
                lightning.Update(Projectile, Projectile.Center, Projectile.Center + Projectile.velocity, 0.7f);
                Lighting.AddLight(Projectile.Center + Projectile.velocity, TumblerVFX.PhaseColor(Projectile.ai[1]).ToVector3() * 1.1f);
            }
            if (timer > Projectile.ai[0] + 14f)
                Projectile.Kill();
        }

        public override bool? CanDamage()
        {
            return timer >= Projectile.ai[0] && timer <= Projectile.ai[0] + 10f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 14f, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float telegraph = MathHelper.Clamp(timer / Math.Max(1f, Projectile.ai[0]), 0f, 1f);
            float strike = timer >= Projectile.ai[0] ? MathHelper.Clamp(1f - (timer - Projectile.ai[0]) / 14f, 0f, 1f) : 0f;
            Color color = Projectile.ai[1] >= 1f ? new Color(255, 182, 48) : new Color(45, 225, 255);
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 end = start + Projectile.velocity;
            if (strike > 0f)
            {
                lightning.Draw(Main.spriteBatch, color, strike, 3f);
                Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Flare/star_07").Value;
                Main.EntitySpriteDraw(star, end, null, TumblerVFX.Glow(color, strike), 0f, star.Size() * 0.5f, new Vector2(0.38f, 0.18f) * strike, SpriteEffects.None);
            }
            else
            {
                TumblerVFX.DrawTelegraph(Main.spriteBatch, start, end, color, 0.18f + telegraph * 0.4f);
                TumblerVFX.DrawCharge(Main.spriteBatch, end, color, telegraph, 13f + 9f * (1f - telegraph), -timer * 0.025f);
            }
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 75);
        }
    }

    public class TumblerConductiveField : ModProjectile
    {
        private int timer;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            Projectile.timeLeft = reader.ReadInt32();
        }
        private readonly TumblerLightningVisual lightning = new();

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            timer++;
            if (!TryGetEndpoints(out Vector2 start, out Vector2 end))
            {
                Projectile.Kill();
                return;
            }
            if (timer >= 120)
                lightning.Update(Projectile, start, end, 0.65f);
            if (timer == 120)
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.6f, Pitch = -0.05f }, Projectile.Center);
        }

        private bool TryGetEndpoints(out Vector2 start, out Vector2 end)
        {
            int first = (int)Projectile.ai[0];
            int second = (int)Projectile.ai[1];
            start = Vector2.Zero;
            end = Vector2.Zero;
            if (first < 0 || first >= Main.maxNPCs || second < 0 || second >= Main.maxNPCs || !Main.npc[first].active || !Main.npc[second].active || Main.npc[first].type != ModContent.NPCType<TumblerConductiveCrystal>() || Main.npc[second].type != ModContent.NPCType<TumblerConductiveCrystal>())
                return false;
            start = Main.npc[first].Center;
            end = Main.npc[second].Center;
            Projectile.Center = (start + end) * 0.5f;
            return true;
        }

        public override bool? CanDamage()
        {
            return timer >= 120 && timer <= 330;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!TryGetEndpoints(out Vector2 start, out Vector2 end))
                return false;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 24f, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!TryGetEndpoints(out Vector2 start, out Vector2 end))
                return false;
            float opacity = timer < 120 ? timer / 120f * 0.32f : MathHelper.Clamp((360f - timer) / 30f, 0f, 1f);
            Color color = TumblerVFX.PhaseColor(Projectile.ai[2]);
            start -= Main.screenPosition;
            end -= Main.screenPosition;
            float charge = MathHelper.Clamp(timer / 120f, 0f, 1f);
            float endpointOpacity = timer < 120 ? 1f : opacity;
            TumblerVFX.DrawCharge(Main.spriteBatch, start, color, charge, 24f, timer * 0.025f, endpointOpacity);
            TumblerVFX.DrawCharge(Main.spriteBatch, end, color, charge, 24f, -timer * 0.025f, endpointOpacity);
            if (timer < 120)
            {
                TumblerVFX.DrawTelegraph(Main.spriteBatch, start, end, color, 0.2f + charge * 0.45f);
                for (int i = 0; i < 3; i++)
                {
                    float flow = (timer / 48f + i / 3f) % 1f;
                    Vector2 point = Vector2.Lerp(start, end, flow);
                    TumblerVFX.DrawCorona(Main.spriteBatch, point, 4f + charge * 4f, color, 0.6f, i);
                }
            }
            else
            {
                lightning.Draw(Main.spriteBatch, color, opacity, 3f);
                Vector2 normal = (end - start).SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * 10f;
                TumblerVFX.DrawElectricLine(Main.spriteBatch, start + normal, end + normal, color, opacity * 0.3f, 28, Projectile.identity + 8f, 1f);
                TumblerVFX.DrawElectricLine(Main.spriteBatch, start - normal, end - normal, color, opacity * 0.3f, 28, Projectile.identity - 8f, 1f);
            }
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 75);
        }
    }

    public class TumblerPlatformField : ModProjectile
    {
        private int timer;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            Projectile.timeLeft = reader.ReadInt32();
        }

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1600;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 270;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            timer++;
        }

        public override bool? CanDamage()
        {
            return timer >= 75 && Projectile.timeLeft >= 15;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 18f, ref point);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = timer < 75 ? timer / 75f * 0.3f : MathHelper.Clamp(Projectile.timeLeft / 25f, 0f, 1f);
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 end = start + Projectile.velocity;
            Color color = TumblerVFX.PhaseColor(1f);
            if (timer < 75)
                TumblerVFX.DrawTelegraph(Main.spriteBatch, start, end, color, 0.16f + opacity);
            else
                TumblerVFX.DrawElectricLine(Main.spriteBatch, start, end, color, opacity, Math.Max(8, (int)(Projectile.velocity.Length() / 24f)), Projectile.identity, 2f);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 60);
        }
    }

    public class TumblerKnifeCrystal : ModProjectile
    {
        private int timer;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            Projectile.timeLeft = reader.ReadInt32();
        }

        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/GroundSpike";

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 110;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 190;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            timer++;
            if (timer == (int)Projectile.ai[0])
                SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.32f, Pitch = 0.3f }, Projectile.Center);
        }

        public override bool? CanDamage()
        {
            return timer >= Projectile.ai[0] && timer <= Projectile.ai[0] + 38f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float rise = CurrentRise();
            Rectangle hitbox = new((int)Projectile.Center.X - 10, (int)(Projectile.Center.Y - 105f * rise), 20, (int)(105f * rise));
            return hitbox.Intersects(targetHitbox);
        }

        private float CurrentRise()
        {
            return MathHelper.Clamp((timer - Projectile.ai[0]) / 10f, 0f, 1f) * MathHelper.Clamp((Projectile.ai[0] + 52f - timer) / 14f, 0f, 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color = TumblerVFX.PhaseColor(Projectile.ai[1]);
            Vector2 basePosition = Projectile.Center - Main.screenPosition;
            if (timer < Projectile.ai[0])
            {
                float charge = MathHelper.Clamp(timer / Math.Max(1f, Projectile.ai[0]), 0f, 1f);
                Vector2 tip = basePosition - new Vector2(0f, 105f);
                TumblerVFX.DrawTelegraph(Main.spriteBatch, basePosition, tip, color, 0.2f + charge * 0.45f, 26f);
                TumblerVFX.DrawLine(Main.spriteBatch, basePosition - new Vector2(9f, 0f), basePosition + new Vector2(9f, 0f), TumblerVFX.Glow(color, 0.4f + charge * 0.5f), 3f);
                TumblerVFX.DrawCorona(Main.spriteBatch, basePosition, 12f + 10f * (1f - charge), color, 0.22f + charge * 0.4f, Projectile.identity);
            }
            else
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                float rise = CurrentRise();
                Vector2 scale = new(20f / texture.Width, 105f * rise / texture.Height);
                Main.EntitySpriteDraw(texture, basePosition, null, Color.Lerp(lightColor, color, 0.45f), 0f, new Vector2(texture.Width * 0.5f, texture.Height), scale, SpriteEffects.None);
                Main.EntitySpriteDraw(texture, basePosition, null, TumblerVFX.Glow(color, rise * 0.35f), 0f, new Vector2(texture.Width * 0.5f, texture.Height), scale, SpriteEffects.None);
            }
            return false;
        }
    }

    public class TumblerChargeBall : ModProjectile
    {
        private int timer;

        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/TumblerOrb";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
        }

        public override void AI()
        {
            timer++;
            Projectile.frame = timer / 6 % 4;
            Projectile.rotation += 0.08f;
            if (timer < 60)
            {
                Projectile.velocity *= 0.94f;
                Projectile.scale = MathHelper.Lerp(0.45f, 1f, timer / 60f);
            }
            else if (timer < 180)
            {
                Projectile.tileCollide = true;
                Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
                Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 8.5f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.018f);
            }
            Lighting.AddLight(Projectile.Center, new Vector3(0.9f, 0.48f, 0.06f));
        }

        public override bool? CanDamage()
        {
            return timer >= 60;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 90);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.35f, Pitch = -0.15f }, Projectile.Center);
            if (Main.netMode != NetmodeID.Server && Vector2.DistanceSquared(Main.LocalPlayer.Center, Projectile.Center) < 1600f * 1600f)
                Main.LocalPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = Math.Max(Main.LocalPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower, 4f);
            if (Main.netMode != NetmodeID.MultiplayerClient && !ArenaData.ClearingEncounter)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TumblerAuraPulse>(), 0, 0f, Main.myPlayer, 58f, 22f, 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TumblerPhaseTextures.Get("TumblerOrb", true);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Color color = new Color(255, 177, 45);
            Rectangle frame = texture.Frame(1, 4, 0, Projectile.frame);
            Main.EntitySpriteDraw(texture, position, frame, TumblerVFX.Glow(Color.White, 0.75f), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 0.44f, SpriteEffects.None);
            TumblerVFX.DrawCharge(Main.spriteBatch, position, color, timer / 60f, Projectile.scale * 23f, -Projectile.rotation);
            if (timer < 60)
            {
                for (int i = 0; i < 3; i++)
                {
                    float angle = Projectile.rotation + i * MathHelper.TwoPi / 3f;
                    Vector2 source = position + angle.ToRotationVector2() * (85f - timer);
                    TumblerVFX.DrawElectricLine(Main.spriteBatch, source, position, color, 0.25f + timer / 120f, 8, Projectile.identity + i, 1f);
                }
            }
            return false;
        }
    }

    public class TumblerKnifeBall : ModProjectile
    {
        private int timer;
        protected virtual bool Charged => false;

        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/TumblerOrb";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1200;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 115;
        }

        public override void AI()
        {
            timer++;
            Projectile.frame = timer / 7 % 4;
            Projectile.velocity *= timer < 32 ? 0.91f : 0.98f;
            Projectile.rotation += 0.075f;
            if (timer <= 45 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
                Projectile.ai[0] = (target.Center - Projectile.Center).ToRotation();
                Projectile.netUpdate = timer % 10 == 0 || timer == 45;
            }
            if (timer == 75 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 direction = Projectile.ai[0].ToRotationVector2();
                int amount = Charged ? 3 : 1;
                for (int i = 0; i < amount; i++)
                {
                    float spread = Charged ? (i - 1) * 0.12f : 0f;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction.RotatedBy(spread) * 10f, ModContent.ProjectileType<ElectricBolt>(), Projectile.damage, 0f, Main.myPlayer, Charged ? 1f : 0f);
                }
                SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.4f, Pitch = Charged ? -0.05f : 0.25f }, Projectile.Center);
            }
            Lighting.AddLight(Projectile.Center, TumblerVFX.PhaseColor(Charged ? 1f : 0f).ToVector3() * 0.55f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TumblerPhaseTextures.Get("TumblerOrb", Charged);
            Color color = TumblerVFX.PhaseColor(Charged ? 1f : 0f);
            Vector2 position = Projectile.Center - Main.screenPosition;
            float fade = MathHelper.Clamp((105f - timer) / 30f, 0f, 1f);
            Rectangle frame = texture.Frame(1, 4, 0, Projectile.frame);
            Main.EntitySpriteDraw(texture, position, frame, TumblerVFX.Glow(Color.White, fade * 0.8f), Projectile.rotation, frame.Size() * 0.5f, Charged ? 0.5f : 0.42f, SpriteEffects.None);
            TumblerVFX.DrawCharge(Main.spriteBatch, position, color, timer / 75f, Charged ? 23f : 18f, -Projectile.rotation, fade);
            if (timer >= 28 && timer < 75)
            {
                float opacity = timer >= 45 ? 0.7f : MathHelper.Clamp((timer - 28f) / 17f, 0f, 1f) * 0.4f;
                int amount = Charged ? 3 : 1;
                for (int i = 0; i < amount; i++)
                {
                    float spread = Charged ? (i - 1) * 0.12f : 0f;
                    Vector2 end = position + (Projectile.ai[0] + spread).ToRotationVector2() * 1000f;
                    TumblerVFX.DrawTelegraph(Main.spriteBatch, position, end, color, opacity);
                }
            }
            return false;
        }
    }

    public class TumblerChargedKnifeBall : TumblerKnifeBall
    {
        protected override bool Charged => true;
    }

    public class TumblerBossAura : ModProjectile
    {
        private int timer;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            Projectile.timeLeft = reader.ReadInt32();
        }
        private float phase;

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            timer++;
            int ownerIndex = (int)Projectile.ai[0];
            if (ownerIndex < 0 || ownerIndex >= Main.maxNPCs || !Main.npc[ownerIndex].active || Main.npc[ownerIndex].type != ModContent.NPCType<CrystalTumbler>() || timer >= Projectile.ai[1])
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = Main.npc[ownerIndex].Center;
            phase = Main.npc[ownerIndex].ai[2];
            Lighting.AddLight(Projectile.Center, TumblerVFX.PhaseColor(phase).ToVector3() * 0.7f);
        }

        private float CurrentRadius()
        {
            float target = Math.Max(40f, Projectile.ai[2]);
            if (target <= 82f)
                return target;
            return MathHelper.Lerp(76f, target, MathHelper.Clamp(timer / Math.Max(1f, Projectile.ai[1]), 0f, 1f));
        }

        public override bool? CanDamage()
        {
            return timer >= 18 && timer <= Projectile.ai[1] - 12f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Vector2.Distance(Projectile.Center, targetHitbox.ClosestPointInRect(Projectile.Center)) <= CurrentRadius();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 90);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ring = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/whiteFireEye").Value;
            float lifetime = Math.Max(1f, Projectile.ai[1]);
            float opacity = MathHelper.Clamp(timer / 18f, 0f, 1f) * MathHelper.Clamp((lifetime - timer) / 18f, 0f, 1f);
            float scale = CurrentRadius() * 2f / ring.Width;
            Color color = TumblerVFX.PhaseColor(phase);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(ring, position, null, TumblerVFX.Glow(color, opacity * 0.22f), timer * 0.012f, ring.Size() * 0.5f, scale * 1.25f, SpriteEffects.None);
            TumblerVFX.DrawCorona(Main.spriteBatch, position, CurrentRadius(), color, opacity, Projectile.identity, 2f);
            TumblerVFX.DrawCorona(Main.spriteBatch, position, CurrentRadius() - 6f, color, opacity * 0.38f, Projectile.identity + 4f);
            return false;
        }
    }

    public class TumblerAuraPulse : ModProjectile
    {
        private int timer;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            Projectile.timeLeft = reader.ReadInt32();
        }

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            timer++;
            if (timer >= Projectile.ai[1])
                Projectile.Kill();
        }

        public override bool? CanDamage()
        {
            return Projectile.damage > 0 && timer >= 4 && timer <= 14;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float radius = Projectile.ai[0] * MathHelper.Clamp(timer / Math.Max(1f, Projectile.ai[1]), 0f, 1f);
            return Vector2.Distance(Projectile.Center, targetHitbox.ClosestPointInRect(Projectile.Center)) <= radius;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ring = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/feather_circle").Value;
            float progress = timer / Math.Max(1f, Projectile.ai[1]);
            float scale = Projectile.ai[0] * progress * 2f / ring.Width;
            Color color = TumblerVFX.PhaseColor(Projectile.ai[2]);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(ring, position, null, TumblerVFX.Glow(color, (1f - progress) * 0.4f), 0f, ring.Size() * 0.5f, scale, SpriteEffects.None);
            TumblerVFX.DrawCorona(Main.spriteBatch, position, Projectile.ai[0] * progress, color, 1f - progress, Projectile.identity, 2f);
            if (timer < 9)
            {
                Texture2D flare = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Flare/star_07").Value;
                Main.EntitySpriteDraw(flare, position, null, TumblerVFX.Glow(Color.Lerp(color, Color.White, 0.5f), 1f - timer / 9f), 0f, flare.Size() * 0.5f, new Vector2(1f, 0.45f) * Projectile.ai[0] / 120f, SpriteEffects.None);
            }
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 60);
        }
    }
}
