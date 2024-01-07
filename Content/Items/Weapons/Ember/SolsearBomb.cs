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
using AerovelenceMod.Content.Projectiles.Other;

namespace AerovelenceMod.Content.Items.Weapons.Ember
{
	public class SolsearBomb : ModProjectile
	{
        public override string Texture => "Terraria/Images/Projectile_0";

        private int timer = 0;
		
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1;
			Projectile.timeLeft = 300; //300
			Projectile.tileCollide = true; //false;
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.alpha = 0;
			Projectile.hide = true;

		}

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public float velocityValue = 30f;
		float currentVelocity = 20f;
		Vector2 velDirection = Vector2.Zero;

        Vector2 stoppedPos = Vector2.Zero;
        bool stopped = false;

        float scaleProgVal = 0f;
        float storedScale = 1f;

        public override void AI()
        {
			if (timer == 0)
			{
                velDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX);
				currentVelocity = velocityValue;
            }

            Projectile.velocity = velDirection * currentVelocity;

			float lerpValue = Math.Clamp(timer / 60f, 0f, 1f);
            currentVelocity = MathHelper.Lerp(velocityValue, 0f, Easings.easeOutExpo(lerpValue));

            Projectile.velocity *= 0.8f;
            if (Projectile.velocity.Length() < 0.5f && !stopped)
            {
                stoppedPos = Projectile.Center;
                stopped = true;
            }

