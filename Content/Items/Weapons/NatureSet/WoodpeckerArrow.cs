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
using AerovelenceMod.Content.Dusts.GlowDusts;


namespace AerovelenceMod.Content.Items.Weapons.NatureSet
{
    public class WoodpeckerArrow : ModProjectile
    {
		//If ai[1] is 2 it means it comes from Slate Bow
		float start = 0;

		private int timer;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 1;

		}

		public override void SetDefaults()
		{
			//Projectile.arrow = true;
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.timeLeft = 200;
			Projectile.tileCollide = true;
			Projectile.scale = 1f;
			Projectile.alpha = 255;

		}

        public override void AI()
        {
			Projectile.alpha -= 15;
			//Projectile.velocity.Y += 0.09f;

			Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
			// Adding Pi to rotation if facing left corrects the drawing
			Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

			//Projectile.rotation = Projectile.velocity.ToRotation();
			//Projectile.spriteDirection = Projectile.direction;
		}

        public override void Kill(int timeLeft)
        {
			float storedRandomRot = Main.rand.NextFloat(-0.25f, 0.25f);


			int a = Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(storedRandomRot) * 4, ModContent.ProjectileType<WoodImpact>(), 0, 0);
			Main.projectile[a].rotation = Projectile.velocity.ToRotation() + storedRandomRot;

			int dustAmount = 10; 
			for (int j = 0; j < dustAmount + 1; j++)
            {
				Dust dust2 = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.WoodFurniture, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.3f, Scale: 0.75f);
				//Dust dust3 = Dust.NewDustPerfect(Projectile.Center, DustID.Stone, Projectile.velocity);
				dust2.noGravity = true;
			}

			SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/RockCollideBetter") with { PitchVariance = 0.3f, Volume = 0.1f, Pitch = -0.2f }, Projectile.Center);

			base.Kill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
		{
			return true;
			
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			SpriteEffects effects1 = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			//Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(10, 4), null, Color.DarkKhaki, Projectile.rotation, texture.Size() / 2, 1.1f, effects1, 0);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			for (int i = 0; i < 1; i++)
            {
				Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(10,4), null, Color.DarkKhaki * 0.5f, Projectile.rotation, texture.Size() / 2, 1.25f, effects1, 0);

				for (int j = 0; j < 7; j++)
				{
					Main.spriteBatch.Draw(texture, Projectile.oldPos[j] + new Vector2(texture.Width / 2, texture.Height / 2) - Main.screenPosition, null, Color.SaddleBrown, Projectile.rotation, texture.Size() / 2, Math.Clamp(1f - (j * 0.05f), 0, 100), effects1, 0);
				}
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			return true;
		}
	}

	public class WoodImpact : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Wood Impact");
			Main.projFrames[Projectile.type] = 8;
		}
		public override void SetDefaults()
		{
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.timeLeft = 200;
			Projectile.penetrate = -1;
			Projectile.damage = 0;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			//projectile.netImportant = true;
			Projectile.scale = 1f;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void AI()
		{
			
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 1)
			{
				if (Projectile.frame == 7)
					Projectile.active = false;

				Projectile.frameCounter = 0;
				Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
			}


		}

		public override bool PreDraw(ref Color lightColor)
		{

			Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/NatureSet/straightsticstickpact2").Value;

			int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;

			// Get this frame on texture
			Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);

			Vector2 origin = sourceRectangle.Size() / 2f;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, Color.SaddleBrown, Projectile.rotation, origin, Projectile.scale * 0.25f, SpriteEffects.None, 0f);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
	}
}
