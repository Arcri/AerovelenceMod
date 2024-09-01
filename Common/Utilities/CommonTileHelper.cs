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
            {
                TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            }

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

        public static void SetBasicTileProperties(ModTile modTile, bool hasOutlines = true, bool canBeSatOnForNPCs = false, bool canBeSatOnForPlayers = false, bool disableSmartCursor = true, bool isContainer = false)
        {
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = true;
            Main.tileWaterDeath[modTile.Type] = true;

            if (hasOutlines)
                TileID.Sets.HasOutlines[modTile.Type] = true;

            if (canBeSatOnForNPCs)
                TileID.Sets.CanBeSatOnForNPCs[modTile.Type] = true;

            if (canBeSatOnForPlayers)
                TileID.Sets.CanBeSatOnForPlayers[modTile.Type] = true;

            if (disableSmartCursor)
                TileID.Sets.DisableSmartCursor[modTile.Type] = true;

            if (isContainer)
            {
                Main.tileContainer[modTile.Type] = true;
                TileID.Sets.IsAContainer[modTile.Type] = true;
            }
        }

        public static void SetupChair(ModTile modTile, int dustType, int itemDropType, Color mapColor)
        {
            SetBasicTileProperties(modTile, hasOutlines: true, canBeSatOnForNPCs: true, canBeSatOnForPlayers: true);

            modTile.DustType = dustType;
            modTile.AdjTiles = [TileID.Chairs];

            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Chair"));

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(modTile.Type);
        }

        public static void SetupDresser(ModTile modTile, int dustType, int itemDropType, Color mapColor)
        {
            SetBasicTileProperties(modTile, hasOutlines: true, canBeSatOnForNPCs: true, canBeSatOnForPlayers: true);

            TileID.Sets.BasicDresser[modTile.Type] = true;
            TileID.Sets.AvoidedByNPCs[modTile.Type] = true;
            TileID.Sets.InteractibleByNPCs[modTile.Type] = true;
            TileID.Sets.IsAContainer[modTile.Type] = true;
            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

            modTile.DustType = dustType;
            modTile.AdjTiles = [TileID.Dressers];

            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Dresser"));

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles =
            [
                TileID.MagicalIceBlock,
                TileID.Boulder,
                TileID.BouncyBoulder,
                TileID.LifeCrystalBoulder,
                TileID.RollingCactus
            ];
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(modTile.Type);

            modTile.RegisterItemDrop(itemDropType);
        }



        public static void SetupChest(ModTile modTile, int dustType, int itemDropType, Color mapColor, string chestName)
        {
            Main.tileSpelunker[modTile.Type] = true;
            Main.tileContainer[modTile.Type] = true;
            Main.tileShine2[modTile.Type] = true;
            Main.tileShine[modTile.Type] = 1200;
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileOreFinderPriority[modTile.Type] = 500;
            TileID.Sets.HasOutlines[modTile.Type] = true;
            TileID.Sets.BasicChest[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileID.Sets.AvoidedByNPCs[modTile.Type] = true;
            TileID.Sets.InteractibleByNPCs[modTile.Type] = true;
            TileID.Sets.IsAContainer[modTile.Type] = true;
            TileID.Sets.FriendlyFairyCanLureTo[modTile.Type] = true;

            modTile.DustType = dustType;
            modTile.AdjTiles = [TileID.Containers];

            modTile.AddMapEntry(mapColor, Language.GetText(chestName));

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles =
            [
                TileID.MagicalIceBlock,
                TileID.Boulder,
                TileID.BouncyBoulder,
                TileID.LifeCrystalBoulder,
                TileID.RollingCactus
            ];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(modTile.Type);

            modTile.RegisterItemDrop(itemDropType);
        }

        public static void SetupClock(ModTile modTile, int dustType, Color mapColor)
        {
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = true;
            TileID.Sets.HasOutlines[modTile.Type] = true;
            TileID.Sets.Clock[modTile.Type] = true;

            modTile.DustType = dustType;
            modTile.AdjTiles = [TileID.GrandfatherClocks];

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
            TileObjectData.addTile(modTile.Type);

            modTile.AddMapEntry(mapColor, Language.GetText("ItemName.GrandfatherClock"));
        }

        public static bool HandleClockRightClick(int x, int y)
        {
            string period = "AM";
            double time = Main.time;

            if (!Main.dayTime)
            {
                time += 54000.0;
            }

            time = (time / 86400.0) * 24.0;
            time = time - 7.5 - 12.0;

            if (time < 0.0)
            {
                time += 24.0;
            }

            if (time >= 12.0)
            {
                period = "PM";
            }

            int intTime = (int)time;
            double deltaTime = time - intTime;
            deltaTime = (int)(deltaTime * 60.0);
            string minutes = deltaTime < 10.0 ? "0" + deltaTime : deltaTime.ToString();

            if (intTime > 12)
            {
                intTime -= 12;
            }

            if (intTime == 0)
            {
                intTime = 12;
            }

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
            {
                left--;
            }

            if (tile.TileFrameY != 0)
            {
                top--;
            }

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
            {
                left--;
            }

            if (tile.TileFrameY != 0)
            {
                top--;
            }

            int chest = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chest < 0)
            {
                player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
            }
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
            {
                top--;
            }
            int chestIndex = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chestIndex < 0)
            {
                player.cursorItemIconText = Language.GetTextValue("LegacyDresserType.0");
            }
            else
            {
                string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);

                if (Main.chest[chestIndex].name != "")
                {
                    player.cursorItemIconText = Main.chest[chestIndex].name;
                }
                else
                {
                    player.cursorItemIconText = defaultName;
                }
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

            info.TargetDirection = -1;
            if (tile.TileFrameX != 0)
            {
                info.TargetDirection = 1;
            }

            info.AnchorTilePosition.X = i;
            info.AnchorTilePosition.Y = j;

            if (tile.TileFrameY % nextStyleHeight == 0)
            {
                info.AnchorTilePosition.Y++;
            }
        }


        public static void SetupCandle(ModTile modTile, Color mapColor, int itemDropType, float lightR, float lightG, float lightB, string flameTexturePath, ref Asset<Texture2D> flameTexture)
        {
            Main.tileLighted[modTile.Type] = true;
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileWaterDeath[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = true;

            modTile.DustType = DustID.t_Slime;
            modTile.AdjTiles = [TileID.Candles];

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorValidTiles = [TileID.Stone, TileID.Dirt];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(modTile.Type);

            modTile.AddMapEntry(mapColor, Language.GetText("ItemName.Candle"));

            modTile.RegisterItemDrop(itemDropType);


            lightR = 1f;
            lightG = 0.75f;
            lightB = 1f;

            flameTexture = ModContent.Request<Texture2D>(flameTexturePath);
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

        public static void SetupBookcase(ModTile modTile, Color mapColor, int itemDropType, int dustType = DustID.WoodFurniture)
        {
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = true;

            modTile.DustType = dustType;
            modTile.AdjTiles = [TileID.Bookcases];

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(modTile.Type);

            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Bookcase"));

            modTile.RegisterItemDrop(itemDropType);
        }

        public static void SetupBathtub(ModTile modTile, Color mapColor, int itemDropType, int dustType = DustID.WoodFurniture)
        {
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = true;

            modTile.DustType = dustType;
            modTile.AdjTiles = [TileID.Bathtubs];

            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = [16, 16];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(modTile.Type);

            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Bathtub"));

            modTile.RegisterItemDrop(itemDropType);
        }

        public static void SetupLamp(ModTile modTile, Color mapColor, int itemDropType, float lightR, float lightG, float lightB, string flameTexturePath, ref Asset<Texture2D> flameTexture)
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.WaterDeath = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.addTile(modTile.Type);

            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.FloorLamp"));

            modTile.RegisterItemDrop(itemDropType);

            flameTexture = ModContent.Request<Texture2D>(flameTexturePath);
        }

        public static void SetupTorchTile(ModTile modTile, string flameTexturePath, ref Asset<Texture2D> flameTexture, Color mapColor, int dustType, int[] adjTiles, int? waterDeathDustType = null)
        {
            Main.tileLighted[modTile.Type] = true;
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileSolid[modTile.Type] = false;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileNoFail[modTile.Type] = true;
            Main.tileWaterDeath[modTile.Type] = true;
            TileID.Sets.FramesOnKillWall[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileID.Sets.DisableSmartInteract[modTile.Type] = true;
            TileID.Sets.Torch[modTile.Type] = true;

            modTile.DustType = dustType;
            modTile.AdjTiles = adjTiles;

            modTile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            modTile.AddMapEntry(mapColor, Language.GetText("ItemName.Torch"));

            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Torches, 0));

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorWall = true;
            TileObjectData.addAlternate(0);

            if (waterDeathDustType != null)
            {
                TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
                TileObjectData.newSubTile.LinkedAlternates = true;
                TileObjectData.newSubTile.WaterDeath = false;
                TileObjectData.newSubTile.LavaDeath = false;
                TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
                TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
                TileObjectData.addSubTile(1);
            }

            TileObjectData.addTile(modTile.Type);

            flameTexture = ModContent.Request<Texture2D>(flameTexturePath);
        }

        public static void HandleTorchDraw(Tile tile, int i, int j, SpriteBatch spriteBatch, Asset<Texture2D> flameTexture)
        {
            if (!TileDrawing.IsVisible(tile))
            {
                return;
            }

            int offsetY = 0;
            if (WorldGen.SolidTile(i, j - 1))
            {
                offsetY = 4;
            }

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);
            Color color = new Color(100, 100, 100, 0);
            int width = 20;
            int height = 20;
            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;

            for (int k = 0; k < 7; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

                spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + offsetY + yy) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
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


        public static void HandlePostDraw(Asset<Texture2D> flameTexture, int i, int j, SpriteBatch spriteBatch, bool isOn, bool applyRandomOffset = false)
        {
            Tile tile = Main.tile[i, j];

            if (!TileDrawing.IsVisible(tile)) return;

            if (isOn)
            {
                Color color = new Color(255, 255, 255, 0);
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

                int width = 16;
                int height = 16;
                int offsetY = WorldGen.SolidTile(i, j - 1) ? 4 : 0;
                short frameX = tile.TileFrameX;
                short frameY = tile.TileFrameY;
                int addFrX = 0;
                int addFrY = 0;

                if (tile.WallType > 0 && !WorldGen.SolidTile(i, j - 1))
                {
                    offsetY = 0;
                }

                TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX, ref frameY);
                TileLoader.SetAnimationFrame(tile.TileType, i, j, ref addFrX, ref addFrY);

                Rectangle drawRectangle = new(tile.TileFrameX, tile.TileFrameY + addFrY, width, height);

                if (applyRandomOffset)
                {
                    ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);
                    for (int k = 0; k < 7; k++)
                    {
                        float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                        float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

                        spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + offsetY + yy) + zero, drawRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }
                else
                {
                    spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + offsetY) + zero, drawRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }



        public static void HandleHitWire(Tile tile, int i, int j)
        {
            int topY = j - tile.TileFrameY / 18 % 3;
            short frameAdjustment = (short)(tile.TileFrameX > 0 ? -18 : 18);
            Main.tile[i, topY].TileFrameX += frameAdjustment;
            Main.tile[i, topY + 1].TileFrameX += frameAdjustment;
            Main.tile[i, topY + 2].TileFrameX += frameAdjustment;
            Wiring.SkipWire(i, topY);
            Wiring.SkipWire(i, topY + 1);
            Wiring.SkipWire(i, topY + 2);
            NetMessage.SendTileSquare(-1, i, topY + 1, 3, TileChangeType.None);
        }

        public static void HandlePostDraw(Tile tile, int i, int j, SpriteBatch spriteBatch)
        {
            if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)))
            {
                short frameX = tile.TileFrameX;
                short frameY = tile.TileFrameY;
                if (Main.rand.NextBool(40) && frameX == 0)
                {
                    int style = frameY / 54;
                    if (frameY / 18 % 3 == 0)
                    {
                        int dustChoice = tile.TileType;
                        if (dustChoice != -1)
                        {
                            int dust = Dust.NewDust(new Vector2(i * 16 + 4, j * 16 + 2), 4, 4, dustChoice, 0f, 0f, 100, default, 1f);
                            if (!Main.rand.NextBool(3))
                            {
                                Main.dust[dust].noGravity = true;
                            }
                            Main.dust[dust].velocity *= 0.3f;
                            Main.dust[dust].velocity.Y -= 1.5f;
                        }
                    }
                }
            }
        }

        public static void SetupWorkbench(ModTile modTile, int itemDropType)
        {
            Main.tileTable[modTile.Type] = true;
            Main.tileSolidTop[modTile.Type] = true;
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[modTile.Type] = true;

            modTile.DustType = DustID.BlueCrystalShard;
            modTile.AdjTiles = [TileID.WorkBenches];

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.CoordinateHeights = [18];
            TileObjectData.addTile(modTile.Type);

            modTile.RegisterItemDrop(itemDropType);

            modTile.AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.WorkBench"));
        }

        public static void SetupChair(ModTile modTile)
        {
            SetBasicTileProperties(modTile, hasOutlines: true, canBeSatOnForNPCs: true, canBeSatOnForPlayers: true);

            modTile.DustType = DustID.BlueCrystalShard;
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

            TileObjectData.addTile(modTile.Type);

            modTile.AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Chair"));
        }

        public static void SetupTable(ModTile modTile, int itemDropType)
        {
            Main.tileTable[modTile.Type] = true;
            Main.tileSolidTop[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = true;
            Main.tileFrameImportant[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[modTile.Type] = true;

            modTile.DustType = DustID.BlueCrystalShard;
            modTile.AdjTiles = [TileID.Tables];

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.addTile(modTile.Type);

            modTile.RegisterItemDrop(itemDropType);

            modTile.AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Table"));
        }

        public static void SetupSofa(ModTile modTile, Color mapColor, int itemDropType, int dustType = DustID.WoodFurniture)
        {
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = true;
            TileID.Sets.HasOutlines[modTile.Type] = true;
            TileID.Sets.CanBeSatOnForPlayers[modTile.Type] = true;

            modTile.DustType = dustType;
            modTile.AdjTiles = [TileID.Benches];
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(modTile.Type);

            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Sofa"));
            modTile.RegisterItemDrop(itemDropType);
        }

        public static void SetupPlatform(ModTile modTile, Color mapColor, int dustType, int itemDropType)
        {
            Main.tileLighted[modTile.Type] = true;
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileSolidTop[modTile.Type] = true;
            Main.tileSolid[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileTable[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = true;
            TileID.Sets.Platforms[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;

            modTile.DustType = dustType;
            modTile.AdjTiles = [TileID.Platforms];
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateHeights = [16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 27;
            TileObjectData.newTile.StyleWrapLimit = 27;
            TileObjectData.newTile.UsesCustomCanPlace = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(modTile.Type);
            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Platform"));

            modTile.RegisterItemDrop(itemDropType);
            Main.tileNoSunLight[modTile.Type] = false;
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


        public static void SetupToilet(ModTile modTile)
        {
            TileID.Sets.CanBeSatOnForNPCs[modTile.Type] = true;
            TileID.Sets.CanBeSatOnForPlayers[modTile.Type] = true;
            TileID.Sets.DisableSmartCursor[modTile.Type] = true;

            modTile.DustType = DustID.BlueCrystalShard;
            modTile.AdjTiles = [TileID.Toilets];

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
            TileObjectData.addTile(modTile.Type);

            modTile.AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Toilet"));
        }

        public static void HandleToiletInteraction(ModTile modTile, int i, int j, Player player)
        {
            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }
        }

        public static void HandleToiletHitWire(ModTile modTile, int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int spawnX = i;
            int spawnY = j - (tile.TileFrameY % 40) / 18;

            Wiring.SkipWire(spawnX, spawnY);
            Wiring.SkipWire(spawnX, spawnY + 1);

            if (Wiring.CheckMech(spawnX, spawnY, 60))
            {
                Projectile.NewProjectile(Wiring.GetProjectileSource(spawnX, spawnY), spawnX * 16 + 8, spawnY * 16 + 12, 0f, 0f, ProjectileID.ToiletEffect, 0, 0f, Main.myPlayer);
            }
        }

        public static void SetupChandelier(ModTile modTile, Color mapColor, int itemDropType, float lightR, float lightG, float lightB, string flameTexturePath, ref Asset<Texture2D> flameTexture)
        {
            Main.tileLighted[modTile.Type] = true;
            Main.tileFrameImportant[modTile.Type] = true;
            Main.tileNoAttach[modTile.Type] = true;
            Main.tileWaterDeath[modTile.Type] = true;
            Main.tileLavaDeath[modTile.Type] = true;

            modTile.DustType = DustID.BlueCrystalShard;
            modTile.AdjTiles = [TileID.Chandeliers];

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16]; 
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(modTile.Type);

            modTile.AddMapEntry(mapColor, Language.GetText("MapObject.Chandelier"));
            modTile.RegisterItemDrop(itemDropType);

            flameTexture = ModContent.Request<Texture2D>(flameTexturePath);
        }
    }
}