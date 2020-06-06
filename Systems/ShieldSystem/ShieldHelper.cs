using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Systems.ShieldSystem
{
	public static class ShieldHelper
	{
		public static void PlayShieldAnimation(Player player, float rotation)
		{
			//Rectangle sourceRect = new Rectangle(); //Todo: Framing
			Texture2D texture = AeroMod.Textures.Shield;

			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
			Main.spriteBatch.Draw(texture, player.Center - Main.screenPosition, null, Color.White, rotation, texture.Size() / 2, 1f, SpriteEffects.None, default);
			Main.spriteBatch.End();
		}
	}
}