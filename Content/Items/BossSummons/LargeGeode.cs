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
using System.Linq;
using AerovelenceMod.Common.Systems.Language;

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

        public static Vector2 Platform1Left, Platform1Right;
        public static int Platform1Width;

        public static Vector2 Platform2Left, Platform2Right;
        public static int Platform2Width;

        public static Vector2 Platform3Left, Platform3Right;
        public static int Platform3Width;

        public static Vector2 Platform4Left, Platform4Right;
        public static int Platform4Width;

        public static Vector2 Platform5Left, Platform5Right;
        public static int Platform5Width;

        public static Vector2 Platform6Left, Platform6Right;
        public static int Platform6Width;
    }

    public class LargeGeode : TranslatableModItem
    {
        private static List<Point> placedTiles = new List<Point>();

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("LargeGeode", "Not consumable\nSummons the Crystal Tumbler\nOnly usable in the Caverns Arena")
            .AddName(Language.Default, "Large Geode").AddTooltip(Language.Default, "Not consumable\nSummons the Crystal Tumbler\nOnly usable in the Caverns Arena")
            .AddName(Language.Spanish, "Geoda Grande").AddTooltip(Language.Spanish, "No consumible\nInvoca al Rodador de Cristal\nSolo usable en la Arena de las Cavernas")
            .AddName(Language.French, "Grande Géode").AddTooltip(Language.French, "Non consommable\nInvoque le Rouleur de Cristal\nUtilisable uniquement dans l'Arène des Cavernes")
            .AddName(Language.German, "Große Geode").AddTooltip(Language.German, "Nicht verbrauchbar\nBeschwört den Kristall-Tumbler\nNur in der Höhlenarena verwendbar")
            .AddName(Language.Italian, "Grande Geode").AddTooltip(Language.Italian, "Non consumabile\nEvoca il Rullo di Cristallo\nUtilizzabile solo nell'Arena delle Caverne")
            .AddName(Language.Polish, "Duża Geoda").AddTooltip(Language.Polish, "Nie zużywa się\nPrzywołuje Kryształowego Tumblera\nMożna używać tylko na Arenie Jaskiń")
            .AddName(Language.PortugueseBrazil, "Grande Geodo").AddTooltip(Language.PortugueseBrazil, "Não consumível\nInvoca o Tumbler de Cristal\nSomente utilizável na Arena das Cavernas")
            .AddName(Language.Russian, "Большая Геода").AddTooltip(Language.Russian, "Не расходуется\nПризывает Кристального Тумблера\nМожно использовать только в Пещерной Арене")
            .AddName(Language.ChineseTraditional, "大型晶洞").AddTooltip(Language.ChineseTraditional, "不可消耗\n召喚水晶翻滾者\n僅限於洞穴競技場使用")
            .AddName(Language.ChineseSimplified, "大型晶洞").AddTooltip(Language.ChineseSimplified, "不可消耗\n召唤水晶翻滚者\n仅限于洞穴竞技场使用");
        }

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

                ArenaData.OuterArenaBoundaryLeft = ArenaBoundaries.leftBoundary;
                ArenaData.OuterArenaBoundaryRight = ArenaBoundaries.rightBoundary;
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
                //CreateDustTowardsLocation(centerOfArena, baseLevelPosition);
                FindArenaPlatforms(centerOfArena);
                //DebugPlatformDust();
                IEntitySource entitySource = player.GetSource_ItemUse(Item);
                int npcID = ModContent.NPCType<CrystalTumbler2>();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPC(entitySource, (int)centerOfArena.X, baseLevelWorldY, npcID);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, number: npcID);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    IEntitySource entitySource3 = player.GetSource_ItemUse(Item);

                    for (int i = 1; i <= 6; i++)
                    {
                        Vector2 platformCenter = GetPlatformCenter(i);
                        float platformWidthTiles = GetPlatformWidth(i);
                        float platformWidthPixels = platformWidthTiles * 16f;
                        Vector2 leftEdgePos = new Vector2(
                            platformCenter.X - ((platformWidthPixels / 2f) - 16),
                            platformCenter.Y - 2
                        );

                        //int projIndex = Projectile.NewProjectile(entitySource3, new Vector2(leftEdgePos.X - 20, leftEdgePos.Y), Vector2.Zero, ModContent.ProjectileType<ElectricSpikeField>(), 50, 0, player.whoAmI);

                        /*if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncProjectile, number: projIndex);
                        if (Main.projectile[projIndex].ModProjectile is ElectricSpikeField fieldProjectile)
                        {
                            fieldProjectile.SetWidth(platformWidthPixels);
                            Main.projectile[projIndex].position = platformCenter - new Vector2(Main.projectile[projIndex].width / 2f - 8, (Main.projectile[projIndex].height / 2f + 10));
                        }*/

                    }
                }

            }
            return true;
        }


        public static Vector2 GetPlatformCenter(int platformIndex)
        {
            switch (platformIndex)
            {
                case 1: return (ArenaData.Platform1Left + ArenaData.Platform1Right) / 2;
                case 2: return (ArenaData.Platform2Left + ArenaData.Platform2Right) / 2;
                case 3: return (ArenaData.Platform3Left + ArenaData.Platform3Right) / 2;
                case 4: return (ArenaData.Platform4Left + ArenaData.Platform4Right) / 2;
                case 5: return (ArenaData.Platform5Left + ArenaData.Platform5Right) / 2;
                case 6: return (ArenaData.Platform6Left + ArenaData.Platform6Right) / 2;
                default: return Vector2.Zero;
            }
        }

        public static float GetPlatformWidth(int platformIndex)
        {
            switch (platformIndex)
            {
                case 1: return ArenaData.Platform1Width;
                case 2: return ArenaData.Platform2Width;
                case 3: return ArenaData.Platform3Width;
                case 4: return ArenaData.Platform4Width;
                case 5: return ArenaData.Platform5Width;
                case 6: return ArenaData.Platform6Width;
                default: return 0f;
            }
        }



        private static void DebugPlatformDust()
        {
            CreateDustTowardsLocation(ArenaData.Platform1Left, ArenaData.Platform1Right);
            CreateDustTowardsLocation(ArenaData.Platform2Left, ArenaData.Platform2Right);
            CreateDustTowardsLocation(ArenaData.Platform3Left, ArenaData.Platform3Right);
            CreateDustTowardsLocation(ArenaData.Platform4Left, ArenaData.Platform4Right);
            CreateDustTowardsLocation(ArenaData.Platform5Left, ArenaData.Platform5Right);
            CreateDustTowardsLocation(ArenaData.Platform6Left, ArenaData.Platform6Right);

            Main.NewText("Debug dust applied to platforms!", Color.Cyan);
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
                //CreateDustTowardsLocation(centerOfArena, position);
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

        private void CreatePlatformDust(Vector2 start, Vector2 end, int platformNumber)
        {
            if (start == Vector2.Zero || end == Vector2.Zero)
            {
                Main.NewText($"Platform {platformNumber}: Invalid position vectors!", Color.Red);
                return;
            }

            int dustType;
            switch (platformNumber)
            {
                case 1: dustType = DustID.GemRuby; break;
                case 2: dustType = DustID.GemTopaz; break;
                case 3: dustType = DustID.GemEmerald; break;
                case 4: dustType = DustID.GemSapphire; break;
                case 5: dustType = DustID.GemAmethyst; break;
                case 6: dustType = DustID.PinkTorch; break;
                default: dustType = DustID.WhiteTorch; break;
            }

            for (int i = 0; i < 10; i++)
            {
                Dust dustLeft = Dust.NewDustPerfect(
                    start + new Vector2(Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-8, 0)),
                    dustType
                );
                dustLeft.noGravity = true;
                dustLeft.scale = 1.5f;

                Dust dustRight = Dust.NewDustPerfect(
                    end + new Vector2(Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-8, 0)),
                    dustType
                );
                dustRight.noGravity = true;
                dustRight.scale = 1.5f;
            }

            float distance = Vector2.Distance(start, end);
            int linePoints = Math.Max(10, (int)(distance / 16));

            for (int i = 0; i <= linePoints; i++)
            {
                float progress = (float)i / linePoints;
                Vector2 position = Vector2.Lerp(start, end, progress);
                position.Y -= 12;

                Dust dust = Dust.NewDustPerfect(position, dustType);
                dust.noGravity = true;
                dust.scale = 1.2f;
                dust.fadeIn = 1.0f;
            }

            Main.NewText($"Platform {platformNumber}: Creatd dust between X:{start.X / 16}-{end.X / 16} at Y:{start.Y / 16}", Color.White);
        }

        private void FindArenaPlatforms(Vector2 centerOfArena)
        {
            List<(Vector2 left, Vector2 right, int width, float yPos)> allPlatforms = new();

            int minX = (int)(ArenaData.InnerArenaBoundaryLeft.X / 16);
            int maxX = (int)(ArenaData.InnerArenaBoundaryRight.X / 16);
            int minY = (int)(centerOfArena.Y / 16) - 45;
            int maxY = (int)(centerOfArena.Y / 16) + 10;

            Main.NewText($"Scanning area: X={minX}-{maxX}, Y={minY}-{maxY}", Color.Yellow);
            Main.NewText($"Center at: ({centerOfArena.X / 16}, {centerOfArena.Y / 16})", Color.Yellow);
            for (int y = minY; y <= maxY; y++)
            {
                int platformStartX = -1;

                for (int x = minX; x <= maxX; x++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    bool isPlatform = tile.HasTile && tile.TileType == ModContent.TileType<GlimmerwoodPlatformTile>();
                    if (isPlatform && platformStartX == -1)
                    {
                        platformStartX = x;
                    }
                    else if ((!isPlatform || x == maxX) && platformStartX != -1)
                    {
                        int platformEndX = isPlatform ? x : x - 1;
                        int platformWidth = platformEndX - platformStartX + 1;
                        if (platformWidth >= 5)
                        {
                            Vector2 leftPos = new Vector2(platformStartX * 16, y * 16);
                            Vector2 rightPos = new Vector2(platformEndX * 16, y * 16);

                            allPlatforms.Add((leftPos, rightPos, platformWidth, y));
                            for (int i = 0; i < 3; i++)
                            {
                                Dust.NewDust(leftPos, 4, 4, DustID.GoldFlame, 0, 0, 0, default, 1.5f);
                                Dust.NewDust(rightPos, 4, 4, DustID.GoldFlame, 0, 0, 0, default, 1.5f);
                            }

                            Main.NewText($"Found platform at Y={y}: X={platformStartX}-{platformEndX}, Width={platformWidth}", Color.Lime);
                        }

                        platformStartX = -1;
                    }
                }
            }

            Main.NewText($"Total platforms found: {allPlatforms.Count}", Color.White);

            if (allPlatforms.Count < 6)
            {
                Main.NewText("Not enough platforms found! Need at least 6.", Color.Red);
                return;
            }

            allPlatforms = allPlatforms.OrderBy(p => p.yPos).ToList();
            List<(Vector2 left, Vector2 right, int width)> leftPlatforms = [];
            List<(Vector2 left, Vector2 right, int width)> rightPlatforms = [];

            foreach (var platform in allPlatforms)
            {
                float platformCenterX = (platform.left.X + platform.right.X) / 2;
                if (platformCenterX < centerOfArena.X)
                {
                    leftPlatforms.Add((platform.left, platform.right, platform.width));
                }
                else
                {
                    rightPlatforms.Add((platform.left, platform.right, platform.width));
                }
            }

            Main.NewText($"Found {leftPlatforms.Count} left platforms and {rightPlatforms.Count} right platforms", Color.Orange);
            leftPlatforms = leftPlatforms.Take(3).ToList();
            rightPlatforms = rightPlatforms.Take(3).ToList();
            if (leftPlatforms.Count < 3 || rightPlatforms.Count < 3)
            {
                Main.NewText($"Found {leftPlatforms.Count} left, {rightPlatforms.Count} right", Color.Red);
                return;
            }

            ArenaData.Platform1Left = leftPlatforms[0].left;
            ArenaData.Platform1Right = leftPlatforms[0].right;
            ArenaData.Platform1Width = leftPlatforms[0].width;
            CreatePlatformDust(ArenaData.Platform1Left, ArenaData.Platform1Right, 1);

            ArenaData.Platform2Left = leftPlatforms[1].left;
            ArenaData.Platform2Right = leftPlatforms[1].right;
            ArenaData.Platform2Width = leftPlatforms[1].width;
            CreatePlatformDust(ArenaData.Platform2Left, ArenaData.Platform2Right, 2);

            ArenaData.Platform3Left = leftPlatforms[2].left;
            ArenaData.Platform3Right = leftPlatforms[2].right;
            ArenaData.Platform3Width = leftPlatforms[2].width;
            CreatePlatformDust(ArenaData.Platform3Left, ArenaData.Platform3Right, 3);

            ArenaData.Platform4Left = rightPlatforms[0].left;
            ArenaData.Platform4Right = rightPlatforms[0].right;
            ArenaData.Platform4Width = rightPlatforms[0].width;
            CreatePlatformDust(ArenaData.Platform4Left, ArenaData.Platform4Right, 4);

            ArenaData.Platform5Left = rightPlatforms[1].left;
            ArenaData.Platform5Right = rightPlatforms[1].right;
            ArenaData.Platform5Width = rightPlatforms[1].width;
            CreatePlatformDust(ArenaData.Platform5Left, ArenaData.Platform5Right, 5);

            ArenaData.Platform6Left = rightPlatforms[2].left;
            ArenaData.Platform6Right = rightPlatforms[2].right;
            ArenaData.Platform6Width = rightPlatforms[2].width;
            CreatePlatformDust(ArenaData.Platform6Left, ArenaData.Platform6Right, 6);

            Main.NewText("All platforms successfully located and visualized!", Color.Green);
        }

        private (Vector2 left, Vector2 right)? GetPlatformEdges(Vector2 start)
        {
            int x = (int)(start.X / 16);
            int y = (int)(start.Y / 16);

            int leftX = x;
            int rightX = x;

            while (Framing.GetTileSafely(leftX - 1, y).HasTile &&
                   Framing.GetTileSafely(leftX - 1, y).TileType == ModContent.TileType<GlimmerwoodPlatformTile>())
            {
                leftX--;
            }

            while (Framing.GetTileSafely(rightX + 1, y).HasTile &&
                   Framing.GetTileSafely(rightX + 1, y).TileType == ModContent.TileType<GlimmerwoodPlatformTile>())
            {
                rightX++;
            }

            return (new Vector2(leftX * 16, y * 16), new Vector2(rightX * 16, y * 16));
        }


        private static void CreateDustTowardsLocation(Vector2 start, Vector2 end)
        {
            if (start == Vector2.Zero || end == Vector2.Zero)
            {
                Main.NewText("Cannot create dust for zero vector", Color.Red);
                return;
            }

            float distance = Vector2.Distance(start, end);
            int dustCount = Math.Max(10, (int)(distance / 8));

            Vector2 direction = (end - start);
            float length = direction.Length();
            direction = direction / length;

            for (int i = 0; i < dustCount; i++)
            {
                float progress = (float)i / dustCount;
                Vector2 position = Vector2.Lerp(start, end, progress);
                position += new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                int dustType;
                if (i == 0 || i == dustCount - 1)
                {
                    dustType = DustID.GemSapphire;
                }
                else if (i % 5 == 0)
                {
                    dustType = DustID.GemAmethyst;
                }
                else
                {
                    dustType = DustID.MagnetSphere;
                }

                Dust dust = Dust.NewDustPerfect(position, dustType);
                dust.noGravity = true;
                dust.noLight = false;
                dust.fadeIn = 1.5f;
                dust.scale = 1.5f;

                dust.velocity = direction * 0.1f + new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f));

                dust.active = true;
                dust.fadeIn = 1f; 
            }

            for (int i = 0; i < 3; i++)
            {
                Dust dustStart = Dust.NewDustPerfect(start, DustID.Pixie);
                dustStart.noGravity = true;
                dustStart.scale = 2f;

                Dust dustEnd = Dust.NewDustPerfect(end, DustID.Pixie);
                dustEnd.noGravity = true;
                dustEnd.scale = 2f;
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