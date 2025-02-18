using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Common.Utilities
{
    public static class CommonTileHelper
    {
        public static void SimpleFramedTile(this ModTile tile, int drop, SoundStyle soundType, int dustType, int minPick,
             bool mergeDirt = false, bool stone = false, params int[] tilesToMergeWith)
        {
            Main.tileBlockLight[tile.Type] = true;
            Main.tileLighted[tile.Type] = true;
            Main.tileSolid[tile.Type] = true;
            Main.tileMergeDirt[tile.Type] = mergeDirt;
            Main.tileStone[tile.Type] = stone;

            if (tilesToMergeWith != null)
            {
                foreach (int i in tilesToMergeWith)
                {
                    Main.tileMerge[tile.Type][i] = true;
                }
            }
            tile.RegisterItemDrop(drop);
            tile.HitSound = soundType;
            tile.DustType = dustType;
            tile.MinPick = minPick;
        }

        public static void SimpleFrameImportantTile(this ModTile tile, int width, int height, SoundStyle soundType, int dustType, Color mapColor,
             bool solid = false, bool solidTop = true, AnchorData anchorBottom = default, AnchorData anchorTop = default)
        {
            Main.tileFrameImportant[tile.Type] = true;
            Main.tileBlockLight[tile.Type] = true;
            Main.tileLavaDeath[tile.Type] = true;
            Main.tileNoAttach[tile.Type] = true;
            Main.tileSolid[tile.Type] = solid;
            Main.tileSolidTop[tile.Type] = solidTop;
            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;
            TileObjectData.newTile.CoordinateHeights = new int[height];
            for (int i = 0; i < height; i++)
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            if (anchorBottom != default)
                TileObjectData.newTile.AnchorBottom = anchorBottom;
            if (anchorTop != default)
                TileObjectData.newTile.AnchorTop = anchorTop;
            TileObjectData.addTile(tile.Type);
            tile.AddMapEntry(mapColor);
            tile.HitSound = soundType;
            tile.DustType = dustType;
        }

        public static void SimpleWall(this ModWall wall, int drop, SoundStyle soundType, int dustType, Color mapColor, bool house = false)
        {
            Main.wallHouse[wall.Type] = house;
            wall.HitSound = soundType;
            wall.DustType = dustType;
            wall.AddMapEntry(mapColor);
        }

        public static void SetupMultiTile(ModTile tile, int width, int height, int[] coordinateHeights, bool placeLeft = true, bool placeRight = true, int styleWrapLimit = 0, int styleMultiplier = 1, bool styleHorizontal = true)
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;
            TileObjectData.newTile.CoordinateHeights = coordinateHeights;
            if (placeLeft)
                TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            if (placeRight)
            {
                TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
                TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
                TileObjectData.addAlternate(1);
            }
            TileObjectData.newTile.StyleWrapLimit = styleWrapLimit;
            TileObjectData.newTile.StyleMultiplier = styleMultiplier;
            TileObjectData.newTile.StyleHorizontal = styleHorizontal;
            TileObjectData.addTile(tile.Type);
        }

        public static void HandleDresserRightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            int left = Main.tile[i, j].TileFrameX / 18;
            left %= 3;
            left = i - left;
            int top = j - Main.tile[i, j].TileFrameY / 18;
            if (Main.tile[i, j].TileFrameY == 0)
            {
                Main.CancelClothesWindow(true);
                Main.mouseRightRelease = false;
                player.CloseSign();
                player.SetTalkNPC(-1);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = "";
                if (Main.editChest)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    Main.editChest = false;
                    Main.npcChatText = string.Empty;
                }
                if (player.editedChestName)
                {
                    NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                    player.editedChestName = false;
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    if (left == player.chestX && top == player.chestY && player.chest != -1)
                    {
                        player.chest = -1;
                        Recipe.FindRecipes();
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
                        Main.stackSplit = 600;
                    }
                }
                else
                {
                    player.piggyBankProjTracker.Clear();
                    player.voidLensChest.Clear();
                    int chestIndex = Chest.FindChest(left, top);
                    if (chestIndex != -1)
                    {
                        Main.stackSplit = 600;
                        if (chestIndex == player.chest)
                        {
                            player.chest = -1;
                            Recipe.FindRecipes();
                            SoundEngine.PlaySound(SoundID.MenuClose);
                        }
                        else if (chestIndex != player.chest && player.chest == -1)
                        {
                            player.OpenChest(left, top, chestIndex);
                            SoundEngine.PlaySound(SoundID.MenuOpen);
                        }
                        else
                        {
                            player.OpenChest(left, top, chestIndex);
                            SoundEngine.PlaySound(SoundID.MenuTick);
                        }
                        Recipe.FindRecipes();
                    }
                }
            }
            else
            {
                Main.playerInventory = false;
                player.chest = -1;
                Recipe.FindRecipes();
                player.SetTalkNPC(-1);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = "";
                Main.interactedDresserTopLeftX = left;
                Main.interactedDresserTopLeftY = top;
                Main.OpenClothesWindow();
            }
        }

        private static string MapDresserName(string name, int i, int j)
        {
            int left = i;
            int top = j;
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX % 36 != 0)
                left--;
            if (tile.TileFrameY != 0)
                top--;
            int chest = Chest.FindChest(left, top);
            if (chest < 0)
                return Language.GetTextValue("LegacyDresserType.0");
            if (Main.chest[chest].name == "")
                return name;
            return name + ": " + Main.chest[chest].name;
        }

        public static bool HandleClockRightClick(int x, int y)
        {
            string period = "AM";
            double time = Main.time;
            if (!Main.dayTime)
                time += 54000.0;

            time = (time / 86400.0) * 24.0;
            time = time - 7.5 - 12.0;

            if (time < 0.0)
                time += 24.0;

            if (time >= 12.0)
                period = "PM";
            int intTime = (int)time;
            double deltaTime = time - intTime;
            deltaTime = (int)(deltaTime * 60.0);
            string minutes = deltaTime < 10.0 ? "0" + deltaTime : deltaTime.ToString();
            if (intTime > 12)
                intTime -= 12;

            if (intTime == 0)
                intTime = 12;
            Main.NewText($"Time: {intTime}:{minutes} {period}", 255, 240, 20);
            return true;
        }

        public static bool HandleRightClick(ModTile modTile, int i, int j, Player player, int keyItemId)
        {
            Tile tile = Main.tile[i, j];
            Main.mouseRightRelease = false;
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0)
                left--;
            if (tile.TileFrameY != 0)
                top--;
            player.CloseSign();
            player.SetTalkNPC(-1);
            Main.npcChatCornerItem = 0;
            Main.npcChatText = "";
            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = string.Empty;
            }
            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                player.editedChestName = false;
            }
            bool isLocked = Chest.IsLocked(left, top);
            if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked)
            {
                if (left == player.chestX && top == player.chestY && player.chest != -1)
                {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
                else
                {
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
                    Main.stackSplit = 600;
                }
            }
            else
            {
                if (isLocked)
                {
                    if (player.HasItemInInventoryOrOpenVoidBag(keyItemId) && Chest.Unlock(left, top) && player.ConsumeItem(keyItemId, includeVoidBag: true))
                    {
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.LockAndUnlock, -1, -1, null, player.whoAmI, 1f, left, top);
                        }
                    }
                }
                else
                {
                    int chest = Chest.FindChest(left, top);
                    if (chest != -1)
                    {
                        Main.stackSplit = 600;
                        if (chest == player.chest)
                        {
                            player.chest = -1;
                            SoundEngine.PlaySound(SoundID.MenuClose);
                        }
                        else
                        {
                            SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                            player.OpenChest(left, top, chest);
                        }
                        Recipe.FindRecipes();
                    }
                }
            }
            return true;
        }

        public static void HandleMouseOver(ModTile modTile, int i, int j, int itemType, int keyItemId = -1)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0)
                left--;

            if (tile.TileFrameY != 0)
                top--;

            int chest = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chest < 0)
                player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
            else
            {
                string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
                if (player.cursorItemIconText == defaultName)
                {
                    player.cursorItemIconID = itemType;
                    if (keyItemId != -1 && Main.tile[left, top].TileFrameX / 36 == 1)
                    {
                        player.cursorItemIconID = keyItemId;
                    }
                    player.cursorItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public static void HandleMouseOverNearAndFarSharedLogic(Player player, int i, int j, int itemType)
        {
            Tile tile = Main.tile[i, j];
            int left = i;
            int top = j;
            left -= tile.TileFrameX % 54 / 18;
            if (tile.TileFrameY % 36 != 0)
                top--;
            int chestIndex = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chestIndex < 0)
                player.cursorItemIconText = Language.GetTextValue("LegacyDresserType.0");
            else
            {
                string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);
                if (Main.chest[chestIndex].name != "")
                    player.cursorItemIconText = Main.chest[chestIndex].name;
                else
                    player.cursorItemIconText = defaultName;
                if (player.cursorItemIconText == defaultName)
                {
                    player.cursorItemIconID = itemType;
                    player.cursorItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public static void HandleChairRightClick(ModTile modTile, int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }
        }

        public static void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info, int nextStyleHeight)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            info.TargetDirection = tile.TileFrameX >= 18 ? 1 : -1;
            info.AnchorTilePosition.X = i;
            info.AnchorTilePosition.Y = j;
            if (tile.TileFrameY % nextStyleHeight == 0)
                info.AnchorTilePosition.Y++;
        }



        public static void SetupCampfire(ModTile modTile, Color mapColor, int itemDropType, float lightR, float lightG, float lightB, string flameTexturePath, ref Asset<Texture2D> flameTexture)
        {
            TileID.Sets.InteractibleByNPCs[modTile.Type] = true;
            TileID.Sets.Campfire[modTile.Type] = true;

            modTile.DustType = -1;
            modTile.AdjTiles = [TileID.Campfire];

            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Campfire, 0));
            TileObjectData.newTile.StyleLineSkip = 9;
            TileObjectData.addTile(modTile.Type);

            lightR = 1f;
            lightG = 0.75f;
            lightB = 1f;

            modTile.AddMapEntry(mapColor, Language.GetText("ItemName.Campfire"));

            flameTexture = ModContent.Request<Texture2D>(flameTexturePath);
        }

        public static void ToggleTile(int i, int j, int frameHeight = 36, int frameWidth = 54)
        {
            Tile tile = Main.tile[i, j];
            int topX = i - tile.TileFrameX % frameWidth / 18;
            int topY = j - tile.TileFrameY % frameHeight / 18;

            short frameAdjustment = (short)(tile.TileFrameY >= frameHeight ? -frameHeight : frameHeight);

            for (int x = topX; x < topX + 3; x++)
            {
                for (int y = topY; y < topY + 2; y++)
                {
                    Main.tile[x, y].TileFrameY += frameAdjustment;

                    if (Wiring.running)
                    {
                        Wiring.SkipWire(x, y);
                    }
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, topX, topY, 3, 2);
            }
        }

        public static void CreateCampfireDust(int i, int j, int frameHeight = 36)
        {
            if (Main.gamePaused || !Main.instance.IsActive) return;

            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY == 0 && Main.rand.NextBool(3) && ((Main.drawToScreen && Main.rand.NextBool(4)) || !Main.drawToScreen))
            {
                Dust dust = Dust.NewDustDirect(new Vector2(i * 16 + 2, j * 16 - 4), 4, 8, DustID.Smoke, 0f, 0f, 100);
                if (tile.TileFrameX == 0) dust.position.X += Main.rand.Next(8);
                if (tile.TileFrameX == 36) dust.position.X -= Main.rand.Next(8);
                dust.alpha += Main.rand.Next(100);
                dust.velocity *= 0.2f;
                dust.velocity.Y -= 0.5f + Main.rand.Next(10) * 0.1f;
                dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;
            }
        }





        public static bool HandleBedRightClick(int i, int j, int bedItemType)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int spawnX = (i - (tile.TileFrameX / 18)) + (tile.TileFrameX >= 72 ? 5 : 2);
            int spawnY = j + 2;

            if (tile.TileFrameY % 38 != 0)
                spawnY--;

            if (!Player.IsHoveringOverABottomSideOfABed(i, j))
            {
                if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
                {
                    player.GamepadEnableGrappleCooldown();
                    player.sleeping.StartSleeping(player, i, j);
                }
            }
            else
            {
                player.FindSpawn();
                if (player.SpawnX == spawnX && player.SpawnY == spawnY)
                {
                    player.RemoveSpawn();
                    Main.NewText(Language.GetTextValue("Game.SpawnPointRemoved"), 255, 240, 20);
                }
                else if (Player.CheckSpawn(spawnX, spawnY))
                {
                    player.ChangeSpawn(spawnX, spawnY);
                    Main.NewText(Language.GetTextValue("Game.SpawnPointSet"), 255, 240, 20);
                }
            }
            return true;
        }

        public static void HandleBedMouseOver(int i, int j, int bedItemType)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            if (!Player.IsHoveringOverABottomSideOfABed(i, j))
            {
                if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
                {
                    player.cursorItemIconID = ItemID.SleepingIcon;
                }
            }
            else
            {
                player.cursorItemIconID = bedItemType;
            }
        }

        public static void ModifyBedSleepingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            info.VisualOffset.Y += 4f;
        }

        public static void SetupCommonProperties(ModTile modTile, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal, bool isChair)
        {
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = lavaDeath;
            Main.tileWaterDeath[modTile.Type] = waterDeath;
            TileObjectData.newTile.LavaPlacement = lavaDeath ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.WaterPlacement = waterDeath ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.StyleHorizontal = styleHorizontal;
            modTile.DustType = dustType;
            modTile.RegisterItemDrop(itemDropType);
            if (isChair)
            {
                TileID.Sets.CanBeSatOnForNPCs[modTile.Type] = true;
                TileID.Sets.CanBeSatOnForPlayers[modTile.Type] = true;
                TileID.Sets.DisableSmartCursor[modTile.Type] = true;
                TileID.Sets.HasOutlines[modTile.Type] = true;
            }
            TileObjectData.addTile(modTile.Type);
        }

        private static void SetupTileDataProperties(TileObjectData tileData, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            tileData.LavaPlacement = lavaDeath ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            tileData.WaterPlacement = waterDeath ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            tileData.StyleHorizontal = styleHorizontal;
        }

        public static void SetupPlatform(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Platform"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
            modTile.AdjTiles = [TileID.Platforms];
            Main.tileLighted[modTile.Type] = true;
            Main.tileSolidTop[modTile.Type] = true;
            Main.tileSolid[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileTable[modTile.Type] = true;
            TileID.Sets.Platforms[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileObjectData.newTile.CoordinateHeights = [16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 27;
            TileObjectData.newTile.StyleWrapLimit = 27;
            TileObjectData.newTile.UsesCustomCanPlace = false;
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal: false, isChair: false);
        }

        public static void SetupWorkbench(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.WorkBench"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            modTile.AdjTiles = [TileID.WorkBenches];
            Main.tileTable[modTile.Type] = true;
            Main.tileSolidTop[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.CoordinateHeights = [18];
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupCandle(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("ItemName.Candle"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            modTile.AdjTiles = [TileID.Candles];
            Main.tileLighted[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinateHeights = [20];
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.StyleLineSkip = 2;
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupLantern(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(new Color(251, 235, 127), Language.GetText("MapObject.Lantern"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            modTile.AdjTiles = [TileID.HangingLanterns];
            Main.tileLighted[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.DrawYOffset = -10;
            TileObjectData.addAlternate(0);
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupLamp(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.FloorLamp"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            modTile.AdjTiles = [TileID.Lamps];
            Main.tileLighted[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.StyleLineSkip = 2;
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupCandelabra(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(new Color(253, 221, 3), Language.GetText("MapObject.Candelabra"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            modTile.AdjTiles = [TileID.Candelabras];
            Main.tileLighted[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.StyleLineSkip = 2;
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupChandelier(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Chandelier"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            modTile.AdjTiles = [TileID.Chandeliers];
            Main.tileLighted[modTile.Type] = true;
            TileID.Sets.MultiTileSway[modTile.Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newTile.StyleLineSkip = 2;
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupTorch(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            TileObjectData.newTile = new TileObjectData();
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = styleHorizontal;
            TileObjectData.newTile.LavaDeath = lavaDeath;
            TileObjectData.newTile.WaterDeath = waterDeath;
            TileObjectData.newTile.WaterPlacement = waterDeath ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaPlacement = lavaDeath ? LiquidPlacement.Allowed : LiquidPlacement.NotAllowed;
            TileObjectData.newTile.DrawYOffset = 0;

            TileObjectData.addTile(modTile.Type);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.addAlternate(1);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.addAlternate(2);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorWall = true;
            TileObjectData.addAlternate(0);

            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Torch"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            modTile.AdjTiles = new int[] { TileID.Torches };
            Main.tileLighted[modTile.Type] = true;
            Main.tileSolid[modTile.Type] = false;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileNoFail[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileID.Sets.Torch[modTile.Type] = true;
            TileID.Sets.FramesOnKillWall[modTile.Type] = true;

            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = lavaDeath;
            Main.tileWaterDeath[modTile.Type] = waterDeath;
            modTile.DustType = dustType;
            modTile.RegisterItemDrop(itemDropType);
        }

        public static void SetupChair(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Chair"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            modTile.AdjTiles = [TileID.Chairs];
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: true);
        }

        public static void SetupToilet(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Toilet"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            modTile.AdjTiles = [TileID.Chairs];
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: true);
        }

        public static void SetupSofa(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal, bool isBench)
        {
            modTile.AddMapEntry(mapColor, isBench ? Language.GetText("ItemName.Bench") : Language.GetText("ItemName.Sofa"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            Main.tileLighted[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: true);
        }


        public static void SetupChest(ModTile modTile, Color mapColor, string chestName, int itemDropType, int dustType, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText(chestName));
            modTile.AdjTiles = [TileID.Containers];
            Main.tileSpelunker[modTile.Type] = true;
            Main.tileContainer[modTile.Type] = true;
            Main.tileShine2[modTile.Type] = true;
            Main.tileShine[modTile.Type] = 1200;
            Main.tileOreFinderPriority[modTile.Type] = 500;
            TileID.Sets.HasOutlines[modTile.Type] = true;
            TileID.Sets.BasicChest[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileID.Sets.AvoidedByNPCs[modTile.Type] = true;
            TileID.Sets.InteractibleByNPCs[modTile.Type] = true;
            TileID.Sets.IsAContainer[modTile.Type] = true;
            TileID.Sets.FriendlyFairyCanLureTo[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = [TileID.MagicalIceBlock, TileID.Boulder, TileID.BouncyBoulder, TileID.LifeCrystalBoulder, TileID.RollingCactus];
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath: false, waterDeath: false, styleHorizontal, isChair: false);
        }

        public static void SetupDresser(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, modTile.CreateMapEntryName(), MapDresserName);
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            modTile.AdjTiles = [TileID.Dressers];
            Main.tileSolidTop[modTile.Type] = true;
            Main.tileTable[modTile.Type] = true;
            Main.tileContainer[modTile.Type] = true;
            TileID.Sets.HasOutlines[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileID.Sets.BasicDresser[modTile.Type] = true;
            TileID.Sets.AvoidedByNPCs[modTile.Type] = true;
            TileID.Sets.InteractibleByNPCs[modTile.Type] = true;
            TileID.Sets.IsAContainer[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = [TileID.MagicalIceBlock, TileID.Boulder, TileID.BouncyBoulder, TileID.LifeCrystalBoulder, TileID.RollingCactus];
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupPiano(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Piano"));
            modTile.AdjTiles = [TileID.Pianos];
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            Main.tileTable[modTile.Type] = true;
            Main.tileSolidTop[modTile.Type] = true;
            Main.tileLighted[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupClock(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("ItemName.GrandfatherClock"));
            modTile.AdjTiles = [TileID.GrandfatherClocks];
            TileID.Sets.HasOutlines[modTile.Type] = true;
            TileID.Sets.Clock[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupBed(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("ItemName.Bed"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            modTile.AdjTiles = [TileID.Beds];
            TileID.Sets.HasOutlines[modTile.Type] = true;
            TileID.Sets.CanBeSleptIn[modTile.Type] = true;
            TileID.Sets.InteractibleByNPCs[modTile.Type] = true;
            TileID.Sets.IsValidSpawnPoint[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupClosedDoor(ModTile modTile, int openDoorType, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Door"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
            modTile.AdjTiles = [TileID.ClosedDoor];
            Main.tileBlockLight[modTile.Type] = true;
            Main.tileSolid[modTile.Type] = true;
            TileID.Sets.NotReallySolid[modTile.Type] = true;
            TileID.Sets.DrawsWalls[modTile.Type] = true;
            TileID.Sets.HasOutlines[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileID.Sets.OpenDoorID[modTile.Type] = openDoorType;     
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.ClosedDoor, 0));
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }
        public static void SetupOpenDoor(ModTile modTile, int closedDoorType, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileSolid[modTile.Type] = false;
            Main.tileLavaDeath[modTile.Type] = true;
            Main.tileNoSunLight[modTile.Type] = true;
            TileID.Sets.HousingWalls[modTile.Type] = true;
            TileID.Sets.HasOutlines[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileID.Sets.CloseDoorID[modTile.Type] = closedDoorType;
            TileID.Sets.DrawTileInSolidLayer[modTile.Type] = true;

            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
            modTile.DustType = dustType;
            modTile.AdjTiles = [TileID.OpenDoor];
            modTile.RegisterItemDrop(itemDropType);

            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Door"));

            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.addAlternate(0);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 2);
            TileObjectData.addAlternate(0);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 0);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 1);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 2);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(modTile.Type);
        }
        public static void SetupSink(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal, bool water = false, bool honey = false, bool lava = false)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Sink"));
            modTile.AdjTiles = [TileID.Sinks];
            Main.tileLighted[modTile.Type] = true;
            TileID.Sets.CountsAsWaterSource[modTile.Type] = water;
            TileID.Sets.CountsAsLavaSource[modTile.Type] = lava;
            TileID.Sets.CountsAsHoneySource[modTile.Type] = honey;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupBookcase(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Bookcase"));
            modTile.AdjTiles = [TileID.Bookcases];
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void SetupTable(ModTile modTile, Color mapColor, int itemDropType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Table"));
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            modTile.AdjTiles = [TileID.Tables];
            Main.tileTable[modTile.Type] = true;
            Main.tileSolidTop[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[modTile.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            SetupCommonProperties(modTile, itemDropType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }


        public static void SetupBathtub(ModTile modTile, Color mapColor, int itemType, int dustType, bool lavaDeath, bool waterDeath, bool styleHorizontal)
        {
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Bathtub"));
            modTile.AdjTiles = [TileID.Bathtubs];
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.newTile.CoordinateHeights = [16, 16];
            SetupCommonProperties(modTile, itemType, dustType, lavaDeath, waterDeath, styleHorizontal, isChair: false);
        }

        public static void PlatformHangOffset(int i, int j, ref int offsetY)
        {
            Tile tile = Main.tile[i, j];
            TileObjectData data = TileObjectData.GetTileData(tile);
            int topLeftX = i - tile.TileFrameX / 18 % data.Width;
            int topLeftY = j - tile.TileFrameY / 18 % data.Height;
            if (WorldGen.IsBelowANonHammeredPlatform(topLeftX, topLeftY))
                offsetY -= 8;
        }

        public static void DrawChandelierSway(int i, int j, int width, int height, Asset<Texture2D> flameTexture, Color flameColor, float jitterMultX = 0.15f, float jitterMultY = 0.35f)
        {
            Tile tile = Main.tile[i, j];
            if (!tile.HasTile) return;
            float windCycle = (float)Math.Sin(i * 0.25 + j * 0.75 + Main.GameUpdateCount * 0.07f) * 1.2f;
            float rotation = windCycle * 0.1f;
            Vector2 screenPosition = Main.Camera.ScaledPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int x = i; x < i + width; x++)
            {
                for (int y = j; y < j + height; y++)
                {
                    Tile currentTile = Main.tile[x, y];
                    if (!currentTile.HasTile || currentTile.TileType != tile.TileType) continue;
                    Texture2D tileTexture = Main.instance.TilesRenderer.GetTileDrawTexture(currentTile, x, y);
                    Rectangle sourceRect = new(currentTile.TileFrameX, currentTile.TileFrameY, 16, 16);
                    Vector2 drawPos = new Vector2(x * 16, y * 16) - screenPosition + new Vector2(8, 16);
                    spriteBatch.Draw(tileTexture, drawPos, sourceRect, Lighting.GetColor(x, y), rotation, new Vector2(8, 16), 1f, SpriteEffects.None, 0f);
                    if (flameTexture != null)
                    {
                        ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)y << 32 | (uint)x);
                        for (int k = 0; k < 7; k++)
                        {
                            Vector2 jitter = new Vector2(
                                Utils.RandomInt(ref randSeed, -10, 11) * jitterMultX,
                                Utils.RandomInt(ref randSeed, -10, 1) * jitterMultY
                            );
                            spriteBatch.Draw(flameTexture.Value, drawPos + jitter, sourceRect, flameColor, rotation, new Vector2(8, 16), 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
        }

        public static void HandleFlameDraw(Tile tile, int i, int j, SpriteBatch spriteBatch, Asset<Texture2D> flameTexture, int offsetX = 0, int offsetY = 0)
        {
            if (tile == null || !tile.HasTile) return;

            try
            {
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                int width = 16;
                int height = 16;

                int yOffset = 0;
                var tileData = TileObjectData.GetTileData(tile);
                if (tileData != null)
                {
                    yOffset = tileData.DrawYOffset;
                }

                ulong randShakeEffect = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
                float drawPositionX = i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f;
                float drawPositionY = j * 16 - (int)Main.screenPosition.Y;

                for (int c = 0; c < 7; c++)
                {
                    float shakeX = Utils.RandomInt(ref randShakeEffect, -10, 11) * 0.15f;
                    float shakeY = Utils.RandomInt(ref randShakeEffect, -10, 1) * 0.35f;
                    spriteBatch.Draw(
                        flameTexture.Value,
                        new Vector2(drawPositionX + shakeX, drawPositionY + shakeY + yOffset) + zero,
                        new Rectangle(tile.TileFrameX + offsetX, tile.TileFrameY + offsetY, width, height),
                        new Color(100, 100, 100, 0),
                        0f,
                        default,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
            catch
            {
                return;
            }
        }

        public static void HandleFlameDust(int dustType, int rarity, int i, int j)
        {
            if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)))
            {
                if (Main.rand.NextBool(rarity))
                {
                    int dust = Dust.NewDust(new Vector2(i * 16 + 4, j * 16 + 2), 4, 4, dustType, 0f, 0f, 100, default, 1f);
                    if (!Main.rand.NextBool(3))
                        Main.dust[dust].noGravity = true;
                    Main.dust[dust].noLightEmittence = true;
                    Main.dust[dust].velocity *= 0.3f;
                    Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1.5f;
                }
            }
        }

        public static void ModifyTorchLight(int i, int j, ref float r, ref float g, ref float b, float torchLightR = 0.9f, float torchLightG = 0.9f, float torchLightB = 0.9f)
        {
            Tile tile = Main.tile[i, j];

            if (tile.TileFrameX < 66)
            {
                r = torchLightR;
                g = torchLightG;
                b = torchLightB;
            }
        }

        public static float GetTorchLuck(Player player, ModBiome specificBiome, float positiveLuck, float negativeLuck)
        {
            bool inSpecificBiome = player.InModBiome(specificBiome);
            return inSpecificBiome ? positiveLuck : negativeLuck;
        }

        public static void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = WorldGen.SolidTile(i, j - 1) ? 4 : 0;
        }

        public static void HandlePostDraw(Asset<Texture2D> flameTexture, int i, int j, SpriteBatch spriteBatch, bool isOn, int flameWidth = 16, int flameHeight = 16, int frameSize = 18, bool applyRandomOffset = false)
        {
            Tile tile = Main.tile[i, j];
            if (!TileDrawing.IsVisible(tile)) return;
            if (tile.TileFrameX % (frameSize * 3) != frameSize &&
                tile.TileFrameY % (frameSize * 3) != frameSize)
                return;
            if (isOn)
            {
                Color color = new Color(255, 255, 255, 0);
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Vector2 position = zero + new Vector2(
                    i * 16 - (int)Main.screenPosition.X - flameWidth / 2 + 8,
                    j * 16 - (int)Main.screenPosition.Y - flameHeight / 2 + 8
                );
                Rectangle drawRectangle = new(0, 0, flameWidth, flameHeight);
                if (applyRandomOffset)
                {
                    ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
                    for (int k = 0; k < 7; k++)
                    {
                        float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                        float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
                        spriteBatch.Draw(flameTexture.Value, position + new Vector2(xx, yy), drawRectangle, color, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                    }
                }
                else
                    spriteBatch.Draw(flameTexture.Value, position, drawRectangle, color, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
        }



        public static void HandleHitWire(int i, int j, int tileWidth, int tileHeight, bool isToilet = false)
        {
            Tile tile = Main.tile[i, j];
            if (!tile.HasTile) return;
            int originX = i - (tile.TileFrameX / 18) % tileWidth;
            int originY = j - (tile.TileFrameY / 18) % tileHeight;
            short frameAdjustment = 54;
            for (int x = originX; x < originX + tileWidth; x++)
            {
                for (int y = originY; y < originY + tileHeight; y++)
                {
                    if (Main.tile[x, y].TileType == tile.TileType)
                    {
                        if (Main.tile[x, y].TileFrameX < frameAdjustment)
                        {
                            Main.tile[x, y].TileFrameX += frameAdjustment;
                        }
                        else
                        {
                            Main.tile[x, y].TileFrameX -= frameAdjustment;
                        }
                    }
                }
            }

            if (isToilet && Wiring.CheckMech(originX, originY, 60))
            {
                Projectile.NewProjectile(Wiring.GetProjectileSource(originX, originY), originX * 16 + (tileWidth * 16 / 2), originY * 16 + 12, 0f, 0f, ProjectileID.ToiletEffect, 0, 0f, Main.myPlayer);
            }

            if (Wiring.running)
            {
                for (int x = originX; x < originX + tileWidth; x++)
                {
                    for (int y = originY; y < originY + tileHeight; y++)
                    {
                        Wiring.SkipWire(x, y);

                    }
                }
            }

            NetMessage.SendTileSquare(-1, originX + tileWidth / 2, originY + tileHeight / 2, Math.Max(tileWidth, tileHeight));
        }


        public static void SetupDecorativeMultiTile(ModTile modTile, string mapEntryKey, Color mapColor, int widthInTiles, int heightInTiles, int itemDropType, int dustType = DustID.Smoke)
        {
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = true;
            modTile.DustType = dustType;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Width = widthInTiles;
            TileObjectData.newTile.Height = heightInTiles;
            TileObjectData.newTile.CoordinateHeights = new int[heightInTiles];
            for (int i = 0; i < heightInTiles; i++)
            {
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            }
            TileObjectData.addTile(modTile.Type);
            modTile.AddMapEntry(mapColor, Language.GetText(mapEntryKey));
            modTile.RegisterItemDrop(itemDropType);
        }

        public static void HandleToiletInteraction(ModTile modTile, int i, int j, Player player)
        {
            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }
        }

        public static void HandleMouseOver(ModTile modTile, int i, int j, int itemType)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = itemType;
        }
    }
}