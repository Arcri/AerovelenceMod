using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AerovelenceMod.Common.Globals.Worlds // MOD NAME HERE
{
    public class IceWorldgen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int genIndex;

            genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
            if (genIndex != -1)
            {
                tasks.Insert(genIndex + 1, new PassLegacy("Ice Extras", IceExtras));
            }
            genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Smooth World"));
            if (genIndex != -1)
            {
                tasks.Insert(genIndex + 1, new PassLegacy("Finish Ice Spikes", IceExtras2));
            }
        }

        private void DrawCircle(float x2, float y2, float radius, float xMultiplier, float yMultiplier, int tile, bool add, bool replace, bool wall)
        {
            for (int y = (int)(y2 - radius * yMultiplier); y <= y2 + radius * yMultiplier; y++)
            {
                for (int x = (int)(x2 - radius * xMultiplier); x <= x2 + radius * xMultiplier; x++)
                {
                    if (x > 1 && x < Main.maxTilesX && y > 1 && y < Main.maxTilesY)
                    {
                        if (Vector2.Distance(new Vector2(x2, y2), new Vector2((x - x2) / xMultiplier + x2, (y - y2) / yMultiplier + y2)) < radius)
                        {
                            if (tile == -2)
                            {
                                Framing.GetTileSafely(x, y).LiquidAmount = 255;
                            }
                            else if (tile == -1)
                            {
                                if (wall == true)
                                {
                                }
                                else
                                {
                                    if (!Framing.GetTileSafely(x, y).HasTile)
                                    {
                                        WorldGen.PlaceTile(x, y, Framing.GetTileSafely(x, y).TileType);
                                    }
                                }
                            }
                            else if (tile == 0)
                            {
                                if (wall == true)
                                {
                                    WorldGen.KillWall(x, y);
                                }
                                else
                                {
                                    if (Framing.GetTileSafely(x, y).HasTile)
                                    {
                                        WorldGen.KillTile(x, y);
                                    }
                                }
                            }
                            else
                            {
                                if (add == true)
                                {
                                    if (wall == true)
                                    {
                                        if (Framing.GetTileSafely(x, y).WallType == 0)
                                        {
                                            WorldGen.PlaceWall(x, y, tile);
                                        }
                                    }
                                    else if (!Framing.GetTileSafely(x, y).HasTile)
                                    {
                                        WorldGen.PlaceTile(x, y, tile);
                                    }
                                }
                                if (replace == true)
                                {
                                    if (wall == true)
                                    {
                                        if (Framing.GetTileSafely(x, y).WallType != 0)
                                        {
                                            Framing.GetTileSafely(x, y).WallType = (ushort)tile;
                                        }
                                    }
                                    else if (Framing.GetTileSafely(x, y).HasTile)
                                    {
                                        Framing.GetTileSafely(x, y).TileType = (ushort)tile;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void IceExtras(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Ice Spikes";

            int structureCount = Main.maxTilesX / 250;

            while (structureCount > 0)
            {
                int structureX = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int structureY = 1;
                bool valid = false;

                while (!Framing.GetTileSafely(structureX, structureY).HasTile)
                {
                    structureY++;
                }

                if (Framing.GetTileSafely(structureX, structureY).TileType == TileID.SnowBlock || Framing.GetTileSafely(structureX, structureY).TileType == TileID.IceBlock)
                {
                    valid = true;
                }

                if (valid == true)
                {
                    int size = WorldGen.genRand.Next(4, 13);
                    float size2 = size;
                    int x = structureX;
                    int y = structureY;

                    while (size > 0 && size2 > 0.5f)
                    {
                        DrawCircle(x, y, size, 0.25f, 0.75f, TileID.IceBlock, true, true, false);

                        y -= 1;
                        size2 = (float)(size2 * 0.8f);
                        size = (int)size2;
                    }

                    //WorldGen.PlaceTile(x - 1, y + 1, ModContent.TileType<SlateOreBlock>());
                    //WorldGen.PlaceTile(x + 1, y + 1, ModContent.TileType<SlateOreBlock>());

                    structureCount--;
                }
            }

            progress.Message = "Ice Chasms";

            structureCount = Main.maxTilesX / 4200;

            while (structureCount > 0)
            {
                int structureX = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int structureY = 1;
                bool valid = false;

                while (!Framing.GetTileSafely(structureX, structureY).HasTile)
                {
                    structureY++;
                }

                if (Framing.GetTileSafely(structureX, structureY).TileType == TileID.SnowBlock || Framing.GetTileSafely(structureX, structureY).TileType == TileID.IceBlock)
                {
                    valid = true;
                }

                if (valid == true)
                {
                    int size = 12;
                    int x = structureX;
                    int y = structureY;

                    while (size > 0)
                    {
                        DrawCircle(x, y, size, 1, 0.5f, 0, true, true, false);
                        if (size <= 8)
                        {
                            DrawCircle(x, y, size, 1, 0.5f, -2, true, true, false);
                        }

                        int x2 = x + WorldGen.genRand.Next(-2, 3);
                        int y2 = y + WorldGen.genRand.Next(-2, 3);

                        DrawCircle(x2, y2, size, 1.25f, 0.25f, 0, true, true, false);

                        DrawCircle(x, y, size + 3, 1, 0.5f, TileID.IceBlock, false, true, false);
                        DrawCircle(x2, y2, size + 3, 1.5f, 0.25f, TileID.IceBlock, false, true, false);

                        x += WorldGen.genRand.Next(-5, 6);
                        y += WorldGen.genRand.Next(size - 2, size);
                        size -= 1;
                    }

                    structureCount--;
                }
            }
        }
        private void IceExtras2(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int y = 1; y < Main.maxTilesY; y++)
            {
                for (int x = 1; x < Main.maxTilesX; x++)
                {
                    //if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<SlateOreBlock>())
                    //{
                      //  WorldGen.KillTile(x, y);
                    //}
                }
            }
        }
    }
}
