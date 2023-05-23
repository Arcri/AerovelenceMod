using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Core
{
    internal static class TileMaker
    {
        /// <summary>
        /// Make <paramref name="tilesToMergeWith"></paramref> null to not merge with any specific tile(s).
        /// </summary>
        public static void SimpleFramedTile(this ModTile tile, int drop, Terraria.Audio.SoundStyle hitSound, int DustType, Color mapColor, int MinPick, 
            string mapName = "", bool mergeDirt = false, bool stone = false, params int[] tilesToMergeWith)
        {
            Main.tileBlockLight[tile.Type] = true;
            Main.tileLighted[tile.Type] = true;
            Main.tileSolid[tile.Type] = true;

            Main.tileMergeDirt[tile.Type] = mergeDirt;
            Main.tileStone[tile.Type] = stone;

            LocalizedText name = tile.CreateMapEntryName();
            // name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);

            if (tilesToMergeWith != null)
            {
                foreach (int i in tilesToMergeWith)
                {
                    Main.tileMerge[tile.Type][i] = true;
                }
            }

            //tile.ItemDrop/* tModPorter Note: Removed. Tiles and walls will drop the item which places them automatically. Use RegisterItemDrop to alter the automatic drop if necessary. */ = drop;
            tile.HitSound = hitSound;
            tile.DustType = DustType;
            tile.MinPick = MinPick;
        }

        public static void SimpleFrameImportantTile(this ModTile tile, int width, int height, Terraria.Audio.SoundStyle hitSound, int DustType, Color mapColor,
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

            LocalizedText name = tile.CreateMapEntryName();
            // name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);

            tile.HitSound = hitSound;
            tile.DustType = DustType;
        }

        public static void SimpleWall(this ModWall wall, int drop, Terraria.Audio.SoundStyle hitSound, int DustType, Color mapColor, bool house = false)
        {
            Main.wallHouse[wall.Type] = house;

            //wall.ItemDrop/* tModPorter Note: Removed. Tiles and walls will drop the item which places them automatically. Use RegisterItemDrop to alter the automatic drop if necessary. */ = drop;
            wall.HitSound = hitSound;
            wall.DustType = DustType;

            wall.AddMapEntry(mapColor);
        }
    }
}
