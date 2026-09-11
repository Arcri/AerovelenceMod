using System;
using System.Collections.Generic;
using AerovelenceMod.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerLightningSystem : ModSystem
    {
        private readonly List<LightningPath> dustPaths = new();
        private readonly Dictionary<Entity, CapturedLightning> captured = new();
        private readonly List<FadingLightning> fading = new();
        private readonly List<CrystalRemnant> crystalRemnants = new();
        private Entity captureOwner;

        private sealed class CapturedLightning
        {
            internal ulong Updated;
            internal readonly List<LightningPath> Paths = new();
        }

        private sealed record FadingLightning(LightningPath Path, ulong Started);
        private sealed record CrystalRemnant(Texture2D Texture, Vector2 Position, Vector2 Size, float Rotation, Color Color, ulong Started);

        public static void DissolveCrystal(NPC npc)
        {
            if (Main.dedServ)
                return;
            Release(npc);
            TumblerLightningSystem system = ModContent.GetInstance<TumblerLightningSystem>();
            Texture2D texture = TextureAssets.Npc[npc.type].Value;
            if (texture == null)
                return;
            if (system.crystalRemnants.Count >= 32)
                system.crystalRemnants.RemoveAt(0);
            Vector2 size = npc.ModNPC is TumblerCarapaceShard ? new Vector2(24f, 48f) * npc.scale : npc.Size;
            Color color = Color.Lerp(Lighting.GetColor(npc.Center.ToTileCoordinates()), Color.White, 0.4f);
            system.crystalRemnants.Add(new CrystalRemnant(texture, npc.Center, size, npc.rotation, color, Main.GameUpdateCount));
            for (int i = 0; i < 8; i++)
                TumblerVFX.SpawnSpark(npc.position + Main.rand.NextVector2Square(0f, 1f) * npc.Size, Main.rand.NextVector2Circular(2.5f, 2.5f), new Color(205, 239, 255), 0.2f);
        }

        public static void BeginCapture(Entity owner)
        {
            if (Main.dedServ)
                return;
            TumblerLightningSystem system = ModContent.GetInstance<TumblerLightningSystem>();
            system.captureOwner = owner;
            if (!system.captured.TryGetValue(owner, out CapturedLightning capture))
                system.captured[owner] = capture = new CapturedLightning();
            capture.Updated = Main.GameUpdateCount;
            capture.Paths.Clear();
        }

        public static void EndCapture()
        {
            if (!Main.dedServ)
                ModContent.GetInstance<TumblerLightningSystem>().captureOwner = null;
        }

        public static void Release(Entity owner)
        {
            if (Main.dedServ)
                return;
            TumblerLightningSystem system = ModContent.GetInstance<TumblerLightningSystem>();
            if (!system.captured.Remove(owner, out CapturedLightning capture) || Main.GameUpdateCount - capture.Updated > 8)
                return;
            int sparkBudget = 18;
            foreach (LightningPath path in capture.Paths)
            {
                if (path.Opacity < 0.08f)
                    continue;
                if (system.fading.Count >= 240)
                    system.fading.RemoveAt(0);
                system.fading.Add(new FadingLightning(path, Main.GameUpdateCount));
                int count = Math.Min(sparkBudget, Math.Clamp((int)(path.Length / 65f), 1, 6));
                for (int i = 0; i < count; i++)
                {
                    int segment = Main.rand.Next(1, path.Points.Length);
                    Vector2 point = Vector2.Lerp(path.Points[segment - 1], path.Points[segment], Main.rand.NextFloat());
                    Vector2 direction = (path.Points[segment] - path.Points[segment - 1]).SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2);
                    TumblerVFX.SpawnSpark(point, direction * Main.rand.NextFloat(1.2f, 3.5f), Color.Lerp(path.Color, Color.White, 0.7f), 0.23f);
                    sparkBudget--;
                }
            }
        }

        private sealed record LightningPath(Vector2[] Points, Color Color, float Opacity, float Width, float Length, float Bloom);

        public static void DrawPath(Vector2[] worldPoints, Color color, float opacity, float width, bool emitDust = true, RenderLayer? layer = null, float bloom = 1f)
        {
            if (Main.dedServ || worldPoints.Length < 2 || opacity <= 0f)
                return;
            Vector2[] points = (Vector2[])worldPoints.Clone();
            float length = 0f;
            for (int i = 1; i < points.Length; i++)
                length += Vector2.Distance(points[i - 1], points[i]);
            LightningPath path = new(points, color, MathHelper.Clamp(opacity, 0f, 1f), width, length, bloom);
            TumblerLightningSystem system = ModContent.GetInstance<TumblerLightningSystem>();
            if (system.captureOwner != null && system.captured.TryGetValue(system.captureOwner, out CapturedLightning capture) && capture.Paths.Count < 64)
                capture.Paths.Add(path);
            if (layer.HasValue)
                ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(layer.Value, () => Draw(path, 1f, true));
            else
                PixellationSystem.QueuePixelationAction(() => Draw(path), PixellationSystem.RenderType.Additive);
            if (emitDust && !Main.gamePaused)
                ModContent.GetInstance<TumblerLightningSystem>().dustPaths.Add(path);
        }

        public override void PostDrawTiles()
        {
            dustPaths.Clear();
            captureOwner = null;
            foreach (FadingLightning tail in fading)
            {
                float age = Main.GameUpdateCount - tail.Started;
                float fade = MathHelper.Clamp(1f - age / 22f, 0f, 1f);
                LightningPath path = tail.Path with { Opacity = tail.Path.Opacity * fade * fade, Width = Math.Max(1f, tail.Path.Width * fade), Bloom = tail.Path.Bloom * fade };
                PixellationSystem.QueuePixelationAction(() => Draw(path), PixellationSystem.RenderType.Additive);
            }
            foreach (CrystalRemnant remnant in crystalRemnants)
            {
                float age = Main.GameUpdateCount - remnant.Started;
                float opacity = MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(1f - age / 30f, 0f, 1f));
                ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderNPCs, () =>
                    Main.spriteBatch.Draw(remnant.Texture, remnant.Position - Main.screenPosition - new Vector2(0f, age * 0.12f), null, remnant.Color * opacity, remnant.Rotation, remnant.Texture.Size() * 0.5f, remnant.Size / remnant.Texture.Size(), SpriteEffects.None, 0f));
            }
        }

        public override void PostUpdateEverything()
        {
            fading.RemoveAll(tail => Main.GameUpdateCount - tail.Started >= 22);
            crystalRemnants.RemoveAll(remnant => Main.GameUpdateCount - remnant.Started >= 30);
            List<Entity> stale = new();
            foreach (var pair in captured)
                if (Main.GameUpdateCount - pair.Value.Updated > 8 || pair.Key is NPC { active: false })
                    stale.Add(pair.Key);
            foreach (Entity owner in stale)
            {
                if (owner is NPC { active: false, ModNPC: TumblerCrystalBud or TumblerConductiveCrystal or TumblerCarapaceShard } npc)
                    DissolveCrystal(npc);
                captured.Remove(owner);
            }
        }

        public override void PostUpdateDusts()
        {
            if (Main.dedServ || dustPaths.Count == 0)
                return;
            int budget = 12;
            int offset = Main.rand.Next(dustPaths.Count);
            for (int i = 0; i < dustPaths.Count && budget > 0; i++)
            {
                LightningPath path = dustPaths[(i + offset) % dustPaths.Count];
                float rate = Math.Min(2f, path.Length / 420f * path.Opacity);
                int count = (int)rate + (Main.rand.NextFloat() < rate % 1f ? 1 : 0);
                for (int j = 0; j < count && budget > 0; j++)
                {
                    int segment = Main.rand.Next(1, path.Points.Length);
                    Vector2 position = Vector2.Lerp(path.Points[segment - 1], path.Points[segment], Main.rand.NextFloat());
                    if (position.X < Main.screenPosition.X - 80f || position.X > Main.screenPosition.X + Main.screenWidth + 80f || position.Y < Main.screenPosition.Y - 80f || position.Y > Main.screenPosition.Y + Main.screenHeight + 80f)
                        continue;
                    Vector2 tangent = (path.Points[segment] - path.Points[segment - 1]).SafeNormalize(Vector2.UnitX);
                    Vector2 velocity = tangent.RotatedBy(Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2).RotatedByRandom(0.6f) * Main.rand.NextFloat(0.6f, 2.3f);
                    Color color = Color.Lerp(path.Color, Color.White, Main.rand.NextFloat(0.15f, 0.55f));
                    if (Main.rand.NextBool(3))
                        Dust.NewDustPerfect(position, ModContent.DustType<TumblerLightningDust>(), velocity, 0, color, Main.rand.NextFloat(0.16f, 0.28f));
                    else
                        TumblerVFX.SpawnSpark(position, velocity, color, Main.rand.NextFloat(0.18f, 0.3f));
                    budget--;
                }
            }
            dustPaths.Clear();
        }

        public override void OnWorldUnload()
        {
            dustPaths.Clear();
            captured.Clear();
            fading.Clear();
            crystalRemnants.Clear();
            captureOwner = null;
        }

        private static void Draw(LightningPath path, float scale = 0.5f, bool alphaBlend = false)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/feather_circle").Value;
            float width = Math.Max(2f, path.Width) * scale;
            float pulse = 0.85f + 0.15f * MathF.Sin(Main.GameUpdateCount * 0.2f);
            Color core = Color.Lerp(path.Color, Color.White, 0.9f) * path.Opacity;
            Color middle = path.Color * (path.Opacity * 0.55f);
            Color outer = path.Color * (path.Opacity * 0.26f);
            Color bloom = path.Color * (path.Opacity * 0.22f * pulse * path.Bloom);
            if (alphaBlend)
            {
                core.A = middle.A = outer.A = bloom.A = 0;
            }
            float glowSpacing = 0f;
            for (int i = 1; i < path.Points.Length; i++)
            {
                Vector2 start = (path.Points[i - 1] - Main.screenPosition) * scale;
                Vector2 end = (path.Points[i] - Main.screenPosition) * scale;
                TumblerVFX.DrawLine(spriteBatch, start, end, outer, width + 6f * scale);
                TumblerVFX.DrawLine(spriteBatch, start, end, middle, width + 3f * scale);
                TumblerVFX.DrawLine(spriteBatch, start, end, core, width);
                float distance = Vector2.Distance(start, end);
                while (path.Bloom > 0f && glowSpacing <= distance)
                {
                    Vector2 position = Vector2.Lerp(start, end, distance > 0f ? glowSpacing / distance : 0f);
                    spriteBatch.Draw(glow, position, null, bloom, 0f, glow.Size() * 0.5f, (48f * scale + width * 6f) / glow.Width, SpriteEffects.None, 0f);
                    glowSpacing += 18f * scale;
                }
                glowSpacing -= distance;
            }
        }
    }

    public class TumblerActorVisuals : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.ModNPC is TumblerCrystalBud or TumblerConductiveCrystal or TumblerCarapaceShard;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            TumblerLightningSystem.BeginCapture(npc);
            return true;
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => TumblerLightningSystem.EndCapture();
    }

    public class TumblerLightningDust : ModDust
    {
        public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/GlorbStrong";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.94f;
            dust.scale *= 0.96f;
            dust.alpha += 8;
            if (dust.alpha >= 255)
                dust.active = false;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Vector2 position = dust.position;
            float scale = dust.scale;
            float opacity = 1f - dust.alpha / 255f;
            Color color = dust.color;
            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
                Main.spriteBatch.Draw(texture, position - Main.screenPosition, null, TumblerVFX.Glow(color, opacity), 0f, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, position - Main.screenPosition, null, TumblerVFX.Glow(Color.White, opacity * 0.8f), 0f, texture.Size() * 0.5f, scale * 0.45f, SpriteEffects.None, 0f);
            });
            return false;
        }
    }
}
