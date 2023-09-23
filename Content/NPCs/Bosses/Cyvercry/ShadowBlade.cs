using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
	public class ShadowBlade : ModProjectile
	{
		private int timer;
		
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("ShadowBlade");
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.damage = 0;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.timeLeft = 560;
			Projectile.ignoreWater = true;
			Projectile.extraUpdates = 1;
			Projectile.tileCollide = false;
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.alpha = 0;
		}

		public override void AI()
        {
			ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
			Player player = Main.player[(int)Projectile.ai[0]];


			Projectile.rotation = MathHelper.ToRadians(90) + Projectile.velocity.ToRotation();
			if (Projectile.timeLeft == 420)
			{

				Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 11;

				SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/hero_fury_charm_burst") with { Pitch = .78f, PitchVariance = 0.2f, Volume = 0.5f };
				SoundEngine.PlaySound(style, Projectile.Center);


				for (int i = 0; i < 360; i += 360)
				{
					Vector2 circular = new Vector2(12, 0).RotatedBy(MathHelper.ToRadians(i));

					Dust d = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleQuadStar>(), Projectile.velocity * -0.5f,
						Color.DeepPink, 1.5f, 0.6f, 0f, dustShader);
					d.noLight = true;
				}

				whiteIntensity = 1f;
			}
			if (Projectile.timeLeft > 420)
			{
				if (player.active)
				{
					Vector2 toPlayer = Projectile.Center - player.Center;
					toPlayer = toPlayer.SafeNormalize(Vector2.Zero) * 0.01f;
					Projectile.velocity = -toPlayer;
				}
			}
			else
			{
				Projectile.hostile = true;

				if (timer % 2 == 0)
                {
					Dust d = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.Zero,	
						Color.DeepPink, 0.5f, 0.6f, 0f, dustShader);
					d.noLight = true;
				}


				//int dust = Dust.NewDust(Projectile.Center + new Vector2(-4, -4), 0, 0, 235, 0, 0, Projectile.alpha, default, 1.25f);
				//Main.dust[dust].noGravity = true;
				//Main.dust[dust].velocity *= 0.1f;
				//Main.dust[dust].scale *= 0.75f;
			}
			//Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 1.8f / 255f, (255 - Projectile.alpha) * 0.0f / 255f, (255 - Projectile.alpha) * 0.0f / 255f);
			if (Projectile.timeLeft <= 25)
				Projectile.alpha += 10;

			whiteIntensity = Math.Clamp(MathHelper.Lerp(whiteIntensity, -0.01f, 0.04f), 0f, 1f);
			startBonus = Math.Clamp(MathHelper.Lerp(startBonus, -0.25f, 0.07f), 0, 1);


			timer++;
		}

		float whiteIntensity = 0f;
		float startBonus = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/ShadowBladeOuter").Value;
			int num156 = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new Rectangle(0, y3, texture.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;

			Vector2 scaleV2 = new Vector2(1f - (whiteIntensity * 0.75f), 1f);
			Color col1 = Color.Lerp(Color.DeepPink, Color.White, whiteIntensity);
			Color col2 = Color.Lerp(Color.DeepPink * 1.5f, Color.White, whiteIntensity);


			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/CyverAura", AssetRequestMode.ImmediateLoad).Value;

			myEffect.Parameters["uColor"].SetValue(col1.ToVector3() * 0.5f * (1f + (0.5f * startBonus)));
			myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/CoolNoise").Value);
			myEffect.Parameters["uTime"].SetValue(timer);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
			myEffect.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, 0f), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, scaleV2 * 1.2f * (1f + (0.5f * startBonus)), SpriteEffects.None, 0f);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			var TexOut = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/ShadowBladeOuter").Value;
			Main.spriteBatch.Draw(TexOut, Projectile.Center - Main.screenPosition, TexOut.Frame(1, 1, 0, 0), col2, Projectile.rotation, TexOut.Size() / 2, scaleV2 * (1f + (0.25f * startBonus)), SpriteEffects.None, 0f);


			var Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/ShadowBlade").Value;
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, scaleV2, SpriteEffects.None, 0f);

			return false;
        }
    }
}


