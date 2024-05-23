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
using AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
	public class TeleportFXCyver : ModProjectile
	{
		private int timer;

		public bool reverse = false;
		public bool blue = false;
		public override string Texture => "Terraria/Images/Projectile_0";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("ShadowBlade");
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;

			Projectile.scale = 1f;
			Projectile.timeLeft = 120;

			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;

			Projectile.width = 1;
			Projectile.height = 1;

			Projectile.hide = true;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
			base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
		}

		public override bool? CanDamage()
        {
			return false;
        }

		float mainAlpha = 1f;

		float starAlpha = 1f;
		public List<StarParticle> stars = new List<StarParticle>();
		bool spawnedStars = false;
		public override void AI()
		{
			
			if (timer == 0 && reverse)
			{
				scale = 0;
			}
			
			bool condition = false;

			if (reverse)
				condition = (scale == 0);
			else
				condition = (scale == 1 && timer > 2);

			if (condition)
			{
                if (!spawnedStars)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        StarParticle newStar = new StarParticle(Projectile.Center, Main.rand.NextVector2CircularEdge(5f, 5f) * Main.rand.NextFloat(0.7f, 1.3f));
                        stars.Add(newStar);
                    }
                    spawnedStars = true;
                }

				if (!reverse)
				{
                    bool anyStarsActive = false;
                    foreach (StarParticle star in stars)
                    {
                        star.Update();
                    }
                }

            }

			if (reverse)
			{
                foreach (StarParticle star in stars)
                {
                    star.Update();
                }
            }


			if (reverse)
			{
				if (timer > 10)
				{
                    alpha = MathHelper.Clamp(alpha - 0.05f, 0, 1);
                    scale = 1 - Math.Clamp(1- scale + 0.03f, 0f, 1.2f);

                }
				else
                    scale = 1 - Math.Clamp(getProgress(timer * 0.3f), 0f, 1f);
            }
            else
			{

                alpha = MathHelper.Clamp(alpha - 0.01f, 0, 1);

                scale = Math.Clamp(getProgress(timer * 0.06f), 0f, 1f);

				if (blue)
					scale = Math.Clamp(getProgress(timer * 0.10f), 0f, 1f);
			}

			Projectile.velocity *= 0.85f;

			timer++;

		}

		float scale = 1f;
		float alpha = 1f;
		public override bool PreDraw(ref Color lightColor)
        {
            
			Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (StarParticle star in stars)
            {
				star.DrawStar(Main.spriteBatch, Mod.Assets.Request<Texture2D>("Assets/TrailImages/GlowStar").Value, blue ? Color.SkyBlue : Color.HotPink);
            }

			if (blue)
            {
				Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/CyverBlueFuzzy").Value;
				Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, Tex.Size() / 2, 1f * (1f - scale), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, Tex.Size() / 2, 0.98f * (1f - scale), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Square(3, 3), Tex.Frame(1, 1, 0, 0), Color.DeepSkyBlue * 0.5f * alpha, Projectile.rotation, Tex.Size() / 2, 1.1f * (1f - scale), SpriteEffects.None, 0f);

			}
			else
            {
				Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/CyvercryGlowy").Value;
				Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, Tex.Size() / 2, 1f * (1f - scale), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, Tex.Size() / 2, 1f * (1f - scale), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Square(3, 3), Tex.Frame(1, 1, 0, 0), Color.HotPink * 0.5f * alpha, Projectile.rotation, Tex.Size() / 2, 1.4f * (1f - scale), SpriteEffects.None, 0f);
			}

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            
			return false;
        }

		public float getProgress(float input)
        {
			float toReturn;

			float c1 = 1.70158f;
			float c3 = c1 + 1f;

			toReturn =  c3 * input * input * input - c1 * input * input;
			return toReturn;
        }
    }

	public class StarParticle
	{
		public Vector2 Velocity;
		public Vector2 Center;

		public float rotation;
		public Color color;
		public float scale;
		public float alpha;

		public int timer;

		public StarParticle(Vector2 pos, Vector2 vel)
		{
			Center = pos;
			alpha = 1f;
			Velocity = vel;
			scale = 1f;
			rotation = vel.ToRotation();
		}

		public void Update()
		{
			float size = 1f;

			if (timer > 20)
				alpha = MathHelper.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.03f), 0f, 1f);

			Center += Velocity;
			Velocity *= 0.95f;
			rotation += Velocity.X * 0.03f;

			scale *= 0.99f;
			timer++;
		}


		public void DrawStar(SpriteBatch sb, Texture2D tex, Color col)
		{
			sb.Draw(tex, Center - Main.screenPosition, null, col * alpha, rotation, tex.Size() / 2, scale * 0.35f, SpriteEffects.None, 0f);
			sb.Draw(tex, Center - Main.screenPosition, null, col * alpha, rotation, tex.Size() / 2, scale * 0.35f, SpriteEffects.None, 0f);
        }

    }

	public class CyverDeathExplosion : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		public bool PinkTrueBlueFalse = false;
		public override void SetDefaults()
		{
			Projectile.damage = 0;
			Projectile.friendly = false;
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.tileCollide = false;

			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.hostile = false;
			Projectile.timeLeft = 800;
			Projectile.scale = 1f;

		}

		public override bool? CanDamage()
		{
			return false;
		}

		int timer = 0;
		public float scale = 0.25f;
		float alpha = 1;

		public override void AI()
		{
			if (timer >= 0)
			{
				scale = scale + 0.09f;

				if (timer >= 6)
					alpha -= 0.065f;
			}

			Projectile.timeLeft = 2;

			if (alpha <= 0)
				Projectile.active = false;

			timer++;
		}

		Effect myEffect = null;
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/spotlight_8").Value;
			Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/bright_star").Value;
			Texture2D Star2 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_16").Value;


			if (myEffect == null)
				myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

			myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
			myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/PinkGrad").Value); //also works well with GreenGrad and softer blue grad
			myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

			myEffect.Parameters["flowSpeed"].SetValue(0.3f);
			myEffect.Parameters["vignetteSize"].SetValue(1f);
			myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
			myEffect.Parameters["distortStrength"].SetValue(0.06f);
			myEffect.Parameters["xOffset"].SetValue(0.0f);
			myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
			myEffect.Parameters["colorIntensity"].SetValue(alpha * 3f);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
			myEffect.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Flare.Size() / 2, scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Flare.Size() / 2, scale, SpriteEffects.None, 0f);

			Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Star.Size() / 2, scale * 2f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Star2, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Star2.Size() / 2, scale * 1f, SpriteEffects.None, 0f);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
	}

	public class CyverRoarPulse : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		public float intensity = 1f;
		public bool forRoar = true;
		public bool pixel = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 9000;
        }
        public override void SetDefaults()
		{
			Projectile.damage = 0;
			Projectile.friendly = false;
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.tileCollide = false;

			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.hostile = false;
			Projectile.timeLeft = 800;
			Projectile.scale = 0.28f;

		}

		public override bool? CanDamage()
		{
			return false;
		}

		int timer = 0;
		public float scale = 0.25f;
		float alpha = 1;

		public bool special = false;

		public override void AI()
		{
			Projectile.velocity *= 0.95f;
			if (timer == 0)
            {
				Projectile.ai[0] = 0;
                //Projectile.ai[0] = Main.rand.NextBool() ? 1 : -1;
                Projectile.rotation = Main.rand.NextFloat(6.28f);
			}

			if (timer <= 40)
			{
				if (special)
                {
					if (timer <= 16)
                    {
						scale = MathHelper.Lerp(0f, 1f, Easings.easeOutQuint(timer / 60f));
					}
                    else
                    {
						scale += 0.08f;
						alpha -= 0.04f;
					}
				}
				else
					scale = MathHelper.Lerp(0f, 1f, Easings.easeOutQuint(timer / 40f));
			}

			if (timer >= 0)
			{
				//if (timer < 12)
					//scale = scale + 0.11f;
				//else
					//scale = scale + 0.07f; 

				if (timer >= (special ? 3 : 6))
					alpha -= special ? 0.08f : 0.065f;
			}

			Projectile.timeLeft = 2;

			if (alpha <= 0)
            {
				Projectile.active = false;
			}

			if (special)
				timer++;
			timer++;
		}

		Effect myEffect = null;
		public override bool PreDraw(ref Color lightColor)
		{
			String toAsset = (pixel ? "Assets/ImpactTextures/Royal_Resonance" : "Content/NPCs/Bosses/Cyvercry/Textures/circle_02");

			if (special) toAsset = "Assets/TrailImages/RainbowRod";//"Assets/Orbs/ElectricPopC";

			Texture2D Flare = Mod.Assets.Request<Texture2D>(toAsset).Value;

			float rot = ((float)Main.timeForVisualEffects * 0.12f * Projectile.ai[0]) + Projectile.rotation;
			float scale2 = scale * (pixel ? 3f : 1f) * 1f;

			if (myEffect == null)
				myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

			myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
			myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/PinkGrad").Value); 
			myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

			myEffect.Parameters["flowSpeed"].SetValue(0.3f);
			myEffect.Parameters["vignetteSize"].SetValue(1f);
			myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
			myEffect.Parameters["distortStrength"].SetValue(0.06f);
			myEffect.Parameters["xOffset"].SetValue(0.0f);
			myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
			myEffect.Parameters["colorIntensity"].SetValue(alpha * (!forRoar ? 1f : 3f) * intensity);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
			myEffect.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, rot, Flare.Size() / 2, scale2 * Projectile.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, rot, Flare.Size() / 2, scale2 * Projectile.scale, SpriteEffects.None, 0f);
			
			if (special)
            {
				Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, -rot, Flare.Size() / 2, scale2 * Projectile.scale, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, -rot, Flare.Size() / 2, scale2 * Projectile.scale, SpriteEffects.None, 0f);

			}


			//if (!forRoar)
			//Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, rot, Flare.Size() / 2, scale2 * Projectile.scale, SpriteEffects.None, 0f);

			//Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, rot + MathHelper.ToRadians(120), Flare.Size() / 2, scale * Projectile.scale, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, rot - MathHelper.ToRadians(120), Flare.Size() / 2, scale * Projectile.scale, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, rot, Flare.Size() / 2, scale * Projectile.scale, SpriteEffects.None, 0f);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			//Reset Again cuz rainbowrod bullshit
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
		}
	}

}


