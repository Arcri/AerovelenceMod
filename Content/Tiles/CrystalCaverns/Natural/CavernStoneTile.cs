
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

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    [LegacyName("CavernStone")]
    public class CavernStoneTile : ModTile
    {
        private Asset<Texture2D> glowTexture;

        public override void SetStaticDefaults()
        {
            MineResist = 2.5f;
            MinPick = 40;
            Main.tileSolid[Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("CrystalDirt").Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("CrystalGrass").Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("ChargedStone").Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("FieldStone").Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("CitadelStone").Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("LushGrowth").Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(075, 075, 095));
            DustType = 59;
            HitSound = SoundID.Tink;
            CommonTileHelper.SetTileProtection(this);
            TileID.Sets.GeneralPlacementTiles[Type] = false;

            glowTexture = ModContent.Request<Texture2D>(Texture + "_Glowmask");
        }
        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 ? Vector2.Zero : Vector2.One * 12;

        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0));
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
    public class CavernStoneItem : ModItem
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
            Item.createTile = ModContent.TileType<CavernStoneTile>();
            Item.rare = ItemRarityID.White;
            Item.value = 5;
        }
    }
}
