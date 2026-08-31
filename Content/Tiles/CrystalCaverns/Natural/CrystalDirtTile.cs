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
    [LegacyName("CrystalDirt")]
    public class CrystalDirtTile : ModTile
    {
        private Asset<Texture2D> glowTexture;

        public override void SetStaticDefaults()
        {
            MineResist = 2.5f;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<CrystalGrassTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<CavernCrystalTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<CavernStoneTile>()] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(80, 80, 110));
            DustType = 116;
            HitSound = SoundID.Dig;
            TileID.Sets.GeneralPlacementTiles[Type] = false;

            glowTexture = ModContent.Request<Texture2D>(Texture + "_Glowmask");
        }
        public override bool CanExplode(int i, int j)
        {
            return true;
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
                * MathHelper.Lerp(0.0f, 2f, ((float)Math.Pow(Math.Sin(NoiseHelper.GetDynamicNoise(new Vector2(i * 0.05f, j * 0.05f), Main.GlobalTimeWrappedHourly * 0.1f)), 6)));

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

    public class CrystalDirtItem : ModItem
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
            Item.createTile = ModContent.TileType<CrystalDirtTile>();
            Item.rare = ItemRarityID.White;
            Item.value = 5;
        }
    }
}