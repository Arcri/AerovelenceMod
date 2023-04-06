using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics;
using ReLogic.Content;

namespace AerovelenceMod.Content.Projectiles
{
	public class ManaLeechStar : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		private int timer;
		public float scale = 1f;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Arrow");
		}

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
        public override bool? CanCutTiles()
        {
			return false;
        }
        public override bool? CanDamage()
        {
			return false;
        }

        public override void AI()
		{
			//Obv not multiplayer compatible
			Player target = Main.player[Main.myPlayer];

			if (timer > 20)
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * (13f + (timer * 0.02f)), .3f);
				/*
				float prime = 12.5f;
				float distX = target.Center.X - Projectile.Center.X;
				float distY = target.Center.Y - Projectile.Center.Y;
				float whyPythagBro = (float)Math.Sqrt((double)distX * (double)distX + (double)distY * (double)distY);
				float primeOverWhyPythag = prime / whyPythagBro;
				float distXTimesPythag = distX * primeOverWhyPythag;
				float distYTimesPythag = distY * primeOverWhyPythag;

				Projectile.velocity.X = (Projectile.velocity.X * (float)(10f - 1) + distXTimesPythag) / 10f; //- 1
				Projectile.velocity.Y = (Projectile.velocity.Y * (float)(10f - 1) + distYTimesPythag) / 10f;
				*/
				if (Projectile.Center.Distance(target.Center) < 30)
                {
					SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = 0.7f, Volume = 0.2f }, target.position);
					target.statMana++;
					//player.HealEffect(2);
					target.ManaEffect(1);
					Projectile.Kill();
					//target.statMana = ;
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
			Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Twinkle");

			Color colToUse = new Color(30, 150, 255); //Color.DodgerBlue;
			colToUse.A = 0;

			Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, colToUse, Projectile.rotation, Star.Size() / 2, Projectile.scale * scale * 0.3f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, colToUse, Projectile.rotation, Star.Size() / 2, Projectile.scale * scale * 0.3f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, colToUse, Projectile.rotation, Star.Size() / 2, Projectile.scale * scale * 0.3f, SpriteEffects.None, 0f);


			return false;
		}
	}

}