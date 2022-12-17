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

namespace AerovelenceMod.Content.Items.Weapons.Frost.DeepFreeze
{
    public class DeepFreezeProj : ModProjectile
    {
		int timer = 0;
		public float colorIntensity = 1f;
		public Color color = Color.White;
		public float size = 1f;
		public float multiplier = 3f;
		public bool rise = false;

		public bool rotDir = Main.rand.NextBool();

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Icy Wind");

		}

        public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 200;
			Projectile.tileCollide = false;
			Projectile.scale = 0f;

		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
        {
			color = Color.SkyBlue;
			//multiplier = 1.5f;
			//size = 0.25f;

			Player player = Main.player[Projectile.owner];
			timer++;

			if (rotDir)
				Projectile.rotation += 0.02f;
			else
				Projectile.rotation -= 0.02f;

			colorIntensity -= 0.02f;

			Projectile.scale = MathHelper.Clamp(MathHelper.Lerp(Projectile.scale, size, 0.025f), 0f, size / 2);
			if (colorIntensity <= 0)
				Projectile.active = false;

			if (rise)
				Projectile.velocity.Y += -0.06f;

			timer++;

        }

		public override bool PreDraw(ref Color lightColor)
		{

            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Frost/DeepFreeze/DeepFreezeProj").Value;

            int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;


			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
			myEffect.Parameters["uColor"].SetValue(color.ToVector3() * (multiplier * colorIntensity));
			myEffect.Parameters["uTime"].SetValue(2);
			myEffect.Parameters["uOpacity"].SetValue(0.5f); //0.6
			myEffect.Parameters["uSaturation"].SetValue(1.2f);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
			myEffect.CurrentTechnique.Passes[0].Apply();

			if (timer > 3)
				Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

			return false;
		}
		public override void PostDraw(Color lightColor)
		{
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
		}
		public Color FetchRainbow()
		{
			float sin1 = (float)Math.Sin(MathHelper.ToRadians(timer));
			float sin2 = (float)Math.Sin(MathHelper.ToRadians(timer + 120));
			float sin3 = (float)Math.Sin(MathHelper.ToRadians(timer + 240));
			int middle = 180;
			int length = 75;
			float r = middle + length * sin1;
			float g = middle + length * sin2;
			float b = middle + length * sin3;
			Color color = new Color((int)r, (int)g, (int)b);
			return color;
		}

	}
}
