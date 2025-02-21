using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ObjectData;
using System.Linq;
using ReLogic.Content;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using Terraria.WorldBuilding;

namespace AerovelenceMod.Common.Utilities.Generation.StructureStamper
{
    public class StructureStamper : ModSystem
    {
        public static StructureStamper Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }

        public static void ExtractStructure(Vector2 point1, Vector2 point2, string structureName)
        {
            int minX = (int)MathHelper.Min(point1.X, point2.X);
            int minY = (int)MathHelper.Min(point1.Y, point2.Y);
            int maxX = (int)MathHelper.Max(point1.X, point2.X);
            int maxY = (int)MathHelper.Max(point1.Y, point2.Y);

            List<StructureData> structure = [];
            HashSet<Vector2> processedTiles = [];

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2 tilePosition = new(x, y);

                    if (processedTiles.Contains(tilePosition))
                        continue;

                    Tile tile = Main.tile[x, y];
                    ModTile modTile = TileLoader.GetTile(tile.TileType);
                    ModWall modWall = WallLoader.GetWall(tile.WallType);

                    StructureData data = new()
                    {
                        X = x - minX,
                        Y = y - minY,
                        ModName = modTile?.Mod.Name ?? "Terraria",
                        TileName = modTile?.Name ?? tile.TileType.ToString(),
                        WallModName = modWall?.Mod.Name ?? "Terraria",
                        WallName = modWall?.Name ?? tile.WallType.ToString(),
                        TileFrameX = tile.TileFrameX,
                        TileFrameY = tile.TileFrameY,
                        LiquidType = (byte)tile.LiquidType,
                        LiquidAmount = tile.LiquidAmount,
                        IsHalfBlock = tile.IsHalfBlock,
                        Slope = (byte)tile.Slope,
                        IsActive = tile.HasTile,
                        TileFrameImportant = Main.tileFrameImportant[tile.TileType],
                        HasRedWire = tile.RedWire,
                        HasBlueWire = tile.BlueWire,
                        HasGreenWire = tile.GreenWire,
                        HasYellowWire = tile.YellowWire,
                        HasActuator = tile.HasActuator,
                        IsActuated = tile.IsActuated,
                        TreeStyle = (byte)(tile.TileType == TileID.Trees ? tile.TileFrameX / 22 : 0),
                        TileColor = tile.TileColor,
                        WallColor = tile.WallColor
                    };
                    structure.Add(data);

                    if (data.TileFrameImportant)
                    {
                        TileObjectData tileData = TileObjectData.GetTileData(tile.TileType, 0);

                        if (tileData != null)
                        {
                            int width = tileData.Width;
                            int height = tileData.Height;

                            for (int dx = 0; dx < width; dx++)
                            {
                                for (int dy = 0; dy < height; dy++)
                                {
                                    processedTiles.Add(new Vector2(x + dx, y + dy));
                                }
                            }
                        }
                        else
                        {
                            processedTiles.Add(tilePosition);
                        }

                    }
                    else
                    {
                        processedTiles.Add(tilePosition);
                    }
                }
            }

            SaveStructureToFile(structure, structureName, maxX - minX + 1, maxY - minY + 1);
        }

        public static void SaveStructureToFile(List<StructureData> structure, string structureName, int width, int height)
        {
            string directoryPath = Path.Combine(Main.SavePath, "Mods", "AerovelenceMod", "Common", "Utilities", "StructureStamper", "Structures");
            string path = Path.Combine(directoryPath, $"{structureName}.dat");

            Directory.CreateDirectory(directoryPath);

            using (FileStream fs = new(path, FileMode.Create))
            using (BinaryWriter writer = new(fs))
            {
                writer.Write(structure.Count);
                writer.Write(width);
                writer.Write(height);
                foreach (var data in structure)
                {
                    writer.Write(data.X);
                    writer.Write(data.Y);
                    writer.Write(data.ModName);
                    writer.Write(data.TileName);
                    writer.Write(data.WallModName);
                    writer.Write(data.WallName);
                    writer.Write(data.TileFrameX);
                    writer.Write(data.TileFrameY);
                    writer.Write(data.LiquidType);
                    writer.Write(data.LiquidAmount);
                    writer.Write(data.IsHalfBlock);
                    writer.Write(data.Slope);
                    writer.Write(data.IsActive);
                    writer.Write(data.TileFrameImportant);
                    writer.Write(data.HasRedWire);
                    writer.Write(data.HasBlueWire);
                    writer.Write(data.HasGreenWire);
                    writer.Write(data.HasYellowWire);
                    writer.Write(data.HasActuator);
                    writer.Write(data.IsActuated);
                    writer.Write(data.TreeStyle);
                    writer.Write(data.TileColor);
                    writer.Write(data.WallColor);
                }
            }

            Main.NewText($"Structure '{structureName}' saved to {path} with size {width}x{height}");
        }


        private static ushort GetTileType(StructureData data)
        {
            if (data.ModName == "Terraria")
            {
                return Convert.ToUInt16(data.TileName);
            }

            Mod modTile = ModLoader.GetMod(data.ModName);
            return modTile?.Find<ModTile>(data.TileName)?.Type ?? 0;
        }

        private static void ProcessTile(Tile tile, StructureData data, int x, int y, HashSet<Vector2> placedTiles, List<Vector2> tilesToFrame)
        {
            Vector2 tilePosition = new(x, y);

            if (placedTiles.Contains(tilePosition))
                return;

            ushort tileType = GetTileType(data);
            ushort wallType = GetWallType(data);
            if (tileType == ModContent.TileType<TheTile>())
                return;
            ushort currentWallType = tile.WallType;
            byte currentWallColor = tile.WallColor;

            if (data.TileFrameImportant)
            {
                TileObjectData tileData = TileObjectData.GetTileData(tileType, 0);
                if (tileData != null)
                {
                    for (int dx = 0; dx < tileData.Width; dx++)
                    {
                        for (int dy = 0; dy < tileData.Height; dy++)
                        {
                            Tile targetTile = Main.tile[x + dx, y + dy];
                            ushort targetCurrentWall = targetTile.WallType;
                            byte targetCurrentWallColor = targetTile.WallColor;
                            targetTile.ClearTile();
                            if (wallType == 0)
                            {
                                targetTile.WallType = targetCurrentWall;
                                targetTile.WallColor = targetCurrentWallColor;
                            }
                        }
                    }
                    int frameX = data.TileFrameX % (tileData.Width * 18);
                    int frameY = data.TileFrameY % (tileData.Height * 18);
                    if (frameX == 0 && frameY == 0)
                    {
                        for (int dx = 0; dx < tileData.Width; dx++)
                        {
                            for (int dy = 0; dy < tileData.Height; dy++)
                            {
                                Vector2 offsetPosition = new(x + dx, y + dy);
                                Tile targetTile = Main.tile[x + dx, y + dy];

                                targetTile.HasTile = true;
                                targetTile.TileType = tileType;
                                targetTile.TileFrameX = (short)(data.TileFrameX + dx * 18);
                                targetTile.TileFrameY = (short)(data.TileFrameY + dy * 18);
                                targetTile.Slope = SlopeType.Solid;
                                targetTile.IsHalfBlock = false;
                                if (wallType != 0)
                                {
                                    targetTile.WallType = wallType;
                                    targetTile.WallColor = data.WallColor;
                                    //WorldGen.SquareWallFrame(x + dx, y + dy, false);
                                }

                                placedTiles.Add(offsetPosition);
                                tilesToFrame.Add(offsetPosition);
                            }
                        }
                    }
                }
                else
                {
                    tile.ClearTile();
                    if (wallType == 0)
                    {
                        tile.WallType = currentWallType;
                        tile.WallColor = currentWallColor;
                    }

                    tile.HasTile = data.IsActive;
                    tile.TileType = tileType;
                    tile.TileFrameX = data.TileFrameX;
                    tile.TileFrameY = data.TileFrameY;
                    tile.Slope = SlopeType.Solid;
                    tile.IsHalfBlock = false;

                    placedTiles.Add(tilePosition);
                    tilesToFrame.Add(tilePosition);
                }
            }
            else
            {
                tile.ClearTile();
                if (wallType == 0)
                {
                    tile.WallType = currentWallType;
                    tile.WallColor = currentWallColor;
                }

                tile.HasTile = data.IsActive;
                tile.TileType = tileType;
                tile.TileFrameX = data.TileFrameX;
                tile.TileFrameY = data.TileFrameY;
                tile.Slope = (SlopeType)data.Slope;
                tile.IsHalfBlock = data.IsHalfBlock;

                placedTiles.Add(tilePosition);
                tilesToFrame.Add(tilePosition);
            }

            if (wallType != 0)
            {
                tile.WallType = wallType;
                tile.WallColor = data.WallColor;
                //WorldGen.SquareWallFrame(x, y, false);
            }
            tile.LiquidType = data.LiquidType;
            tile.LiquidAmount = data.LiquidAmount;
            tile.RedWire = data.HasRedWire;
            tile.BlueWire = data.HasBlueWire;
            tile.GreenWire = data.HasGreenWire;
            tile.YellowWire = data.HasYellowWire;
            tile.HasActuator = data.HasActuator;
            tile.IsActuated = data.IsActuated;
            tile.TileColor = data.TileColor;
        }



        private static void ProcessChestTile(int x, int y, StructureData data)
        {
            ushort tileType = GetTileType(data);
            if (!TileID.Sets.BasicChest[tileType]) return;

            TileObjectData tileData = TileObjectData.GetTileData(tileType, 0);
            if (tileData == null) return;
            ushort wallType = GetWallType(data);
            Dictionary<Point, (ushort type, byte color)> existingWalls = new();
            for (int dx = 0; dx < tileData.Width; dx++)
            {
                for (int dy = 0; dy < tileData.Height; dy++)
                {
                    Tile tile = Main.tile[x + dx, y + dy];
                    existingWalls[new Point(dx, dy)] = (tile.WallType, tile.WallColor);
                }
            }
            for (int dx = 0; dx < tileData.Width; dx++)
            {
                for (int dy = 0; dy < tileData.Height; dy++)
                {
                    Tile tile = Main.tile[x + dx, y + dy];
                    tile.ClearTile();
                    if (wallType != 0)
                    {
                        tile.WallType = wallType;
                        tile.WallColor = data.WallColor;
                    }
                    else
                    {
                        var (type, color) = existingWalls[new Point(dx, dy)];
                        tile.WallType = type;
                        tile.WallColor = color;
                    }
                }
            }
            int chestIndex = WorldGen.PlaceChest(x, y, tileType, false, style: 0);
            if (chestIndex != -1 && chestIndex < Main.chest.Length)
            {
                Main.chest[chestIndex] = new Chest
                {
                    x = x,
                    y = y,
                    item = new Item[40]
                };

                for (int slot = 0; slot < 40; slot++)
                {
                    Main.chest[chestIndex].item[slot] = new Item();
                }
            }
            /*for (int dx = 0; dx < tileData.Width; dx++)
            {
                for (int dy = 0; dy < tileData.Height; dy++)
                {
                    WorldGen.TileFrame(x + dx, y + dy, false, false);
                    if (wallType != 0)
                    {
                        WorldGen.SquareWallFrame(x + dx, y + dy, true);
                    }
                }
            }*/
        }


        private static ushort GetWallType(StructureData data)
        {
            if (data.WallModName == "Terraria")
            {
                return Convert.ToUInt16(data.WallName);
            }

            Mod modWall = ModLoader.GetMod(data.WallModName);
            return modWall?.Find<ModWall>(data.WallName)?.Type ?? 0;
        }

        public static AeroStructure LoadStructure(AeroStructure aeroStructure, List<ChestConfiguration> chestConfigs = null, bool placeStructure = true, bool checkIfProtected = false)
        {
            return LoadStructure(aeroStructure.StartPosition, aeroStructure.Name, chestConfigs, placeStructure, checkIfProtected);
        }

        public static AeroStructure LoadStructure(Vector2 startPosition, string structureName, List<ChestConfiguration> chestConfigs = null, bool placeStructure = true, bool checkIfProtected = false)
        {
            string assetPath = $"Common/Utilities/Generation/StructureStamper/Structures/{structureName}.dat";
            int height = 0;
            int width = 0;
            AeroStructure aeroStructure;

            List<StructureData> structure = [];
            Mod mod = ModLoader.GetMod("AerovelenceMod");

            byte[] structureBytes = mod.GetFileBytes(assetPath);

            try
            {
                using (MemoryStream ms = new(structureBytes))
                using (BinaryReader reader = new(ms))
                {
                    int count = reader.ReadInt32();
                    width = reader.ReadInt32();
                    height = reader.ReadInt32();
                    aeroStructure = new AeroStructure(startPosition, width, height, structureName);
                    for (int i = 0; i < count; i++)
                    {
                        StructureData data = new()
                        {
                            X = reader.ReadInt32(),
                            Y = reader.ReadInt32(),
                            ModName = reader.ReadString(),
                            TileName = reader.ReadString(),
                            WallModName = reader.ReadString(),
                            WallName = reader.ReadString(),
                            TileFrameX = reader.ReadInt16(),
                            TileFrameY = reader.ReadInt16(),
                            LiquidType = reader.ReadByte(),
                            LiquidAmount = reader.ReadByte(),
                            IsHalfBlock = reader.ReadBoolean(),
                            Slope = reader.ReadByte(),
                            IsActive = reader.ReadBoolean(),
                            TileFrameImportant = reader.ReadBoolean(),
                            HasRedWire = reader.ReadBoolean(),
                            HasBlueWire = reader.ReadBoolean(),
                            HasGreenWire = reader.ReadBoolean(),
                            HasYellowWire = reader.ReadBoolean(),
                            HasActuator = reader.ReadBoolean(),
                            IsActuated = reader.ReadBoolean(),
                            TreeStyle = reader.ReadByte(),
                            TileColor = reader.ReadByte(),
                            WallColor = reader.ReadByte()
                        };

                        structure.Add(data);
                    }
                }

                if (checkIfProtected && !aeroStructure.CanPlace())
                {
                    return AeroStructure.Empty;
                }

                if (placeStructure)
                {
                    HashSet<Vector2> placedTiles = [];
                    List<Vector2> tilesToFrame = [];
                    List<StructureData> multiTiles = [];
                    List<StructureData> normalTiles = [];
                    List<StructureData> wallData = [];

                    //tile sorting by type
                    foreach (StructureData data in structure)
                    {
                        if (data.WallModName != "Terraria" || Convert.ToUInt16(data.WallName) != 0)
                        {
                            wallData.Add(data);
                        }

                        if (data.TileFrameImportant)
                        {
                            multiTiles.Add(data);
                        }
                        else
                        {
                            normalTiles.Add(data);
                        }
                    }

                    //walls !!!
                    foreach (StructureData data in wallData)
                    {
                        int x = (int)(startPosition.X + data.X);
                        int y = (int)(startPosition.Y + data.Y);

                        ushort wallType = GetWallType(data);
                        if (wallType != 0)
                        {
                            Tile tile = Main.tile[x, y];
                            tile.WallType = wallType;
                            tile.WallColor = data.WallColor;
                            //WorldGen.SquareWallFrame(x, y, true);
                        }
                    }

                    //normal tiles
                    foreach (StructureData data in normalTiles)
                    {
                        int x = (int)(startPosition.X + data.X);
                        int y = (int)(startPosition.Y + data.Y);

                        Tile tile = Main.tile[x, y];
                        ushort tileType = GetTileType(data);

                        if (tileType == ModContent.TileType<TheTile>())
                            continue;

                        tile.HasTile = data.IsActive;
                        tile.TileType = tileType;
                        tile.TileFrameX = data.TileFrameX;
                        tile.TileFrameY = data.TileFrameY;
                        tile.LiquidType = data.LiquidType;
                        tile.LiquidAmount = data.LiquidAmount;
                        tile.IsHalfBlock = data.IsHalfBlock;
                        tile.Slope = (SlopeType)data.Slope;
                        tile.RedWire = data.HasRedWire;
                        tile.BlueWire = data.HasBlueWire;
                        tile.GreenWire = data.HasGreenWire;
                        tile.YellowWire = data.HasYellowWire;
                        tile.HasActuator = data.HasActuator;
                        tile.IsActuated = data.IsActuated;
                        tile.TileColor = data.TileColor;

                        tilesToFrame.Add(new Vector2(x, y));
                    }

                    //multitiles
                    foreach (StructureData data in multiTiles)
                    {
                        int x = (int)(startPosition.X + data.X);
                        int y = (int)(startPosition.Y + data.Y);
                        ushort tileType = GetTileType(data);

                        if (tileType == ModContent.TileType<TheTile>())
                            continue;

                        TileObjectData tileData = TileObjectData.GetTileData(tileType, 0);
                        if (tileData != null)
                        {
                            int frameX = data.TileFrameX % (tileData.Width * 18);
                            int frameY = data.TileFrameY % (tileData.Height * 18);

                            if (frameX == 0 && frameY == 0)
                            {
                                if (TileID.Sets.BasicChest[tileType])
                                {
                                    for (int dx = 0; dx < tileData.Width; dx++)
                                    {
                                        for (int dy = 0; dy < tileData.Height; dy++)
                                        {
                                            Tile targetTile = Main.tile[x + dx, y + dy];
                                            ushort existingWall = targetTile.WallType;
                                            byte existingWallColor = targetTile.WallColor;
                                            targetTile.TileType = 0;
                                            targetTile.TileFrameX = 0;
                                            targetTile.TileFrameY = 0;
                                            targetTile.HasTile = false;
                                            targetTile.IsHalfBlock = false;
                                            targetTile.Slope = SlopeType.Solid;
                                            targetTile.WallType = existingWall;
                                            targetTile.WallColor = existingWallColor;
                                            targetTile.HasTile = true;
                                            targetTile.TileType = tileType;
                                            targetTile.TileFrameX = (short)(data.TileFrameX + dx * 18);
                                            targetTile.TileFrameY = (short)(data.TileFrameY + dy * 18);

                                            tilesToFrame.Add(new Vector2(x + dx, y + dy));
                                        }
                                    }
                                    int chestIndex = Chest.CreateChest(x, y);
                                    if (chestIndex == -1)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    for (int dx = 0; dx < tileData.Width; dx++)
                                    {
                                        for (int dy = 0; dy < tileData.Height; dy++)
                                        {
                                            Tile targetTile = Main.tile[x + dx, y + dy];
                                            ushort existingWall = targetTile.WallType;
                                            byte existingWallColor = targetTile.WallColor;
                                            targetTile.TileType = 0;
                                            targetTile.TileFrameX = 0;
                                            targetTile.TileFrameY = 0;
                                            targetTile.HasTile = false;
                                            targetTile.IsHalfBlock = false;
                                            targetTile.Slope = SlopeType.Solid;
                                            targetTile.WallType = existingWall;
                                            targetTile.WallColor = existingWallColor;
                                            targetTile.HasTile = true;
                                            targetTile.TileType = tileType;
                                            targetTile.TileFrameX = (short)(data.TileFrameX + dx * 18);
                                            targetTile.TileFrameY = (short)(data.TileFrameY + dy * 18);

                                            tilesToFrame.Add(new Vector2(x + dx, y + dy));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Tile tile = Main.tile[x, y];
                            tile.HasTile = false;
                            tile.HasTile = true;
                            tile.TileType = tileType;
                            tile.TileFrameX = data.TileFrameX;
                            tile.TileFrameY = data.TileFrameY;
                            tile.Slope = SlopeType.Solid;
                            tile.IsHalfBlock = false;

                            tilesToFrame.Add(new Vector2(x, y));
                        }
                    }
                    foreach (Vector2 position in tilesToFrame.Distinct())
                    {
                        Framing.GetTileSafely((int)position.X, (int)position.Y).Clear(Terraria.DataStructures.TileDataType.Slope);
                    }
                }

                return aeroStructure;
            }

            catch (Exception ex)
            {
                throw new FileNotFoundException($"Structure file {structureName}.dat could not be found or loaded.", ex);
            }
        }

        [Serializable]
        public class StructureData
        {
            public int X;
            public int Y;
            public string ModName;
            public string TileName;
            public string WallModName;
            public string WallName;
            public short TileFrameX;
            public short TileFrameY;
            public byte LiquidType;
            public byte LiquidAmount;
            public bool IsHalfBlock;
            public byte Slope;
            public bool IsActive;
            public bool TileFrameImportant;
            public bool HasRedWire;
            public bool HasBlueWire;
            public bool HasGreenWire;
            public bool HasYellowWire;
            public bool HasActuator;
            public bool IsActuated;
            public byte TreeStyle;
            public byte TileColor;
            public byte WallColor;
        }
    }
}