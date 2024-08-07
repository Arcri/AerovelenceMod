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
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
	public class CyverHyperBeam : ModProjectile
	{

		public Vector2 endPoint;
		public float LaserRotation = 0;
		public override void SetStaticDefaults()
		{
			//ALWAYS DRAW CODE
			ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
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
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 1;
		}


        public override void AI()
		{
            if (timer == 0)
            {
                SoundStyle style32 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_fire") with { Volume = 0.48f, Pitch = 0f, MaxInstances = -1, PitchVariance = 0.1f };
                SoundEngine.PlaySound(style32, Projectile.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .15f, Pitch = 0.2f, MaxInstances = -1 };
                SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle styla = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .86f, PitchVariance = 0.11f, Volume = 0.4f, MaxInstances = -1 };
                SoundEngine.PlaySound(styla, Projectile.Center);

                SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .75f, Pitch = .93f, PitchVariance = .11f, MaxInstances = -1 };
                SoundEngine.PlaySound(styleb, Projectile.Center);

                //SoundStyle style4 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .2f, Pitch = .8f, PitchVariance = 0.1f, MaxInstances = -1 };
                //SoundEngine.PlaySound(style4, Projectile.Center);

                Projectile.ai[0] = 300;

                LaserRotation = Projectile.velocity.ToRotation();// + (float)Math.PI;
                storedCenter = Projectile.Center + (LaserRotation.ToRotationVector2() * 1500);
                Projectile.velocity = Vector2.Zero;

                Projectile.ai[1] = Main.rand.NextFloat(-1, 2);
                Projectile.ai[2] = LaserRotation.ToRotationVector2().X > 0f ? 1f : -1f;

            }

            if (timer < 40)
            {
                //todo optimize
                for (int i = 0; i < 2; i++)
                {
                    float randomDustPercent = Main.rand.NextFloat(0f, 0.85f);
                    Vector2 dustPercentPoint = (endPoint - Projectile.Center) * randomDustPercent;
                    Vector2 dustVel = LaserRotation.ToRotationVector2() * Main.rand.NextFloat(4.5f, 6.1f) * 4f;
                    //new Color(255,155,190)
                    Dust.NewDustPerfect(Projectile.Center + dustPercentPoint + Main.rand.NextVector2Circular(65f, 65f), ModContent.DustType<MuraLineBasic>(), dustVel, 15, Color.HotPink, Main.rand.NextFloat(0.45f, 0.65f) * 0.75f);
                }

            }


            //LaserRotation += 0.005f;
            Projectile.scale = 1f;
            if (timer >= 35) //15
                Projectile.ai[0] = Math.Clamp(MathHelper.Lerp(Projectile.ai[0], -45, 0.06f), 0, 400);

            timer++;
        }


        public override bool PreDraw(ref Color lightColor) 
		{
            
			if (timer > 0) //Laser might be fucked up on first frame if we don't do this
			{
				Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/LaserShader", AssetRequestMode.ImmediateLoad).Value;

				myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * 0.6f);
				myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
				myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value);
				myEffect.Parameters["uTime"].SetValue(timer * -0.015f + Projectile.ai[1]);
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
                Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0); //e196


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

            var spotTex = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_21").Value;
            var spotTex2 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_1").Value;

            Texture2D glowTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/GlowLine1Half");

            Texture2D Ball = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_05");
            Texture2D BallHalf = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_05half");
            Texture2D Ball2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/star_05");

            Vector2 thisPos = endPoint - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * (-14 * (Projectile.ai[0] / 300));



            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value; 

			myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 2.5f);
			myEffect.Parameters["uTime"].SetValue(2);
			myEffect.Parameters["uOpacity"].SetValue(0.9f);
			myEffect.Parameters["uSaturation"].SetValue(1.2f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, Ball.Frame(1, 1, 0, 0), Color.HotPink * 0.5f, 0, Ball.Size() / 2, 0.6f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(Ball, thisPos, Ball.Frame(1, 1, 0, 0), Color.HotPink * 0.5f, 0, Ball.Size() / 2, 0.4f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Ball, thisPos, Ball.Frame(1, 1, 0, 0), Color.HotPink, 0, Ball.Size() / 2, 0.4f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);

            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.HotPink * 1.5f, Projectile.rotation + MathHelper.ToRadians((5f * Projectile.ai[2]) * timer), spotTex.Size() / 2, 1f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation + MathHelper.ToRadians((-3f * Projectile.ai[2]) * timer), spotTex.Size() / 2, 0.75f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);


            Main.spriteBatch.Draw(Ball2, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, Ball2.Frame(1, 1, 0, 0), Color.DeepPink * 1.1f, (timer * 0.06f), Ball2.Size() / 2, 0.68f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Ball2, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 30, Ball2.Frame(1, 1, 0, 0), Color.DeepPink * 1.1f, (timer * -0.06f), Ball2.Size() / 2, 0.68f * (Projectile.ai[0] / 300), SpriteEffects.None, 0);


            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * (24.9f), glowTex.Frame(1, 1, 0, 0), Color.DeepPink, LaserRotation + MathHelper.Pi, glowTex.Size() / 2, 4f * (Projectile.ai[0] / 300), SpriteEffects.FlipHorizontally, 0);
			Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * (24.9f), glowTex.Frame(1, 1, 0, 0), Color.DeepPink, LaserRotation + MathHelper.Pi, glowTex.Size() / 2, 4f * (Projectile.ai[0] / 300), SpriteEffects.FlipHorizontally, 0);


            Main.spriteBatch.Draw(BallHalf, endPoint - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * (126.5f * (Projectile.ai[0] / 300)), BallHalf.Frame(1, 1, 0, 0), Color.DeepPink * 0.6f, LaserRotation, BallHalf.Size() / 2, 1f * (Projectile.ai[0] / 300), SpriteEffects.FlipHorizontally, 0);

            //Fix this later (ping me)
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

    public class PhantomLaserTelegraph : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public float LaserRotation = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dark Beam");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

        Vector2 startingPos = Vector2.Zero;
        public bool tethered = false;
        public bool pulse = false;
        public NPC NPCTetheredTo = null;
 
        int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1000;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.scale = 2;
                startingPos = Projectile.Center;

            }

            if (timer == 50)
                Projectile.velocity = Vector2.Zero;
            
            //if (timer == 100)
                //Release();
            
            LaserRotation = (startingPos - Projectile.Center).ToRotation();
            Projectile.rotation += 0.1f;
            timer++;
        }

        float pulseScale = 0f;
        public void Release()
        {
            Projectile.velocity = Vector2.Zero;

            int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PhantomLaser>(), Projectile.damage, 0);
            if (Main.projectile[a].ModProjectile is PhantomLaser Laser)
            {
                Laser.startingPos = startingPos;
            }

            Projectile.active = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 1.5f);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.8f); //0.8
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

                int width = (int)(Projectile.Center - startingPos).Length();// + 24;

                var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation);
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1f));

                Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);

            }

            return false;

        }

        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override void OnKill(int timeLeft)
        {
            
        }
    }

    public class PhantomLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public float LaserRotation = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dark Beam");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

        public Vector2 startingPos = Vector2.Zero;
        public bool tethered = false;
        public bool pulse = false;
        public NPC NPCTetheredTo = null;

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
                //startingPos = Projectile.Center + new Vector2(500, 0);
                Projectile.velocity = Vector2.Zero;
            }

            if (timer < 30)
                pulseScale = Math.Clamp(MathHelper.Lerp(pulseScale, 1.3f, 0.1f), 0, 1);
            else
                pulseScale = Math.Clamp(MathHelper.Lerp(pulseScale, -0.2f, 0.1f), 0, 1);

            if (pulseScale == 0)
                Projectile.active = false;

            LaserRotation = (startingPos - Projectile.Center).ToRotation();
            Projectile.rotation += 0.1f;
            timer++;
        }

        float pulseScale = 0.2f;

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0)
                return false;
            
            Texture2D Glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Glorb").Value;

            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/flare_1").Value;
            Vector2 starPos = startingPos - Main.screenPosition + (LaserRotation.ToRotationVector2() * 20);
            Main.spriteBatch.Draw(star, starPos, star.Frame(1, 1, 0, 0), new Color(0, 0, 0, 255), Projectile.rotation, star.Size() / 2, pulseScale * 0.75f, SpriteEffects.None, 0f);


            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/LaserShader", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 1f);
            myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/CoolNoise").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5").Value);
            myEffect.Parameters["uTime"].SetValue(timer * 0.01f);
            myEffect.Parameters["uSaturation"].SetValue(2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/GlowTrailMoreRes").Value; ;


            Vector2 origin2 = new Vector2(0, texture.Height / 2);

            float height = (100 * pulseScale); //25

            //if (height == 0)
            //Projectile.active = false;

            int width = (int)(Projectile.Center - startingPos).Length() + 24;

            var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation);// * 24;
            var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.4f));

            Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);

            //Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.8f));

            //Main.spriteBatch.Draw(texture, target2, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(texture, target2, null, Color.DeepPink, LaserRotation, origin2, 0, 0);


            Main.spriteBatch.Draw(star, starPos, star.Frame(1, 1, 0, 0), Color.HotPink, Projectile.rotation, star.Size() / 2, pulseScale * 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(star, starPos, star.Frame(1, 1, 0, 0), Color.HotPink, Projectile.rotation, star.Size() / 2, pulseScale * 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(star, starPos, star.Frame(1, 1, 0, 0), Color.HotPink, Projectile.rotation, star.Size() / 2, pulseScale * 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(star, starPos, star.Frame(1, 1, 0, 0), Color.HotPink, Projectile.rotation, star.Size() / 2, pulseScale * 1.25f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Glow, starPos, Glow.Frame(1, 1, 0, 0), Color.HotPink, Projectile.rotation, Glow.Size() / 2, pulseScale * 2.25f, SpriteEffects.None, 0f);


            return false;

        }

        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {

            Vector2 unit = LaserRotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            if (timer >= 1 && timer < 30)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                    Projectile.Center + (unit * 1500), 22, ref point);
            }
            return false;
        }
    }

    public class FinaleBeam : ModProjectile {

        public override string Texture => "Terraria/Images/Projectile_0";

        public Vector2 endPoint;
        public float LaserRotation = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
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
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            if (timer == 0)
            {

                LaserRotation = Projectile.velocity.ToRotation();
                storedCenter = Projectile.Center + (LaserRotation.ToRotationVector2() * 2500); //1500
                Projectile.velocity = Vector2.Zero;

            }

            widthMultiplier = Math.Clamp(MathHelper.Lerp(widthMultiplier, -0.5f, 0.05f), 0, 1);

            timer++;
        }

        float widthMultiplier = 1f;
        float alpha = 1f;

        public override bool PreDraw(ref Color lightColor)
        {
            			
			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GradientLaser", AssetRequestMode.ImmediateLoad).Value;

			myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/CoolNoise").Value);
			myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5Loop").Value);
            myEffect.Parameters["sampleTexture3"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
            myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/PinkPurpleGrad").Value);

            myEffect.Parameters["uTime"].SetValue(timer * 0.009f); //0.006

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/GlowTrailMoreRes").Value;
            //Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Aurora/AuroraPillar").Value; ;

            //AerovelenceMod\Content\Items\Weapons\Aurora

            Vector2 origin2 = new Vector2(0, texture.Height / 2);

            float height = (300 * widthMultiplier); //25

            //if (height == 0)
            //Projectile.active = false;

            int width = (int)(Projectile.Center - storedCenter).Length();

            var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
            var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1f));
            var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.8f));
            var target3 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.5f));

            for (int i = 0; i < 1; i++)
            {
                Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texture, target2, null, Color.White, LaserRotation, origin2, 0, 0);


            }
            Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);
            //Main.spriteBatch.Draw(texture, target3, null, Color.White, LaserRotation, origin2, 0, 0);
            //Main.spriteBatch.Draw(texture, target3, null, Color.White, LaserRotation, origin2, 0, 0);
            //Main.spriteBatch.Draw(texture, target2, null, Color.White, LaserRotation, origin2, 0, 0);


            //Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);
            //Main.spriteBatch.Draw(texture, target3, null, Color.White, LaserRotation, origin2, 0, 0);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}

