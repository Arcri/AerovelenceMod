using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common;

namespace AerovelenceMod.Content.Projectiles
{
	public class ManaLeechStar : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		private int timer;
		public float scale = 1f;

		public override void SetDefaults()
		{
			Projectile.scale = 1;
			Projectile.width = 2;
			Projectile.height = 2;

			Projectile.friendly = false;
			Projectile.hostile = false;

			Projectile.timeLeft = 300;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;

		}
		public override bool? CanCutTiles() => false;

		public override bool? CanDamage() => false;

        public override void AI()
		{
			//TODO: Very not multiplayer compatible
			Player target = Main.player[Main.myPlayer];

			if (timer > 20)
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * (13f + (timer * 0.02f)), .3f);

				if (Projectile.Center.Distance(target.Center) < 30)
                {
					SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = 0.7f, Volume = 0.2f }, target.position);
					target.statMana += 3;
					target.ManaEffect(3);
					Projectile.Kill();
                }
			}
			else
			{
				Projectile.velocity *= 0.96f;
			}

			scale = Math.Clamp(MathHelper.Lerp(scale, 1.25f, 0.08f), 0f, 1f);

			if (Projectile.velocity.X > 0)
				Projectile.rotation += 0.3f;
			else
				Projectile.rotation -= 0.3f;
			timer++;

		}


		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/Twinkle");

			Color betweenBlueA = Color.Lerp(Color.DodgerBlue, Color.DeepSkyBlue, 0.5f);
            Color betweenBlueB = Color.Lerp(Color.Blue, Color.DodgerBlue, 0.5f);


            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color[] cols = { Color.White * 1f, betweenBlueA * 0.75f, betweenBlueB * 0.525f };
            float[] scales = { 1.15f, 1.6f, 2.5f };

            float orbAlpha = 2f;
			float orbScale = 0.2f * Projectile.scale * scale;
            Vector2 orbOrigin = Star.Size() / 2f;

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.07f) * 0.15f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.13f) * 0.1f;

            Main.EntitySpriteDraw(Star, drawPos, null, cols[0] with { A = 0 } * orbAlpha, Projectile.rotation, orbOrigin, orbScale * scales[0], SpriteEffects.None);
            Main.EntitySpriteDraw(Star, drawPos, null, cols[1] with { A = 0 } * orbAlpha, Projectile.rotation, orbOrigin, orbScale * scales[1] * sineScale1, SpriteEffects.None);
            Main.EntitySpriteDraw(Star, drawPos, null, cols[2] with { A = 0 } * orbAlpha, Projectile.rotation, orbOrigin, orbScale * scales[2] * sineScale2, SpriteEffects.None);

            return false;
		}
	}

}