using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameInput;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;
using AerovelenceMod.Content.Walls.CrystalCaverns.Natural;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture;
using static Terraria.WorldGen;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Building;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using AerovelenceMod.Common.Utilities.Generation.StructureStamper;

namespace AerovelenceMod.Common.Utilities.Generation
{
    public class CaveHousePlayer : ModPlayer
    {
        /*public static bool JustPressed(Keys key)
        {
            return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (JustPressed(Keys.D1))
            {
                int tileX = (int)(Player.position.X / 16f);
                int tileY = (int)(Player.position.Y / 16f);
                HouseGenerator.GenerateCaveHouse(tileX, tileY);
            }
        }*/
    }

    public static class HouseGenerator
    {
        private struct HouseInfo
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;

            public Rectangle ToRectangle()
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }

        #region Loot Pool Configuration
        public class PrimaryItemConfiguration
        {
            public int ItemID;
            public int MinAmount;
            public int MaxAmount;
            public float Chance;
            public PrimaryItemConfiguration(int itemID, int min, int max, float chance)
            {
                ItemID = itemID;
                MinAmount = min;
                MaxAmount = max;
                Chance = chance;
            }
        }
        public class ItemConfiguration
        {
            public List<int> ItemIDs;
            public int MinAmount;
            public int MaxAmount;
            public ItemConfiguration(int itemID, int min, int max)
            {
                ItemIDs = new List<int>() { itemID };
                MinAmount = min;
                MaxAmount = max;
            }
            public ItemConfiguration(List<int> itemIDs, int min, int max)
            {
                ItemIDs = itemIDs;
                MinAmount = min;
                MaxAmount = max;
            }
        }

        //primary
        private static readonly List<PrimaryItemConfiguration> crystalShrinePrimary = new()
        {
            new PrimaryItemConfiguration(ItemID.BandofRegeneration, 1, 1, 1f),
            new PrimaryItemConfiguration(ItemID.MagicMirror, 1, 1, 1f),
            new PrimaryItemConfiguration(ItemID.CloudinaBottle, 1, 1, 1f),
            new PrimaryItemConfiguration(ItemID.HermesBoots, 1, 1, 1f),
            new PrimaryItemConfiguration(ItemID.EnchantedBoomerang, 1, 1, 1f),
            new PrimaryItemConfiguration(ItemID.ShoeSpikes, 1, 1, 1f),
            new PrimaryItemConfiguration(ItemID.FlareGun, 1, 1, 1f),
            new PrimaryItemConfiguration(ItemID.Extractinator, 1, 1, 1f),
            new PrimaryItemConfiguration(ItemID.LavaCharm, 1, 1, 1f),
            new PrimaryItemConfiguration(ItemID.LuckyHorseshoe, 1, 1, 1f),
            new PrimaryItemConfiguration(ModContent.ItemType<Eos>(), 1, 1, 1f)
        };

        //secondary
        private static readonly List<ItemConfiguration> crystalShrineSecondary = new()
        {
            new ItemConfiguration(ItemID.SuspiciousLookingEye, 1, 1),
            new ItemConfiguration(ItemID.Dynamite, 1, 1),
            new ItemConfiguration(ItemID.JestersArrow, 25, 50),
            new ItemConfiguration(new List<int> { ItemID.SilverBar, ItemID.TungstenBar, ItemID.GoldBar, ItemID.PlatinumBar }, 3, 10),
            new ItemConfiguration(new List<int> { ItemID.FlamingArrow, ItemID.ThrowingKnife }, 25, 50),
            new ItemConfiguration(ItemID.HealingPotion, 3, 5),
            new ItemConfiguration(new List<int>
            {
                ItemID.SpelunkerPotion, ItemID.FeatherfallPotion, ItemID.NightOwlPotion, ItemID.WaterWalkingPotion,
                ItemID.ArcheryPotion, ItemID.GravitationPotion, ItemID.ThornsPotion, ItemID.InvisibilityPotion,
                ItemID.HunterPotion, ItemID.BattlePotion, ItemID.TeleportationPotion
            }, 1, 2),
            new ItemConfiguration(ItemID.RecallPotion, 1, 2),
            new ItemConfiguration(new List<int> { ItemID.Torch, ItemID.Glowstick }, 15, 29),
            new ItemConfiguration(ItemID.GoldCoin, 1, 2)
        };

        #endregion


