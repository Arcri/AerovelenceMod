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
using AerovelenceMod.Common.Globals.SkillStrikes;


namespace AerovelenceMod.Content.Items.Weapons.Ember
{
	public class SolsearLaser : ModProjectile
	{
        public override string Texture => "Terraria/Images/Projectile_0";

        public Vector2 endPoint;
		public float LaserRotation = 0;
		Vector2 mousePos = Vector2.Zero;

		Vector2 storedCenter = Vector2.Zero;
		int timer = 0;

		float distFromPlayer = 0;

		Vector2 storedMousePos = Vector2.Zero;
		public float baseDamage = 0f;
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
			Projectile.DamageType = DamageClass.Ranged;
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
						GlowDustHelper.DrawGlowDustPerfect(storedCenter + Vector2.UnitX.RotatedBy(LaserRotation + Math.PI) * i, ModContent.DustType<GlowCircleDust>(),
							Main.rand.NextVector2CircularEdge(2f, 2f) + ( (LaserRotation + MathHelper.Pi).ToRotationVector2() * 3f ), Color.OrangeRed, 0.2f, dustShader); //0.2f
						
					}

				}
			}

			distFromPlayer = Math.Clamp(distFromPlayer + 2, 0, 270);
			if (timer == 0)
            {
				baseDamage = Projectile.damage;
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

            if (true && timer > 0) //Laser might be fucked up on first frame if we don't do this
            {

                Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/CheapScroll", AssetRequestMode.ImmediateLoad).Value;
                #region Shader Params
                myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value);
                myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);

                Color c1 = new Color(255, 80, 25);
                Color c2 = new Color(255, 80, 25);

                myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
                myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
                myEffect.Parameters["Color1Mult"].SetValue(1f);
                myEffect.Parameters["Color2Mult"].SetValue(1f);
                myEffect.Parameters["totalMult"].SetValue(1f);

                myEffect.Parameters["tex1reps"].SetValue(0.25f);
                myEffect.Parameters["tex2reps"].SetValue(0.25f);
                myEffect.Parameters["satPower"].SetValue(1f);
                myEffect.Parameters["time1Mult"].SetValue(1f);
                myEffect.Parameters["time2Mult"].SetValue(1f);
                myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.018f);
                #endregion

                //Texture2D LaserTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/FlashLightBeam").Value;
                Texture2D LaserTexture = Mod.Assets.Request<Texture2D>("Assets/ThinLineGlowClear").Value;

                Texture2D Black = Mod.Assets.Request<Texture2D>("Assets/GlowTrailMoreRes").Value;

                Vector2 origin2 = new Vector2(0, LaserTexture.Height / 2);

                float height = (140f); //25

                int width = (int)(Projectile.Center - storedCenter).Length() - 24;

                var pos = Projectile.Center - Main.screenPosition;
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.7f));
                var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.65f));
                var target3 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.55f));

                Main.spriteBatch.Draw(Black, target3, null, Color.Black * 1f, LaserRotation, new Vector2(0, Black.Height / 2), 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

                //Activate Shader
                myEffect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);

                //Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Main.spriteBatch.Draw(LaserTexture, target2, null, Color.Red * 0.5f, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(LaserTexture, target2, null, Color.Red * 0.25f, LaserRotation, origin2, 0, 0);
                //for (int i = 0; i < width; i += 6)
                //Lighting.AddLight(pos + Vector2.UnitX.RotatedBy(LaserRotation) * i + Main.screenPosition, Color.Orange.ToVector3() * height * 0.015f); //0.030


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


                return false;
            }

            #region oldFireLaser

            Projectile.scale = 2f;

			if (timer > 0) //Laser might be fucked up on first frame if we don't do this
			{


				Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/LaserShader", AssetRequestMode.ImmediateLoad).Value;

				//Extra_196
				//spark_07

				myEffect.Parameters["uColor"].SetValue(Color.OrangeRed.ToVector3() * 0.6f);
				myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
				myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value);
				myEffect.Parameters["uTime"].SetValue(timer * 0.002f);
				myEffect.Parameters["uSaturation"].SetValue(1);


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);


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
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


                float height2 = (20f); //25

				if (height2 == 0)
					Projectile.active = false;

				int width2 = (int)(Projectile.Center - endPoint).Length() - 24;

				var pos2 = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
				var target2 = new Rectangle((int)pos2.X, (int)pos2.Y, width2, (int)(height2 * 1.2f));

				Main.spriteBatch.Draw(texBeam, target2, null, Color.OrangeRed, LaserRotation, origin2, 0, 0);

				for (int i = 0; i < width; i += 6)
					Lighting.AddLight(pos + Vector2.UnitX.RotatedBy(LaserRotation) * i + Main.screenPosition, Color.Orange.ToVector3() * height * 0.015f); //0.030

			}
			#endregion

			return false;
			
		}

		public override void PostDraw(Color lightColor)
        {
            Texture2D spotTex = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/CrispStarPMA").Value;
			Texture2D glowTex = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;
            Vector2 thisPos = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(glowTex, thisPos, glowTex.Frame(1, 1, 0, 0), Color.Black * 0.65f, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), glowTex.Size() / 2, 0.15f, SpriteEffects.None, 0);

            //Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Black, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 0.6f * 2f, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Black, Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.37f * 2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


			//Main.spriteBatch.Draw(glowTex, thisPos, glowTex.Frame(1, 1, 0, 0), new Color(255, 75, 50) with { A = 255 }, 0, glowTex.Size() / 2, 1.3f, SpriteEffects.None, 0);
			//Main.spriteBatch.Draw(glowTex, thisPos, glowTex.Frame(1, 1, 0, 0), new Color(255, 75, 50) with { A = 255 }, 0, glowTex.Size() / 2, 1.3f, SpriteEffects.None, 0);

			Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.OrangeRed with { A = 255 }, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 1.2f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.OrangeRed with { A = 255 }, Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Orange with { A = 255 }, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 0.6f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.White with { A = 255 }, Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.37f, SpriteEffects.None, 0);
            
			Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (timer == 1)
				return false;

			if (targetHitbox.Distance(Projectile.Center) < 40)
            {
				Projectile.damage = (int)(baseDamage * 1.5f);
				//Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
            } else
            {
				//Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;
				Projectile.damage = (int)(baseDamage);

			}


			Player Player = Main.player[Projectile.owner];
			Vector2 unit = LaserRotation.ToRotationVector2();
			float point = 0f;
			// Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
			// It will look for collisions on the given line using AABB
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Vector2.UnitX.RotatedBy(LaserRotation) * 12,
				Player.Center, 22, ref point);

		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
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
			hit.Damage = (int)hit.Damage / 4;
		}

		public Vector2 GetTipperPosition()
        {
			return Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
		}
	}
}

