#region Using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Projectiles
{
	public static class ProjectileExtensions
	{
		public static bool DrawAroundOrigin(this ModProjectile p, SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[p.Projectile.type];
			Rectangle frame = texture.Frame(1, Main.projFrames[p.Projectile.type], 0, p.Projectile.frame);
			Vector2 origin = frame.Size() / 2 + new Vector2(p.drawOriginOffsetX, p.drawOriginOffsetY);
			SpriteEffects effects = p.Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Vector2 drawPosition = p.Projectile.Center - Main.screenPosition + new Vector2(p.drawOffsetX, 0);

			spriteBatch.Draw(texture, drawPosition, frame, lightColor, p.Projectile.rotation, origin, p.Projectile.scale, effects, 0f);

			return (false);
		}

		public static void DrawProjectileTrailCentered(this ModProjectile p, SpriteBatch spriteBatch, Color drawColor, float initialOpacity = 0.8f, float opacityDegrade = 0.2f, int stepSize = 1)
		{
			Texture2D texture = Main.projectileTexture[p.Projectile.type];
			Rectangle frame = texture.Frame(1, Main.projFrames[p.Projectile.type], 0, p.Projectile.frame);
			Vector2 origin = frame.Size() / 2;
			SpriteEffects effects = p.Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[p.Projectile.type]; i += stepSize)
			{
				float opacity = initialOpacity - opacityDegrade * i;
				spriteBatch.Draw(texture, p.Projectile.oldPos[i] + p.Projectile.Hitbox.Size() / 2 - Main.screenPosition, frame, drawColor * opacity, p.Projectile.oldRot[i], origin, p.Projectile.scale, effects, 0f);
			}
		}
	}
}
