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


namespace AerovelenceMod.Content.Items.Weapons.SlateSet
{
    public class SlateChunk : ModProjectile
    {
		//If ai[1] is 2 it means it comes from Slate Bow
		float start = 0;

		private int timer;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slate Chunk");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

		}

		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.aiStyle = 2;
			Projectile.DamageType = Projectile.ai[1] == 2 ? DamageClass.Ranged : DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.damage = 10;
			Projectile.hostile = false;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.timeLeft = 100;
			Projectile.tileCollide = true;
			Projectile.scale = 1f;
			Projectile.alpha = 255;

		}

        public override void AI()
        {
			Projectile.alpha -= 15;
			Projectile.velocity.X -= 0.02f;
			if (Projectile.ai[1] != 2)
            {
				Projectile.ai[0]++;
				Projectile.velocity.Y += 0.04f;

			}
			else
            {
				Projectile.DamageType = DamageClass.Ranged;
				Projectile.scale = 0.8f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return lightColor * 2f;
		}

        public override void Kill(int timeLeft)
        {
			if (Projectile.ai[1] == 2)
            {
				//SoundEngine.PlaySound(new SoundStyle("Redux/Sounds/Item/RockCollideBetter") with { PitchVariance = 0.3f, Volume = 0.4f, Pitch = 0.4f }, Projectile.Center);
			}
			else
            {
				//SoundEngine.PlaySound(new SoundStyle("Redux/Sounds/Item/RockCollideBetter") with { PitchVariance = 0.3f, Volume = 0.7f }, Projectile.Center);
			}
			//SoundEngine.PlaySound(SoundID.Item73 with { Volume = 0.5f, Pitch = 0.5f, PitchVariance = 0.2f });

			int dustAmount = Projectile.ai[1] == 2 ? 5 : 10;

			for (int i = 0; i < dustAmount; i++)
			{
				
				//int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RisingSmokeDust>(), 0f, -2f, 0, default, 0.3f);
				//Main.dust[num].noGravity = true;
				//Dust dust = Main.dust[num];
				//dust.position.X += (Main.rand.Next(-30, 31) / 20) - 1.5f;
				//dust.position.Y += (Main.rand.Next(-30, 31) / 20) - 1.5f;
				//if (dust.position != Projectile.Center)
				//	dust.velocity = Projectile.DirectionTo(Main.dust[num].position) * 0.1f;

				/*
				Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<RisingSmokeDust>(), 0, 0, 100, Scale: 0.2f + Main.rand.NextFloat(-0.1f, 0.3f));
				dust.velocity.X *= 0.2f;
				dust.velocity.Y = Math.Abs(dust.velocity.Y) * -1;
				dust.noGravity = true;
				dust.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/TestShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic").UseColor(Color.Gray * 0.5f);
				*/
				
				//SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
			}

			for (int j = 0; j < dustAmount - 2; j++)
            {
				Dust dust2 = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Stone, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, Scale: 1.5f * Projectile.scale);
				//Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.Stone, Projectile.velocity);
				dust2.noGravity = true;
			}

			base.Kill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
		{

			return true;
		}
	}
}
