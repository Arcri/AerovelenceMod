using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Flora;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles
{
    public class CavernStone : ModTile
    {
        public override void SetStaticDefaults()
        {
            MineResist = 2.5f;
            MinPick = 40;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CrystalDirt").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CrystalGrass").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("ChargedStone").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("FieldStone").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CitadelStone").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("LushGrowth").Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(065, 065, 085));
            DustType = 59;
            SoundType = SoundID.Tink;
            ItemDrop = ModContent.ItemType<Items.Placeables.Blocks.CavernStone>();

        }
        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 ? Vector2.Zero : Vector2.One * 12;

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
            Main.Main.EntitySpriteDraw(mod.GetTexture("Blocks/CrystalCaverns/Tiles/CavernStone_Glowmask"), new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.frameX, tile.frameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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
            if (WorldGen.genRand.NextBool(25) && !tileAbove.HasTile && tile.LiquidType != LiquidID.Lava)
            {
                if (!tile.BottomSlope && !tile.TopSlope && !tile.IsHalfBlock && !tile.TopSlope)
                {
                    tileAbove.TileType = (ushort)ModContent.TileType<CrystalGrowth>();
                    tileAbove.HasTile = true;
                    tileAbove.TileFrameY = 0;
                    tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(15) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }
            if (WorldGen.genRand.NextBool(25) && !tileAbove.HasTile && tile.LiquidType != LiquidID.Lava)
            {
                if (!tile.BottomSlope && !tile.TopSlope && !tile.IsHalfBlock && !tile.TopSlope)
                {
                    tileAbove.TileType = (ushort)ModContent.TileType<CavernsRubbleFloor>();
                    tileAbove.HasTile = true;
                    tileAbove.TileFrameY = 0;
                    tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(13) * 18);
                    WorldGen.SquareTileFrame(i, j + 2, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }
            if (WorldGen.genRand.NextBool(25) && tileAbove.LiquidAmount > 250 && !tileAbove.HasTile && tile.LiquidType != LiquidID.Lava)
            {
                if (!tile.BottomSlope && !tile.TopSlope && !tile.IsHalfBlock && !tile.TopSlope)
                {
                    tileAbove.TileType = (ushort)ModContent.TileType<LuminVines>();
                    tileAbove.HasTile = true;
                    WorldGen.SquareTileFrame(i, j - 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }
            if (WorldGen.genRand.NextBool(15) && tileBelow.LiquidAmount > 250 && !tileBelow.HasTile && tile.LiquidType != LiquidID.Lava)
            {
                if (!tile.BottomSlope)
                {
                    tileBelow.TileType = (ushort)ModContent.TileType<LuminVines>();
                    tileBelow.HasTile = true;
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