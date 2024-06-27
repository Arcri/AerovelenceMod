using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Projectiles.Other
{
    public class H3Impact : ModProjectile
    {
		float timer = 0;
		public float opacity = 1f;
		public Color color = Color.White;
		public float size = 1f;
		public float speed = 0.2f;


		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("H3 Impact");
		}

        public override void SetDefaults()
		{
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.timeLeft = 200;
			Projectile.tileCollide = false;
			Projectile.scale = 1f;

		}

		public override bool? CanDamage()
		{
			return false;
		}

		float xScale = 0.3f;
		float yScale = 1f;
		public override void AI()
        {
			Player player = Main.player[Projectile.owner];

			//Random rot of flare
			if (timer == 1)
				Projectile.ai[1] = Main.rand.NextFloat(6.28f);

			if (timer > 1)
            {
				xScale = Math.Clamp(MathHelper.Lerp(xScale, 3, speed * 0.3f), 0, 10);
				yScale = Math.Clamp(MathHelper.Lerp(yScale, -0.2f, speed), 0.02f, 1);
			}


			if (yScale <= 0.02f)
				Projectile.active = false;

			Projectile.ai[0] = Math.Clamp(MathHelper.Lerp(Projectile.ai[0], 1.1f, 0.04f), 0, 1);

			Projectile.ai[1] += 0.04f;
			timer++;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Projectiles/Other/H3Impact").Value;
			Texture2D Tex3 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_5").Value;

			Vector2 scale = new Vector2(xScale * size, yScale * size);

			Main.spriteBatch.Draw(Tex3, Projectile.Center - Main.screenPosition, null, Color.Black * (0.35f - Projectile.ai[0]), Projectile.ai[1], Tex3.Size() / 2, 0.25f + (0.75f * yScale), SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


			Main.spriteBatch.Draw(Tex3, Projectile.Center - Main.screenPosition, null, color * (1.25f - Projectile.ai[0]), Projectile.ai[1], Tex3.Size() / 2, 0.25f + (0.75f * yScale), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Tex3, Projectile.Center - Main.screenPosition, null, Color.White * (1.25f - Projectile.ai[0]), -Projectile.ai[1], Tex3.Size() / 2, 0.25f + (0.75f * yScale) * 0.5f, SpriteEffects.None, 0f);

			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
			myEffect.Parameters["uColor"].SetValue(color.ToVector3() * 3.2f);
			myEffect.Parameters["uTime"].SetValue(2);
			myEffect.Parameters["uOpacity"].SetValue(0.6f); //0.6
			myEffect.Parameters["uSaturation"].SetValue(1.2f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

			//Activate Shader
			myEffect.CurrentTechnique.Passes[0].Apply();
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, null, color * opacity, Projectile.rotation, Tex.Size() / 2, scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, null, color * opacity, Projectile.rotation, Tex.Size() / 2, scale, SpriteEffects.None, 0f);

			Main.spriteBatch.End(); //make this restart better later
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}

    }
}
