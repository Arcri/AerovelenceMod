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
			//Projectile.scale = (float)Math.Sin(timer / 60) + 1f;
			//Projectile.ai[0] = ((float)Math.Cos(timer * 0.15) + 1f) * 50f;
			//Projectile.scale = MathHelper.Lerp(Projectile.scale, -0.5f, 0.03f);
			if (timer == 0)
            {
				Projectile.scale = 2;
				LaserRotation = Projectile.velocity.ToRotation();
				storedCenter = Projectile.Center + (LaserRotation.ToRotationVector2() * 2600);
				Projectile.velocity = Vector2.Zero;
			}

			//if (timer > 25)
				//LaserRotation += MathHelper.ToRadians(0.75f);
			Player player = Main.player[(int)Projectile.ai[0]];
			//Projectile.scale = 2f;

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
			endPoint = storedCenter;

			/*
			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/RimeLaser", AssetRequestMode.ImmediateLoad).Value;

			myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * 0.5f);
			myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/lightning3").Value);
			myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
			myEffect.Parameters["uTime"].SetValue(timer * -0.007f); //0.006
			*/

			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/LaserShader", AssetRequestMode.ImmediateLoad).Value;
			myEffect.Parameters["uColor"].SetValue(Color.SkyBlue.ToVector3() * 0.3f);
			myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/lightning3").Value);
			myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196").Value);
			myEffect.Parameters["uTime"].SetValue(timer * -0.01f);
			myEffect.Parameters["uSaturation"].SetValue(2);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


			//Activate Shader
			myEffect.CurrentTechnique.Passes[0].Apply();

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;


			Vector2 origin2 = new Vector2(0, texture.Height / 2);

			float height = (50 * Projectile.scale); //25

			if (height == 0)
				Projectile.active = false;

			int width = (int)(Projectile.Center - endPoint).Length() - 24;

			var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
			var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

			Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);
			Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);

			//Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.8f));

			Main.spriteBatch.Draw(texture, target2, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
			Main.spriteBatch.Draw(texture, target2, null, Color.DeepPink, LaserRotation, origin2, 0, 0);


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

