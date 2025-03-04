using AerovelenceMod.Common.Systems;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    public class LightningTrapItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<LightningTrapTile>();
            Item.rare = ItemRarityID.White;
            Item.value = 5;
        }
    }

    public class LightningTrapTile : ModTile
    {
        private const int WIND_UP_FRAMES = 6;
        private const int CHARGE_FRAMES = 4;
        private const int FIRE_FRAME = 10;
        private const int COOLDOWN_FRAMES = 4;
        private const int TOTAL_FRAMES = 17;
        private const int DETECTOR_ACTIVE_FRAME = 15;
        private const int DETECTOR_INACTIVE_FRAME = 16;
        private const int MAX_LINK_DISTANCE = 10;
        private const int FRAME_WIDTH = 18;
        private const int FRAME_HEIGHT = 18;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = false;
            Main.tileTable[Type] = false;
            Main.tileNoFail[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 900;

            /*TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, 1, 0);
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
            TileObjectData.addAlternate(2);

            TileObjectData.addTile(Type);*/

            AddMapEntry(new Color(200, 200, 200), CreateMapEntryName());

            DustType = DustID.BlueCrystalShard;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (player.inventory[player.selectedItem].type == ModContent.ItemType<LightningTrapItem>() ||
                player.inventory[player.selectedItem].type == ItemID.WireCutter)
            {
                Tile tile = Main.tile[i, j];
                int direction = tile.TileFrameY / FRAME_HEIGHT;
                bool isHorizontal = direction < 2;
                int searchDirection = direction % 2 == 0 ? 1 : -1;

                for (int distance = 0; distance <= MAX_LINK_DISTANCE; distance++)
                {
                    int dustX = i + (isHorizontal ? distance * searchDirection : 0);
                    int dustY = j + (!isHorizontal ? distance * searchDirection : 0);
                    if (WorldGen.InWorld(dustX, dustY))
                    {
                        CreateDust(dustX, dustY, direction);
                    }
                }
            }
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            tile.TileFrameY = (short)(((tile.TileFrameY / FRAME_HEIGHT) + 1) % 4 * FRAME_HEIGHT);
            ForceUpdatePairedTraps(i, j, tile);
            return true;
        }

        private void ForceUpdatePairedTraps(int i, int j, Tile tile)
        {
            bool isPaired = false;
            int direction = tile.TileFrameY / FRAME_HEIGHT;
            bool isHorizontal = direction < 2;
            int searchDirection = direction % 2 == 0 ? 1 : -1;
            for (int distance = 1; distance <= MAX_LINK_DISTANCE; distance++)
            {
                int checkX = i + (isHorizontal ? distance * searchDirection : 0);
                int checkY = j + (!isHorizontal ? distance * searchDirection : 0);
                if (!WorldGen.InWorld(checkX, checkY))
                    break;
                Tile checkTile = Main.tile[checkX, checkY];
                if (checkTile.TileType == Type)
                {
                    int checkDirection = checkTile.TileFrameY / FRAME_HEIGHT;
                    bool canPair = (direction == 0 && checkDirection == 1) ||
                                  (direction == 1 && checkDirection == 0) ||
                                  (direction == 2 && checkDirection == 3) ||
                                  (direction == 3 && checkDirection == 2);
                    if (canPair)
                    {
                        isPaired = true;
                        tile.TileFrameX = 0;
                        checkTile.TileFrameX = 0;
                        break;
                    }
                }
            }

            if (!isPaired)
                tile.TileFrameX = DETECTOR_INACTIVE_FRAME * FRAME_WIDTH;
        }

        private static void AdvanceFrame(Tile tile, bool charging)
        {
            int currentFrame = tile.TileFrameX / (FRAME_WIDTH);
            if (charging)
            {
                if (currentFrame < 6 || currentFrame >= 10)
                    tile.TileFrameX = 6 * (FRAME_WIDTH);
                else
                    tile.TileFrameX = (short)(((currentFrame + 1) % 4 + 6) * (FRAME_WIDTH));
            }
            else
            {
                if (currentFrame >= TOTAL_FRAMES - 1)
                    tile.TileFrameX = 0;
                else
                    tile.TileFrameX += FRAME_WIDTH;
            }
        }

        private const int POWER_BIT = 4;
        private static bool GetPowerState(Tile tile)
        {
            return tile.TileFrameX < (17 * FRAME_WIDTH);
        }

        private static void SetPowerState(Tile tile, bool powered)
        {
            if (!powered && tile.TileFrameX < (17 * FRAME_WIDTH))
                tile.TileFrameX = 17 * FRAME_WIDTH;
            else if (powered && tile.TileFrameX >= (17 * FRAME_WIDTH))
                tile.TileFrameX = 0;
        }


        public override void HitWire(int i, int j)
        {
            if (Wiring.CheckMech(i, j, 60))
            {
                Tile tile = Main.tile[i, j];
                bool isDetectorMode = IsInDetectorMode(i, j, tile);
                if (isDetectorMode)
                {
                    int currentFrame = tile.TileFrameX / FRAME_WIDTH;
                    if (currentFrame != DETECTOR_ACTIVE_FRAME && currentFrame != DETECTOR_INACTIVE_FRAME)
                        tile.TileFrameX = DETECTOR_INACTIVE_FRAME * FRAME_WIDTH;
                }
                else if (IsValidPair(i, j, tile))
                {
                    int direction = tile.TileFrameY / FRAME_HEIGHT;
                    bool isFirstTrap = direction == 0 || direction == 2;
                    if (isFirstTrap)
                    {
                        bool isHorizontal = direction < 2;
                        int searchDirection = direction % 2 == 0 ? 1 : -1;
                        int pairedX = -1, pairedY = -1;
                        for (int distance = 1; distance <= MAX_LINK_DISTANCE; distance++)
                        {
                            int checkX = i + (isHorizontal ? distance * searchDirection : 0);
                            int checkY = j + (!isHorizontal ? distance * searchDirection : 0);
                            if (!WorldGen.InWorld(checkX, checkY)) break;
                            Tile checkTile = Main.tile[checkX, checkY];
                            if (checkTile.TileType == Type)
                            {
                                int checkDirection = checkTile.TileFrameY / FRAME_HEIGHT;
                                if ((direction == 0 && checkDirection == 1) ||
                                    (direction == 2 && checkDirection == 3))
                                {
                                    pairedX = checkX;
                                    pairedY = checkY;
                                    break;
                                }
                            }
                        }
                        if (pairedX != -1 && pairedY != -1)
                        {
                            bool currentPowerState = GetPowerState(tile);
                            bool newPowerState = !currentPowerState;
                            SetPowerState(tile, newPowerState);
                            SetPowerState(Main.tile[pairedX, pairedY], newPowerState);
                            Main.NewText(newPowerState ? "Trap Activated" : "Trap Deactivated", Color.Red);
                        }
                    }
                }
            }
        }

        private bool AreTrapsWired(int i, int j, Tile tile)
        {
            int direction = tile.TileFrameY / FRAME_HEIGHT;
            bool isHorizontal = direction < 2;
            int searchDirection = direction % 2 == 0 ? 1 : -1;
            int pairedX = -1, pairedY = -1;
            for (int distance = 1; distance <= MAX_LINK_DISTANCE; distance++)
            {
                int checkX = i + (isHorizontal ? distance * searchDirection : 0);
                int checkY = j + (!isHorizontal ? distance * searchDirection : 0);
                if (!WorldGen.InWorld(checkX, checkY)) break;

                Tile checkTile = Main.tile[checkX, checkY];
                if (checkTile.TileType == Type)
                {
                    int checkDirection = checkTile.TileFrameY / FRAME_HEIGHT;
                    bool isPaired = (direction == 0 && checkDirection == 1) ||
                                  (direction == 1 && checkDirection == 0) ||
                                  (direction == 2 && checkDirection == 3) ||
                                  (direction == 3 && checkDirection == 2);
                    if (isPaired)
                    {
                        pairedX = checkX;
                        pairedY = checkY;
                        break;
                    }
                }
            }
            if (pairedX == -1 || pairedY == -1) return false;
            bool hasWire = false;
            for (int x = Math.Min(i, pairedX); x <= Math.Max(i, pairedX); x++)
            {
                for (int y = Math.Min(j, pairedY); y <= Math.Max(j, pairedY); y++)
                {
                    Tile currentTile = Main.tile[x, y];
                    if (currentTile.RedWire || currentTile.BlueWire ||
                        currentTile.YellowWire || currentTile.GreenWire)
                    {
                        hasWire = true;
                        break;
                    }
                }
                if (hasWire) break;
            }

            return hasWire;
        }


        private bool IsInDetectorMode(int i, int j, Tile tile)
        {
            if (IsValidPair(i, j, tile))
            {
                return false;
            }
            return true;
        }



        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile tile = Main.tile[i, j];
            if (!tile.HasTile) return;
            bool isDetectorMode = IsInDetectorMode(i, j, tile);
            if (isDetectorMode)
            {
                bool playerInDetectionRange = CheckPlayerInDetectionRange(i, j, tile);
                int currentFrame = tile.TileFrameX / FRAME_WIDTH;
                if (playerInDetectionRange && currentFrame != DETECTOR_ACTIVE_FRAME)
                {
                    tile.TileFrameX = DETECTOR_ACTIVE_FRAME * FRAME_WIDTH;
                    Wiring.TripWire(i, j, 1, 1);
                }
                else if (!playerInDetectionRange && currentFrame != DETECTOR_INACTIVE_FRAME)
                    tile.TileFrameX = DETECTOR_INACTIVE_FRAME * FRAME_WIDTH;
            }
            else
            {
                bool hasPair = IsValidPair(i, j, tile);
                bool trapsConnected = AreTrapsWired(i, j, tile);
                bool isPowered = GetPowerState(tile);
                if (hasPair && isPowered && trapsConnected)
                {
                    if (!Main.gamePaused && Main.hasFocus && Main.GameUpdateCount % 5 == 0)
                    {
                        bool shouldAttack = CheckForValidTargets(i, j, tile);
                        if (shouldAttack)
                            FireProjectile(i, j, tile);
                    }
                }
            }
        }

        private bool CheckForValidTargets(int i, int j, Tile tile)
        {
            int direction = tile.TileFrameY / FRAME_HEIGHT;
            bool isHorizontal = direction < 2;
            int searchDirection = direction % 2 == 0 ? 1 : -1;
            for (int distance = 1; distance <= MAX_LINK_DISTANCE; distance++)
            {
                int checkX = i + (isHorizontal ? distance * searchDirection : 0);
                int checkY = j + (!isHorizontal ? distance * searchDirection : 0);
                if (!WorldGen.InWorld(checkX, checkY))
                    break;
                Rectangle checkArea = new(
                    checkX * 16,
                    checkY * 16,
                    16,
                    16
                );
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (player.active && !player.dead && checkArea.Intersects(player.Hitbox))
                    {
                        return true;
                    }
                }
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && !npc.townNPC && checkArea.Intersects(npc.Hitbox))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private static bool CheckPlayerInDetectionRange(int i, int j, Tile tile)
        {
            int direction = tile.TileFrameY / FRAME_HEIGHT;
            bool isHorizontal = direction < 2;
            int searchDirection = direction % 2 == 0 ? 1 : -1;
            Rectangle detectionArea;
            if (isHorizontal)
            {
                detectionArea = new Rectangle(
                    i * 16 + (searchDirection > 0 ? 16 : -48),
                    j * 16 - 8,
                    48,
                    32
                );
            }
            else
            {
                detectionArea = new Rectangle(
                    i * 16 - 8,
                    j * 16 + (searchDirection > 0 ? 16 : -48),
                    32,
                    48
                );
            }
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player player = Main.player[p];
                if (player.active && !player.dead && detectionArea.Intersects(player.Hitbox))
                {
                    return true;
                }
            }
            return false;
        }

        private static void FireProjectile(int i, int j, Tile tile)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            int direction = tile.TileFrameY / FRAME_HEIGHT;
            Vector2 velocity = Vector2.Zero;
            switch (direction)
            {
                case 0: velocity = new Vector2(8f, 0f); break;
                case 1: velocity = new Vector2(-8f, 0f); break;
                case 2: velocity = new Vector2(0f, 8f); break;
                case 3: velocity = new Vector2(0f, -8f); break;
            }
            Projectile.NewProjectile(
                Wiring.GetProjectileSource(i, j),
                new Vector2(i * 16 + 8, j * 16 + 8),
                velocity,
                ModContent.ProjectileType<LightningProjectile>(),
                50,
                2f,
                Main.myPlayer
            );
        }


        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Main.tile[i, j];
            if (!fail && !effectOnly)
            {
                CreateDust(i, j, tile.TileFrameY / 16);
            }
        }

        private static void CreateDust(int i, int j, int direction)
        {
            Vector2 dustVelocity = Vector2.Zero;
            switch (direction)
            {
                case 0: //right
                    dustVelocity = new Vector2(2f, 0f);
                    break;
                case 1: //left
                    dustVelocity = new Vector2(-2f, 0f);
                    break;
                case 2: //down
                    dustVelocity = new Vector2(0f, 2f);
                    break;
                case 3: //up
                    dustVelocity = new Vector2(0f, -2f);
                    break;
            }

            for (int d = 0; d < 3; d++)
                Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.BlueCrystalShard, dustVelocity.X, dustVelocity.Y);
        }


        private bool IsValidPair(int i, int j, Tile tile, bool checkDetectorMode = false)
        {
            int direction = tile.TileFrameY / FRAME_HEIGHT;
            bool isHorizontal = direction < 2;
            int searchDirection = direction % 2 == 0 ? 1 : -1;
            for (int distance = 1; distance <= MAX_LINK_DISTANCE; distance++)
            {
                int checkX = i + (isHorizontal ? distance * searchDirection : 0);
                int checkY = j + (!isHorizontal ? distance * searchDirection : 0);
                if (!WorldGen.InWorld(checkX, checkY))
                    break;
                Tile checkTile = Main.tile[checkX, checkY];
                if (checkTile.TileType == Type)
                {
                    int checkDirection = checkTile.TileFrameY / FRAME_HEIGHT;
                    bool isPaired = (direction == 0 && checkDirection == 1) || //right faces left
                                  (direction == 1 && checkDirection == 0) || //left faces right
                                  (direction == 2 && checkDirection == 3) || //down faces up
                                  (direction == 3 && checkDirection == 2);   //up faces down

                    if (isPaired)
                    {
                        if (!checkDetectorMode)
                        {
                            UpdateTrapFrames(i, j, tile, true);
                            UpdateTrapFrames(checkX, checkY, checkTile, true);
                        }
                    }
                    return isPaired;
                }
            }
            return false;
        }

        private static void UpdateTrapFrames(int i, int j, Tile tile, bool isPaired)
        {
            if (isPaired)
            {
                if (tile.TileFrameX >= (DETECTOR_INACTIVE_FRAME * FRAME_WIDTH) && tile.TileFrameX < (17 * FRAME_WIDTH))
                {
                    tile.TileFrameX = 0;
                }
            }
            else
            {
                if (tile.TileFrameX < (DETECTOR_INACTIVE_FRAME * FRAME_WIDTH))
                {
                    tile.TileFrameX = DETECTOR_INACTIVE_FRAME * FRAME_WIDTH;
                }
            }
        }
    }

    public class LightningProjectile : ModProjectile
    {
        private const int MAX_SEGMENTS = 12;
        private const float BRANCH_CHANCE = 1f;
        private const int MAX_BRANCHES = 2;
        private Vector2[] segmentPositions;
        private Vector2 targetPosition;
        private float[] segmentOffsets;
        private List<Branch> branches;
        private float alpha = 1f;
        private bool initialized;
        private float distanceToTarget;

        private class Branch
        {
            public Vector2[] Positions { get; set; }
            public float[] Offsets { get; set; }
            public float Alpha { get; set; }
            public int LifeTime { get; set; }
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.light = 0.8f;
        }

        public override void AI()
        {
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }

            UpdateSegments();
            UpdateBranches();

            PixellationSystem.QueuePixelationAction(() =>
            {
                for (int i = 0; i < 0.2; i++)
                {
                    Vector2 randomSegment = segmentPositions[Main.rand.Next(0, MAX_SEGMENTS)];
                    Vector2 dir = (segmentPositions[MAX_SEGMENTS - 1] - segmentPositions[0]).SafeNormalize(Vector2.Zero);

                    Color dustColor = Color.Lerp(
                        new Color(0, 236, 255),
                        new Color(0, 255, 191),
                        Main.rand.NextFloat()
                    );

                    Dust a = Dust.NewDustPerfect((randomSegment + dir * 2f) / 2,
                        ModContent.DustType<GlowStrong>(),
                        dir.RotatedByRandom(0.5f) * Main.rand.NextFloat(1f, 3f),
                        0, newColor: dustColor, Main.rand.NextFloat(0.5f, 2f));
                    a.alpha = 2;
                }

                foreach (Branch branch in branches)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int i = 0; i < branch.Positions.Length - 1; i++)
                        {
                            Vector2 dustPos = Vector2.Lerp(
                                branch.Positions[i],
                                branch.Positions[i + 1],
                                Main.rand.NextFloat()
                            );

                            Color dustColor = Color.Lerp(
                                Color.Aqua,
                                Color.LightBlue,
                                Main.rand.NextFloat()
                            );

                            Dust dust = Dust.NewDustPerfect(
                                dustPos,
                                DustID.Electric,
                                Vector2.Zero,
                                0,
                                dustColor * branch.Alpha,
                                Main.rand.NextFloat(0.6f, 0.9f) * branch.Alpha
                            );
                            dust.noGravity = true;
                            dust.fadeIn = 0f;
                        }
                    }
                }

            }, PixellationSystem.RenderType.Additive);

            if (Projectile.timeLeft < 10)
            {
                alpha *= 0.7f;
            }
        }

        private void Initialize()
        {
            FindTargetPosition();

            segmentPositions = new Vector2[MAX_SEGMENTS];
            segmentOffsets = new float[MAX_SEGMENTS];
            branches = [];

            Vector2 direction = targetPosition - Projectile.Center;
            distanceToTarget = direction.Length();
            float segmentLength = distanceToTarget / (MAX_SEGMENTS - 1);
            direction.Normalize();

            for (int i = 0; i < MAX_SEGMENTS; i++)
            {
                segmentPositions[i] = Projectile.Center + direction * (segmentLength * i);
                segmentOffsets[i] = 0f;
            }
            SoundEngine.PlaySound(SoundID.NPCHit53 with { Volume = 0.5f, Pitch = 0.3f });
        }

        private void UpdateSegments()
        {
            float time = Main.GameUpdateCount;
            float globalIntensity = (float)(Math.Sign(Math.Sin(time * 0.1f)) * 0.2f + Math.Sign(Math.Cos(time * 0.15f)) * 0.1f + 0.3f);
            for (int i = 1; i < MAX_SEGMENTS - 1; i++)
            {
                float centerEmphasis = (float)Math.Exp(-(Math.Pow(i - MAX_SEGMENTS / 2f, 2) / (2 * Math.Pow(MAX_SEGMENTS / 4f, 2)))) * 0.7f;
                float noise = (float)( Math.Sign(Math.Sin(time * 0.8f + i * 0.5f)) * 1.2f + Math.Sign(Math.Cos(time * 0.5f + i * 0.7f)) * 1.0f + (Math.Sin(time * 1.2f + i * 0.2f) > 0 ? 1 : -1) * globalIntensity * 1.8f) * centerEmphasis;
                if (Main.rand.NextBool(30) && i > MAX_SEGMENTS / 4 && i < MAX_SEGMENTS * 3 / 4)
                {
                    noise += Main.rand.NextFloat(-1f, 1f) * centerEmphasis;
                    if (Main.rand.NextBool(2))
                        noise *= 1.5f;
                }
                float finalAmplitude = Math.Min(5f, distanceToTarget * 0.06f);
                segmentOffsets[i] = noise * finalAmplitude;
                Vector2 normal = (segmentPositions[i + 1] - segmentPositions[i - 1]).RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
                Vector2 tangent = (segmentPositions[i + 1] - segmentPositions[i - 1]).SafeNormalize(Vector2.Zero);
                float tangentOffset = Math.Sign(Math.Sin(time * 0.6f + i * 0.8f)) * 0.7f * centerEmphasis;
                float suddenMultiplier = Main.rand.NextBool(20) ? 1.5f : 1f;
                segmentPositions[i] += (normal * segmentOffsets[i] + tangent * tangentOffset) * suddenMultiplier;
            }
            if (Main.rand.NextBool(6))
            {
                int segment = Main.rand.Next(MAX_SEGMENTS / 4, (MAX_SEGMENTS * 3) / 4);
                float displacementAmount = Main.rand.NextFloat(-4f, 4f);
                Vector2 normal = (segmentPositions[segment + 1] - segmentPositions[segment - 1]).RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
                segmentPositions[segment] += normal * displacementAmount;
                if (Main.rand.NextBool(2))
                {
                    int adjacentSegment = segment + (Main.rand.NextBool() ? 1 : -1);
                    if (adjacentSegment > 0 && adjacentSegment < MAX_SEGMENTS - 1)
                        segmentPositions[adjacentSegment] += normal * displacementAmount * 0.7f;
                }
            }
            if (Main.rand.NextFloat() < BRANCH_CHANCE && branches.Count < MAX_BRANCHES)
                CreateBranch();
        }

        private void CreateBranch()
        {
            int startSegment = Main.rand.Next(1, MAX_SEGMENTS - 2);
            int branchSegments = Main.rand.Next(3, 6);
            Branch branch = new()
            {
                Positions = new Vector2[branchSegments],
                Offsets = new float[branchSegments],
                Alpha = 0.7f,
                LifeTime = Main.rand.Next(10, 20)
            };
            Vector2 branchDirection = (segmentPositions[startSegment + 1] - segmentPositions[startSegment]).RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f));
            branchDirection.Normalize();
            for (int i = 0; i < branchSegments; i++)
            {
                branch.Positions[i] = segmentPositions[startSegment] + branchDirection * (i * 8);
                branch.Offsets[i] = 0f;
            }
            branches.Add(branch);
        }

        private void UpdateBranches()
        {
            for (int i = branches.Count - 1; i >= 0; i--)
            {
                Branch branch = branches[i];
                branch.LifeTime--;
                if (branch.LifeTime <= 0)
                {
                    branches.RemoveAt(i);
                    continue;
                }
                branch.Alpha *= 0.95f;
                float time = Main.GameUpdateCount;
                for (int j = 1; j < branch.Positions.Length - 1; j++)
                {
                    float noise = (float)(Math.Sin(time * 0.7f + j * 0.3f) * 1.5f + Math.Cos(time * 0.4f + j * 0.6f) * 1.0f);
                    branch.Offsets[j] = noise;
                    Vector2 normal = (branch.Positions[j + 1] - branch.Positions[j - 1]).RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
                    branch.Positions[j] += normal * (branch.Offsets[j] - branch.Offsets[j]);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (segmentPositions == null) return false;
            PixellationSystem.QueuePixelationAction(() =>
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D lineTexture = TextureAssets.MagicPixel.Value;
                Rectangle sourceRect = new(0, 0, 1, 1);
                float spawnProgress = 1f - (Projectile.timeLeft / 30f);
                float flashIntensity = (float)Math.Pow(1f - spawnProgress, 2);
                float energyPulse = (float)Math.Sin(Main.GameUpdateCount * 0.2f) * 0.3f + 0.7f;
                for (int i = 0; i < MAX_SEGMENTS - 1; i++)
                {
                    Vector2 start = (segmentPositions[i] - Main.screenPosition) / 2;
                    Vector2 end = (segmentPositions[i + 1] - Main.screenPosition) / 2;
                    Vector2 direction = end - start;
                    float distance = direction.Length();
                    float rotation = direction.ToRotation();
                    if (flashIntensity > 0)
                    {
                        spriteBatch.Draw(
                            lineTexture,
                            start,
                            sourceRect,
                            Color.Aqua * flashIntensity,
                            rotation,
                            new Vector2(0, 0.5f),
                            new Vector2(distance, 3f),
                            SpriteEffects.None,
                            0
                        );
                    }

                    //core beam
                    spriteBatch.Draw(
                        lineTexture,
                        start,
                        sourceRect,
                        Color.Yellow * alpha * energyPulse,
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 1f),
                        SpriteEffects.None,
                        0
                    );

                    //middle glow
                    spriteBatch.Draw(
                        lineTexture,
                        start,
                        sourceRect,
                        new Color(150, 220, 255) * (alpha * 0.5f * energyPulse),
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 2f),
                        SpriteEffects.None,
                        0
                    );

                    //outer glow
                    spriteBatch.Draw(
                        lineTexture,
                        start,
                        sourceRect,
                        new Color(100, 180, 255) * (alpha * 0.3f * energyPulse),
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 3f),
                        SpriteEffects.None,
                        0
                    );

                    //distortion
                    float distortionOffset = (float)Math.Sin(Main.GameUpdateCount * 0.8f + i * 0.5f);
                    spriteBatch.Draw(
                        lineTexture,
                        start + new Vector2(0, distortionOffset),
                        sourceRect,
                        new Color(200, 230, 255) * (alpha * 0.2f),
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(distance, 1.5f),
                        SpriteEffects.None,
                        0
                    );
                }

                Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/GlowTrailSlice").Value;
                for (int i = 0; i < MAX_SEGMENTS - 1; i++)
                {
                    Vector2 start = (segmentPositions[i] - Main.screenPosition) / 2;
                    Vector2 end = (segmentPositions[i + 1] - Main.screenPosition) / 2;
                    Vector2 direction = end - start;
                    float distance = direction.Length();
                    float rotation = direction.ToRotation();
                    float glowWidth = 0.4f * (1f + (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.1f);
                    Color glowColor = new Color(150, 220, 255) * (alpha * 0.2f);
                    for (int g = 0; g < 2; g++)
                    {
                        float offsetAngle = g * MathHelper.PiOver2;
                        Vector2 offset = new((float)Math.Cos(offsetAngle + Main.GameUpdateCount * 0.05f) * 0.5f, (float)Math.Sin(offsetAngle + Main.GameUpdateCount * 0.05f) * 0.5f);
                        spriteBatch.Draw(
                            glowTexture,
                            start + offset,
                            null,
                            glowColor * (1f - g * 0.3f),
                            rotation,
                            new Vector2(0, glowTexture.Height / 2f),
                            new Vector2(distance / (glowTexture.Width / 1f), glowWidth * (1f - g * 0.2f)),
                            SpriteEffects.None,
                            0
                        );
                    }
                }
                //tiny impact points
                void DrawImpactPoint(Vector2 position, float size)
                {
                    position = (position - Main.screenPosition) / 2;
                    float time = Main.GameUpdateCount * 0.1f;
                    float pulseSize = 1f + (float)Math.Sin(time) * 0.2f;

                    Texture2D starTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/CrispStarPMA").Value;

                    //rotating pixels
                    for (int i = 0; i < 4; i++)
                    {
                        float angle = i * MathHelper.PiOver2 + time;
                        Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * size * pulseSize;

                        spriteBatch.Draw(
                            lineTexture,
                            position + offset,
                            sourceRect,
                            new Color(150, 220, 255) * (alpha * 0.5f),
                            angle,
                            new Vector2(0.5f),
                            new Vector2(size * 0.25f, 1f),
                            SpriteEffects.None,
                            0
                        );
                    }

                    //first star
                    Color color1 = Color.Lerp(
                        new Color(0, 236, 255),
                        Color.White,
                        0.5f + (float)Math.Sin(time) * 0.2f
                    );
                    spriteBatch.Draw(
                        starTexture,
                        position,
                        null,
                        color1 * alpha,
                        time * 0.5f,
                        starTexture.Size() / 2f,
                        0.2f * pulseSize,
                        SpriteEffects.None,
                        0
                    );

                    //second star
                    Color color2 = Color.Lerp(
                        new Color(0, 255, 191),
                        Color.White,
                        0.3f + (float)Math.Sin(time * 1.5f) * 0.2f
                    );
                    spriteBatch.Draw(
                        starTexture,
                        position,
                        null,
                        color2 * alpha,
                        -time * 0.7f,
                        starTexture.Size() / 2f,
                        0.125f * pulseSize,
                        SpriteEffects.None,
                        0
                    );
                }

                DrawImpactPoint(segmentPositions[0], 4f);
                DrawImpactPoint(segmentPositions[MAX_SEGMENTS - 1], 4f);

                //draw branches
                foreach (Branch branch in branches)
                {
                    float branchEnergy = (float)Math.Sin(Main.GameUpdateCount * 0.3f) * 0.2f + 0.8f;

                    for (int i = 0; i < branch.Positions.Length - 1; i++)
                    {
                        Vector2 start = (branch.Positions[i] - Main.screenPosition) / 2;
                        Vector2 end = (branch.Positions[i + 1] - Main.screenPosition) / 2;
                        Vector2 direction = end - start;
                        float distance = direction.Length();
                        float rotation = direction.ToRotation();

                        //core
                        spriteBatch.Draw(
                            lineTexture,
                            start,
                            sourceRect,
                            Color.White * branch.Alpha * branchEnergy,
                            rotation,
                            new Vector2(0, 0.5f),
                            new Vector2(distance, 0.5f),
                            SpriteEffects.None,
                            0
                        );

                        //glow
                        spriteBatch.Draw(
                            lineTexture,
                            start,
                            sourceRect,
                            new Color(150, 220, 255) * (branch.Alpha * 0.3f * branchEnergy),
                            rotation,
                            new Vector2(0, 0.5f),
                            new Vector2(distance, 1f),
                            SpriteEffects.None,
                            0
                        );
                    }
                }
            }, PixellationSystem.RenderType.Additive);
            return false;
        }

        private void FindTargetPosition()
        {
            Point tileCoords = Projectile.Center.ToTileCoordinates();
            Vector2 direction = Projectile.velocity;
            direction.Normalize();

            for (int distance = 1; distance <= 10; distance++)
            {
                int checkX = tileCoords.X + (int)(direction.X * distance);
                int checkY = tileCoords.Y + (int)(direction.Y * distance);

                if (!WorldGen.InWorld(checkX, checkY)) break;

                Tile checkTile = Main.tile[checkX, checkY];
                if (checkTile.TileType == ModContent.TileType<LightningTrapTile>())
                {
                    targetPosition = new Vector2(checkX * 16 + 8, checkY * 16 + 8);
                    return;
                }
            }
            targetPosition = Projectile.Center + direction * 160;
        }
    }
}