            if (globalScale >= 0.5f && Projectile.timeLeft > 20)
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
                                Dust a = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Scale: 1f, newColor: Color.Orange);
                                a.noGravity = true;
                                a.velocity *= 4f;

                                scaleProgVal = Math.Clamp(scaleProgVal + 0.025f, 0f, 1f);
                                globalScale = MathHelper.Lerp(1f, 2f, Easings.easeInQuad(scaleProgVal));


                                if (globalScale == 2f)
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

            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * globalScale); //0.030

            timer++;

            if (stopped)
            {
                Projectile.width = (int)MathHelper.Clamp((int)(100 * globalScale), 20, 150);
                Projectile.height = (int)MathHelper.Clamp((int)(100 * globalScale), 20, 150);
                Projectile.Center = stoppedPos;
            }

            if (Projectile.timeLeft <= 20)
            {
                if (Projectile.timeLeft == 20)
                    storedScale = globalScale;
                float outProg = 1f - (Projectile.timeLeft / 20f);
                globalScale = MathHelper.Lerp(storedScale, 0f, Easings.easeInBack(outProg));//Math.Clamp(MathHelper.Lerp(globalScale, -0.5f, 0.2f), 0f, 10f);
            }

            drawnScale = Math.Clamp(MathHelper.Lerp(drawnScale, 1.25f, 0.1f), 0f, 1f);
        }

		float globalScale = 1f;
        float drawnScale = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
            Texture2D ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/bigCircle2").Value;
            Texture2D ball2 = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;

            Texture2D border = Mod.Assets.Request<Texture2D>("Assets/Orbs/zFadeCircle").Value; //zFadeCircle
            Texture2D starB = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/star_07").Value;

            float drawScale = globalScale * 0.55f * drawnScale;

			//globalScale = 2f;

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

            Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, Color.Black * 0.95f, Projectile.rotation, ball.Size() / 2, 0.5f * drawScale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, Color.Orange, Projectile.rotation, ball.Size() / 2, 0.5f * drawScale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(ball2, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 0.6f, Projectile.rotation, ball2.Size() / 2, 0.6f * drawScale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(border, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), null, new Color(255, 70, 20) * 2f, (float)Main.timeForVisualEffects * -0.11f, border.Size() / 2, 0.3f * drawScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(border, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), null, new Color(255, 110, 50) * 2f, (float)Main.timeForVisualEffects * 0.08f + 2f, border.Size() / 2, 0.3f * drawScale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(starB, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(globalScale, globalScale), null, new Color(255, 120, 30) * 2f * (globalScale * globalScale), (float)Main.timeForVisualEffects * -0.04f, starB.Size() / 2, 0.38f * drawScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(starB, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(globalScale, globalScale), null, new Color(255, 120, 30) * 2f * (globalScale * globalScale), (float)Main.timeForVisualEffects * -0.04f + MathHelper.PiOver4, starB.Size() / 2, 0.5f * drawScale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(starB, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), null, new Color(255, 120, 30) * 1f, (float)Main.timeForVisualEffects * -0.04f, starB.Size() / 2, 0.38f * drawScale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(starB, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), null, new Color(255, 120, 30) * 1f, (float)Main.timeForVisualEffects * -0.04f + MathHelper.PiOver4, starB.Size() / 2, 0.5f * drawScale, SpriteEffects.None, 0f);

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
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
		}

        public override void OnKill(int timeLeft)
        {
			int explo = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SolsearBombExplosion>(), 
				(int)(Projectile.damage * 3f * globalScale), 2f, Main.player[Projectile.owner].whoAmI);
			(Main.projectile[explo].ModProjectile as SolsearBombExplosion).size = 0.35f * globalScale;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(Projectile.Center, Main.npc[i].Center) < 170 * globalScale && false)
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
					for (int j = 0; j < 0; j++)
					{
						Dust d = GlowDustHelper.DrawGlowDustPerfect(Main.npc[i].Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 4),
							Main.rand.NextBool() ? Color.OrangeRed : Color.Red, 0.65f, 0.4f, 0f, thisDustShader);
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


            int explosion = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FadeExplosionHandler>(), 0, 0, Main.myPlayer);

            if (Main.projectile[explosion].ModProjectile is FadeExplosionHandler feh)
            {
                feh.color = Color.Lerp(Color.OrangeRed, Color.Red, 0.15f);
                feh.colorIntensity = 1f;
                feh.fadeSpeed = 0.045f;
                for (int m = 0; m < 10; m++)
                {
                    FadeExplosionClass newSmoke = new FadeExplosionClass(Projectile.Center, Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(0.5f, 2f) * 2f);

                    newSmoke.size = 0.55f + Main.rand.NextFloat(-0.15f, 0.15f);
                    feh.Smokes.Add(newSmoke);

                }
            }

            for (int fg = 0; fg < 10; fg++)
            {
                Vector2 randomStart = Main.rand.NextVector2CircularEdge(1f, 1f) * 6f;
                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), randomStart * Main.rand.NextFloat(0.3f, 1.35f) * 1.5f, newColor: new Color(255, 130, 0) * 0.85f, Scale: Main.rand.NextFloat(1f, 1.4f) * 0.45f);
                gd.alpha = 2;

            }

            for (int i = 0; i < 3 + Main.rand.Next(3); i++)
            {
                Vector2 v = Main.rand.NextVector2CircularEdge(1f, 1f) * 1f;
                Dust sa = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, v * Main.rand.NextFloat(2f, 6f), 0,
                    Color.Orange, Main.rand.NextFloat(0.4f, 0.7f) * 1.35f);

                if (sa.velocity.Y > 0)
                    sa.velocity.Y *= -1;

                //sa.velocity += vec * 2f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			//float damage = Projectile.damage * globalScale;
            //base.ModifyHitNPC(ref damage);
        }
    }

    public class SolsearBombExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        public float opacity = 1f;
		public float size = 1f;
		public bool maxPower = false;
        
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.scale = 0.1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        Vector2 startingCenter;
		float rotVal = 0f;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (timer == 0)
			{
                startingCenter = Projectile.Center;
				timer = Main.rand.Next(0, 200);
            }

            timer++;

            Projectile.scale = MathHelper.Clamp(MathHelper.Lerp(Projectile.scale, 1.25f * size, 0.08f), 0f, 1.25f * size);

            if (Projectile.scale >= 0.8f * size)
                opacity = MathHelper.Clamp(MathHelper.Lerp(opacity, -0.2f, 0.15f), 0, 2);
			//else
                //opacity = MathHelper.Clamp(MathHelper.Lerp(opacity, 1.5f, 0.15f), 0f, 1f);

            if (opacity <= 0)
                Projectile.active = false;

            Projectile.width = (int)(190 * Projectile.scale);
            Projectile.height = (int)(190 * Projectile.scale);
            Projectile.Center = startingCenter;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/Orbs/ElectricPopDA").Value;
            Texture2D Tex2 = Mod.Assets.Request<Texture2D>("Assets/Orbs/ElectricPopE").Value;

            Texture2D Orb = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_16").Value;


            //Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/Burst_09").Value;

            float scale = Projectile.scale * 0.25f;


			float timeFade = 1f - (0.25f * (Projectile.scale / size));

            float timeA = timer * 0.045f * timeFade;
			float timeB = timer * -0.07f * timeFade;

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.Black * opacity * 0.35f, timeA, Tex.Size() / 2, scale * 1.65f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.Black * opacity * 0.35f, timeB, Tex.Size() / 2, scale * 1.65f + (0.15f * scale), SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Orb, Projectile.Center - Main.screenPosition, Orb.Frame(1, 1, 0, 0), Color.Black * opacity * 1f, timeB, Orb.Size() / 2, scale * 9f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.spriteBatch.Draw(Orb, Projectile.Center - Main.screenPosition, Orb.Frame(1, 1, 0, 0), new Color(255, 130, 30) * opacity * 1f, timeB, Orb.Size() / 2, scale * 8f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Orb, Projectile.Center - Main.screenPosition, Orb.Frame(1, 1, 0, 0), Color.Orange * opacity * 1f, timeB, Orb.Size() / 2, scale * 5f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), new Color(255, 130, 30) * opacity, timeA, Tex.Size() / 2, scale * 1.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.Red * opacity, timeB, Tex.Size() / 2, scale * 1.5f + (0.15f * scale), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), new Color(255, 130, 30) * opacity, timeA, Tex.Size() / 2, scale * 1.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.OrangeRed * opacity, timeB, Tex.Size() / 2, scale * 1.5f + (0.15f * scale), SpriteEffects.None, 0f);



            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), new Color(255, 130, 30) * opacity, timeA, Tex.Size() / 2, scale * 1.15f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.OrangeRed * opacity, timeB, Tex.Size() / 2, scale * 1.15f + (0.15f * scale), SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition, Tex2.Frame(1, 1, 0, 0), new Color(255, 130, 30) * opacity * 1f, timeA, Tex.Size() / 2, scale * 1.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition, Tex2.Frame(1, 1, 0, 0), Color.OrangeRed * opacity * 1f, timeB, Tex.Size() / 2, scale * 1.5f + (0.15f * scale), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition, Tex2.Frame(1, 1, 0, 0), new Color(255, 130, 30) * opacity * 1f, timeA, Tex.Size() / 2, scale * 1.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition, Tex2.Frame(1, 1, 0, 0), Color.OrangeRed * opacity * 1f, timeB, Tex.Size() / 2, scale * 1.5f + (0.15f * scale), SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 100);
        }
    }

}


