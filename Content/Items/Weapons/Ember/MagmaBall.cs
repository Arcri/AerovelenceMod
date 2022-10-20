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

namespace AerovelenceMod.Content.Items.Weapons.Ember
{
	public class MagmaBall : ModProjectile
	{
		private int timer = 0;

		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burning Blaze Ball");
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true; //false;
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.alpha = 0;
			Projectile.hide = false;

		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			//behindNPCs.Add(index);

			base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
		}

        public override void AI()
        {
			if (globalScale >= 0.5f)
            {
				foreach (Projectile projectileWho in Main.projectile)
				{
					//This should be first because it weeds trash out the most 
					if (projectileWho.type == ModContent.ProjectileType<SolsearLaser>() && projectileWho.active == true)
                    {
						if (projectileWho.ModProjectile is SolsearLaser laser)
                        {
							//Dust.NewDustPerfect(laser.GetTipperPosition(), DustID.AmberBolt);
							//Dust.NewDustPerfect(projectileWho.Center, DustID.Water);


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
			//globalScale = Math.Clamp(MathHelper.Lerp(globalScale, 1.25f * 2, 0.02f), 0, 2f);

			Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * globalScale); //0.030

			Projectile.velocity.Y += 0.02f; //0.01
			timer++;
			Projectile.width = (int)MathHelper.Clamp((int)(100 * globalScale), 20, 150);
			Projectile.height = (int)MathHelper.Clamp((int)(100 * globalScale), 20, 150);
		}

		float globalScale = 0f;
		float globalMax = 0.5f;
		float globalGoal = 0.75f;

		public override bool PreDraw(ref Color lightColor)
		{
			
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

			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value;
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
					Main.npc[i].StrikeNPC(Projectile.damage * 3, Projectile.knockBack, Direction);

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
			ArmorShaderData dustShader3 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

			SoundStyle style1 = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_1") with { Pitch = -.53f, };
			SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.53f, };
			//SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_3") with { Pitch = -.53f, };

			SoundEngine.PlaySound(style2, Projectile.Center);
			//SoundEngine.PlaySound(style2, Projectile.Center);
			//SoundEngine.PlaySound(style3, Projectile.Center);

			SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_45") with { Pitch = .75f, PitchVariance = 0.2f };
			SoundEngine.PlaySound(stylea, Projectile.Center);

			SoundStyle styleb = new SoundStyle("Terraria/Sounds/Item_105") with { Pitch = .55f, Volume = 1f };
			SoundEngine.PlaySound(styleb, Projectile.Center);


			//remember that global scale ranges from 0 -> 0.5 -> 1.25
			for (int i = 0; i < 24 * globalScale; i++) //2
			{
				float rotValue = i * 30;
				Vector2 bonus = new Vector2(5, 0).RotatedBy(rotValue);
				Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + bonus * (10 * globalScale), ModContent.DustType<GlowCircleDust>(),
					(bonus * 2 * globalScale) * -1, Color.OrangeRed, 0.5f, 0.45f, 0f, dustShader);
				p.scale = 0.75f;
				p.alpha = 0;
				//p.rotation = Main.rand.NextFloat(6.28f);
			}

			for (int i = 0; i < 4 ; i++)
			{
				for (int j = 1; j < 3; j++)
				{
					float rotValue = i * 90;
					Vector2 bonus = new Vector2(7.5f, 0).RotatedBy(MathHelper.ToRadians(rotValue)).RotatedBy(timer * 0.02f);
					Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleQuadStar>(),
						bonus * (2 * globalScale), Color.OrangeRed, 2 * globalScale, 0.45f, 0f, dustShader2);
					p.velocity *= j * 0.5f;
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

        public override void ModifyDamageScaling(ref float damageScale)
        {
			float damage = Projectile.damage * globalScale;
            base.ModifyDamageScaling(ref damage);
        }
    }
}


