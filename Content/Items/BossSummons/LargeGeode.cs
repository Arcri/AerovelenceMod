using System;
using System.Collections.Generic;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.BossSummons
{
    public readonly record struct ArenaPlatform(Vector2 Left, Vector2 Right);

    public static class ArenaData
    {
        public const int StructureWidth = 125;
        public const int StructureHeight = 97;
        public const int GatewayOffsetX = 53;
        public const int GatewayOffsetY = 41;

        public static Rectangle TileBounds { get; private set; }
        public static Rectangle WorldBounds { get; private set; }
        public static Vector2 InnerArenaBoundaryLeft { get; private set; }
        public static Vector2 InnerArenaBoundaryRight { get; private set; }
        public static Vector2 OuterArenaBoundaryLeft { get; private set; }
        public static Vector2 OuterArenaBoundaryRight { get; private set; }
        public static Vector2 ArenaCenter { get; private set; }
        public static Vector2[] CrystalPositions { get; private set; } = [];
        public static List<ArenaPlatform> Platforms { get; private set; } = [];
        public static float FloorY { get; private set; }
        public static bool ClearingEncounter { get; private set; }
        public static bool Valid => TileBounds.Width == StructureWidth && TileBounds.Height == StructureHeight;
        private static readonly List<Point> temporaryBarrierTiles = [];

        public static void Reset()
        {
            TileBounds = Rectangle.Empty;
            WorldBounds = Rectangle.Empty;
            InnerArenaBoundaryLeft = Vector2.Zero;
            InnerArenaBoundaryRight = Vector2.Zero;
            OuterArenaBoundaryLeft = Vector2.Zero;
            OuterArenaBoundaryRight = Vector2.Zero;
            ArenaCenter = Vector2.Zero;
            CrystalPositions = [];
            Platforms = [];
            FloorY = 0f;
            ClearingEncounter = false;
            temporaryBarrierTiles.Clear();
        }

        public static bool TryInitializeFromWorld(Vector2 worldPosition, int searchRadius = 90)
        {
            Point center = worldPosition.ToTileCoordinates();
            int gatewayType = ModContent.TileType<CavernGatewayTile>();
            int minX = Math.Max(10, center.X - searchRadius);
            int maxX = Math.Min(Main.maxTilesX - 10, center.X + searchRadius);
            int minY = Math.Max(10, center.Y - searchRadius);
            int maxY = Math.Min(Main.maxTilesY - 10, center.Y + searchRadius);
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!tile.HasTile || tile.TileType != gatewayType)
                        continue;
                    int gatewayLeft = x - tile.TileFrameX / 18;
                    int gatewayTop = y - tile.TileFrameY / 18;
                    Initialize(new Point(gatewayLeft - GatewayOffsetX, gatewayTop - GatewayOffsetY));
                    return Valid;
                }
            }
            return false;
        }

        public static void Initialize(Point structureStart)
        {
            if (structureStart.X < 1 || structureStart.Y < 1 || structureStart.X + StructureWidth >= Main.maxTilesX || structureStart.Y + StructureHeight >= Main.maxTilesY)
            {
                Reset();
                return;
            }
            TileBounds = new Rectangle(structureStart.X, structureStart.Y, StructureWidth, StructureHeight);
            WorldBounds = new Rectangle(structureStart.X * 16, structureStart.Y * 16, StructureWidth * 16, StructureHeight * 16);
            FloorY = (structureStart.Y + 56) * 16f;
            ArenaCenter = new Vector2((structureStart.X + 60.5f) * 16f, FloorY);
            OuterArenaBoundaryLeft = new Vector2((structureStart.X + 8f) * 16f, FloorY);
            OuterArenaBoundaryRight = new Vector2((structureStart.X + 112f) * 16f, FloorY);
            InnerArenaBoundaryLeft = new Vector2((structureStart.X + 18f) * 16f, FloorY);
            InnerArenaBoundaryRight = new Vector2((structureStart.X + 102f) * 16f, FloorY);
            CrystalPositions = [];
            RefreshGeometry();
        }

        public static void RefreshGeometry()
        {
            if (!Valid)
                return;
            FindPlatforms();
            FindCrystalTips();
        }

        private static void FindCrystalTips()
        {
            int crystalType = ModContent.TileType<ArenaCavernCrystalTile>();
            bool[,] visited = new bool[TileBounds.Width, TileBounds.Height];
            List<(Vector2 Tip, int Size)> clusters = [];
            Queue<Point> pending = new();
            int floorTile = (int)(FloorY / 16f) - 4;
            for (int x = TileBounds.Left; x < TileBounds.Right; x++)
            {
                for (int y = TileBounds.Top; y < floorTile; y++)
                {
                    Tile origin = Framing.GetTileSafely(x, y);
                    if (visited[x - TileBounds.Left, y - TileBounds.Top] || !origin.HasTile || origin.IsActuated || origin.TileType != crystalType)
                        continue;
                    pending.Enqueue(new Point(x, y));
                    visited[x - TileBounds.Left, y - TileBounds.Top] = true;
                    int count = 0;
                    int deepestY = -1;
                    float tipX = 0f;
                    int tipCount = 0;
                    while (pending.Count > 0)
                    {
                        Point point = pending.Dequeue();
                        Tile tile = Framing.GetTileSafely(point.X, point.Y);
                        count++;
                        if (point.Y >= deepestY)
                        {
                            if (point.Y > deepestY)
                            {
                                deepestY = point.Y;
                                tipX = 0f;
                                tipCount = 0;
                            }
                            float offsetX = tile.Slope == SlopeType.SlopeUpLeft ? 0f : tile.Slope == SlopeType.SlopeUpRight ? 16f : 8f;
                            tipX += point.X * 16f + offsetX;
                            tipCount++;
                        }
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int nextX = point.X + dx;
                                int nextY = point.Y + dy;
                                if (!TileBounds.Contains(nextX, nextY) || nextY >= floorTile || visited[nextX - TileBounds.Left, nextY - TileBounds.Top])
                                    continue;
                                Tile next = Framing.GetTileSafely(nextX, nextY);
                                if (!next.HasTile || next.IsActuated || next.TileType != crystalType)
                                    continue;
                                visited[nextX - TileBounds.Left, nextY - TileBounds.Top] = true;
                                pending.Enqueue(new Point(nextX, nextY));
                            }
                        }
                    }
                    if (count >= 12 && tipCount > 0)
                        clusters.Add((new Vector2(tipX / tipCount, (deepestY + 1) * 16f), count));
                }
            }
            clusters.Sort((first, second) => second.Size.CompareTo(first.Size));
            if (clusters.Count < 3)
                return;
            Vector2[] tips = [clusters[0].Tip, clusters[1].Tip, clusters[2].Tip];
            Array.Sort(tips, (first, second) => first.X.CompareTo(second.X));
            CrystalPositions = tips;
        }

        public static void FindPlatforms()
        {
            Platforms = [];
            int platformType = ModContent.TileType<GlimmerwoodPlatformTile>();
            for (int y = TileBounds.Top; y < TileBounds.Bottom; y++)
            {
                int runStart = -1;
                for (int x = TileBounds.Left; x <= TileBounds.Right; x++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    bool platform = x < TileBounds.Right && tile.HasTile && tile.TileType == platformType;
                    if (platform && runStart < 0)
                        runStart = x;
                    if ((!platform || x == TileBounds.Right) && runStart >= 0)
                    {
                        int runEnd = x - 1;
                        if (runEnd - runStart + 1 >= 4)
                            Platforms.Add(new ArenaPlatform(new Vector2(runStart * 16f, y * 16f), new Vector2((runEnd + 1) * 16f, y * 16f)));
                        runStart = -1;
                    }
                }
            }
        }

        public static float FindGroundWorldY(float worldX, float startWorldY)
        {
            if (!Valid)
                return startWorldY;
            int x = Math.Clamp((int)(worldX / 16f), TileBounds.Left, TileBounds.Right - 1);
            int startY = Math.Clamp((int)(startWorldY / 16f), TileBounds.Top, TileBounds.Bottom - 1);
            for (int y = startY; y < TileBounds.Bottom; y++)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && !tile.IsActuated && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                {
                    float groundY = y * 16f;
                    float tileOffsetX = Math.Clamp(worldX - x * 16f, 0f, 16f);
                    if (tile.IsHalfBlock)
                        groundY += 8f;
                    else if (tile.Slope == SlopeType.SlopeDownRight)
                        groundY += 16f - tileOffsetX;
                    else if (tile.Slope == SlopeType.SlopeDownLeft)
                        groundY += tileOffsetX;
                    return groundY;
                }
            }
            return FloorY;
        }

        public static Vector2 ClosestCrystal(Vector2 position)
        {
            Vector2 closest = position;
            float closestDistance = float.MaxValue;
            foreach (Vector2 crystal in CrystalPositions)
            {
                float distance = Vector2.DistanceSquared(position, crystal);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = crystal;
                }
            }
            return closest;
        }

        public static void ClearEncounterEntities(bool clearBarriers = true, bool preservePlatforms = false)
        {
            ClearingEncounter = true;
            int budType = ModContent.NPCType<TumblerCrystalBud>();
            int crystalType = ModContent.NPCType<TumblerConductiveCrystal>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && (npc.type == budType || npc.type == crystalType))
                {
                    npc.active = false;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, number: i);
                }
            }
            HashSet<int> projectileTypes =
            [
                ModContent.ProjectileType<ElectricBolt>(),
                ModContent.ProjectileType<CrystalShard>(),
                ModContent.ProjectileType<Stalactite>(),
                ModContent.ProjectileType<TumblerSpark>(),
                ModContent.ProjectileType<TumblerStar>(),
                ModContent.ProjectileType<TumblerAimLine>(),
                ModContent.ProjectileType<TumblerLightningBolt>(),
                ModContent.ProjectileType<TumblerConductiveField>(),
                ModContent.ProjectileType<TumblerPlatformField>(),
                ModContent.ProjectileType<TumblerKnifeCrystal>(),
                ModContent.ProjectileType<TumblerMagneticRock>(),
                ModContent.ProjectileType<TumblerChargeBall>(),
                ModContent.ProjectileType<TumblerKnifeBall>(),
                ModContent.ProjectileType<TumblerChargedKnifeBall>(),
                ModContent.ProjectileType<TumblerBossAura>(),
                ModContent.ProjectileType<TumblerAuraPulse>(),
                ModContent.ProjectileType<TumblerFilamentRamp>(),
                ModContent.ProjectileType<TumblerPylonField>(),
                ModContent.ProjectileType<TumblerLoopRail>(),
                ModContent.ProjectileType<TumblerResidualField>(),
                ModContent.ProjectileType<TumblerRazeBeam>(),
                ModContent.ProjectileType<TumblerConvergenceOrb>(),
                ModContent.ProjectileType<TumblerArenaGate>(),
                ModContent.ProjectileType<TumblerMagneticPlatform>()
            ];
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectileTypes.Contains(projectile.type))
                {
                    if (!clearBarriers && projectile.type == ModContent.ProjectileType<TumblerArenaGate>())
                        continue;
                    if (preservePlatforms && projectile.type == ModContent.ProjectileType<TumblerMagneticPlatform>())
                        continue;
                    if (projectile.TryGetGlobalProjectile(out TumblerSharedProjectile shared) && (!shared.FromEncounter || projectile.friendly))
                        continue;
                    projectile.Kill();
                }
            }
            if (clearBarriers)
                ClearTemporaryBarriers();
            ClearingEncounter = false;
        }

        public static void CreateTemporaryBarriers()
        {
            if (!Valid || Main.netMode == NetmodeID.MultiplayerClient)
                return;
            ClearTemporaryBarriers();
            PlaceTemporaryBarrier((int)(OuterArenaBoundaryLeft.X / 16f));
            PlaceTemporaryBarrier((int)(OuterArenaBoundaryRight.X / 16f));
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendTileSquare(-1, TileBounds.Center.X, TileBounds.Top + TileBounds.Height / 2, TileBounds.Width, TileBounds.Height, TileChangeType.None);
        }

        private static void PlaceTemporaryBarrier(int x)
        {
            int floorTile = Math.Clamp((int)(FloorY / 16f), TileBounds.Top, TileBounds.Bottom - 1);
            for (int y = floorTile - 1; y >= TileBounds.Top; y--)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile)
                    break;
                tile.ResetToType(TileID.SapphireGemspark);
                temporaryBarrierTiles.Add(new Point(x, y));
                WorldGen.SquareTileFrame(x, y, true);
            }
        }

        public static void SetGateClosure(int x, float bottom)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            bool changed = false;
            foreach (Point point in temporaryBarrierTiles)
            {
                if (point.X != x)
                    continue;
                Tile tile = Framing.GetTileSafely(point.X, point.Y);
                bool open = (point.Y + 1) * 16f > bottom;
                if (tile.IsActuated == open)
                    continue;
                tile.IsActuated = open;
                changed = true;
            }
            if (changed && Main.netMode == NetmodeID.Server)
                NetMessage.SendTileSquare(-1, x, TileBounds.Top, 1, (int)(FloorY / 16f) - TileBounds.Top + 1);
        }

        public static void ClearTemporaryBarriers()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            HashSet<Point> tilesToClear = [.. temporaryBarrierTiles];
            if (Valid)
            {
                int left = (int)(OuterArenaBoundaryLeft.X / 16f);
                int right = (int)(OuterArenaBoundaryRight.X / 16f);
                int floorTile = Math.Clamp((int)(FloorY / 16f), TileBounds.Top, TileBounds.Bottom - 1);
                for (int y = TileBounds.Top; y < floorTile; y++)
                {
                    tilesToClear.Add(new Point(left, y));
                    tilesToClear.Add(new Point(right, y));
                }
            }
            bool changed = false;
            foreach (Point point in tilesToClear)
            {
                Tile tile = Framing.GetTileSafely(point.X, point.Y);
                if (tile.HasTile && tile.TileType == TileID.SapphireGemspark)
                {
                    tile.ClearTile();
                    WorldGen.SquareTileFrame(point.X, point.Y, true);
                    changed = true;
                }
            }
            if (changed && Main.netMode == NetmodeID.Server)
                NetMessage.SendTileSquare(-1, TileBounds.Center.X, TileBounds.Top + TileBounds.Height / 2, TileBounds.Width, TileBounds.Height, TileChangeType.None);
            temporaryBarrierTiles.Clear();
        }
    }

    public class LargeGeode : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("LargeGeode", "Not consumable\nSummons the Crystal Tumbler\nOnly usable in the Caverns Arena")
                .AddName(Language.Default, "Large Geode").AddTooltip(Language.Default, "Not consumable\nSummons the Crystal Tumbler\nOnly usable in the Caverns Arena")
                .AddName(Language.Spanish, "Geoda Grande").AddTooltip(Language.Spanish, "No consumible\nInvoca al Rodador de Cristal\nSolo usable en la Arena de las Cavernas")
                .AddName(Language.French, "Grande Géode").AddTooltip(Language.French, "Non consommable\nInvoque le Rouleur de Cristal\nUtilisable uniquement dans l'Arène des Cavernes")
                .AddName(Language.German, "Große Geode").AddTooltip(Language.German, "Nicht verbrauchbar\nBeschwört den Kristall-Tumbler\nNur in der Höhlenarena verwendbar")
                .AddName(Language.Italian, "Grande Geode").AddTooltip(Language.Italian, "Non consumabile\nEvoca il Rullo di Cristallo\nUtilizzabile solo nell'Arena delle Caverne")
                .AddName(Language.Russian, "Большая Геода").AddTooltip(Language.Russian, "Не расходуется\nПризывает Кристального Тумблера\nМожно использовать только в Пещерной Арене");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ItemRarityID.Orange;
        }

        public override bool CanUseItem(Player player)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<CrystalTumbler>()))
                return false;
            if (!ArenaData.Valid && !ArenaData.TryInitializeFromWorld(player.Center))
                return false;
            return ArenaData.WorldBounds.Contains(player.Center.ToPoint());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return true;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<CrystalTumbler>());
                return true;
            }
            ArenaData.ClearEncounterEntities();
            int index = NPC.NewNPC(player.GetSource_ItemUse(Item), (int)ArenaData.ArenaCenter.X, (int)(ArenaData.FloorY - 90f), ModContent.NPCType<CrystalTumbler>());
            if (index >= 0 && index < Main.maxNPCs)
                Main.npc[index].netUpdate = true;
            return true;
        }
    }
}
