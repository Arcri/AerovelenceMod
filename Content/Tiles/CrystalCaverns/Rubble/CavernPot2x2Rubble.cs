using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble
{
    public class CavernPot2x2Rubble : ModTile
    {
        public override string Texture => "AerovelenceMod/Content/Tiles/CrystalCaverns/Rubble/CavernPot2x2Rubble";
        private Asset<Texture2D> glowTexture;
        private int drawYOffset;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileGlowMask[Type] = 1;

            DustType = DustID.BlueTorch;
            HitSound = SoundID.Shatter;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 3;
            drawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddMapEntry(new Microsoft.Xna.Framework.Color(70, 70, 85));

            // Tile breaks when placed over
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;

            glowTexture = ModContent.Request<Texture2D>(Texture + "_Glowmask");
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            // Draw original texture
            spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + drawYOffset) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
                Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

            // Pulsating color for glowmask
            Color maskColor = Color.White
                * MathHelper.Lerp(0.1f, 1f, ((float)Math.Pow(Math.Sin(NoiseHelper.GetDynamicNoise(new Vector2(i * 0.2f, j * 0.2f), Main.GlobalTimeWrappedHourly * 0.2f)), 4)));

            // Draw glowmask
            spriteBatch.Draw(
                glowTexture.Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + drawYOffset) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
                maskColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Return false to stop vanilla draw
            return false;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ItemID.SuspiciousLookingEye);
            yield return new Item(ItemID.Torch, 5);
        }
    }
}
