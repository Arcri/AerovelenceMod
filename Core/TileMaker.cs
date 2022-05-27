using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Core
{
    internal static class TileMaker
    {
        /// <summary>
        /// Make <paramref name="tilesToMergeWith"></paramref> null to not merge with any specific tile(s).
        /// </summary>
        public static void SimpleFramedTile(this ModTile tile, int drop, int soundType, int dustType, Color mapColor, int minPick, 
            string mapName = "", bool mergeDirt = false, bool stone = false, params int[] tilesToMergeWith)
        {
            Main.tileBlockLight[tile.Type] = true;
            Main.tileLighted[tile.Type] = true;
            Main.tileSolid[tile.Type] = true;

            Main.tileMergeDirt[tile.Type] = mergeDirt;
            Main.tileStone[tile.Type] = stone;

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);

            if (tilesToMergeWith != null)
            {
                foreach (int i in tilesToMergeWith)
                {
                    Main.tileMerge[tile.Type][i] = true;
                }
            }

            tile.drop = drop;
            tile.soundType = soundType;
            tile.dustType = dustType;
            tile.minPick = minPick;
        }

        public static void SimpleFrameImportantTile(this ModTile tile, int width, int height, int soundType, int dustType, Color mapColor,
            string mapName = "", bool solid = false, bool solidTop = true, AnchorData anchorBottom = default, AnchorData anchorTop = default)
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

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);

            tile.soundType = soundType;
            tile.dustType = dustType;
        }

        public static void SimpleWall(this ModWall wall, int drop, int soundType, int dustType, Color mapColor, bool house = false)
        {
            Main.wallHouse[wall.Type] = house;

            wall.drop = drop;
            wall.soundType = soundType;
            wall.dustType = dustType;

            wall.AddMapEntry(mapColor);
        }
    }
}
