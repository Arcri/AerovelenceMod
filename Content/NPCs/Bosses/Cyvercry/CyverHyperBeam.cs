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
using rail;
using Terraria.Audio;
using System.Net;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
	public class CyverHyperBeam : ModProjectile
	{

		public Vector2 endPoint;
		public float LaserRotation = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hyper Beam Shot");

			//ALWAYS DRAW CODE
			ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
			//ProjectileID.Sets.TrailCacheLength[Projectile.type] = 80;
			//ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		Vector2 storedCenter = Vector2.Zero;
		int timer = 0;

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 1000;
			Projectile.tileCollide = true;
			//Projectile.extraUpdates = 100; //200
			Projectile.extraUpdates = 1;
			Projectile.hide = true;
		}


        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			behindNPCs.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override void AI()
		{
			if (timer == 0)
            {
                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .12f, Pitch = 0.2f, MaxInstances = 1 };
                SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle styla = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .86f, PitchVariance = 0.11f, Volume = 0.4f, MaxInstances = -1 };
                SoundEngine.PlaySound(styla, Projectile.Center);

                SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .75f, Pitch = .93f, PitchVariance = .11f, MaxInstances = -1 };
                SoundEngine.PlaySound(styleb, Projectile.Center);
                Projectile.ai[0] = 300;

				LaserRotation = Projectile.velocity.ToRotation();// + (float)Math.PI;
				storedCenter = Projectile.Center + (LaserRotation.ToRotationVector2() * 1500);
				Projectile.velocity = Vector2.Zero;

				Projectile.ai[1] = Main.rand.NextFloat(-1, 2);

			}
			//LaserRotation += 0.005f;
			Projectile.scale = 1f;
			if (timer >= 35) //15
				Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], -45, 0.06f);
			timer++;
		}


        public override bool PreDraw(ref Color lightColor) 
		{
            
			#region RimeBeam
			/*
			endPoint = storedCenter;
			float height2 = 65; 

            if (height2 == 0)
				Projectile.active = false;

			int width2 = (int)(Projectile.Center - endPoint).Length() - 24;

			var pos2 = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
			var target2 = new Rectangle((int)pos2.X, (int)pos2.Y, width2, (int)(height2 * 1.2f));

			Texture2D newTex = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Aurora/LineBlack").Value;
			Vector2 origin22 = new Vector2(0, newTex.Height / 2);

			//Main.spriteBatch.Draw(newTex, target2, null, Color.Black * 2, LaserRotation, origin22, 0, 0);

			//Texture2D newTex = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/SolsearLaser").Value;


			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/RimeLaser", AssetRequestMode.ImmediateLoad).Value;

			myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * 2f);
			//myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
			//myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value);

			myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value);
			myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
			myEffect.Parameters["uTime"].SetValue(timer * -0.01f); //0.006
			myEffect.Parameters["uSaturation"].SetValue(2);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


			//Activate Shader
			myEffect.CurrentTechnique.Passes[0].Apply();

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;


			Vector2 origin2 = new Vector2(0, texture.Height / 2);

			float height = Math.Clamp(Projectile.ai[0], 0, 500); //25

			if (height == 0)
				Projectile.active = false;

			int width = (int)(Projectile.Center - endPoint).Length() - 24;

			var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
			var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

			Main.spriteBatch.Draw(texture, target, null, Color.Black, LaserRotation, origin2, 0, 0);
			Main.spriteBatch.Draw(texture, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
			//Main.spriteBatch.Draw(texture, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
			//Main.spriteBatch.Draw(texture, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			*/
			#endregion

            
			if (timer > 0) //Laser might be fucked up on first frame if we don't do this
			{


				Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/LaserShader", AssetRequestMode.ImmediateLoad).Value;

				//Extra_196
				//spark_07

				myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * 0.6f);
				myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value);
				myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value);
				myEffect.Parameters["uTime"].SetValue(timer * -0.01f + Projectile.ai[1]);
				myEffect.Parameters["uSaturation"].SetValue(2);


				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


				//Activate Shader
				myEffect.CurrentTechnique.Passes[0].Apply();

				Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;


				endPoint = storedCenter;
				var texBeam = Mod.Assets.Request<Texture2D>("Assets/GlowTrailMoreRes").Value;


				Vector2 origin2 = new Vector2(0, texBeam.Height / 2);

				float height = Math.Clamp(Projectile.ai[0], 0, 500); //25

				if (height <= 20)
					Projectile.active = false;

				int width = (int)(Projectile.Center - endPoint).Length() - 24;

				var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
				var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

				Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);


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

				//Main.spriteBatch.Draw(texBeam, target2, null, Color.OrangeRed, LaserRotation, origin2, 0, 0);

				//for (int i = 0; i < width; i += 6)
				//Lighting.AddLight(pos + Vector2.UnitX.RotatedBy(LaserRotation) * i + Main.screenPosition, Color.HotPink.ToVector3() * height * 0.015f); //0.030

				//timer++;
			}
			
            return false;
		}

		public override void PostDraw(Color lightColor)
        {
			var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			var spotTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
			Texture2D glowTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/GlowLine1").Value;
			Texture2D Ball = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_05");


			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
			//myEffect.Parameters["uColor"].SetValue(Color.WhiteSmoke.ToVector3() * 1f); 

			myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 2.5f);
			myEffect.Parameters["uTime"].SetValue(2);
			myEffect.Parameters["uOpacity"].SetValue(0.9f);
			myEffect.Parameters["uSaturation"].SetValue(1.2f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

			Vector2 thisPos = endPoint - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * (-12 * (Projectile.ai[0] / 300));

			Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, Ball.Frame(1, 1, 0, 0), Color.HotPink * 0.5f, 0, Ball.Size() / 2, 0.39f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(Ball, thisPos, Ball.Frame(1, 1, 0, 0), Color.HotPink * 0.5f, 0, Ball.Size() / 2, 0.5f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
			//Main.spriteBatch.Draw(Ball, thisPos, Ball.Frame(1, 1, 0, 0), Color.HotPink, 0, Ball.Size() / 2, 0.5f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);

			//Activate Shader
			myEffect.CurrentTechnique.Passes[0].Apply();

			//Main.spriteBatch.Draw(spotTex, storedCenter - Main.screenPosition, spotTex.Frame(1, 1, 0, 0), Color.Orange, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.2f), spotTex.Size() / 2, 0.15f, SpriteEffects.None, 0);
			//Main.spriteBatch.Draw(spotTex, storedCenter - Main.screenPosition, spotTex.Frame(1, 1, 0, 0), Color.Orange, Projectile.rotation + MathHelper.ToRadians(timer * 0.1f), spotTex.Size() / 2, 0.1f, SpriteEffects.None, 0);

			Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Orange, Projectile.rotation + MathHelper.ToRadians(-1 * timer), spotTex.Size() / 2, 0.45f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Orange, Projectile.rotation + MathHelper.ToRadians(timer), spotTex.Size() / 2, 0.30f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, glowTex.Frame(1, 1, 0, 0), Color.HotPink, LaserRotation, glowTex.Size() / 2, 3f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, glowTex.Frame(1, 1, 0, 0), Color.DeepPink, LaserRotation, glowTex.Size() / 2, 4.5f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, glowTex.Frame(1, 1, 0, 0), Color.DeepPink, LaserRotation, glowTex.Size() / 2, 4.5f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, glowTex.Frame(1, 1, 0, 0), Color.DeepPink, LaserRotation, glowTex.Size() / 2, 4.5f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, glowTex.Frame(1, 1, 0, 0), Color.DeepPink, LaserRotation, glowTex.Size() / 2, 4.5f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, glowTex.Frame(1, 1, 0, 0), Color.DeepPink, LaserRotation, glowTex.Size() / 2, 4.5f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, glowTex.Frame(1, 1, 0, 0), Color.DeepPink, LaserRotation, glowTex.Size() / 2, 4.5f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);

			//Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, glowTex.Frame(1, 1, 0, 0), Color.HotPink, 0, glowTex.Size() / 2, 0.7f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
			//Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, glowTex.Frame(1, 1, 0, 0), Color.HotPink, 0, glowTex.Size() / 2, 0.7f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
            Vector2 unit = LaserRotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
			if (timer < 20 && timer >= 1)
			{
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * 12, 
					endPoint, 22, ref point);
            }

			return false;
		}
	}

    public class PhantomLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public float LaserRotation = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Beam");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

		Vector2 startingPos = Vector2.Zero;
		public bool tethered = false;
		public bool pulse = false;
        public NPC NPCTetheredTo = null;


        Vector2 storedCenter = Vector2.Zero;
        int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.scale = 2;
				startingPos = Projectile.Center;


                //LaserRotation = Projectile.velocity.ToRotation();
                //storedCenter = Projectile.Center + (LaserRotation.ToRotationVector2() * 2600);
                //Projectile.velocity = Vector2.Zero;
            }

			if (timer == 100)
				Release();

			if (pulse)
			{

				if (timer < 170)
					pulseScale = Math.Clamp(MathHelper.Lerp(pulseScale, 1.3f, 0.2f), 0, 1);
				else
                    pulseScale = Math.Clamp(MathHelper.Lerp(pulseScale, -0.2f, 0.1f), 0, 1);

            }

            LaserRotation = (startingPos - Projectile.Center).ToRotation();
			Projectile.rotation += 0.1f;
            timer++;
        }

		float pulseScale = 0f;
		public void Release()
		{
			Projectile.velocity = Vector2.Zero;
			pulse = true;
		}

		//Shitty af but w/e
        public override bool PreDraw(ref Color lightColor)
        {
			if (pulse)
			{
                Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/LaserShader", AssetRequestMode.ImmediateLoad).Value;
                myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 1f);
                myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value);
                myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value);
                myEffect.Parameters["uTime"].SetValue(timer * -0.01f);
                myEffect.Parameters["uSaturation"].SetValue(2);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


                //Activate Shader
                myEffect.CurrentTechnique.Passes[0].Apply();

                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;


                Vector2 origin2 = new Vector2(0, texture.Height / 2);

                float height = (50 * pulseScale); //25

                //if (height == 0)
                    //Projectile.active = false;

                int width = -10;

                var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.4f));

                Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);

                //Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
                var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.8f));

                //Main.spriteBatch.Draw(texture, target2, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texture, target2, null, Color.DeepPink, LaserRotation, origin2, 0, 0);


                Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/flare_1").Value;
                Vector2 starPos = startingPos - Main.screenPosition + (LaserRotation.ToRotationVector2() * 20);
                Main.spriteBatch.Draw(star, starPos, star.Frame(1, 1, 0, 0), Color.HotPink, Projectile.rotation, star.Size() / 2, pulseScale * 0.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(star, starPos, star.Frame(1, 1, 0, 0), Color.HotPink, Projectile.rotation, star.Size() / 2, pulseScale * 0.5f, SpriteEffects.None, 0f);
                //Main.spriteBatch.Draw(star, starPos, star.Frame(1, 1, 0, 0), Color.HotPink, Projectile.rotation, star.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
                //Main.spriteBatch.Draw(star, starPos, star.Frame(1, 1, 0, 0), Color.HotPink, Projectile.rotation, star.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0f);

            }
			else
			{
                Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
                myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 1.5f);
                myEffect.Parameters["uTime"].SetValue(2);
                myEffect.Parameters["uOpacity"].SetValue(0.7f); //0.8
                myEffect.Parameters["uSaturation"].SetValue(1.2f);


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

                if (timer > 0)
                {
                    var texBeam = Mod.Assets.Request<Texture2D>("Assets/ThinLineGlowClear").Value;

                    Vector2 origin2 = new Vector2(0, texBeam.Height / 2);

                    float height = 50f * Projectile.scale; //15

                    if (height == 0)
                        Projectile.active = false;

                    int width = (int)(Projectile.Center - startingPos).Length() - 24;

                    var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
                    var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

                    Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);

                }
                
            }

            return false;

        }

        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override void Kill(int timeLeft)
        {
            //Main.NewText("amg");
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {

            Vector2 unit = LaserRotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            if (timer >= 1)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                    Projectile.Center + (unit * 1500), 22, ref point);
            }
            return false;
        }
    }


	//PhantomLaserLineTelegraph


	//ActualPhantomLaser
}

