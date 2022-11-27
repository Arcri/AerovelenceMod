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

namespace AerovelenceMod.Content.Projectiles.Other
{
    public class HollowPulse : ModProjectile
    {
		int timer = 0;
		float opacity = 1f;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hollow Pulse");

		}

        public override void SetDefaults()
		{
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
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
			Player player = Main.player[Projectile.owner];
			timer++;

			Projectile.scale = MathHelper.Clamp(MathHelper.Lerp(Projectile.scale, 0.75f, 0.08f), 0f, 0.5f);

			if (Projectile.scale == 0.5f)
				opacity = MathHelper.Clamp(MathHelper.Lerp(opacity, -0.2f, 0.1f), 0, 2);

			if (opacity <= 0)
				Projectile.active = false;

        }

		public override bool PreDraw(ref Color lightColor)
		{

            var Tex = Mod.Assets.Request<Texture2D>("Content/Projectiles/Other/HollowOvalPulse").Value;

            int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;


            Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            return false;
		}
		public override void PostDraw(Color lightColor)
		{
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
		}


    }
}
