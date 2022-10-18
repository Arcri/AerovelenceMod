using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Ember
{
	public class SolsearLaser : ModProjectile
	{

		public Vector2 endPoint;
		public float LaserRotation = 0;
		Vector2 mousePos = Vector2.Zero;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Solsear");

		}

		Vector2 storedCenter = Vector2.Zero;
		int timer = 0;

		float distFromPlayer = 0;

		Vector2 storedMousePos = Vector2.Zero;

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 1000;
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 10; //200
			Projectile.scale = 1f;

		}

		public override void AI()
		{
			if (timer % 120 == 0 && timer != 0)
            {
				ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
				for (int i = 50; i < 280; i += 6) //i = 0 ; i = 350
				{

					if (Main.rand.NextBool() && Main.rand.NextBool() && Main.rand.NextBool() && Main.rand.NextBool())
					{
						GlowDustHelper.DrawGlowDustPerfect(storedCenter + Vector2.UnitX.RotatedBy(LaserRotation + Math.PI) * i, ModContent.DustType<GlowCircleDust>()
							, Main.rand.NextVector2CircularEdge(2f, 2f) + ( (LaserRotation + MathHelper.Pi).ToRotationVector2() * 3f ), Color.OrangeRed, 0.2f, dustShader); //0.2f
						
					}

				}
			}
			distFromPlayer = Math.Clamp(distFromPlayer + 2, 0, 300);
			if (timer == 0)
            {
				storedMousePos = Main.MouseWorld;
				LaserRotation = Projectile.velocity.ToRotation() + (float)Math.PI;
				storedCenter = Projectile.Center;
			}
			if (timer == 200)
				Projectile.velocity = Vector2.Zero;

			Player Player = Main.player[Projectile.owner];

			storedMousePos = Vector2.Lerp(storedMousePos, Main.MouseWorld, 0.045f); //0.0075

			Projectile.Center = Player.Center + new Vector2(distFromPlayer, 0).RotatedBy((storedMousePos - Player.Center).ToRotation());

			if (!Player.channel)
				Projectile.active = false;
			LaserRotation = (Player.Center - Projectile.Center).ToRotation();
			Projectile.timeLeft++;
			storedCenter = Player.Center;
			timer++;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Projectile.velocity = Vector2.Zero;
			return false;
        }

        public override bool PreDraw(ref Color lightColor) 
		{

            #region oldFireLaser
			
            Projectile.scale = 2f;

			if (timer > 0) //Laser might be fucked up on first frame if we don't do this
			{


				Effect myEffect = ModContent.Request<Effect>("Redux/Effects/LaserShader", AssetRequestMode.ImmediateLoad).Value;

				//Extra_196
				//spark_07

				myEffect.Parameters["uColor"].SetValue(Color.OrangeRed.ToVector3() * 0.6f);
				myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
				myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value);
				myEffect.Parameters["uTime"].SetValue(timer * 0.002f);
				myEffect.Parameters["uSaturation"].SetValue(2);


				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


				//Activate Shader
				myEffect.CurrentTechnique.Passes[0].Apply();

				Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;


				endPoint = storedCenter;
				var texBeam = Mod.Assets.Request<Texture2D>("Assets/GlowTrailMoreRes").Value;


				Vector2 origin2 = new Vector2(0, texBeam.Height / 2);

				float height = (55f); //25

				if (height == 0)
					Projectile.active = false;

				int width = (int)(Projectile.Center - endPoint).Length() - 24;

				var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
				var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

				Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
				//Main.spriteBatch.Draw(texture, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);


				//for (int i = 0; i < width; i += 6)
				//Lighting.AddLight(pos + Vector2.UnitX.RotatedBy(LaserRotation) * i + Main.screenPosition, Color.DeepPink.ToVector3() * height * 0.020f); //0.030

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

				float height2 = (20f); //25

				if (height2 == 0)
					Projectile.active = false;

				int width2 = (int)(Projectile.Center - endPoint).Length() - 24;

				var pos2 = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
				var target2 = new Rectangle((int)pos2.X, (int)pos2.Y, width2, (int)(height2 * 1.2f));

				Main.spriteBatch.Draw(texBeam, target2, null, Color.OrangeRed, LaserRotation, origin2, 0, 0);

				for (int i = 0; i < width; i += 6)
					Lighting.AddLight(pos + Vector2.UnitX.RotatedBy(LaserRotation) * i + Main.screenPosition, Color.Orange.ToVector3() * height * 0.020f); //0.030

			}
			#endregion

			return false;
			
		}

		public override void PostDraw(Color lightColor)
        {
			var spotTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
			Texture2D glowTex = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;

			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
			//myEffect.Parameters["uColor"].SetValue(Color.WhiteSmoke.ToVector3() * 1f); 

			myEffect.Parameters["uColor"].SetValue(new Color(255, 75, 50).ToVector3() * 2.5f); 
			myEffect.Parameters["uTime"].SetValue(2);
			myEffect.Parameters["uOpacity"].SetValue(0.9f); 
			myEffect.Parameters["uSaturation"].SetValue(1.2f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

			Vector2 thisPos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;

			Main.spriteBatch.Draw(glowTex, thisPos, glowTex.Frame(1, 1, 0, 0), new Color(255, 75, 50), 0, glowTex.Size() / 2, 1.3f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(glowTex, thisPos, glowTex.Frame(1, 1, 0, 0), new Color(255, 75, 50), 0, glowTex.Size() / 2, 1.3f, SpriteEffects.None, 0);

			//Activate Shader
			myEffect.CurrentTechnique.Passes[0].Apply();

			//Main.spriteBatch.Draw(spotTex, storedCenter - Main.screenPosition, spotTex.Frame(1, 1, 0, 0), Color.Orange, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.2f), spotTex.Size() / 2, 0.15f, SpriteEffects.None, 0);
			//Main.spriteBatch.Draw(spotTex, storedCenter - Main.screenPosition, spotTex.Frame(1, 1, 0, 0), Color.Orange, Projectile.rotation + MathHelper.ToRadians(timer * 0.1f), spotTex.Size() / 2, 0.1f, SpriteEffects.None, 0);

			Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Orange, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.2f), spotTex.Size() / 2, 0.15f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Orange, Projectile.rotation + MathHelper.ToRadians(timer * 0.1f), spotTex.Size() / 2, 0.1f, SpriteEffects.None, 0);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
		}

        public override void Kill(int timeLeft)
        {
			//Main.NewText("amg");
        }

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{

			Player Player = Main.player[Projectile.owner];
			Vector2 unit = LaserRotation.ToRotationVector2();
			float point = 0f;
			// Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
			// It will look for collisions on the given line using AABB
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
				Player.Center, 22, ref point);

		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

			for (int j = 0; j < 1; j++)
			{
				Dust d = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 4),
					Color.OrangeRed, 0.55f, 0.4f, 0f, dustShader);
				d.velocity *= 0.5f;

			}

			SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.53f, PitchVariance = 0.25f, MaxInstances = -1, Volume = 0.2f };
			SoundEngine.PlaySound(style, target.Center);

			target.immune[Projectile.owner] = 5; //20 
		}
	}
}

