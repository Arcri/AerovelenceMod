using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.Audio;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using static Terraria.NPC;
using static tModPorter.ProgressUpdate;
using static AerovelenceMod.Common.Utilities.DustBehaviorUtil;

namespace AerovelenceMod.Content.Items.Weapons.Ember
{
	public class MagmaBall : ModProjectile
	{
		private int timer = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Burning Blaze Ball");
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1;
			Projectile.timeLeft = 3000; //300
			Projectile.tileCollide = true; //false;
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.alpha = 0;
			Projectile.hide = false;

		}

		public float velocityValue = 30f;
		float currentVelocity = 20f;
		Vector2 velDirection = Vector2.Zero;

        public override void AI()
        {

			#region old
			/*
            if (globalScale >= 0.5f)
            {
				foreach (Projectile projectileWho in Main.projectile)
				{
					//This should be first because it weeds trash out the most 
					if (projectileWho.type == ModContent.ProjectileType<SolsearLaser>() && projectileWho.active == true)
                    {
						if (projectileWho.ModProjectile is SolsearLaser laser)
                        {

							if (Projectile.Center.Distance(laser.GetTipperPosition()) < 50 * globalScale)
							{
								Dust a = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Scale: 0.4f, newColor: Color.Orange);
								a.noGravity = true;
								
								globalMax = Math.Clamp(globalMax + 0.006f, 0f, 1f); //0.004
								globalGoal = Math.Clamp(globalGoal + 0.006f, 0f, 1.25f);

								if (globalMax == 1f)
                                {
									Player myPlayer = Main.player[Projectile.owner];

									myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 30;
									Projectile.Kill();
								}
							}
						}
                    }

				}
			}

			globalScale = Math.Clamp(MathHelper.Lerp(globalScale, globalGoal, 0.02f), 0, globalMax);
			Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * globalScale); //0.030
			Projectile.velocity.Y += 0.02f; //0.01
			*/
			#endregion

			if (timer == 0)
			{
                velDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX);
				currentVelocity = velocityValue;
            }

            Projectile.velocity = velDirection * currentVelocity;

			float lerpValue = Math.Clamp(timer / 60f, 0f, 1f);
            currentVelocity = MathHelper.Lerp(velocityValue, 0f, Easings.easeOutExpo(lerpValue));

            Projectile.velocity *= 0.8f;

