using System;
using System.Collections.Generic;
using AerovelenceMod.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerLightningSystem : ModSystem
    {
        private readonly List<LightningPath> dustPaths = new();

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
