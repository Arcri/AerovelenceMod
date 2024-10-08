using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural.Flora;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    public class CrystalGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<CrystalGrass>()] = true;
            Main.tileBlendAll[this.Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(100, 155, 255));
            //ItemDrop = ModContent.ItemType<Items.Placeable.CrystalDirtItem>();
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<CrystalDirt>();
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!effectOnly)
            {
                fail = true;
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<CrystalDirt>();
                WorldGen.SquareTileFrame(i, j, true);
                Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.Marble, 0f, 0f, 0, new Color(121, 121, 121), 1f);
            }
        }

        public static bool PlaceObject(int x, int y, int type, bool mute = false, int style = 0, int alternate = 0, int random = -1, int direction = -1)
        {
            TileObject toBePlaced;
            if (!TileObject.CanPlace(x, y, type, style, direction, out toBePlaced, false))
            {
                return false;
            }
            toBePlaced.random = random;
            if (TileObject.Place(toBePlaced) && !mute)
            {
                WorldGen.SquareTileFrame(x, y, true);
            }
            return false;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (WorldGen.genRand.NextBool(25) && !tileAbove.HasTile && tile.LiquidType != LiquidID.Lava)
            {
                if (!tile.BottomSlope && !tile.TopSlope && !tile.IsHalfBlock && !tile.TopSlope)
                {
                    tileAbove.TileType = (ushort)ModContent.TileType<CrystalFlora>();
                    tileAbove.HasTile = true;
                    tileAbove.TileFrameY = 0;
                    tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }
            if (WorldGen.genRand.NextBool(15) && !tileBelow.HasTile && tile.LiquidType != LiquidID.Lava)
            {
                if (!tile.BottomSlope)
                {
                    tileBelow.TileType = (ushort)ModContent.TileType<CrystalVines>();
                    tileBelow.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }
            int tileX, tileY;
            for (int y = -1; y <= 1; y++)
            {
                for (int x1 = -1; x1 <= 1; x1++)
                {
                    tileX = i + x1;
                    tileY = j + y;
                    if (!WorldGen.InWorld(i, j, 0)) continue;
                    if (Main.tile[tileX, tileY].TileType == TileID.MushroomGrass && Main.rand.NextBool(4))
                    {
                        Main.tile[tileX, tileY].TileType = (ushort)ModContent.TileType<CrystalGrass>();
                        WorldGen.SquareTileFrame(tileX, tileY, true);
                    }
                }
            }
            for (int x = i - 1; x <= i + 1; x++)
            {
                for (int y = j - 1; y <= j + 1; y++)
                {
                    if ((x != i || j != y) && Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<CrystalDirt>())
                    {
                        WorldGen.SpreadGrass(x, y, ModContent.TileType<CrystalDirt>(), Type, false, new TileColorCache());
                        if (Main.tile[x, y].TileType == Type)
                        {
                            WorldGen.SquareTileFrame(x, y, true);
                        }
                    }
                    if ((x != i || j != y) && Main.tile[x, y].HasTile && Main.tile[x, y].TileType == TileID.Dirt)
                    {
                        WorldGen.SpreadGrass(x, y, TileID.Dirt, Type, false, new TileColorCache());
                        if (Main.tile[x, y].TileType == Type)
                        {
                            WorldGen.SquareTileFrame(x, y, true);
                        }
                    }
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0f;
            g = 0.050f;
            b = 0.200f;
        }
    }
}