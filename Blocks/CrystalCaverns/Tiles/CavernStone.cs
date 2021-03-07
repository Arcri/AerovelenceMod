using Terraria;
using Terraria.ModLoader;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Blocks.CrystalCaverns.Tiles.Flora;
using AerovelenceMod.Blocks.CrystalCaverns.Rubble;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles
{
    public class CavernStone : ModTile
    {
        public override void SetDefaults()
        {
            mineResist = 2.5f;
            minPick = 40;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][mod.TileType("CrystalDirt")] = true;
            Main.tileMerge[Type][mod.TileType("CrystalGrass")] = true;
            Main.tileMerge[Type][mod.TileType("ChargedStone")] = true;
            Main.tileMerge[Type][mod.TileType("FieldStone")] = true;
            Main.tileMerge[Type][mod.TileType("CitadelStone")] = true;
            Main.tileMerge[Type][mod.TileType("LushGrowth")] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(065, 065, 085));
            dustType = 59;
            soundType = SoundID.Tink;
            drop = ModContent.ItemType<CavernStoneItem>();

        }
        public static Vector2 TileOffset => Lighting.lightMode > 1 ? Vector2.Zero : Vector2.One * 12;

        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0));
        }
      /*  public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            int height = tile.frameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(mod.GetTexture("Blocks/CrystalCaverns/Tiles/CavernStone_Glowmask"), new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.frameX, tile.frameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }*/
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
            if (WorldGen.genRand.NextBool(25) && !tileAbove.active() && !tileBelow.lava())
            {
                if (!tile.bottomSlope() && !tile.topSlope() && !tile.halfBrick() && !tile.topSlope())
                {
                    tileAbove.type = (ushort)ModContent.TileType<CrystalGrowth>();
                    tileAbove.active(true);
                    tileAbove.frameY = 0;
                    tileAbove.frameX = (short)(WorldGen.genRand.Next(15) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }
            if (WorldGen.genRand.NextBool(25) && !tileAbove.active() && !tileBelow.lava())
            {
                if (!tile.bottomSlope() && !tile.topSlope() && !tile.halfBrick() && !tile.topSlope())
                {
                    tileAbove.type = (ushort)ModContent.TileType<CrystalCavernsPiles>();
                    tileAbove.active(true);

                    if (Framing.GetTileSafely(i, j).active())
                    {
                        WorldGen.KillTile(i, j);
                        WorldGen.PlaceObject(i, j, (ushort)ModContent.TileType<CrystalCavernsPiles>());
                    }
                    tileAbove.frameY = 0;
                    tileAbove.frameX = (short)(WorldGen.genRand.Next(3) * 36);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }
            if (WorldGen.genRand.NextBool(25) && !tileAbove.active() && !tileBelow.lava())
            {
                if (!tile.bottomSlope() && !tile.topSlope() && !tile.halfBrick() && !tile.topSlope())
                {
                    tileAbove.type = (ushort)ModContent.TileType<CavernsRubbleFloor>();
                    tileAbove.active(true);
                    tileAbove.frameY = 0;
                    tileAbove.frameX = (short)(WorldGen.genRand.Next(13) * 18);
                    WorldGen.SquareTileFrame(i, j + 2, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }
            if (WorldGen.genRand.NextBool(25) && tileAbove.liquid > 250 && !tileAbove.active() && !tileBelow.lava())
            {
                if (!tile.bottomSlope() && !tile.topSlope() && !tile.halfBrick() && !tile.topSlope())
                {
                    tileAbove.type = (ushort)ModContent.TileType<LuminVines>();
                    tileAbove.active(true);
                    WorldGen.SquareTileFrame(i, j - 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }
            if (WorldGen.genRand.NextBool(15) && tileBelow.liquid > 250 && !tileBelow.active() && !tileBelow.lava())
            {
                if (!tile.bottomSlope())
                {
                    tileBelow.type = (ushort)ModContent.TileType<LuminVines>();
                    tileBelow.active(true);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }
        }
    }
}