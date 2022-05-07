using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AerovelenceMod.Common.Utilities
{
    internal static class GlowmaskUtilities
    {
        public static void DrawNPCGlowmask(SpriteBatch spriteBatch, Texture2D texture, NPC npc)
        {
            SpriteEffects effects = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw
            (
                texture,
                npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY),
                npc.frame,
                Color.White,
                npc.rotation,
                npc.frame.Size() / 2f,
                npc.scale,
                effects,
                0
            );
        }

        public static void DrawItemGlowmask(SpriteBatch spriteBatch, Texture2D texture, Item item, float rotation, float scale)
        {
            Vector2 drawPosition = new Vector2
            (
                item.position.X - Main.screenPosition.X + item.width * 0.5f,
                item.position.Y - Main.screenPosition.Y + item.height - texture.Height * 0.5f + 2f
            );

            Main.EntitySpriteDraw
            (
                texture,
                drawPosition,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                rotation,
                texture.Size() / 2f,
                scale,
                SpriteEffects.None,
                0
            );
        }

        public static void DrawTileGlowmask(SpriteBatch spriteBatch, Texture2D texture, int i, int j)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 36 ? 18 : 16;

            Vector2 drawPosition = new Vector2
            (
                i * 16 - (int)Main.screenPosition.X,
                j * 16 - (int)Main.screenPosition.Y
            ) + zero;

            Main.EntitySpriteDraw
            (
                texture,
                drawPosition,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height),
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0
            );
        }
    }
}
