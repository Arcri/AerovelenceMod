using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Building;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;
using Terraria.DataStructures;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;

namespace AerovelenceMod.Content.Items.BossSummons
{
    public static class ArenaData
    {
        public static Vector2 InnerArenaBoundaryLeft;
        public static Vector2 InnerArenaBoundaryRight;
        public static Vector2 OuterArenaBoundaryLeft;
        public static Vector2 OuterArenaBoundaryRight;
        public static Vector2 ArenaCenter;
        public static int ArenaWidth;
        public static int WaterLayer;
        public static int ArenaY;
    }

    public class LargeGeode : ModItem
    {
        private static List<Point> placedTiles = new List<Point>();

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = false;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ItemRarityID.Red;
        }

        public static bool IsBossAlive()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.boss)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            if (IsNearCavernGateway(player) && !IsBossAlive())
            {
                return true;
            }
            else
            {
                Main.NewText("You must be near the Cavern Gateway and no boss should be active.", 255, 0, 0);
                return false;
            }
        }

        public static class ArenaBoundaries
        {
            public static Vector2 leftBoundary;
            public static Vector2 rightBoundary;
        }

        public static Vector2[] crystalPositions;

        public override bool? UseItem(Player player)
        {
            Vector2 centerOfArena = GetCenterOfArena(player);
            if (centerOfArena != Vector2.Zero)
            {
                ArenaBoundaries.leftBoundary = FindArenaBoundary(centerOfArena, -1, out int baseLevelY);
                ArenaBoundaries.rightBoundary = FindArenaBoundary(centerOfArena, 1, out baseLevelY);
                SetupArenaBoundaries(centerOfArena);

                ArenaData.OuterArenaBoundaryLeft = new Vector2(ArenaBoundaries.leftBoundary.X, ArenaBoundaries.leftBoundary.Y);
                ArenaData.OuterArenaBoundaryRight = new Vector2(ArenaBoundaries.rightBoundary.X, ArenaBoundaries.rightBoundary.Y);
                ArenaData.ArenaCenter = centerOfArena;
                ArenaData.ArenaWidth = (int)Math.Abs(ArenaBoundaries.rightBoundary.X - ArenaBoundaries.leftBoundary.X);
                ArenaData.ArenaY = ((int)(ArenaBoundaries.rightBoundary.Y / 16)) + 5;

                ArenaData.InnerArenaBoundaryLeft = new Vector2(ArenaData.OuterArenaBoundaryLeft.X + 10 * 16, ArenaData.OuterArenaBoundaryLeft.Y);
                ArenaData.InnerArenaBoundaryRight = new Vector2(ArenaData.OuterArenaBoundaryRight.X - 10 * 16, ArenaData.OuterArenaBoundaryRight.Y);

                int startTileY = (int)(centerOfArena.Y / 16);
                int waterTileY = startTileY;
                for (int y = startTileY; y < Main.maxTilesY; y++)
                {
                    Tile tile = Framing.GetTileSafely((int)(centerOfArena.X / 16), y);
                    if (tile.LiquidAmount > 0)
                    {
                        waterTileY = y;
                        break;
                    }
                }
                ArenaData.WaterLayer = waterTileY;

                int baseLevelWorldY = baseLevelY * 16;
                Vector2 baseLevelPosition = new Vector2(centerOfArena.X, baseLevelWorldY);
                CreateDustTowardsLocation(centerOfArena, baseLevelPosition);

                IEntitySource entitySource = player.GetSource_ItemUse(Item);
                int npcID = ModContent.NPCType<CrystalTumbler2>();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPC(entitySource, (int)centerOfArena.X, baseLevelWorldY, npcID);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: npcID);
                    }
                }
            }
            return true;
        }


        private bool IsNearCavernGateway(Player player)
        {
            var tileCoords = player.Center.ToTileCoordinates();
            int tileX = tileCoords.X;
            int tileY = tileCoords.Y;

            for (int x = tileX - 7; x <= tileX + 7; x++)
            {
                for (int y = tileY - 7; y <= tileY + 7; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.HasTile && tile.TileType == ModContent.TileType<CavernGatewayTile>())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Vector2 GetCenterOfArena(Player player)
        {
            var tileCoords = player.Center.ToTileCoordinates();
            int tileX = tileCoords.X;
            int tileY = tileCoords.Y;
            int searchRadius = 7;

            for (int x = tileX - searchRadius; x <= tileX + searchRadius; x++)
            {
                for (int y = tileY - searchRadius; y <= tileY + searchRadius; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.HasTile && tile.TileType == ModContent.TileType<CavernGatewayTile>())
                    {
                        int bottomY = y;
                        while (true)
                        {
                            Tile tileBelow = Framing.GetTileSafely(x, bottomY + 1);
                            if (tileBelow.HasTile && tileBelow.TileType == ModContent.TileType<CavernGatewayTile>())
                            {
                                bottomY++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        int leftX = x;
                        int rightX = x;
                        while (true)
                        {
                            Tile tileRight = Framing.GetTileSafely(rightX + 1, y);
                            if (tileRight.HasTile && tileRight.TileType == ModContent.TileType<CavernGatewayTile>())
                            {
                                rightX++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        float centerX = (leftX + rightX + 1) / 2f;
                        float centerY = bottomY + 0.5f;
                        return new Vector2(centerX, centerY) * 16f;
                    }
                }
            }
            return Vector2.Zero;
        }



        private void SetupArenaBoundaries(Vector2 centerOfArena)
        {
            crystalPositions = FindNearestArenaCrystals(centerOfArena);
            foreach (var position in crystalPositions)
            {
                CreateDustTowardsLocation(centerOfArena, position);
            }
            int baseLevelY;
            Vector2 leftBoundary = FindArenaBoundary(centerOfArena, -1, out baseLevelY);
            Vector2 rightBoundary = FindArenaBoundary(centerOfArena, 1, out baseLevelY);
            CreateTemporaryArenaBorders(leftBoundary, rightBoundary);
        }

        public static void RemoveArenaBoundaries()
        {
            foreach (var tilePos in placedTiles)
            {
                if (Framing.GetTileSafely(tilePos.X, tilePos.Y).TileType == TileID.SapphireGemspark)
                {
                    WorldGen.KillTile(tilePos.X, tilePos.Y, fail: false, effectOnly: false, noItem: true);
                }
            }
            placedTiles.Clear();
        }

        private Vector2[] FindNearestArenaCrystals(Vector2 center)
        {
            Vector2[] crystalPositions = new Vector2[3];
            HashSet<Point> foundPositions = new HashSet<Point>();
            for (int i = 0; i < 3; i++)
            {
                crystalPositions[i] = FindCrystalInRadius(center, foundPositions);
                if (crystalPositions[i] != center)
                {
                    foundPositions.Add(crystalPositions[i].ToTileCoordinates());
                }
            }
            return crystalPositions;
        }

        private Vector2 FindCrystalInRadius(Vector2 center, HashSet<Point> foundPositions)
        {
            int searchRadius = 50;
            float minDistance = float.MaxValue;
            Vector2 nearestCrystal = center;

            for (int x = (int)(center.X / 16) - searchRadius; x <= (int)(center.X / 16) + searchRadius; x++)
            {
                for (int y = (int)(center.Y / 16) - searchRadius; y <= (int)(center.Y / 16) + searchRadius; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.HasTile && tile.TileType == ModContent.TileType<ArenaCavernCrystalTile>())
                    {
                        Point tilePos = new Point(x, y);
                        bool tooClose = false;
                        foreach (var foundPos in foundPositions)
                        {
                            if (Vector2.Distance(tilePos.ToVector2(), foundPos.ToVector2()) < 15f)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        if (!tooClose)
                        {
                            float distance = Vector2.Distance(center, tilePos.ToVector2() * 16f);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                nearestCrystal = tilePos.ToVector2() * 16f;
                            }
                        }
                    }
                }
            }
            return nearestCrystal;
        }

        private static void CreateDustTowardsLocation(Vector2 start, Vector2 end)
        {
            int dustCount = 100;
            Vector2 direction = (end - start).SafeNormalize(Vector2.Zero);

            for (int i = 0; i < dustCount; i++)
            {
                Vector2 position = start + direction * (i * 16f);
                Dust dust = Dust.NewDustPerfect(position, DustID.MagnetSphere);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
                dust.velocity = direction * 2f;
                dust.scale = 1.5f;
                dust.customData = () => IsBossAlive() ? dust.scale : 0;
            }
        }

        public static int baseLevelY = 0;

        public static void SetBaseLevelY(int newBaseLevelTileY)
        {
            baseLevelY = newBaseLevelTileY * 16;
        }

        public static int GetBaseLevelY()
        {
            return baseLevelY;
        }

        private Vector2 FindArenaBoundary(Vector2 center, int directionX, out int baseLevelY)
        {
            baseLevelY = 0;
            int playerTileY = (int)(center.Y / 16);
            int searchRadius = 100;
            for (int x = (int)(center.X / 16) + directionX; Math.Abs(x - (int)(center.X / 16)) <= searchRadius; x += directionX)
            {
                for (int y = playerTileY - 10; y <= playerTileY + 10; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    Tile adjacentTile = Framing.GetTileSafely(x + directionX, y);
                    if (tile.HasTile && tile.TileType == ModContent.TileType<GlimmerwoodPlatformTile>() &&
                        adjacentTile.HasTile && adjacentTile.TileType == ModContent.TileType<SmoothCavernStoneTile>())
                    {
                        baseLevelY = y;
                        return new Vector2(x * 16, y * 16);
                    }
                }
            }
            baseLevelY = playerTileY;
            return center;
        }

        private void CreateTemporaryArenaBorders(Vector2 leftBoundary, Vector2 rightBoundary)
        {
            int arenaHeight = 20;
            int startHeightOffset = 15;
            for (int y = (int)(leftBoundary.Y / 16) - startHeightOffset; y <= (int)(leftBoundary.Y / 16) - startHeightOffset + arenaHeight; y++)
            {
                Point tilePos = new Point((int)(leftBoundary.X / 16), y);
                Tile tile = Framing.GetTileSafely(tilePos.X, tilePos.Y);
                if (!tile.HasTile)
                {
                    WorldGen.PlaceTile(tilePos.X, tilePos.Y, TileID.SapphireGemspark, mute: true, forced: false);
                    placedTiles.Add(tilePos);
                }
            }
            for (int y = (int)(rightBoundary.Y / 16) - startHeightOffset; y <= (int)(rightBoundary.Y / 16) - startHeightOffset + arenaHeight; y++)
            {
                Point tilePos = new Point((int)(rightBoundary.X / 16), y);
                Tile tile = Framing.GetTileSafely(tilePos.X, tilePos.Y);
                if (!tile.HasTile)
                {
                    WorldGen.PlaceTile(tilePos.X, tilePos.Y, TileID.SapphireGemspark, mute: true, forced: false);
                    placedTiles.Add(tilePos);
                }
            }
        }
    }
}