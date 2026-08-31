using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural.Flora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    public class CrystalGrassDevItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<CrystalGrassTile>();
            Item.rare = ItemRarityID.White;
            Item.value = 5;
        }
    }

    [LegacyName("CrystalGrass")]
    public class CrystalGrassTile : ModTile
    {
        private Asset<Texture2D> glowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<CrystalGrassTile>()] = true;
            Main.tileBlendAll[this.Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(80, 130, 200));
            //ItemDrop = ModContent.ItemType<Items.Placeable.CrystalDirtItem>();
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<CrystalDirtTile>();
            TileID.Sets.GeneralPlacementTiles[Type] = false;

            glowTexture = ModContent.Request<Texture2D>(Texture + "_Glowmask");
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!effectOnly)
            {
                fail = true;
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<CrystalDirtTile>();
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
                        Main.tile[tileX, tileY].TileType = (ushort)ModContent.TileType<CrystalGrassTile>();
                        WorldGen.SquareTileFrame(tileX, tileY, true);
                    }
                }
            }
            for (int x = i - 1; x <= i + 1; x++)
            {
                for (int y = j - 1; y <= j + 1; y++)
                {
                    if ((x != i || j != y) && Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<CrystalDirtTile>())
                    {
                        WorldGen.SpreadGrass(x, y, ModContent.TileType<CrystalDirtTile>(), Type, false, new TileColorCache());
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

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            // Draw original texture
            spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
                Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

            // Pulsating color for glowmask
            Color maskColor = Color.White   
                * MathHelper.Lerp(0.1f, 0.7f, ((float)Math.Pow(Math.Sin(NoiseHelper.GetDynamicNoise(new Vector2(i * 0.02f, j * 0.02f), Main.GlobalTimeWrappedHourly * 0.25f)), 2)));

            // Draw glowmask
            spriteBatch.Draw(
                glowTexture.Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
                maskColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Return false to stop vanilla draw
            return false;
        }
    }
}