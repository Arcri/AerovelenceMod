using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics;
using ReLogic.Content;

namespace AerovelenceMod.Content.Projectiles
{
	public class ArrowTelegraph : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		private int timer;
		public Color col = Color.White;
		public Vector2 scale = new Vector2(1f, 1f);

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Arrow");
		}

		public override void SetDefaults()
		{
			Projectile.scale = 2f;
			Projectile.width = 2;
			Projectile.height = 2;

			Projectile.friendly = false;
			Projectile.hostile = false;

			Projectile.timeLeft = 40;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;

			Projectile.alpha = 0;

		}
        public override bool? CanCutTiles()
        {
			return false;
        }
        public override bool? CanDamage()
        {
			return false;
        }

        public override void AI()
		{
			Projectile.scale -= 0.01f;
			Projectile.alpha += 7;
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
			Projectile.velocity *= 0.99f;
			timer++;

		}
		public override Color? GetAlpha(Color lightColor)
		{
			return (col * 3f) * Projectile.Opacity;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value; 

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(1, 1, 0, 0), col * (Projectile.Opacity * 1.5f), Projectile.rotation, texture.Size() / 2, Projectile.scale * scale, SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			return false;
		}
	}

}