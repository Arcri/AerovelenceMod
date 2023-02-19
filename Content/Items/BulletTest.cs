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
using System.Collections.Generic;
using AerovelenceMod.Content.Projectiles;

namespace AerovelenceMod.Content.Items
{
    public class BulletTest : TrailProjBase
    {
		float timer = 0;
		public Color color = Color.White;
		public float overallSize = 1f;
		public int lineWidth = 3;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bullet Test");
		}

        public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 400;
			Projectile.tileCollide = true;
			Projectile.scale = 1f;
			Projectile.extraUpdates = 2;

		}

		public float xScale = 1f;
		public float yScale = 1f;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value;
            trailColor = new Color(255, 111, 20);
            trailTime = timer * 0.02f;

            trailPointLimit = 120;
            trailWidth = 20;
            trailMaxLength = 200;

            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center;

            TrailLogic();

			timer++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_54") with { Pitch = .28f, PitchVariance = .28f, };
            SoundEngine.PlaySound(style, Projectile.Center);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        }

        public float widthIntensity = 0;
		//public List<Projectile> InkProj = new List<Projectile>();
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Starlight").Value;
			Vector2 scale = new Vector2(Projectile.scale * 2, Projectile.scale) * 0.5f;

            //Contenders:
            //TrailImages/Starlight/EnergyTex/tri * -10
            //TrailImages/Starlight/196_Black/tri
            //TrailImages/Starlight/EnergyTex

            Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //(Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20)
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -10), Tex.Frame(1 ,1, 0, 0), Color.OrangeRed * 2, Projectile.rotation + MathHelper.PiOver2, Tex.Size() / 2, scale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -10), Tex.Frame(1, 1, 0, 0), Color.OrangeRed * 2, Projectile.rotation + MathHelper.PiOver2, Tex.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -10), Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation + MathHelper.PiOver2, Tex.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition - (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20), Tex.Frame(1, 1, 0, 0), Color.Orange, Projectile.rotation, Tex.Size() / 2, scale * 0.06f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition - (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20), Tex.Frame(1, 1, 0, 0), Color.Orange, Projectile.rotation, Tex.Size() / 2, scale * 0.06f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            TrailDrawing();

            return false;
		}

        public override float WidthFunction(float progress)
        {
            
            /*
            if (progress < 0.5f)
            {
                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, 30f, num) * 0.4f;
            }
            else if (progress >= 0.5)
            {
                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.6f, 1 - progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, 30f, num) * 0.4f;
            }
            */
            
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num) * 0.4f;
            
            return 0;
        }
    }
}
