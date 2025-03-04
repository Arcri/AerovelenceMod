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

            List<StructureData> structure = new List<StructureData>();
            HashSet<Vector2> processedTiles = new HashSet<Vector2>();

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2 tilePosition = new Vector2(x, y);

                    if (processedTiles.Contains(tilePosition))
                        continue;

                    Tile tile = Main.tile[x, y];
                    ModTile modTile = TileLoader.GetTile(tile.TileType);
                    ModWall modWall = WallLoader.GetWall(tile.WallType);

                    StructureData data = new StructureData
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

            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs))
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

        public static AeroStructure LoadStructure(Vector2 startPosition, string structureName, List<ChestConfiguration> chestConfigs = null, bool placeStructure = true, bool checkIfProtected = false)
        {
            string assetPath = $"Common/Utilities/Generation/StructureStamper/Structures/{structureName}.dat";
            int height = 0;
            int width = 0;
            AeroStructure aeroStructure;

            List<StructureData> structure = new List<StructureData>();
            Mod mod = ModLoader.GetMod("AerovelenceMod");

            byte[] structureBytes = mod.GetFileBytes(assetPath);

            try
            {
                using (MemoryStream ms = new MemoryStream(structureBytes))
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    int count = reader.ReadInt32();
                    width = reader.ReadInt32();
                    height = reader.ReadInt32();
                    aeroStructure = new AeroStructure(startPosition, width, height, structureName);
                    for (int i = 0; i < count; i++)
                    {
                        StructureData data = new StructureData
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
                    HashSet<Vector2> placedTiles = new HashSet<Vector2>();
                    List<Vector2> tilesToFrame = new List<Vector2>();
                    List<StructureData> multiTiles = new List<StructureData>();
                    List<StructureData> normalTiles = new List<StructureData>();
                    List<StructureData> wallData = new List<StructureData>();
                    foreach (StructureData data in structure)
                    {
                        wallData.Add(data);

                        if (data.TileFrameImportant)
                            multiTiles.Add(data);
                        else
                            normalTiles.Add(data);
                    }
                    foreach (StructureData data in wallData)
                    {
                        if (GetTileType(data) == ModContent.TileType<TheTile>())
                            continue;

                        int x = (int)(startPosition.X + data.X);
                        int y = (int)(startPosition.Y + data.Y);

                        Tile tile = Main.tile[x, y];
                        if (tile == null) continue;
                        ushort wallType = GetWallType(data);
                        tile.WallType = wallType;
                        tile.WallColor = data.WallColor;
                    }
                    foreach (StructureData data in normalTiles)
                    {
                        int x = (int)(startPosition.X + data.X);
                        int y = (int)(startPosition.Y + data.Y);
                        if (GetTileType(data) == ModContent.TileType<TheTile>())
                            continue;

                        Tile tile = Main.tile[x, y];
                        tile.ClearTile();
                        if (data.IsActive)
                        {
                            ushort tileType = GetTileType(data);
                            tile.HasTile = true;
                            tile.TileType = tileType;
                            tile.TileFrameX = data.TileFrameX;
                            tile.TileFrameY = data.TileFrameY;
                        }
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
                                for (int dx = 0; dx < tileData.Width; dx++)
                                {
                                    for (int dy = 0; dy < tileData.Height; dy++)
                                    {
                                        Tile targetTile = Main.tile[x + dx, y + dy];
                                        targetTile.ClearTile();

                                        if (data.IsActive)
                                        {
                                            targetTile.HasTile = true;
                                            targetTile.TileType = tileType;
                                            targetTile.TileFrameX = (short)(data.TileFrameX + dx * 18);
                                            targetTile.TileFrameY = (short)(data.TileFrameY + dy * 18);
                                        }
                                        targetTile.WallType = GetWallType(data);
                                        targetTile.WallColor = data.WallColor;
                                        targetTile.LiquidType = data.LiquidType;
                                        targetTile.LiquidAmount = data.LiquidAmount;
                                        targetTile.RedWire = data.HasRedWire;
                                        targetTile.BlueWire = data.HasBlueWire;
                                        targetTile.GreenWire = data.HasGreenWire;
                                        targetTile.YellowWire = data.HasYellowWire;
                                        targetTile.HasActuator = data.HasActuator;
                                        targetTile.IsActuated = data.IsActuated;
                                        targetTile.TileColor = data.TileColor;
                                        targetTile.IsHalfBlock = data.IsHalfBlock;
                                        targetTile.Slope = (SlopeType)data.Slope;

                                        tilesToFrame.Add(new Vector2(x + dx, y + dy));
                                    }
                                }
                                if (TileID.Sets.BasicChest[tileType])
                                {
                                    int chestIndex = Chest.CreateChest(x, y);
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
                                }
                            }
                        }
                        else
                        {
                            Tile tile = Main.tile[x, y];
                            tile.ClearTile();

                            if (data.IsActive)
                            {
                                tile.HasTile = true;
                                tile.TileType = tileType;
                                tile.TileFrameX = data.TileFrameX;
                                tile.TileFrameY = data.TileFrameY;
                            }
                            tile.WallType = GetWallType(data);
                            tile.WallColor = data.WallColor;
                            tile.LiquidType = data.LiquidType;
                            tile.LiquidAmount = data.LiquidAmount;
                            tile.RedWire = data.HasRedWire;
                            tile.BlueWire = data.HasBlueWire;
                            tile.GreenWire = data.HasGreenWire;
                            tile.YellowWire = data.HasYellowWire;
                            tile.HasActuator = data.HasActuator;
                            tile.IsActuated = data.IsActuated;
                            tile.TileColor = data.TileColor;
                            tile.IsHalfBlock = data.IsHalfBlock;
                            tile.Slope = (SlopeType)data.Slope;

                            tilesToFrame.Add(new Vector2(x, y));
                        }
                    }
                    foreach (Vector2 pos in tilesToFrame.Distinct())
                    {
                        WorldGen.SquareTileFrame((int)pos.X, (int)pos.Y);
                    }
                }

                int placedLeft = (int)startPosition.X;
                int placedTop = (int)startPosition.Y;
                int placedRight = placedLeft + width;
                int placedBottom = placedTop + height;

                int totalWiresRed = 0;
                int totalWiresGreen = 0;
                int totalWiresBlue = 0;
                int totalWiresYellow = 0;

                for (int ix = placedLeft; ix < placedRight; ix++)
                {
                    for (int iy = placedTop; iy < placedBottom; iy++)
                    {
                        Tile t = Main.tile[ix, iy];
                        if (t == null) continue;

                        if (t.RedWire) totalWiresRed++;
                        if (t.GreenWire) totalWiresGreen++;
                        if (t.BlueWire) totalWiresBlue++;
                        if (t.YellowWire) totalWiresYellow++;
                    }
                }
                string wireMessage = $"DEBUG: Placed structure '{structureName}' => " +
                    $"Red:{totalWiresRed} Green:{totalWiresGreen} Blue:{totalWiresBlue} Yellow:{totalWiresYellow}";
                ModContent.GetInstance<AerovelenceMod>()?.Logger.Info(wireMessage);

                return aeroStructure;
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException($"Structure file {structureName}.dat could not be found or loaded.", ex);
            }
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

        private static ushort GetWallType(StructureData data)
        {
            if (data.WallModName == "Terraria")
            {
                return Convert.ToUInt16(data.WallName);
            }

            Mod modWall = ModLoader.GetMod(data.WallModName);
            return modWall?.Find<ModWall>(data.WallName)?.Type ?? 0;
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