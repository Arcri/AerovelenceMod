using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace AerovelenceMod.Common.Utilities
{
    internal static class DrawUtils
    {

        //Me when steal from infernum
        internal static void SwapToRenderTarget(this RenderTarget2D renderTarget, Color? flushColor = null)
        {
            // Local variables for convinience.
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            // If we are in the menu, a server, or any of these are null, return.
            if (Main.gameMenu || Main.dedServ || renderTarget is null || graphicsDevice is null || spriteBatch is null)
                return;

            // Otherwise set the render target.
            graphicsDevice.SetRenderTarget(renderTarget);

            // "Flush" the screen, removing any previous things drawn to it.
            flushColor ??= Color.Transparent;
            graphicsDevice.Clear(flushColor.Value);
        }

        internal static void DrawSlopedTile(
            Texture2D texture,
            Vector2 position,
            Tile tile,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteEffects effects,
            float layerDepth)
        {
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            for (int x = 0; x < 8; x++)
            {
                int sliceWidth;
                int xOffset;

                int sliceHeight = 2;
                int yOffset = x * 2;

                switch (tile.Slope)
                {
                    default:
                    case SlopeType.Solid:
                        sliceWidth = 16;
                        xOffset = 0;
                        break;
                    case SlopeType.SlopeDownLeft:
                        sliceWidth = 2 + x * 2;
                        xOffset = 0;
                        break;
                    case SlopeType.SlopeDownRight:
                        sliceWidth = 2 + x * 2;
                        xOffset = 16 - sliceWidth;
                        break;
                    case SlopeType.SlopeUpLeft:
                        sliceWidth = 16 - x * 2;
                        xOffset = 0;
                        break;
                    case SlopeType.SlopeUpRight:
                        sliceWidth = 16 - x * 2;
                        xOffset = 16 - sliceWidth;
                        break;
                }

                Main.spriteBatch.Draw(
                    texture,
                    position + new Vector2(xOffset, yOffset + tile.IsHalfBlock.ToInt() * 8) + zero,
                    new Rectangle(tile.TileFrameX + xOffset, tile.TileFrameY + yOffset, sliceWidth, sliceHeight),
                    color, rotation, origin, scale, effects, layerDepth);
            }
        }
    }
}