        public static void GenerateCaveHouse(int startX, int startY, bool checkIfProtected = false)
        {
            int houseCount = Main.rand.Next(1, 4);
            HouseInfo[] houses = new HouseInfo[houseCount];
            int currentBottom = startY;
            for (int i = 0; i < houseCount; i++)
            {
                int w = Main.rand.Next(20, 31);
                int h = Main.rand.Next(7, 9);
                int randomOffset = Main.rand.Next(-8, 9);

                int bottomRow = currentBottom;
                int topRow = bottomRow - (h - 1);

                houses[i] = new HouseInfo
                {
                    X = startX + randomOffset,
                    Y = topRow,
                    Width = w,
                    Height = h
                };

                currentBottom = topRow;
            }

            if (checkIfProtected)
            {
                Rectangle houseRectangles = Rectangle.Empty;
                foreach (var house in houses)
                {
                    if (AeroStructure.ProtectedStructures.Any(x => x.Intersects(house.ToRectangle())))
                    {
                        return;
                    }
                }
            }

            foreach (var h in houses)
                ClearHouseRegion(h);

            bool connectionDirection = Main.rand.NextBool();
            for (int i = 0; i < houseCount; i++)
            {
                bool withAirGaps = Main.rand.NextBool();
                GenerateRoom(houses[i].X, houses[i].Y, houses[i].Width, houses[i].Height, withAirGaps);
                foreach (var house in houses)
                {
                    PlaceFloorCrystals(house);
                    PlaceCeilingCrystals(house);
                }
                if (i > 0)
                {
                    connectionDirection = !connectionDirection;
                    ConnectHouses(houses[i - 1], houses[i], connectionDirection);


                }
            }

            foreach (var house in houses)
            {
                PlaceCrystalGrowthOnExposed(house, 0.20f);
            }

            foreach (var house in houses)
            {
                PlaceChainLinesInHouse(house);
            }
            ApplyPerlinWallRemoval(houses, 0.5f, 0.2f, Main.rand.Next(2000));
            ReplaceLargeAirBlobsWithCrystalGrassWall(houses, 13);
            PlaceBeamsUnderHouse(houses[0]);


            TryPlaceBookshelf(houses[0]);
            PlaceSingleChestWithPadding(houses);
            PlaceRandomPots(houses);
            foreach (var h in houses)
            {
                PlaceCobwebBlobsNearBorder(h);
            }
            RandomlyRemoveSomeWalls(houses, 0.30f);
            PlaceTopPlatformPassage(houses[houseCount - 1]);
            FrameGeneratedArea(houses);
        }

        #region Crystal Placement

        /// <summary>
        /// Places 1–3 crystal clusters along the floor (bottom edge) of the house.
        /// Each cluster is either a 3×3 or a 2×2 triangle pattern, and the pattern may be flipped horizontally.
        /// </summary>
        private static void PlaceFloorCrystals(HouseInfo house)
        {
            int clusterCount = Main.rand.Next(1, 2);
            for (int i = 0; i < clusterCount; i++)
            {
                bool useLarge = Main.rand.NextBool();
                bool flipHorizontally = Main.rand.NextBool();
                int formationWidth = useLarge ? 3 : 2;
                int minX = house.X + 1;
                int maxX = house.X + house.Width - formationWidth - 1;
                if (maxX < minX) continue;

                int x = Main.rand.Next(minX, maxX + 1);
                int floorY = house.Y + house.Height - 1;

                if (useLarge)
                    PlaceLargeFloorCrystal(x, floorY, flipHorizontally);
               // else
                    //Main.NewText("is");
                //PlaceSmallFloorCrystal(x, floorY, flipHorizontally);
            }
        }

        /// <summary>
        /// Places a 3x3 downward–pointing triangle pattern on the floor.
        /// If flipped, the pattern is mirrored horizontally.
        /// </summary>
        private static void PlaceLargeFloorCrystal(int x, int floorY, bool flip)
        {
            if (!flip)
            {
                ForciblyPlaceTile(x, floorY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, floorY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 2, floorY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, floorY - 1, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 2, floorY - 1, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 2, floorY - 2, ModContent.TileType<CavernCrystalTile>());
            }
            else
            {
                ForciblyPlaceTile(x + 2, floorY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, floorY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x, floorY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, floorY - 1, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x, floorY - 1, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x, floorY - 2, ModContent.TileType<CavernCrystalTile>());
            }
        }

