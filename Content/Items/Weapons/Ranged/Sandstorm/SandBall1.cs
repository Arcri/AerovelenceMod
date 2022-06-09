using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.Ranged.Sandstorm
{
	public class SandBall1 : ModProjectile
	{

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sand Clump");

		}

		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 16;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.penetrate = 1;
			projectile.tileCollide = true;
			projectile.timeLeft = 120;

		}

		public override void AI()
		{
			//projectile.scale = 2f;
			//projectile.alpha = 100;
			Player owner = Main.player[projectile.owner]; //Makes a player variable of owner set as the player using the projectile
														  //projectile.rotation += (float)projectile.direction * 0.25f; //Spins in a good speed
														  //projectile.scale = .75f;
														  //projectile.velocity.Y += 0.2f; //0.2'
			int du1 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.AmberBolt, 0f, 0f, 75, default(Color), .9f);
			int du2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.AmberBolt, 0f, 0f, 75, default(Color), .9f);
			int du3 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.AmberBolt, 0f, 0f, 75, default(Color), .9f);

			Main.dust[du1].noGravity = true;
			Main.dust[du2].noGravity = true;
			Main.dust[du3].noGravity = true;

			Main.dust[du1].alpha += 5;
			Main.dust[du2].alpha += 5;
			Main.dust[du3].alpha += 5;

			if (Main.dust[du1].alpha > 250)
				Main.dust[du1].active = false;

			if (Main.dust[du2].alpha > 250)
				Main.dust[du2].active = false;

			if (Main.dust[du3].alpha > 250)
				Main.dust[du3].active = false;

			Dust.NewDust(projectile.Center + new Vector2(-5,-5), 5, 5, DustID.AmberBolt, 0, 0, 0, Color.SandyBrown, 0.4f);
			Dust.NewDust(projectile.Center + new Vector2(-9, -9), 9, 9, DustID.AmberBolt, 0, 0, 0, Color.SandyBrown, 0.4f);

			Dust d = Dust.NewDustPerfect(projectile.Center, DustID.AmberBolt, Vector2.Zero, 0, Color.SandyBrown, .8f);
			d.noGravity = true;
			Dust x = Dust.NewDustPerfect(projectile.Center, DustID.AmberBolt, Vector2.Zero, 0, Color.SandyBrown, 1f);
			x.noGravity = true;
			projectile.velocity.Y += 0.1f;
		}


		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			spriteBatch.End(); //Additive Blending my beloved
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			base.PostDraw(spriteBatch, lightColor);
        }

        public override void Kill(int timeLeft)
        {
			for (int i = 0; i < 40; i++)
			{
				int mydust = Dust.NewDust(projectile.Center, 4, 4, DustID.Rainbow, new Vector2(Main.rand.NextFloat(6, 3), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).X, new Vector2(Main.rand.NextFloat(6, 3), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).Y, 0, Color.SandyBrown, 1f);
				Main.dust[mydust].noGravity = true;
				//Main.dust[mydust] = 
				//int d = Dust.NewDust(projectile.Center, projectile.width, projectile.height, DustID.RubyBolt);
				//Main.dust[d].velocity = new Vector2(Main.rand.NextFloat(8, 13), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))); // Vector2.Normalize(Main.dust[d].position - projectile.Center) * Main.rand.NextFloat(8,13);
				//Main.dust[d].noGravity = true;
			}
			Main.PlaySound(SoundID.DD2_BetsyWindAttack.WithVolume(0.3f), projectile.Center);

			Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<SandBallAura>(), 0, 0);
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			projectile.Kill();
            //base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}