            globalScale = Math.Clamp(MathHelper.Lerp(globalScale, globalGoal, 1f), 0, globalMax);
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * globalScale); //0.030

            timer++;
			Projectile.width = (int)MathHelper.Clamp((int)(100 * globalScale), 20, 150);
			Projectile.height = (int)MathHelper.Clamp((int)(100 * globalScale), 20, 150);
		}

		float globalScale = 0.5f;
		float globalMax = 0.5f;
		float globalGoal = 0.75f;

		public override bool PreDraw(ref Color lightColor)
		{
            Texture2D ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/bigCircle2").Value;
            Texture2D ball2 = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;

            Texture2D border = Mod.Assets.Request<Texture2D>("Assets/Orbs/zFadeCircle").Value; //zFadeCircle

            Texture2D starA = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_4").Value;
            Texture2D starB = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/star_07").Value;

			globalScale = 2f;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/FireGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);

            myEffect.Parameters["flowSpeed"].SetValue(-0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(0.1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.32f);
            myEffect.Parameters["distortStrength"].SetValue(0.1f);
            myEffect.Parameters["squashValue"].SetValue(0.0f);
            myEffect.Parameters["colorIntensity"].SetValue(1.5f);
            myEffect.Parameters["xOffset"].SetValue(0f);


            myEffect.Parameters["uTime"].SetValue(timer * 0.007f);

			//1f black looks really good but wrong palette for weapon
            Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, Color.Black * 0.75f, Projectile.rotation, ball.Size() / 2, 0.5f * globalScale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, Color.Orange, Projectile.rotation, ball.Size() / 2, 0.5f * globalScale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(starB, Projectile.Center - Main.screenPosition, null, Color.White * 1f, (float)Main.timeForVisualEffects * -0.04f, starB.Size() / 2, 0.6f * globalScale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(ball2, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 0.6f, Projectile.rotation, ball2.Size() / 2, 0.6f * globalScale, SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(starA, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 1f, (float)Main.timeForVisualEffects * 0.05f, starA.Size() / 2, 0.7f * globalScale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(starA, Projectile.Center - Main.screenPosition, null, Color.Orange * 1f, (float)Main.timeForVisualEffects * -0.07f, starA.Size() / 2, 0.77f * globalScale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(border, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), null, new Color(255, 70, 20) * 2f, (float)Main.timeForVisualEffects * -0.11f, border.Size() / 2, 0.3f * globalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(border, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), null, new Color(255, 110, 50) * 2f, (float)Main.timeForVisualEffects * 0.08f + 2f, border.Size() / 2, 0.3f * globalScale, SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(starB, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, (float)Main.timeForVisualEffects * -0.04f, starB.Size() / 2, 0.4f * globalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(starB, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), null, new Color(255, 120, 30) * 1f, (float)Main.timeForVisualEffects * -0.04f, starB.Size() / 2, 0.38f * globalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(starB, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), null, new Color(255, 120, 30) * 1f, (float)Main.timeForVisualEffects * -0.04f + MathHelper.PiOver4, starB.Size() / 2, 0.5f * globalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(starB, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), null, new Color(255, 120, 30) * 1f, (float)Main.timeForVisualEffects * -0.04f, starB.Size() / 2, 0.38f * globalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(starB, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), null, new Color(255, 120, 30) * 1f, (float)Main.timeForVisualEffects * -0.04f + MathHelper.PiOver4, starB.Size() / 2, 0.5f * globalScale, SpriteEffects.None, 0f);


            //Main.spriteBatch.Draw(starB, Projectile.Center - Main.screenPosition, null, Color.White * 2f, (float)Main.timeForVisualEffects * -0.04f, starB.Size() / 2, 0.66f * globalScale, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
			Player Player = Main.player[Projectile.owner];
			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/MagmaBall").Value;
			//Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/circle_03").Value;

			Texture2D texture2 = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/circle_05").Value;
			var star = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;


			int height1 = texture.Height;
			Vector2 origin1 = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);

			int height2 = texture.Height;
			Vector2 origin2 = new Vector2((float)texture2.Width / 2f, (float)height2 / 2f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.OrangeRed * 0.7f, timer * 0.02f, star.Size() / 2, 0.4f * globalScale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.Orange * 0.7f, timer * 0.02f, star.Size() / 2, 0.4f * globalScale, SpriteEffects.None, 0f);

			Main.spriteBatch.Draw(texture2, Projectile.Center + new Vector2(0, 150 * globalScale) - Main.screenPosition, null, Color.OrangeRed, 0, origin2, 0.3f * globalScale, SpriteEffects.None, 0.0f);
			Main.spriteBatch.Draw(texture2, Projectile.Center + new Vector2(0, 100 * globalScale) - Main.screenPosition, null, Color.Red * 2, 0, origin2, 0.2f * globalScale, SpriteEffects.None, 0.0f);
			Main.spriteBatch.Draw(texture2, Projectile.Center + new Vector2(0, 50 * globalScale) - Main.screenPosition, null, Color.White * 2, 0, origin2, 0.1f * globalScale, SpriteEffects.None, 0.0f);

			//gradientTex
			//time
			//distort
			//caustics tex

			//for purple pulse -> Uses timer * 0.08 and the Alpenine for the gradient input

			Effect myEffect2 = ModContent.Request<Effect>("AerovelenceMod/Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value;
			myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/FireBallGradientTrim").Value);
			myEffect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);
			myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/FireBallGradientTrim").Value);
			myEffect.Parameters["uTime"].SetValue(timer * 0.06f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
			myEffect.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, 0, origin1, 0.1f * globalScale, SpriteEffects.None, 0.0f);
			//Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, 0, origin1, Projectile.scale, SpriteEffects.None, 0.0f);
			return false;
			
		}

        public override void PostDraw(Color lightColor)
        {
			//Main.pixelShader.CurrentTechnique.Passes[0].Apply();
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
		}

        public override void Kill(int timeLeft)
        {

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(Projectile.Center, Main.npc[i].Center) < 170 * globalScale)
				{
					int Direction = 0;
					if (Projectile.position.X - Main.npc[i].position.X < 0)
						Direction = 1;
					else
						Direction = -1;

					HitInfo myHit = new HitInfo();
					myHit.Damage = Projectile.damage * 3;
					myHit.Knockback = Projectile.knockBack;
					myHit.HitDirection = Direction;

					Main.npc[i].StrikeNPC(myHit);

					ArmorShaderData thisDustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
					for (int j = 0; j < 4; j++)
					{
						Dust d = GlowDustHelper.DrawGlowDustPerfect(Main.npc[i].Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 4),
							Color.OrangeRed, 0.65f, 0.4f, 0f, thisDustShader);
						d.velocity *= 0.5f;

					}

				}
			}

			ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
			ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

			SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.53f, };
			SoundEngine.PlaySound(style2, Projectile.Center);

			SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_45") with { Pitch = .75f, PitchVariance = 0.2f };
			SoundEngine.PlaySound(stylea, Projectile.Center);

			SoundStyle styleb = new SoundStyle("Terraria/Sounds/Item_105") with { Pitch = .55f, Volume = 1f };
			SoundEngine.PlaySound(styleb, Projectile.Center);


			//remember that global scale ranges from 0 -> 0.5 -> 1.25
			for (int i = 0; i < 24 * globalScale; i++) //2
			{
				float rotValue = i * 30;
				Vector2 bonus = new Vector2(5, 0).RotatedBy(rotValue);
				Dust nd = Dust.NewDustPerfect(Projectile.Center + bonus * (10 * globalScale), ModContent.DustType<GlowStrong>(),
					(bonus * 2 * globalScale) * -1, newColor: Color.OrangeRed * 2f, Scale: 3f * globalScale);
				//Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + bonus * (10 * globalScale), ModContent.DustType<GlowCircleDust>(),
					//(bonus * 2 * globalScale) * -1, Color.OrangeRed, 0.5f, 0.45f, 0f, dustShader);
				//p.scale = 0.75f;
				//p.alpha = 0;
				//p.rotation = Main.rand.NextFloat(6.28f);
			}

			for (int i = 0; i < 4 ; i++)
			{
				for (int j = 1; j < 4; j++)
				{
					float rotValue = i * 90;
					Vector2 bonus = new Vector2(7.5f, 0).RotatedBy(MathHelper.ToRadians(rotValue)).RotatedBy(timer * 0.02f);
                    Dust p = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStarSharp>(), 
						bonus * (2f * globalScale) * -1, newColor: Color.OrangeRed * 2f, Scale: 1f * globalScale);
                    p.velocity *= j * 0.5f;

                    StarDustDrawInfo info = new StarDustDrawInfo(false, true, false, true, false, 1f);
                    p.customData = AssignBehavior_GSSBase(rotPower: 0.04f, timeBeforeSlow: 5, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.8f, shouldFadeColor: false, sdci: info);

                    //Dust nd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStarSharp>(),
                    //bonus * (2 * globalScale) * -1, newColor: Color.OrangeRed, Scale: 1f * globalScale);
                    //nd.velocity *= j * 0.35f;

                    //Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleQuadStar>(),
                    //bonus * (2 * globalScale), Color.OrangeRed, 2 * globalScale, 0.45f, 0f, dustShader2);
                    //p.velocity *= j * 0.5f;
                }

			}

			/*
			for (int i = 0; i < 4; i++) //2
			{
				Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(0, 0), ModContent.DustType<GlowCircleDust>(),
					Main.rand.NextVector2Circular(7, 7), new Color(255, 75, 50), Main.rand.NextFloat(0.4f, 0.65f), 0.4f, 0f, dustShader3);
				p.alpha = 0;
				//p.rotation = Main.rand.NextFloat(6.28f);
			}
			*/

			/*
			for (int i = 0; i < 8; i++) //2
			{
				Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(0, 0), ModContent.DustType<GlowCircleQuadStar>(),
					Main.rand.NextVector2CircularEdge(6, 6), Color.OrangeRed, Main.rand.NextFloat(0.7f, 0.9f), 0.4f, 0f, dustShader);
				p.alpha = 0;
				//p.rotation = Main.rand.NextFloat(6.28f);
			}
			*/

		}

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			float damage = Projectile.damage * globalScale;
            //base.ModifyHitNPC(ref damage);
        }
    }
}