        /// <summary>
        /// Places a 2x2 downward–pointing triangle pattern on the floor.
        /// If flipped, the pattern is mirrored horizontally.
        /// </summary>
        private static void PlaceSmallFloorCrystal(int x, int floorY, bool flip)
        {
            if (!flip)
            {
                ForciblyPlaceTile(x, floorY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, floorY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, floorY - 1, ModContent.TileType<CavernCrystalTile>());
            }
            else
            {
                ForciblyPlaceTile(x + 1, floorY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x, floorY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x, floorY - 1, ModContent.TileType<CavernCrystalTile>());
            }
        }

        /// <summary>
        /// Places 1–3 crystal clusters along the ceiling (top edge) of the house.
        /// This pattern is similar to the floor version, but placed at the ceiling.
        /// </summary>
        private static void PlaceCeilingCrystals(HouseInfo house)
        {
            int clusterCount = Main.rand.Next(1, 2);
            for (int i = 0; i < clusterCount; i++)
            {
                bool useLarge = Main.rand.NextBool();
                bool flipHorizontally = Main.rand.NextBool();

                int formationWidth = useLarge ? 3 : 2;
                int minX = house.X + 1;
                int maxX = house.X + house.Width - formationWidth - 1;
                if (maxX < minX) continue;
                int x = Main.rand.Next(minX, maxX + 1);
                int ceilingY = house.Y;

                if (useLarge)
                    PlaceLargeCeilingCrystal(x, ceilingY, flipHorizontally);
                //else
                   // Main.NewText("is");
                //PlaceSmallCeilingCrystal(x, ceilingY, flipHorizontally);
            }
        }

        /// <summary>
        /// Places a 3x3 upward–pointing triangle pattern on the ceiling.
        /// </summary>
        private static void PlaceLargeCeilingCrystal(int x, int ceilingY, bool flip)
        {
            if (!flip)
            {
                ForciblyPlaceTile(x, ceilingY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, ceilingY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 2, ceilingY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x, ceilingY + 1, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, ceilingY + 1, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x, ceilingY + 2, ModContent.TileType<CavernCrystalTile>());
            }
            else
            {
                ForciblyPlaceTile(x + 2, ceilingY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, ceilingY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x, ceilingY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, ceilingY + 1, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x, ceilingY + 1, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 2, ceilingY + 2, ModContent.TileType<CavernCrystalTile>());
            }
        }

        /// <summary>
        /// Places a 2x2 upward–pointing triangle pattern on the ceiling.
        /// </summary>
        private static void PlaceSmallCeilingCrystal(int x, int ceilingY, bool flip)
        {
            if (!flip)
            {
                ForciblyPlaceTile(x, ceilingY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, ceilingY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x, ceilingY + 1, ModContent.TileType<CavernCrystalTile>());
            }
            else
            {
                ForciblyPlaceTile(x + 1, ceilingY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x, ceilingY, ModContent.TileType<CavernCrystalTile>());
                ForciblyPlaceTile(x + 1, ceilingY + 1, ModContent.TileType<CavernCrystalTile>());
            }
        }

        private static void ForciblyPlaceTile(int x, int y, int tileType)
        {
            KillTile(x, y, false, false, true);
            PlaceTile(x, y, tileType, mute: true, forced: true);
        }

        #endregion

        private static void PlaceCobwebBlobsNearBorder(HouseInfo house)
        {
            int left = house.X + 1;
            int right = house.X + house.Width - 2;
            int top = house.Y + 1;
            int bottom = house.Y + house.Height - 2;
            List<Point> ring = new();

            //top row
            for (int x = left; x <= right; x++)
                ring.Add(new Point(x, top));
            //bottom row
            for (int x = left; x <= right; x++)
                ring.Add(new Point(x, bottom));
            //left col
            for (int y = top; y <= bottom; y++)
                ring.Add(new Point(left, y));
            //right col
            for (int y = top; y <= bottom; y++)
                ring.Add(new Point(right, y));

            //shuffles ring
            ring = [.. ring.OrderBy(_ => Main.rand.Next())];

            //places a few BFS lumps
            int lumps = Main.rand.Next(2, 5); // 2–4 lumps
            for (int i = 0; i < lumps && ring.Count > 0; i++)
            {
                Point start = ring[0];
                ring.RemoveAt(0);
                int size = Main.rand.Next(6, 10);
                PlaceCobwebBlob(house, start.X, start.Y, size);
            }
        }

