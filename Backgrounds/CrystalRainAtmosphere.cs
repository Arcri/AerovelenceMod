using System;
using System.Collections.Generic;
using AerovelenceMod.Content.Biomes;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace AerovelenceMod.Backgrounds
{
    public partial class CrystalRainAtmosphere : ModSystem
    {
        private const int ChargeDuration = 110;
        private const int BoltDuration = 32;
        private readonly List<Drop> drops = new();
        private LightningUtils.LightningData bolt;
        private Vector2 strikeEnd;
        private Vector2 previousCenter;
        private float intensity;
        private int strikeDelay;
        private int distantDelay;
        private int strikeAge;

        public static bool IsRainActive => !Main.dedServ && !Main.gameMenu && Main.raining
            && Main.LocalPlayer.active && !Main.LocalPlayer.dead
            && (Main.LocalPlayer.ZoneOverworldHeight || Main.LocalPlayer.ZoneSkyHeight)
            && Main.LocalPlayer.InModBiome<CrystalCavernsSurfaceBiome>();

        private struct Drop
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public float Depth;
        }

        public override void OnWorldLoad() => Clear();
        public override void OnWorldUnload() => Clear();

        private void Clear()
        {
            ResetFog();
            drops.Clear();
            bolt = null;
            intensity = 0f;
            strikeAge = 0;
            strikeDelay = 0;
            distantDelay = 0;
            previousCenter = Vector2.Zero;
        }

        public override void PostUpdateEverything()
        {
            if (Main.dedServ || Main.gameMenu)
                return;
            Player player = Main.LocalPlayer;
            if (Vector2.DistanceSquared(previousCenter, player.Center) > 1600f * 1600f)
                Clear();
            previousCenter = player.Center;
            UpdateFog();
            bool active = IsRainActive;
            intensity = MathHelper.Clamp(intensity + (active ? 0.006f : -0.025f), 0f, 1f);
            if (!active)
            {
                bolt = null;
                strikeDelay = 0;
                distantDelay = 0;
            }
            else
            {
                if (strikeDelay == 0)
                    strikeDelay = NextStrikeDelay();
                if (--strikeDelay <= 0)
                {
                    TryStartStrike();
                    strikeDelay = NextStrikeDelay();
                }
                if (distantDelay == 0)
                    distantDelay = Main.rand.Next(720, 1500);
                if (--distantDelay <= 0)
                {
                    Main.NewLightning();
                    distantDelay = Main.rand.Next(720, 1500);
                }
                UpdateStrike();
                if (drops.Count < 180)
                    SpawnDrop();
            }

            for (int i = drops.Count - 1; i >= 0; i--)
            {
                Drop drop = drops[i];
                drop.Position += drop.Velocity;
                bool hit = !OpenAir(drop.Position);
                if (hit || intensity == 0f || drop.Position.Y > Main.screenPosition.Y + Main.screenHeight + 100f)
                {
                    if (hit && active && Main.rand.NextBool(5))
                    {
                        Dust splash = Dust.NewDustPerfect(drop.Position - drop.Velocity, DustID.Water,
                            new Vector2(Main.rand.NextFloat(-0.8f, 0.8f), -0.7f), 160, default, 0.55f);
                        splash.noGravity = false;
                    }
                    drops.RemoveAt(i);
                }
                else
                    drops[i] = drop;
            }
        }

        private static int NextStrikeDelay() => Main.hardMode
            ? Main.rand.Next(25 * 60, 50 * 60) : Main.rand.Next(45 * 60, 90 * 60);

        private static bool OpenAir(Vector2 position)
        {
            int x = (int)(position.X / 16f);
            int y = (int)(position.Y / 16f);
            if (!WorldGen.InWorld(x, y, 10) || y >= Main.worldSurface)
                return false;
            Tile tile = Main.tile[x, y];
            return tile.WallType == WallID.None && tile.LiquidAmount < 100 && !WorldGen.SolidTile(x, y);
        }

        private static bool Exposed(Vector2 position)
        {
            if (!OpenAir(position))
                return false;
            int x = (int)(position.X / 16f);
            for (int y = (int)(position.Y / 16f) - 1; y > 10; y--)
                if (WorldGen.SolidTile(x, y))
                    return false;
            return true;
        }

        private void SpawnDrop()
        {
            Vector2 position = Main.screenPosition + new Vector2(
                Main.rand.NextFloat(-100f, Main.screenWidth + 100f), Main.rand.NextFloat(-80f, Main.screenHeight));
            if (!Exposed(position))
                return;
            float depth = Main.rand.NextFloat(0.55f, 1f);
            drops.Add(new Drop { Position = position, Depth = depth,
                Velocity = new Vector2(Main.windSpeedCurrent * 3f + 0.5f, 10f) * depth });
        }

        private void TryStartStrike()
        {
            for (int attempt = 0; attempt < 8; attempt++)
            {
                int x = (int)((Main.LocalPlayer.Center.X + Main.rand.NextFloat(280f, 750f)
                    * (Main.rand.NextBool() ? 1 : -1)) / 16f);
                int top = Math.Max(12, (int)(Main.screenPosition.Y / 16f) - 30);
                int bottom = Math.Min((int)Main.worldSurface - 1,
                    (int)((Main.screenPosition.Y + Main.screenHeight) / 16f));
                for (int y = top; y < bottom; y++)
                {
                    if (!WorldGen.InWorld(x, y, 12))
                        break;
                    if (!WorldGen.SolidTile(x, y))
                        continue;
                    strikeEnd = new Vector2(x * 16f + 8f, y * 16f - 4f);
                    if (!Exposed(strikeEnd))
                        break;
                    bolt = new LightningUtils.LightningData((Projectile)null, LightningUtils.LightningStyle.Static)
                    {
                        MaxSegments = 60, StaticMaxTime = BoltDuration,
                        StartThickness = 5f, EndThickness = 0.7f,
                        CoreColorOverride = new Color(230, 245, 255),
                        OuterColorOverride = new Color(135, 155, 255),
                        DisplacementIntensity = 1.2f
                    };
                    LightningUtils.InitializeBetweenPoints(bolt,
                        strikeEnd + new Vector2(Main.rand.NextFloat(-130f, 130f), -950f),
                        strikeEnd, LightningUtils.LightningStyle.Static);
                    strikeAge = 0;
                    return;
                }
            }
        }

        private void UpdateStrike()
        {
            if (bolt == null)
                return;
            strikeAge++;
            if (strikeAge < ChargeDuration)
            {
                float charge = strikeAge / (float)ChargeDuration;
                if (Main.rand.NextFloat() < 0.12f + charge * charge * 0.7f)
                {
                    Vector2 position = strikeEnd + Main.rand.NextVector2Circular(26f, 12f) - new Vector2(0f, 6f);
                    Dust dust = Dust.NewDustPerfect(position, DustID.Electric,
                        new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), -0.4f - charge),
                        120, new Color(160, 190, 255), 0.35f + charge * 0.5f);
                    dust.noGravity = true;
                }
                Lighting.AddLight(strikeEnd, new Vector3(0.12f, 0.17f, 0.3f) * charge * charge);
                return;
            }
            if (strikeAge == ChargeDuration)
            {
                lightningGlow = 1f;
                StirFog(strikeEnd - new Vector2(0f, 80f), strikeEnd, Vector2.UnitY * 8f, 115f, 1f);
                Main.NewLightning();
                AeroPlayer aero = Main.LocalPlayer.GetModPlayer<AeroPlayer>();
                aero.ScreenShakePower = Math.Max(aero.ScreenShakePower, 1.2f);
            }
            LightningUtils.UpdateSegments(bolt);
            LightningUtils.UpdateBranches(bolt);
            for (int i = 0; i < bolt.MaxSegments; i += 4)
                Lighting.AddLight(bolt.SegmentPositions[i], new Vector3(0.35f, 0.45f, 0.7f) * bolt.Alpha);
            if (strikeAge >= ChargeDuration + BoltDuration)
                bolt = null;
        }

        public override void PostDrawTiles()
        {
            if (Main.dedServ || Main.gameMenu || intensity <= 0f)
                return;
            SpriteBatch batch = Main.spriteBatch;
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (Drop drop in drops)
            {
                Color light = Lighting.GetColor((int)(drop.Position.X / 16f), (int)(drop.Position.Y / 16f));
                batch.Draw(TextureAssets.MagicPixel.Value, drop.Position - Main.screenPosition,
                    new Rectangle(0, 0, 1, 1), light.MultiplyRGB(new Color(170, 190, 235)) * (0.32f * intensity),
                    drop.Velocity.ToRotation(), Vector2.Zero, new Vector2(11f * drop.Depth, 0.8f), SpriteEffects.None, 0f);
            }
            if (bolt != null && strikeAge >= ChargeDuration)
                LightningUtils.DrawTaperedLightning(bolt, batch);
            batch.End();
        }
    }
}
