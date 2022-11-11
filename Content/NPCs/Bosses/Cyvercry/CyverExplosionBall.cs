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
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
	public class CyverExplosionBall : ModProjectile
	{
		private int timer;

		public Vector2 outVector = new Vector2(0,400);

		public float intensity = 1f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cross Bomb");
			Main.projFrames[Projectile.type] = 7;
		}

		public override void SetDefaults()
		{
			Projectile.width = 48;
			Projectile.height = 42;
			Projectile.timeLeft = 320;
			Projectile.penetrate = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.damage = 54;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void AI()
        {


			if (timer <= 200)
            {
				Player player = Main.player[(int)Projectile.ai[0]];
				Projectile.Center = player.Center + outVector.RotatedBy(MathHelper.ToRadians(timer));

				if (timer == 200)
				{
					Projectile.localAI[0] = player.Center.X;
					Projectile.localAI[1] = player.Center.Y;
				}
			}



			if (timer > 200)
            {
				Projectile.Center = new Vector2(Projectile.localAI[0], Projectile.localAI[1]) + outVector.RotatedBy(MathHelper.ToRadians(200)) * intensity;


				intensity -= 0.012f;
            }

			if (intensity < 0)
            {
				Projectile.active = false;
				int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, outVector.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(timer)) * 9, ModContent.ProjectileType<Cyver2EnergyBall>(), 4, 2, Main.myPlayer);
				Projectile cahser = Main.projectile[a];

				if (cahser.ModProjectile is Cyver2EnergyBall ball)
                {
					ball.timer = 30;
                }

			}

			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
			}
			timer++;
		}

        public override bool PreDraw(ref Color lightColor)
        {
			/*
			var lTex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/PinkL").Value;
			Rectangle frame = lTex.Frame(1, 1, 0, 0);
			Vector2 origin2 = frame.Size() / 2f;

			const float TwoPi = (float)Math.PI * 2f;
			float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 1.75f);

			Vector2 topLeft = destination + new Vector2(-20, -20) + new Vector2(-2, -2) * offset;
			Vector2 topRight = destination + new Vector2(20, -20) + new Vector2(2, -2) * offset;
			Vector2 BottomLeft = destination + new Vector2(-20, 20) + new Vector2(-2, 2) * offset;
			Vector2 BottomRight = destination + new Vector2(20, 20) + new Vector2(2, 2) * offset;
			Vector2 storedPos = Vector2.Zero; //Yes I need to do all this

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


			//Retical
			Main.spriteBatch.Draw(lTex, topLeft - Main.screenPosition + new Vector2(0.5f, -0.5f), new Rectangle?(frame), Color.White, MathHelper.ToRadians(90 + rotation), origin2, 0.7f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(lTex, topRight - Main.screenPosition, new Rectangle?(frame), Color.White, MathHelper.ToRadians(180 + rotation), origin2, 0.7f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(lTex, BottomLeft - Main.screenPosition, new Rectangle?(frame), Color.White, MathHelper.ToRadians(0 + rotation), origin2, 0.7f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(lTex, BottomRight - Main.screenPosition + new Vector2(-0.5f, .5f), new Rectangle?(frame), Color.White, MathHelper.ToRadians(270 + rotation), origin2, 0.7f, SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
			*/

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			var Tex = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.HotPink, Projectile.rotation, Tex.Size() / 2, 1.5f, SpriteEffects.None, 0f);
			
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			return true;
        }
    }
}


