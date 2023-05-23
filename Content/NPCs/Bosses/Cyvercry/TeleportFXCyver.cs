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
using AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
	public class TeleportFXCyver : ModProjectile
	{
		private int timer;

		public bool reverse = false;

		public override string Texture => "Terraria/Images/Projectile_0";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("ShadowBlade");
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;

			Projectile.scale = 1f;
			Projectile.timeLeft = 120;

			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;

			Projectile.width = 1;
			Projectile.height = 1;

		}

        public override bool? CanDamage()
        {
			return false;
        }

		float mainAlpha = 1f;

		float starAlpha = 1f;
		public List<StarParticle> stars = new List<StarParticle>();
		bool spawnedStars = false;
		public override void AI()
		{
			
			if (timer == 0 && reverse)
			{
				scale = 0;
			}
			
			bool condition = false;

			if (reverse)
				condition = (scale == 0);
			else
				condition = (scale == 1 && timer > 2);

			if (condition)
			{
                if (!spawnedStars)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        StarParticle newStar = new StarParticle(Projectile.Center, Main.rand.NextVector2CircularEdge(5f, 5f) * Main.rand.NextFloat(0.7f, 1.3f));
                        stars.Add(newStar);
                    }
                    spawnedStars = true;
                }

                bool anyStarsActive = false;
                foreach (StarParticle star in stars)
                {
                    star.Update();
                }
            }


			if (reverse)
			{
				if (timer > 5)
					alpha = MathHelper.Clamp(alpha - 0.05f, 0, 1);

                scale = 1 - Math.Clamp(getProgress(timer * 0.1f), 0f, 1f);
            } 
			else
			{

                alpha = MathHelper.Clamp(alpha - 0.01f, 0, 1);

                scale = Math.Clamp(getProgress(timer * 0.06f), 0f, 1f);
            }


			timer++;

		}

		float scale = 1f;
		float alpha = 1f;
		public override bool PreDraw(ref Color lightColor)
        {
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			foreach (StarParticle star in stars)
            {
				star.DrawStar(Main.spriteBatch, Mod.Assets.Request<Texture2D>("Assets/TrailImages/GlowStar").Value);
            }

			Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/CyvercryGlowy").Value;
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, Tex.Size() / 2, 0.8f * (1f - scale), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, Tex.Size() / 2, 0.8f * (1f - scale), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Square(3,3), Tex.Frame(1, 1, 0, 0), Color.HotPink * 0.5f * alpha, Projectile.rotation, Tex.Size() / 2, 1.2f * (1f - scale), SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
        }

		public float getProgress(float input)
        {
			float toReturn;

			float c1 = 1.70158f;
			float c3 = c1 + 1f;

			toReturn =  c3 * input * input * input - c1 * input * input;
			return toReturn;
        }
    }

	public class StarParticle
	{
		public Vector2 Velocity;
		public Vector2 Center;

		public float rotation;
		public Color color;
		public float scale;
		public float alpha;

		public int timer;

		public StarParticle(Vector2 pos, Vector2 vel)
		{
			Center = pos;
			alpha = 1f;
			Velocity = vel;
			scale = 1f;
			rotation = vel.ToRotation();
		}

		public void Update()
		{
			float size = 1f;

			if (timer > 20)
				alpha = MathHelper.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.03f), 0f, 1f);
			//scale = MathHelper.Clamp(MathHelper.Lerp(scale, size, 0.025f), 0f, size / 2);
			Center += Velocity;
			Velocity *= 0.95f;
			rotation += Velocity.X * 0.03f;

			scale *= 0.99f;
			timer++;
		}


		public void DrawStar(SpriteBatch sb, Texture2D tex)
		{
			sb.Draw(tex, Center - Main.screenPosition, null, Color.HotPink * alpha, rotation, tex.Size() / 2, scale * 0.35f, SpriteEffects.None, 0f);
			sb.Draw(tex, Center - Main.screenPosition, null, Color.HotPink * alpha, rotation, tex.Size() / 2, scale * 0.35f, SpriteEffects.None, 0f);
        }

    }

	public class BigExplosionCyver : ModProjectile
    {
		private int timer;
		public override string Texture => "Terraria/Images/Projectile_0";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("ShadowBlade");
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;

			Projectile.scale = 1f;
			Projectile.timeLeft = 700;

			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;

			Projectile.width = 1;
			Projectile.height = 1;

		}

		public override bool? CanDamage()
		{
			return false;
		}

        bool firstFrame = true;
        float colorIntensity = 1f;
        float scale2 = 0;
        float scale3 = 0;

        float randomRot = 0;
        public override void AI()
        {
            if (firstFrame)
            {
                firstFrame = false;

                Projectile.rotation = Main.rand.NextFloat(6.28f);

                //Spawn Dust
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                for (int i = 0; i < 30; i++)
                {
                    if (true)
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(5, 5);
                        Dust gd = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f) * 3f, Color.HotPink, 0.15f, 0.2f, 0f, dustShader2);
                        gd.fadeIn = 45 + Main.rand.NextFloat(-3f, 14f);
                        gd.scale *= Main.rand.NextFloat(0.9f, 2.1f);
                    }
                    else if (i % 2 == 0)
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(6, 6);
                        Dust gd = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleFlare>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.HotPink, 0.7f, 0.4f, 0f, dustShader2);
                        gd.fadeIn = 1;
                    }
                }

            }

            Projectile.scale = Math.Clamp(MathHelper.Lerp(Projectile.scale, 2.6f, 0.15f), 0f, 2.5f);
            scale2 = Math.Clamp(MathHelper.Lerp(scale2, 2.6f, 0.05f), 0f, 2.5f);
            scale3 = Math.Clamp(MathHelper.Lerp(scale3, 2.6f, 0.10f), 0f, 2.5f);

            //Projectile.Center = Main.player[Projectile.owner].Center;
            Projectile.velocity = Vector2.Zero;

            if (timer > 10)
            {
                //Projectile.scale *= 0.9f;
                colorIntensity -= 0.12f;
                if (colorIntensity <= 0)
                    Projectile.active = false;
            }
            Projectile.rotation += 0.06f;

            timer++;
        }

        //OrangeRed, Orange, Gold, Gold, Wheat, White
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sixStar = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/star_05");
            Texture2D circle = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
            Texture2D circle2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_05");
            Texture2D color = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/color_burst_cyver");
            Texture2D fourStar = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/star_06");


            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.Black * 0.85f * colorIntensity, Projectile.rotation * 1.5f, circle2.Size() / 2, Projectile.scale * 0.75f, 0, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.DeepPink * 0.7f * colorIntensity, Projectile.rotation * 2f, sixStar.Size() / 2, Projectile.scale * 0.5f, 0, 0f);

            Main.spriteBatch.Draw(sixStar, Projectile.Center - Main.screenPosition, null, Color.DeepPink * colorIntensity, Projectile.rotation, sixStar.Size() / 2, Projectile.scale * 0.75f, 0, 0f);
            Main.spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, null, Color.HotPink * colorIntensity, Projectile.rotation, circle.Size() / 2, scale3 * 0.17f, 0, 0f);
            Main.spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, null, Color.HotPink * colorIntensity, Projectile.rotation, circle.Size() / 2, scale3 * 0.17f, 0, 0f);

            Main.spriteBatch.Draw(color, Projectile.Center - Main.screenPosition, null, Color.White * colorIntensity, randomRot, color.Size() / 2, Projectile.scale * 0.5f, 0, 0f);
            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.HotPink * 1f * colorIntensity, Projectile.rotation * 1.5f, circle2.Size() / 2, Projectile.scale * 0.5f, 0, 0f);
            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.White * 1f * colorIntensity, Projectile.rotation * 1.5f, circle2.Size() / 2, Projectile.scale * 0.25f, 0, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}


