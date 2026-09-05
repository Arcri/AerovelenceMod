using AerovelenceMod.Common.Utilities;
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
    public class LushGrowthTile : ModTile
    {
        private Asset<Texture2D> glowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LushGrowthTile>()] = true;
            Main.tileBlendAll[this.Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(50, 100, 70));
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

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0f;
            g = 0.050f;
            b = 0.200f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y);

            // Pulsating color for glowmask
            Color maskColor = Color.White
                * MathHelper.Lerp(0.0f, 2f, ((float)Math.Pow(Math.Sin(NoiseHelper.GetDynamicNoise(new Vector2(i * 0.05f, j * 0.05f), Main.GlobalTimeWrappedHourly * 0.1f)), 8)));

            DrawUtils.DrawSlopedTile(glowTexture.Value, position, tile, maskColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }

    public class LushGrowthItem : ModItem 
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
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
}