using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace AerovelenceMod.Backgrounds
{
    public partial class CrystalRainAtmosphere
    {
        private const int FogCellSize = 16;
        private Asset<Effect> fogEffect;
        private Texture2D fogNoise;
        private Texture2D fogLighting;
        private Texture2D fogTerrain;
        private Texture2D fogFlow;
        private RenderTarget2D fogTarget;
        private bool fogTargetReady;
        private Color[] lightingPixels;
        private Color[] terrainPixels;
        private Color[] flowPixels;
        private FlowCell[] flowCells;
        private FlowCell[] flowScratch;
        private Vector2[] terrainScratch;
        private static readonly float[] TerrainWeights = [1f, 4f, 7f, 10f, 12f, 10f, 7f, 4f, 1f];
        private int mapWidth;
        private int mapHeight;
        private Point mapTileOrigin;
        private ulong terrainTick;
        private ulong flowTick = ulong.MaxValue;
        private float fogTime;
        private Vector2 windOffset;
        private float lightningGlow;

        private struct FlowCell
        {
            public Vector2 Velocity;
            public float Clearing;
            public float Curl;
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;
            fogEffect = ModContent.Request<Effect>("AerovelenceMod/Assets/Shaders/CrystalRainFog", AssetRequestMode.AsyncLoad);
            On_Main.DrawDust += DrawFogOverWorld;
            On_Main.CheckMonoliths += RenderFog;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;
            On_Main.DrawDust -= DrawFogOverWorld;
            On_Main.CheckMonoliths -= RenderFog;
            Texture2D noise = fogNoise;
            Texture2D lighting = fogLighting;
            Texture2D terrain = fogTerrain;
            Texture2D flow = fogFlow;
            RenderTarget2D target = fogTarget;
            Main.QueueMainThreadAction(() =>
            {
                noise?.Dispose();
                lighting?.Dispose();
                terrain?.Dispose();
                flow?.Dispose();
                target?.Dispose();
            });
            fogNoise = null;
            fogLighting = null;
            fogTerrain = null;
            fogFlow = null;
            fogTarget = null;
            fogEffect = null;
            lightingPixels = null;
            terrainPixels = null;
            flowPixels = null;
            flowCells = null;
            flowScratch = null;
            terrainScratch = null;
        }

        private void ResetFog()
        {
            if (flowCells != null)
                Array.Clear(flowCells);
            if (flowScratch != null)
                Array.Clear(flowScratch);
            terrainTick = 0;
            flowTick = ulong.MaxValue;
            lightningGlow = 0f;
            fogTargetReady = false;
        }

        private void UpdateFog()
        {
            fogTime += 1f / 60f;
            windOffset += new Vector2(-0.17f - Main.windSpeedCurrent * 0.32f, 0.025f);
            lightningGlow *= 0.91f;
        }

        private void DrawFogOverWorld(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (!CanDrawFog() || !fogTargetReady || fogTarget == null || fogTarget.IsDisposed)
                return;

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            Main.spriteBatch.Draw(fogTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            Main.spriteBatch.End();
        }

        private bool CanDrawFog() => !Main.dedServ && !Main.gameMenu && intensity > 0.001f
            && !Main.mapFullscreen && !(CaptureManager.Instance?.IsCapturing ?? false);

        private void RenderFog(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            fogTargetReady = false;
            if (!CanDrawFog())
                return;

            Matrix inverse = Matrix.Invert(Main.GameViewMatrix.TransformationMatrix);
            Vector2 topLeft = Vector2.Transform(Vector2.Zero, inverse) + Main.screenPosition;
            Vector2 topRight = Vector2.Transform(new Vector2(Main.screenWidth, 0f), inverse) + Main.screenPosition;
            Vector2 bottomLeft = Vector2.Transform(new Vector2(0f, Main.screenHeight), inverse) + Main.screenPosition;
            Vector2 bottomRight = topRight + bottomLeft - topLeft;
            Vector2 minimum = Vector2.Min(Vector2.Min(topLeft, topRight), Vector2.Min(bottomLeft, bottomRight));
            Vector2 maximum = Vector2.Max(Vector2.Max(topLeft, topRight), Vector2.Max(bottomLeft, bottomRight));
            PrepareFogMaps(minimum, maximum);

            Effect effect = fogEffect.Value;
            effect.Parameters["LightTexture"].SetValue(fogLighting);
            effect.Parameters["TerrainTexture"].SetValue(fogTerrain);
            effect.Parameters["FlowTexture"].SetValue(fogFlow);
            effect.Parameters["WorldOrigin"].SetValue(topLeft);
            effect.Parameters["WorldAxisX"].SetValue(topRight - topLeft);
            effect.Parameters["WorldAxisY"].SetValue(bottomLeft - topLeft);
            effect.Parameters["MapOrigin"].SetValue(mapTileOrigin.ToVector2() * FogCellSize);
            effect.Parameters["MapSize"].SetValue(new Vector2(mapWidth, mapHeight) * FogCellSize);
            effect.Parameters["WindOffset"].SetValue(windOffset);
            effect.Parameters["Time"].SetValue(fogTime);
            effect.Parameters["Intensity"].SetValue(intensity);
            effect.Parameters["Daylight"].SetValue(Main.dayTime ? 0.72f : 0.08f);
            effect.Parameters["Lightning"].SetValue(Math.Max(lightningGlow, MathHelper.Clamp(Main.lightning, 0f, 1f)));

            GraphicsDevice graphics = Main.instance.GraphicsDevice;
            Texture oldLight = graphics.Textures[1];
            Texture oldTerrain = graphics.Textures[2];
            Texture oldFlow = graphics.Textures[3];
            SamplerState oldSampler0 = graphics.SamplerStates[0];
            SamplerState oldSampler1 = graphics.SamplerStates[1];
            SamplerState oldSampler2 = graphics.SamplerStates[2];
            SamplerState oldSampler3 = graphics.SamplerStates[3];
            RenderTargetBinding[] oldTargets = graphics.GetRenderTargets();
            Viewport oldViewport = graphics.Viewport;
            int reduction = Math.Max(2, (int)Math.Ceiling(Main.screenWidth / 1280f));
            int targetWidth = Math.Max(1, Main.screenWidth / reduction);
            int targetHeight = Math.Max(1, Main.screenHeight / reduction);
            if (fogTarget == null || fogTarget.IsDisposed || fogTarget.Width != targetWidth || fogTarget.Height != targetHeight)
            {
                fogTarget?.Dispose();
                fogTarget = new RenderTarget2D(graphics, targetWidth, targetHeight, false,
                    SurfaceFormat.Color, DepthFormat.None);
            }
            try
            {
                graphics.SetRenderTarget(fogTarget);
                graphics.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap,
                    DepthStencilState.None, RasterizerState.CullNone, effect, Matrix.Identity);
                Main.spriteBatch.Draw(fogNoise, new Rectangle(0, 0, targetWidth, targetHeight), Color.White);
                Main.spriteBatch.End();
                fogTargetReady = true;
            }
            finally
            {
                graphics.SetRenderTargets(oldTargets);
                graphics.Viewport = oldViewport;
                graphics.Textures[1] = oldLight;
                graphics.Textures[2] = oldTerrain;
                graphics.Textures[3] = oldFlow;
                graphics.SamplerStates[0] = oldSampler0;
                graphics.SamplerStates[1] = oldSampler1;
                graphics.SamplerStates[2] = oldSampler2;
                graphics.SamplerStates[3] = oldSampler3;
            }
        }

        private void PrepareFogMaps(Vector2 minimum, Vector2 maximum)
        {
            GraphicsDevice graphics = Main.instance.GraphicsDevice;
            if (fogNoise == null || fogNoise.IsDisposed)
                CreateFogNoise(graphics);

            Point origin = new((int)Math.Floor(minimum.X / FogCellSize) - 10,
                (int)Math.Floor(minimum.Y / FogCellSize) - 10);
            int width = (int)Math.Ceiling((maximum.X - minimum.X) / FogCellSize) + 22;
            int height = (int)Math.Ceiling((maximum.Y - minimum.Y) / FogCellSize) + 22;
            bool resized = width != mapWidth || height != mapHeight || fogLighting == null || fogLighting.IsDisposed;
            if (resized)
            {
                fogLighting?.Dispose();
                fogTerrain?.Dispose();
                fogFlow?.Dispose();
                mapWidth = width;
                mapHeight = height;
                fogLighting = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
                fogTerrain = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
                fogFlow = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
                lightingPixels = new Color[width * height];
                terrainPixels = new Color[width * height];
                flowPixels = new Color[width * height];
                flowCells = new FlowCell[width * height];
                flowScratch = new FlowCell[width * height];
                terrainScratch = new Vector2[width * height];
                mapTileOrigin = origin;
                flowTick = ulong.MaxValue;
            }

            bool shifted = origin != mapTileOrigin;
            if (shifted)
            {
                ShiftFlow(origin.X - mapTileOrigin.X, origin.Y - mapTileOrigin.Y);
                mapTileOrigin = origin;
            }
            if (resized || shifted || terrainTick == 0 || Main.GameUpdateCount - terrainTick >= 6)
            {
                BuildTerrainMap();
                fogTerrain.SetData(terrainPixels);
                terrainTick = Main.GameUpdateCount;
            }

            if (flowTick != Main.GameUpdateCount || resized || shifted)
            {
                if (!Main.gamePaused && flowTick != Main.GameUpdateCount)
                {
                    AdvectFlow();
                    StirEntities();
                }
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        int index = x + y * mapWidth;
                        int tileX = mapTileOrigin.X + x;
                        int tileY = mapTileOrigin.Y + y;
                        lightingPixels[index] = WorldGen.InWorld(tileX, tileY, 1)
                            ? Lighting.GetColor(tileX, tileY) : Color.Black;
                        FlowCell cell = flowCells[index];
                        flowPixels[index] = new Color(
                            MathHelper.Clamp(0.5f + cell.Velocity.X / 32f, 0f, 1f),
                            MathHelper.Clamp(0.5f + cell.Velocity.Y / 32f, 0f, 1f),
                            MathHelper.Clamp(cell.Clearing, 0f, 1f), MathHelper.Clamp(cell.Curl, 0f, 1f));
                    }
                }
                fogLighting.SetData(lightingPixels);
                fogFlow.SetData(flowPixels);
                flowTick = Main.GameUpdateCount;
            }
        }

        private void BuildTerrainMap()
        {
            int deepest = Math.Min(Main.maxTilesY - 12, (int)Main.worldSurface + 64);
            for (int x = 0; x < mapWidth; x++)
            {
                int tileX = mapTileOrigin.X + x;
                int ground = deepest + 40;
                int bottom = Math.Min(deepest, mapTileOrigin.Y + mapHeight + 24);
                for (int tileY = bottom; tileY >= mapTileOrigin.Y; tileY--)
                {
                    bool valid = WorldGen.InWorld(tileX, tileY, 10);
                    bool solid = valid && WorldGen.SolidOrSlopedTile(tileX, tileY);
                    bool water = valid && Main.tile[tileX, tileY].LiquidAmount > 100;
                    if (solid || water)
                        ground = tileY;
                    int row = tileY - mapTileOrigin.Y;
                    if (row < 0 || row >= mapHeight)
                        continue;
                    int index = x + row * mapWidth;
                    bool noWall = valid && Main.tile[tileX, tileY].WallType == WallID.None;
                    float depthFade = 1f - Utils.GetLerpValue((float)Main.worldSurface + 8f,
                        (float)Main.worldSurface + 40f, tileY, true);
                    float exposure = noWall && !solid && !water ? depthFade : 0f;
                    float nearGround = (float)Math.Exp(-Math.Max(0, ground - tileY) * FogCellSize / 145f);
                    terrainPixels[index] = new Color(exposure, nearGround, solid || water ? 0f : 1f,
                        noWall && !solid && !water ? depthFade : 0f);
                }
                for (int y = Math.Max(0, bottom - mapTileOrigin.Y + 1); y < mapHeight; y++)
                    terrainPixels[x + y * mapWidth] = Color.Transparent;
            }

            SoftenTerrain(1, 0);
            SoftenTerrain(0, 1);
        }

        private void SoftenTerrain(int stepX, int stepY)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float exposure = 0f;
                    float ground = 0f;
                    float groundWeight = 0f;
                    for (int tap = -4; tap <= 4; tap++)
                    {
                        int sampleX = Math.Clamp(x + stepX * tap, 0, mapWidth - 1);
                        int sampleY = Math.Clamp(y + stepY * tap, 0, mapHeight - 1);
                        Color sample = terrainPixels[sampleX + sampleY * mapWidth];
                        float weight = TerrainWeights[tap + 4];
                        exposure += sample.R * weight;
                        float airWeight = weight * sample.B / 255f;
                        ground += sample.G * airWeight;
                        groundWeight += airWeight;
                    }
                    int index = x + y * mapWidth;
                    terrainScratch[index] = new Vector2(exposure / 56f,
                        groundWeight > 0f ? ground / groundWeight : terrainPixels[index].G);
                }
            }
            for (int i = 0; i < terrainPixels.Length; i++)
            {
                terrainPixels[i].R = (byte)MathHelper.Clamp(terrainScratch[i].X, 0f, 255f);
                terrainPixels[i].G = (byte)MathHelper.Clamp(terrainScratch[i].Y, 0f, 255f);
            }
        }

        private void ShiftFlow(int offsetX, int offsetY)
        {
            Array.Clear(flowScratch);
            for (int y = 0; y < mapHeight; y++)
                for (int x = 0; x < mapWidth; x++)
                {
                    int sourceX = x + offsetX;
                    int sourceY = y + offsetY;
                    if (sourceX >= 0 && sourceX < mapWidth && sourceY >= 0 && sourceY < mapHeight)
                        flowScratch[x + y * mapWidth] = flowCells[sourceX + sourceY * mapWidth];
                }
            (flowCells, flowScratch) = (flowScratch, flowCells);
        }

        private void AdvectFlow()
        {
            Array.Clear(flowScratch);
            for (int y = 1; y < mapHeight - 1; y++)
            {
                for (int x = 1; x < mapWidth - 1; x++)
                {
                    int index = x + y * mapWidth;
                    if (terrainPixels[index].A == 0 || terrainPixels[index].R == 0)
                        continue;
                    FlowCell current = flowCells[index];
                    Vector2 velocity = current.Velocity;
                    float backX = MathHelper.Clamp(x - (velocity.X + 0.2f + Main.windSpeedCurrent * 0.3f) / FogCellSize, 0f, mapWidth - 1.001f);
                    float backY = MathHelper.Clamp(y - velocity.Y / FogCellSize, 0f, mapHeight - 1.001f);
                    int sampleX = (int)backX;
                    int sampleY = (int)backY;
                    float blendX = backX - sampleX;
                    float blendY = backY - sampleY;
                    FlowCell above = BlendFlow(flowCells[sampleX + sampleY * mapWidth],
                        flowCells[sampleX + 1 + sampleY * mapWidth], blendX);
                    FlowCell below = BlendFlow(flowCells[sampleX + (sampleY + 1) * mapWidth],
                        flowCells[sampleX + 1 + (sampleY + 1) * mapWidth], blendX);
                    FlowCell advected = BlendFlow(above, below, blendY);
                    Vector2 adjacent = (flowCells[index - 1].Velocity + flowCells[index + 1].Velocity
                        + flowCells[index - mapWidth].Velocity + flowCells[index + mapWidth].Velocity) * 0.25f;
                    advected.Velocity = Vector2.Lerp(advected.Velocity, adjacent, 0.09f) * 0.979f;
                    advected.Clearing *= 0.984f;
                    advected.Curl *= 0.972f;
                    flowScratch[index] = advected;
                }
            }
            (flowCells, flowScratch) = (flowScratch, flowCells);
        }

        private static FlowCell BlendFlow(FlowCell first, FlowCell second, float blend) => new()
        {
            Velocity = Vector2.Lerp(first.Velocity, second.Velocity, blend),
            Clearing = MathHelper.Lerp(first.Clearing, second.Clearing, blend),
            Curl = MathHelper.Lerp(first.Curl, second.Curl, blend)
        };

        private void StirEntities()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead && player.velocity.LengthSquared() > 0.5f)
                    StirFog(player.Center - player.velocity, player.Center, player.velocity, 32f, 0.46f);
            }
            int budget = 48;
            for (int i = 0; i < Main.maxProjectiles && budget > 0; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active || projectile.velocity.LengthSquared() < 1f || !InsideFogMap(projectile.Center, 120f))
                    continue;
                Vector2 velocity = projectile.velocity * Math.Min(projectile.extraUpdates + 1, 4);
                float speed = velocity.Length();
                float radius = MathHelper.Clamp(Math.Max(projectile.width, projectile.height) * 0.3f + speed * 0.8f, 18f, 68f);
                Vector2 travel = velocity * Math.Min(1f, 120f / Math.Max(1f, speed));
                StirFog(projectile.Center - travel, projectile.Center, velocity, radius, MathHelper.Clamp(speed * 0.07f, 0.25f, 0.95f));
                budget--;
            }
            budget = 12;
            for (int i = 0; i < Main.maxNPCs && budget > 0; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.velocity.LengthSquared() < 2f || !InsideFogMap(npc.Center, 100f))
                    continue;
                StirFog(npc.Center - npc.velocity, npc.Center, npc.velocity,
                    MathHelper.Clamp(npc.width * 0.45f, 22f, 64f), 0.35f);
                budget--;
            }
        }

        private bool InsideFogMap(Vector2 position, float margin)
        {
            Vector2 local = position - mapTileOrigin.ToVector2() * FogCellSize;
            return local.X >= -margin && local.Y >= -margin
                && local.X < mapWidth * FogCellSize + margin && local.Y < mapHeight * FogCellSize + margin;
        }

        private void StirFog(Vector2 start, Vector2 end, Vector2 velocity, float radius, float strength)
        {
            if (flowCells == null || !InsideFogMap(end, radius))
                return;
            Vector2 origin = mapTileOrigin.ToVector2() * FogCellSize;
            Vector2 minimum = Vector2.Min(start, end) - origin - new Vector2(radius * 1.65f);
            Vector2 maximum = Vector2.Max(start, end) - origin + new Vector2(radius * 1.65f);
            int left = Math.Clamp((int)(minimum.X / FogCellSize), 1, mapWidth - 2);
            int right = Math.Clamp((int)(maximum.X / FogCellSize) + 1, 1, mapWidth - 2);
            int top = Math.Clamp((int)(minimum.Y / FogCellSize), 1, mapHeight - 2);
            int bottom = Math.Clamp((int)(maximum.Y / FogCellSize) + 1, 1, mapHeight - 2);
            Vector2 line = end - start;
            float lengthSquared = Math.Max(1f, line.LengthSquared());
            Vector2 direction = velocity.SafeNormalize(Vector2.UnitX);
            Vector2 perpendicular = new(-direction.Y, direction.X);
            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    int index = x + y * mapWidth;
                    if (terrainPixels[index].R == 0 || terrainPixels[index].A == 0)
                        continue;
                    Vector2 position = origin + new Vector2(x + 0.5f, y + 0.5f) * FogCellSize;
                    float along = MathHelper.Clamp(Vector2.Dot(position - start, line) / lengthSquared, 0f, 1f);
                    Vector2 offset = position - (start + line * along);
                    float distance = offset.Length() / radius;
                    if (distance > 1.65f)
                        continue;
                    float channel = (float)Math.Exp(-distance * distance * 3f) * strength;
                    float rim = (float)Math.Exp(-MathF.Pow((distance - 0.98f) * 3.3f, 2f)) * strength;
                    float side = Math.Sign(Vector2.Dot(offset, perpendicular));
                    ref FlowCell cell = ref flowCells[index];
                    cell.Velocity += direction * channel * 1.6f + perpendicular * side * rim * 1.15f;
                    float speed = cell.Velocity.Length();
                    if (speed > 13f)
                        cell.Velocity *= 13f / speed;
                    cell.Clearing = Math.Max(cell.Clearing, channel);
                    cell.Curl = Math.Max(cell.Curl, rim * 0.8f);
                }
            }
        }

        private void CreateFogNoise(GraphicsDevice graphics)
        {
            const int size = 256;
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    pixels[x + y * size] = new Color(NoiseChannel(x, y, 17), NoiseChannel(x, y, 89),
                        NoiseChannel(x, y, 157), NoiseChannel(x, y, 241));
            fogNoise = new Texture2D(graphics, size, size, false, SurfaceFormat.Color);
            fogNoise.SetData(pixels);
        }

        private static float NoiseChannel(int x, int y, int seed)
        {
            float result = 0f;
            float amplitude = 0.57f;
            float total = 0f;
            for (int octave = 0; octave < 4; octave++)
            {
                int period = 4 << octave;
                float sampleX = x * period / 256f;
                float sampleY = y * period / 256f;
                int cellX = (int)sampleX;
                int cellY = (int)sampleY;
                float fractionX = sampleX - cellX;
                float fractionY = sampleY - cellY;
                fractionX = fractionX * fractionX * (3f - 2f * fractionX);
                fractionY = fractionY * fractionY * (3f - 2f * fractionY);
                float top = MathHelper.Lerp(NoiseHash(cellX % period, cellY % period, seed + octave * 7),
                    NoiseHash((cellX + 1) % period, cellY % period, seed + octave * 7), fractionX);
                float bottom = MathHelper.Lerp(NoiseHash(cellX % period, (cellY + 1) % period, seed + octave * 7),
                    NoiseHash((cellX + 1) % period, (cellY + 1) % period, seed + octave * 7), fractionX);
                result += MathHelper.Lerp(top, bottom, fractionY) * amplitude;
                total += amplitude;
                amplitude *= 0.47f;
            }
            return MathHelper.Clamp((result / total - 0.5f) * 1.35f + 0.5f, 0f, 1f);
        }

        private static float NoiseHash(int x, int y, int seed)
        {
            uint value = unchecked((uint)(x * 374761393 + y * 668265263 + seed * 1442695041));
            value = unchecked((value ^ (value >> 13)) * 1274126177u);
            return ((value ^ (value >> 16)) & 65535) / 65535f;
        }
    }
}