        /// <summary>
        /// BFS or flood approach to place `count` cobwebs near (startX, startY).
        /// </summary>
        private static void PlaceCobwebBlob(HouseInfo house, int startX, int startY, int count)
        {
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(startX, startY));
            int placed = 0;

            while (queue.Count > 0 && placed < count)
            {
                var p = queue.Dequeue();
                if (!InHouseBounds(p.X, p.Y, house)) continue;
                if (!Main.tile[p.X, p.Y].HasTile)
                {
                    KillTile(p.X, p.Y, false, false, true);
                    PlaceTile(p.X, p.Y, TileID.Cobweb, mute: true, forced: true);
                    placed++;
                    //enqueue neighbors
                    foreach (var n in Get4Neighbors(p.X, p.Y))
                        queue.Enqueue(n);
                }
            }
        }

        private static bool InHouseBounds(int x, int y, HouseInfo h)
        {
            return (x >= h.X && x < h.X + h.Width &&
                    y >= h.Y && y < h.Y + h.Height);
        }

        private static IEnumerable<Point> Get4Neighbors(int x, int y)
        {
            yield return new Point(x + 1, y);
            yield return new Point(x - 1, y);
            yield return new Point(x, y + 1);
            yield return new Point(x, y - 1);
        }


        private static void RandomlyRemoveSomeWalls(HouseInfo[] houses, float removeChance)
        {
            foreach (var h in houses)
            {
                int left = h.X;
                int right = h.X + h.Width - 1;
                int top = h.Y;
                int bottom = h.Y + h.Height - 1;

                for (int x = left; x <= right; x++)
                {
                    for (int y = top; y <= bottom; y++)
                    {
                        if (Main.tile[x, y].WallType != 0 && Main.rand.NextFloat() < removeChance)
                        {
                            Main.tile[x, y].WallType = 0;
                        }
                    }
                }
            }
        }


        private static void ClearHouseRegion(HouseInfo h)
        {
            for (int x = h.X; x <= h.X + h.Width - 1; x++)
            {
                for (int y = h.Y; y <= h.Y + h.Height - 1; y++)
                    Main.tile[x, y].ClearTile();
            }
        }

        #region House Construction
        private static void GenerateRoom(int x, int top, int width, int height, bool withAirGaps)
        {
            int left = x;
            int right = x + width - 1;
            int bottom = top + height - 1;

            for (int i = left; i <= right; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    bool perimeter = (i == left || i == right || j == top || j == bottom);
                    if (perimeter)
                    {
                        KillTile(i, j, false, false, true);
                        Main.tile[i, j].WallType = 0;
                        if (Main.rand.NextFloat() < 0.15f)
                        {
                            PlaceTile(i, j, ModContent.TileType<CrackedCavernBrickTile>(), mute: true, forced: true);
                        }
                        else
                        {
                            PlaceTile(i, j, ModContent.TileType<CavernBrickTile>(), mute: true, forced: true);
                        }
                    }
                    else
                    {
                        Main.tile[i, j].WallType = (ushort)ModContent.WallType<GlimmerwoodPlankedWall>();
                    }
                }
            }
        }

        #endregion

        #region Perlin & BFS for CrystalGrassWall

        private static void ApplyPerlinWallRemoval(HouseInfo[] houses, float threshold, float frequency, int seed)
        {
            (int minX, int maxX, int minY, int maxY) = GetBoundingBox(houses);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    float noise = (SimpleNoise(x, y, frequency, seed) + 1f) / 2f;
                    if (noise < threshold)
                    {
                        Main.tile[x, y].WallType = 0;
                    }
                }
            }
        }

        /// <summary>
        /// BFS pass to find contiguous "wall=0" areas of at least minSize, replace them with CrystalGrassWall
        /// </summary>
        /// <param name="houses"></param>
        /// <param name="minSize"></param>
        private static void ReplaceLargeAirBlobsWithCrystalGrassWall(HouseInfo[] houses, int minSize)
        {
            foreach (var h in houses)
            {
                //BFS only within [h.X, h.X+h.Width-1] × [h.Y, h.Y+h.Height-1].
                bool[,] visited = new bool[h.Width, h.Height];

                for (int x = 0; x < h.Width; x++)
                {
                    for (int y = 0; y < h.Height; y++)
                    {
                        int worldX = h.X + x;
                        int worldY = h.Y + y;

                        if (!visited[x, y] && Main.tile[worldX, worldY].WallType == 0)
                        {
                            //BFS in local coords
                            List<Point> blob = [];
                            Queue<Point> queue = new();
                            queue.Enqueue(new Point(x, y));
                            visited[x, y] = true;
                            while (queue.Count > 0)
                            {
                                var p = queue.Dequeue();
                                blob.Add(p);
                                foreach (var n in GetNeighborsLocal(p.X, p.Y, h.Width, h.Height))
                                {
                                    if (!visited[n.X, n.Y])
                                    {
                                        int wx = h.X + n.X;
                                        int wy = h.Y + n.Y;
                                        if (Main.tile[wx, wy].WallType == 0)
                                        {
                                            visited[n.X, n.Y] = true;
                                            queue.Enqueue(n);
                                        }
                                    }
                                }
                            }
                            if (blob.Count >= minSize)
                            {
                                foreach (var pt in blob)
                                {
                                    int wx = h.X + pt.X;
                                    int wy = h.Y + pt.Y;
                                    Main.tile[wx, wy].WallType = (ushort)ModContent.WallType<CrystalGrassWall>();
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region House Connections (Platforms + Diagonal)

        private static void ConnectHouses(HouseInfo lower, HouseInfo upper, bool connectOnLeft)
        {
            int connectingRow = lower.Y;
            int overlapLeft = Math.Max(lower.X, upper.X);
            int overlapRight = Math.Min(lower.X + lower.Width - 1, upper.X + upper.Width - 1);
            if (overlapRight < overlapLeft) return;

            int platformCount = 6;
            int offsetFromEdge = 1;
            int pxStart = connectOnLeft
                ? overlapLeft + offsetFromEdge
                : overlapRight - offsetFromEdge - (platformCount - 1);

            for (int i = 0; i < platformCount; i++)
            {
                int x = pxStart + i;
                KillTile(x, connectingRow, false, false, true);
                PlaceTile(x, connectingRow, ModContent.TileType<GlimmerwoodPlatformTile>(), mute: true, forced: true);
            }
            int stairX = connectOnLeft ? pxStart : (pxStart + platformCount - 1);
            PlaceDiagonalPlatform(stairX, connectingRow, connectOnLeft, true);
            stairX = connectOnLeft ? pxStart + 1 : (pxStart + platformCount - 2);
            int stepDir = connectOnLeft ? +1 : -1;
            int houseBottom = lower.Y + lower.Height - 1;

            for (int y = connectingRow + 1; y < houseBottom; y++)
            {
                if (stairX < lower.X || stairX > (lower.X + lower.Width - 1)) break;
                PlaceDiagonalPlatform(stairX, y, connectOnLeft, false);
                stairX += stepDir;
            }
        }

        private static void PlaceDiagonalPlatform(int x, int y, bool bottomLeftToTopRight, bool isTop)
        {
            KillTile(x, y, false, false, true);
            PlaceTile(x, y, ModContent.TileType<GlimmerwoodPlatformTile>(), mute: true, forced: true);

            Tile tile = Main.tile[x, y];
            if (tile != null && tile.HasTile && tile.TileType == ModContent.TileType<GlimmerwoodPlatformTile>())
            {
                tile.Slope = bottomLeftToTopRight ? SlopeType.SlopeDownLeft : SlopeType.SlopeDownRight;
                tile.IsHalfBlock = false;
                tile.TileFrameY = 0;
                int frameIndex = isTop
                    ? (bottomLeftToTopRight ? 25 : 26)
                    : (bottomLeftToTopRight ? 8 : 10);
                tile.TileFrameX = (short)(frameIndex * 18);
                SquareTileFrame(x, y, true);
            }
        }

        #endregion

        #region Bookshelf
        private static bool TryPlaceBookshelf(HouseInfo house, int attempts = 50)
        {
            int shelfWidth = 4;
            int shelfHeight = 3;
            int left = house.X + 1;
            int right = house.X + house.Width - 1 - shelfWidth;
            int top = house.Y + 1;
            int bottom = house.Y + house.Height - 1 - shelfHeight;

            if (left > right || top > bottom)
                return false;

            for (int i = 0; i < attempts; i++)
            {
                int x = Main.rand.Next(left, right + 1);
                int y = Main.rand.Next(top, bottom + 1);

                if (CheckMultiTileSpace(x, y, shelfWidth, shelfHeight))
                {
                    int floorY = y + (shelfHeight - 1);
                    if (CheckFloorForMultiTile(x, floorY, shelfWidth))
                    {
                        int style = 0; // e.g. default style
                        WorldGen.PlaceObject(x, y, TileID.Bookcases, false, style);
                        NetMessage.SendObjectPlacement(-1, x, y, TileID.Bookcases, style, 0, -1, -1);
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Single Chest with Padding
        private static void PlaceSingleChestWithPadding(HouseInfo[] houses)
        {
            //Main.NewText("begin", 255, 200, 50);
            var shuffled = houses.OrderBy(_ => Main.rand.Next()).ToList();
            foreach (var house in shuffled)
            {
                if (TryPlaceChestOnHouseFloor(house))
                    return;
            }
            ForceChestInFirstHouse(houses[0]);
        }

        /// <summary>
        /// Attempts to place a chest along the bottom row of the house.
        /// If successful, also configures the chest’s loot.
        /// </summary>
        private static bool TryPlaceChestOnHouseFloor(HouseInfo house)
        {
            int floorY = house.Y + house.Height - 1;
            int left = house.X + 1;
            int right = house.X + house.Width - 3;
            if (left > right)
                return false;
            for (int attempt = 1; attempt <= 50; attempt++)
            {
                int x = Main.rand.Next(left, right + 1);
                if (CheckFloorChestSpot(x, floorY))
                {
                    int chestIndex = WorldGen.PlaceChest(x, floorY - 1, (ushort)ModContent.TileType<CavernChestTile>(), false);
                    if (chestIndex != -1)
                    {
                        ConfigureChestLoot(chestIndex, house);
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Checks if placing a 2-wide chest at (x, y-1) is valid,
        /// meaning (x,y) & (x+1,y) are solid blocks, and (x,y-1) & (x+1,y-1) are empty.
        /// The chest’s bottom-left corner is at (x, y-1).
        /// </summary>
        private static bool CheckFloorChestSpot(int x, int floorY)
        {
            Tile below1 = Main.tile[x, floorY];
            Tile below2 = Main.tile[x + 1, floorY];
            if (!IsSolidBlock(below1) || !IsSolidBlock(below2))
                return false;
            Tile chestTile1 = Main.tile[x, floorY - 1];
            Tile chestTile2 = Main.tile[x + 1, floorY - 1];
            if (chestTile1.HasTile || chestTile2.HasTile)
                return false;

            return true;
        }

        /// <summary>
        /// Forces a chest in the first house and configures its loot.
        /// </summary>
        private static void ForceChestInFirstHouse(HouseInfo house)
        {
            int x = house.X + house.Width / 2;
            int floorY = house.Y + house.Height - 1;
            KillTile(x, floorY - 1, false, false, true);
            KillTile(x + 1, floorY - 1, false, false, true);
            int chestIndex = WorldGen.PlaceChest(x, floorY - 1, TileID.Containers, false, style: 1);
            if (chestIndex != -1)
            {
                ConfigureChestLoot(chestIndex, house);
            }
        }
        #endregion

        #region Loot Generation

        /// <summary>
        /// Configures the loot of a placed chest using a primary pool, secondary pool,
        /// and optionally adds common loot based on the house’s depth.
        /// </summary>
        private static void ConfigureChestLoot(int chestIndex, HouseInfo house)
        {
            Chest chest = Main.chest[chestIndex];
            if (chest == null)
                return;
            for (int i = 0; i < chest.item.Length; i++)
            {
                chest.item[i].TurnToAir();
            }

            //primary loot: Always add one item from the primary pool in slot 0.
            var primaryChoice = crystalShrinePrimary[Main.rand.Next(crystalShrinePrimary.Count)];
            int primaryAmount = Main.rand.Next(primaryChoice.MinAmount, primaryChoice.MaxAmount + 1);
            chest.item[0] = new Item();
            chest.item[0].SetDefaults(primaryChoice.ItemID);
            chest.item[0].stack = primaryAmount;

            //secondary loot: For each secondary item, add it with a 50% chance.
            int chestSlot = 1;
            foreach (var config in crystalShrineSecondary)
            {
                if (Main.rand.NextFloat() < 0.5f)
                {
                    int selectedItem = config.ItemIDs.Count > 1
                        ? config.ItemIDs[Main.rand.Next(config.ItemIDs.Count)]
                        : config.ItemIDs[0];
                    int amount = Main.rand.Next(config.MinAmount, config.MaxAmount + 1);
                    if (chestSlot < chest.item.Length)
                    {
                        chest.item[chestSlot] = new Item();
                        chest.item[chestSlot].SetDefaults(selectedItem);
                        chest.item[chestSlot].stack = amount;
                        chestSlot++;
                    }
                }
            }

            //common loot: Example—if the house is deep underground, add extra gold coins.
            if (house.Y > Main.maxTilesY / 2 && chestSlot < chest.item.Length)
            {
                int coinAmount = Main.rand.Next(10, 50);
                chest.item[chestSlot] = new Item();
                chest.item[chestSlot].SetDefaults(ItemID.GoldCoin);
                chest.item[chestSlot].stack = coinAmount;
            }
        }

        #endregion


        #region Pots & Cobwebs
        private static void PlaceRandomPots(HouseInfo[] houses)
        {
            int potCount = Main.rand.Next(1, 14);
            for (int i = 0; i < potCount; i++)
                TryPlaceOnePot(houses);
        }

        private static void TryPlaceOnePot(HouseInfo[] houses)
        {
            var house = houses[Main.rand.Next(houses.Length)];
            int left = house.X + 1;
            int right = house.X + house.Width - 2;
            int top = house.Y + 1;
            int bottom = house.Y + house.Height - 2;

            for (int attempt = 0; attempt < 50; attempt++)
            {
                int x = Main.rand.Next(left, right + 1);
                int y = Main.rand.Next(top, bottom + 1);
                if (!Main.tile[x, y].HasTile)
                {
                    KillTile(x, y, false, false, true);
                    PlaceTile(x, y, TileID.Pots, mute: true, forced: true);
                    break;
                }
            }
        }
        #endregion

        #region Top Passage & Doors
        private static void PlaceTopPlatformPassage(HouseInfo topHouse)
        {
            int passageWidth = Main.rand.Next(4, 7);
            int maxStart = topHouse.Width - passageWidth;
            if (maxStart < 0) return;
            int passageX = topHouse.X + Main.rand.Next(0, maxStart + 1);
            int passageRow = topHouse.Y;
            int checkRow = passageRow - 1;
            for (int x = passageX; x < passageX + passageWidth; x++)
            {
                if (Main.tile[x, checkRow].HasTile || Main.tile[x, checkRow].WallType != 0)
                    return;
            }
            for (int x = passageX; x < passageX + passageWidth; x++)
            {
                KillTile(x, passageRow, false, false, true);
                PlaceTile(x, passageRow, ModContent.TileType<GlimmerwoodPlatformTile>(), mute: true, forced: true);
            }
        }

        #endregion

        #region Beams & Framing

        private static void PlaceBeamsUnderHouse(HouseInfo bottomHouse)
        {
            List<int> beamColumns = GetBeamColumns(bottomHouse.X, bottomHouse.Width);
            int houseBottom = bottomHouse.Y + bottomHouse.Height - 1;
            foreach (int x in beamColumns)
            {
                int y = houseBottom + 1;
                while (y < Main.maxTilesY && !Main.tile[x, y].HasTile)
                {
                    PlaceTile(x, y, ModContent.TileType<GlimmerwoodBeamTile>(), mute: true, forced: true);
                    y++;
                }
            }
        }

        private static List<int> GetBeamColumns(int left, int width)
        {
            int totalBeams = 4 + (width - 20) / 5;
            if (totalBeams < 2) totalBeams = 2;
            if (totalBeams > 6) totalBeams = 6;
            List<int> columns = new() { left, left + width - 1 };
            int extra = totalBeams - 2;
            if (extra > 0)
            {
                float step = (width - 1) / (extra + 1f);
                for (int i = 1; i <= extra; i++)
                {
                    int col = left + (int)Math.Round(step * i);
                    if (!columns.Contains(col))
                        columns.Add(col);
                }
            }
            columns.Sort();
            return columns;
        }

        private static void FrameGeneratedArea(HouseInfo[] houses)
        {
            (int minX, int maxX, int minY, int maxY) = GetBoundingBox(houses);

            minX = Math.Max(0, minX - 2);
            minY = Math.Max(0, minY - 2);
            maxX = Math.Min(Main.maxTilesX - 1, maxX + 2);
            maxY = Math.Min(Main.maxTilesY - 1, maxY + 2);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    WorldGen.TileFrame(x, y, false, false);
                    WorldGen.SquareTileFrame(x, y, true);
                    WorldGen.SquareWallFrame(x, y, true);
                }
            }
        }

        #endregion

        #region Chains
        private static void PlaceChainLinesInHouse(HouseInfo house)
        {
            int lineCount = Main.rand.Next(1, 4);
            for (int i = 0; i < lineCount; i++)
            {
                int x = Main.rand.Next(house.X + 1, house.X + house.Width - 1);
                int startY = house.Y + 1;
                int chainLength = Main.rand.Next(2, 6);
                for (int y = startY; y < startY + chainLength && y < house.Y + house.Height; y++)
                {
                    KillTile(x, y, false, false, true);
                    PlaceTile(x, y, TileID.Chain, mute: true, forced: true);
                }
            }
        }
        #endregion

        /// <summary>
        /// Iterates over all tiles in the house’s rectangle. For any tile that is one of our wall–tiles
        /// (e.g. CavernBrickTile or CrackedCavernBrickTile), it checks its four neighbors (up, down, left, right).
        /// If a neighbor is “exposed” (has no tile and is not a platform), then with a given chance, it places a
        /// CrystalGrowthTile there.
        /// </summary>
        private static void PlaceCrystalGrowthOnExposed(HouseInfo house, float chancePerTile)
        {
            for (int x = house.X; x < house.X + house.Width; x++)
            {
                for (int y = house.Y; y < house.Y + house.Height; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile != null && tile.HasTile && (tile.TileType == ModContent.TileType<CavernBrickTile>() || tile.TileType == ModContent.TileType<CrackedCavernBrickTile>()))
                    {
                        foreach (Point offset in new Point[] { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) })
                        {
                            int nx = x + offset.X;
                            int ny = y + offset.Y;
                            if (nx < 0 || nx >= Main.maxTilesX || ny < 0 || ny >= Main.maxTilesY)
                                continue;
                            if (Main.tile[nx, ny].HasTile && Main.tile[nx, ny].TileType == TileID.Platforms)
                                continue;
                            if (!Main.tile[nx, ny].HasTile && Main.rand.NextFloat() < chancePerTile)
                            {
                                KillTile(nx, ny, false, false, true);
                                PlaceTile(nx, ny, ModContent.TileType<CrystalGrowthTile>(), mute: true, forced: true);
                            }
                        }
                    }
                }
            }
        }

        #region Helpers

        /// <summary>
        /// Checks that all tiles in [x..x+width-1] × [y..y+height-1] are empty (no tile),
        /// and no slopes, etc.
        /// </summary>
        private static bool CheckMultiTileSpace(int x, int y, int width, int height)
        {
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    if (Main.tile[i, j].HasTile) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// For the bottom row (floorY), we want [x..x+width-1] to be solid blocks for the multi‐tile to stand on.
        /// </summary>
        private static bool CheckFloorForMultiTile(int x, int floorY, int width)
        {
            for (int i = x; i < x + width; i++)
            {
                Tile t = Main.tile[i, floorY];
                if (!IsSolidBlock(t))
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Local neighbors in a house region.
        /// </summary>
        private static IEnumerable<Point> GetNeighborsLocal(int x, int y, int width, int height)
        {
            if (x > 0) yield return new Point(x - 1, y);
            if (x < width - 1) yield return new Point(x + 1, y);
            if (y > 0) yield return new Point(x, y - 1);
            if (y < height - 1) yield return new Point(x, y + 1);
        }

        /// <summary>
        /// True if the tile is active, slope=solid, not half-block, etc.
        /// </summary>
        private static bool IsSolidBlock(Tile tile)
        {
            if (tile == null) return false;
            if (!tile.HasTile) return false;
            if (tile.Slope != SlopeType.Solid) return false;
            if (tile.IsHalfBlock) return false;
            return true;
        }

        private static (int minX, int maxX, int minY, int maxY) GetBoundingBox(HouseInfo[] houses)
        {
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            foreach (var h in houses)
            {
                if (h.X < minX) minX = h.X;
                if (h.X + h.Width - 1 > maxX) maxX = h.X + h.Width - 1;
                if (h.Y < minY) minY = h.Y;
                if (h.Y + h.Height - 1 > maxY) maxY = h.Y + h.Height - 1;
            }
            return (minX, maxX, minY, maxY);
        }

        private static float SimpleNoise(int x, int y, float frequency, int seed)
        {
            int n = x + y * 57 + seed * 131;
            n = (n << 13) ^ n;
            float noise = (1.0f - ((n * (n * n * 15731 + 789221) + 1376312589)
                     & 0x7fffffff) / 1073741824f);
            return noise * frequency;
        }
        #endregion
    }
}