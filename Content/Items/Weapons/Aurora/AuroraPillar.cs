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
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.NPCs.Bosses.Rimegeist;

namespace AerovelenceMod.Content.Items.Weapons.Aurora
{
	public class AuroraPillar : ModProjectile
	{

		public Vector2 endPoint;
		public float LaserRotation = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dark Beam");
			ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
		}

		Vector2 storedCenter = Vector2.Zero;
		int timer = 0;

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 600;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
		}

		public override void AI()
		{
			//Main.time = 12600 + 3598; //midnight - 2


			if (timer % 20 == 0 && false)
            {
				for (int i = 50; i < 1500; i += 150) //i = 0 ; i = 350
				{

					if (false)
					{
						int a =Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * i, new Vector2(0,0).RotatedBy(Main.rand.NextFloat(6.28f)), 
							ModContent.ProjectileType<WispSouls>(), 3, 1);

						Main.projectile[a].timeLeft = 300;
						//GlowDustHelper.DrawGlowDustPerfect(storedCenter + Vector2.UnitX.RotatedBy(LaserRotation + Math.PI) * i, ModContent.DustType<GlowCircleDust>()
							//, Main.rand.NextVector2CircularEdge(2f, 2f) + ((LaserRotation + MathHelper.Pi).ToRotationVector2() * 3f), Color.OrangeRed, 0.2f, dustShader); //0.2f

					}

				}
			}


			if (timer == 0)
            {
				LaserRotation = Projectile.velocity.ToRotation();
				storedCenter = Projectile.Center + (LaserRotation.ToRotationVector2() * 2600);
				Projectile.velocity = Vector2.Zero;
			}

			if (timer > 25)
				LaserRotation += MathHelper.ToRadians(0.75f);
			Player player = Main.player[(int)Projectile.ai[0]];
			Projectile.scale = 2f;

			timer++;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Projectile.velocity = Vector2.Zero;
			return false;
        }

		float teleScale = 3f;
		float drawAlpha = 1f;
		public override bool PreDraw(ref Color lightColor) 
		{

			for (float i = 0f; i < 6.28f; i += 6.28f)
			{
				Texture2D RayTex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/Medusa_Gray").Value;
				Main.spriteBatch.Draw(RayTex, Projectile.Center - Main.screenPosition + new Vector2(50 * teleScale, 0).RotatedBy(LaserRotation + 3.14f), RayTex.Frame(1, 1, 0, 0), Color.Black * 1f, LaserRotation + i + 3.14f, RayTex.Size() / 2, teleScale, SpriteEffects.None, 0); ;
			}


			endPoint = storedCenter;
			float height2 = (65f * Projectile.scale); //25

			if (height2 == 0)
				Projectile.active = false;

			int width2 = (int)(Projectile.Center - endPoint).Length() - 24;

			var pos2 = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
			var target2 = new Rectangle((int)pos2.X, (int)pos2.Y, width2, (int)(height2 * 1.2f));

			Texture2D newTex = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Aurora/LineBlack").Value;
			Vector2 origin22 = new Vector2(0, newTex.Height / 2);

			Main.spriteBatch.Draw(newTex, target2, null, Color.Black * 2, LaserRotation, origin22, 0, 0);

			//Texture2D newTex = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/SolsearLaser").Value;


			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/RimeLaser", AssetRequestMode.ImmediateLoad).Value;

			myEffect.Parameters["uColor"].SetValue(Color.Black.ToVector3() * 5);
			//myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
			//myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value);

			myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
			myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
			myEffect.Parameters["uTime"].SetValue(timer * -0.007f); //0.006
			myEffect.Parameters["uSaturation"].SetValue(2);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


			//Activate Shader
			myEffect.CurrentTechnique.Passes[0].Apply();

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;


			Vector2 origin2 = new Vector2(0, texture.Height / 2);

			float height = (65f * Projectile.scale); //25

			if (height == 0)
				Projectile.active = false;

			int width = (int)(Projectile.Center - endPoint).Length() - 24;

			var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
			var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

			Main.spriteBatch.Draw(texture, target, null, Color.Black, LaserRotation, origin2, 0, 0);
			//Main.spriteBatch.Draw(texture, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
			//Main.spriteBatch.Draw(texture, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
			//Main.spriteBatch.Draw(texture, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			/*
			float height2 = (80f); //25

			if (height2 == 0)
				Projectile.active = false;

			int width2 = (int)(Projectile.Center - endPoint).Length() - 24;

			var pos2 = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
			var target2 = new Rectangle((int)pos2.X, (int)pos2.Y, width2, (int)(height2 * 1.2f));

			Texture2D newTex = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Aurora/LineBlack").Value;
			Vector2 origin22 = new Vector2(0, newTex.Height / 2);

			Main.spriteBatch.Draw(newTex, target2, null, Color.White * 2, LaserRotation, origin22, 0, 0);
			//Main.spriteBatch.Draw(texture, target2, null, Color.DarkGray * 10, LaserRotation, origin2, 0, 0);
			*/


			/*
			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
			myEffect.Parameters["uColor"].SetValue(Color.MediumPurple.ToVector3() * 2f); //2.5f makes it more spear like
			myEffect.Parameters["uTime"].SetValue(2);
			myEffect.Parameters["uOpacity"].SetValue(0.6f); //0.6
			myEffect.Parameters["uSaturation"].SetValue(1.2f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


			//Activate Shader
			myEffect.CurrentTechnique.Passes[0].Apply();

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;


			endPoint = storedCenter;
			//var texBeam = Mod.Assets.Request<Texture2D>("Assets/GlowTrailMoreRes").Value;

			Vector2 origin2 = new Vector2(0, texture.Height / 2);

			float height = (25f * Projectile.scale); //15

			if (height == 0)
				Projectile.active = false;

			int width = (int)(Projectile.Center - endPoint).Length() - 24;

			var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
			var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

			Main.spriteBatch.Draw(texture, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
			Main.spriteBatch.Draw(texture, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);


			//for (int i = 0; i < width; i += 6)
				//Lighting.AddLight(pos + Vector2.UnitX.RotatedBy(LaserRotation) * i + Main.screenPosition, Color.DeepPink.ToVector3() * height * 0.020f); //0.030

			//Main.spriteBatch.End();
			//Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			var spotTex = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
			Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation, spotTex.Size() / 2, 1f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation, spotTex.Size() / 2, 1f, SpriteEffects.None, 0);
			*/
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
}

