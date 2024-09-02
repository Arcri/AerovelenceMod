using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ObjectData;
using System.Linq;

namespace AerovelenceMod.Common.Utilities.StructureStamper
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
            string path = Path.Combine(directoryPath, $"{structureName}.bin");

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

        public static (int width, int height) LoadStructure(Vector2 startPosition, string structureName, List<ChestConfiguration> chestConfigs = null, bool placeStructure = true)
        {
            string directoryPath = Path.Combine(Main.SavePath, "Mods", "AerovelenceMod", "Common", "Utilities", "StructureStamper", "Structures");
            string path = Path.Combine(directoryPath, $"{structureName}.bin");

            if (File.Exists(path))
            {
                List<StructureData> structure = [];

                int width, height;

                using (FileStream fs = new(path, FileMode.Open))
                using (BinaryReader reader = new(fs))
                {
                    int count = reader.ReadInt32();
                    width = reader.ReadInt32();
                    height = reader.ReadInt32();

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

                if (placeStructure)
                {
                    HashSet<Vector2> placedTiles = [];
                    List<Vector2> tilesToFrame = [];
                    int chestIndex = 0;

                    foreach (StructureData data in structure)
                    {
                        int x = (int)(startPosition.X + data.X);
                        int y = (int)(startPosition.Y + data.Y);
                        Vector2 tilePosition = new(x, y);

                        Tile tile = Main.tile[x, y];

                        ushort wallType;
                        ushort tileType;

                        if (data.ModName == "Terraria")
                        {
                            tileType = Convert.ToUInt16(data.TileName);
                        }
                        else
                        {
                            Mod modTile = ModLoader.GetMod(data.ModName);
                            tileType = (modTile?.Find<ModTile>(data.TileName)?.Type ?? 0);
                        }

                        if (data.WallModName == "Terraria")
                        {
                            wallType = Convert.ToUInt16(data.WallName);
                        }
                        else
                        {
                            Mod modWall = ModLoader.GetMod(data.WallModName);
                            wallType = (modWall?.Find<ModWall>(data.WallName)?.Type ?? 0);
                        }

                        if (tileType == TileID.LesionBlock && data.TileColor == PaintID.DeepRedPaint)
                        {
                            continue;
                        }

                        tile.ClearTile();
                        tile.WallType = 0;
                        tile.LiquidAmount = 0;
                        tile.LiquidType = 0;
                        tile.Slope = 0;
                        tile.IsHalfBlock = false;

                        if (wallType != 0)
                        {
                            tile.WallType = wallType;
                            tile.WallColor = data.WallColor;
                            WorldGen.SquareWallFrame(x, y, true);
                        }

                        placedTiles.Add(tilePosition);
                    }

                    foreach (StructureData data in structure)
                    {
                        int x = (int)(startPosition.X + data.X);
                        int y = (int)(startPosition.Y + data.Y);
                        Vector2 tilePosition = new(x, y);

                        Tile tile = Main.tile[x, y];

                        ushort tileType;

                        if (data.ModName == "Terraria")
                        {
                            tileType = Convert.ToUInt16(data.TileName);
                        }
                        else
                        {
                            Mod modTile = ModLoader.GetMod(data.ModName);
                            tileType = (modTile?.Find<ModTile>(data.TileName)?.Type ?? 0);
                        }

                        if (tileType == TileID.LesionBlock && data.TileColor == PaintID.DeepRedPaint)
                        {
                            continue;
                        }

                        if (data.TileFrameImportant)
                        {
                            TileObjectData tileData = TileObjectData.GetTileData(tileType, 0);
                            if (tileData != null)
                            {
                                int tileWidth = tileData.Width;
                                int tileHeight = tileData.Height;

                                for (int dx = 0; dx < tileWidth; dx++)
                                {
                                    for (int dy = 0; dy < tileHeight; dy++)
                                    {
                                        Vector2 offsetPosition = new(x + dx, y + dy);
                                        Tile targetTile = Main.tile[(int)offsetPosition.X, (int)offsetPosition.Y];

                                        targetTile.WallType = 0;
                                        targetTile.LiquidAmount = 0;
                                        targetTile.LiquidType = 0;
                                        targetTile.Slope = 0;
                                        targetTile.IsHalfBlock = false;
                                    }
                                }
                            }
                        }

                        tile.HasTile = data.IsActive;
                        tile.TileType = tileType;
                        tile.TileFrameX = data.TileFrameX;
                        tile.TileFrameY = data.TileFrameY;
                        tile.LiquidType = data.LiquidType;
                        tile.LiquidAmount = data.LiquidAmount;
                        tile.IsHalfBlock = data.IsHalfBlock;
                        tile.Slope = 0;
                        tile.RedWire = data.HasRedWire;
                        tile.BlueWire = data.HasBlueWire;
                        tile.GreenWire = data.HasGreenWire;
                        tile.YellowWire = data.HasYellowWire;
                        tile.HasActuator = data.HasActuator;
                        tile.IsActuated = data.IsActuated;
                        tile.TileColor = data.TileColor;

                        placedTiles.Add(tilePosition);

                        if (data.TileFrameImportant)
                        {
                            TileObjectData tileData = TileObjectData.GetTileData(tile.TileType, 0);

                            if (tileData != null)
                            {
                                int tileWidth = tileData.Width;
                                int tileHeight = tileData.Height;

                                for (int dx = 0; dx < tileWidth; dx++)
                                {
                                    for (int dy = 0; dy < tileHeight; dy++)
                                    {
                                        Vector2 offsetPosition = new(x + dx, y + dy);
                                        Tile targetTile = Main.tile[(int)offsetPosition.X, (int)offsetPosition.Y];

                                        targetTile.HasTile = true;
                                        targetTile.TileType = tileType;
                                        targetTile.TileFrameX = (short)(data.TileFrameX + dx * 18);
                                        targetTile.TileFrameY = (short)(data.TileFrameY + dy * 18);
                                        targetTile.Slope = 0;
                                        targetTile.IsHalfBlock = false;

                                        tilesToFrame.Add(offsetPosition);
                                    }
                                }
                            }
                            else
                            {
                                tilesToFrame.Add(tilePosition);
                            }

                            if (TileID.Sets.BasicChest[tile.TileType] && chestConfigs != null && chestIndex < chestConfigs.Count)
                            {
                                ChestConfigurator.ApplyConfiguration(x, y, chestConfigs[chestIndex]);
                                chestIndex++;
                            }
                        }
                        else
                        {
                            tilesToFrame.Add(tilePosition);
                        }
                    }

                    foreach (Vector2 position in tilesToFrame)
                    {
                        int x = (int)position.X;
                        int y = (int)position.Y;
                        WorldGen.SquareTileFrame(x, y, true);
                    }

                    Main.NewText($"Structure '{structureName}' loaded. Width: {width}, Height: {height}!");
                }

                return (width, height);
            }
            else
            {
                Main.NewText($"What the hay structure file '{structureName}' was not found");
                return (0, 0);
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