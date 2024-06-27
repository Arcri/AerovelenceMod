/*
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
using AerovelenceMod.Content.Dusts.GlowDusts;


namespace AerovelenceMod.Content.Items.Weapons.SlateSet
{
    public class StaffRock : ModProjectile
    {

		private int timer;
		Vector2 destination = Vector2.Zero;
		Vector2 home = new Vector2(-85, 0);
		float lerpe = 0;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slate Chunk");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 1;

		}

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.damage = 10;
			Projectile.hostile = false;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.timeLeft = 100;
			Projectile.tileCollide = true;
			Projectile.scale = 1f;

		}

        public override void AI()
        {
			Player player = Main.player[Projectile.owner];

			if (timer == 0)
            {
				ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

				Projectile.timeLeft = 50;
				Projectile.rotation = Main.rand.NextFloat(-3, 4);
			}

			//Projectile.velocity = Vector2.Lerp(Vector2.Normalize(Projectile.velocity), Vector2.Normalize(destination - player.MountedCenter), 0.12f); //slowly move towards direction of cursor

			if (timer > 2)
            {
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, destination, 0.003f);
			}
			Projectile.velocity.Normalize();

			Projectile.Center = player.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 85;


			timer++;
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return lightColor * 2f;
		}

        public override void OnKill(int timeLeft)
        {
			SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/RockCollide") with { PitchVariance = 0.1f, Volume = 0.2f, Pitch = 0.7f }, Projectile.Center);


			for (int i = 0; i < 3; i++)
			{
				ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

				int d = GlowDustHelper.DrawGlowDust(Projectile.Center, 0, 0, ModContent.DustType<GlowCircleRise>(), 0, Projectile.velocity.Y * 0.1f, Color.Gray * 0.7f, 0.6f + Main.rand.NextFloat(-0.1f, 0.2f), 0.8f, 0, dustShader);
				Main.dust[d].velocity.Y = Math.Abs(Main.dust[d].velocity.Y) * -1;
				Main.dust[d].velocity.X *= 0.6f;

				//int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RisingSmokeDust>(), 0f, -2f, 0, default, 0.3f);
				//Main.dust[num].noGravity = true;
				//Dust dust = Main.dust[num];
				//dust.position.X += (Main.rand.Next(-30, 31) / 20) - 1.5f;
				//dust.position.Y += (Main.rand.Next(-30, 31) / 20) - 1.5f;
				//if (dust.position != Projectile.Center)
				//	dust.velocity = Projectile.DirectionTo(Main.dust[num].position) * 0.1f;


				//SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
			}

			for (int j = 0; j < 2; j++)
            {
				Dust dust2 = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Stone, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.3f, Scale: 1.5f * Projectile.scale);
				//Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.Stone, Projectile.velocity);
				dust2.noGravity = true;
			}

			base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			for (int i = 0; i < 2; i++)
            {
				for (int j = 0; j < 6; j++)
				{
					Main.spriteBatch.Draw(texture, Projectile.oldPos[j] + new Vector2(texture.Width / 2, texture.Height / 2) - Main.screenPosition, null, Color.DeepPink, Projectile.rotation, texture.Size() / 2, 1.25f - (j * 0.2f), SpriteEffects.None, 0);
				}
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, 1f, SpriteEffects.None, 0f);


			return true;
		}

		public void setDestination(Vector2 input)
        {
			destination = input;
        }
	}
}
*/