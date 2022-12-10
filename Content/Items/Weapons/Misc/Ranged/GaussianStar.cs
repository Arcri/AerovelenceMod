using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
{
    public class GaussianStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gaussian Star");
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 46;
			Projectile.height = 46;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.light = 1f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
		}
		public override void AI()
		{
			Projectile.scale = 1f;
			if (Projectile.direction == 1)
			{
				Projectile.rotation += 0.15f;
			}
			else
			{
				Projectile.rotation -= 0.15f;
			}
			if (Projectile.ai[0] < 15)
			{
				Projectile.ai[0]++;
			}
			else
			{
				Vector2 move = Vector2.Zero;
				float distance = 250f;
				bool target = false;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && Main.npc[i].lifeMax > 5 && Main.npc[i].type != NPCID.TargetDummy)
					{
						Vector2 newMove = Main.npc[i].Center - Projectile.Center;
						float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
						if (distanceTo < distance)
						{
							move = newMove;
							distance = distanceTo;
							target = true;
						}
					}
				}
				if (target)
				{
					AdjustMagnitude(ref move);
					Projectile.velocity *= 2f;
					Projectile.velocity = 2f * Projectile.velocity + move;
					AdjustMagnitude(ref Projectile.velocity);
				}
			}
		}
		private void AdjustMagnitude(ref Vector2 vector)
		{
			float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
			if (magnitude > 10f)
			{
				vector *= 10f / magnitude; 
			}
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			AoE();
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			AoE();
			return true;
		}
		public void AoE() 
		{
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GaussExplosion>(), 0, 0, Projectile.owner);
			
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(Projectile.Center, Main.npc[i].Center) < 85f)
				{
					int Direction = 0;
					if (Projectile.position.X - Main.npc[i].position.X < 0)
						Direction = 1;
					else
						Direction = -1;
					Main.npc[i].StrikeNPC(Projectile.damage, Projectile.knockBack, Direction);
					Main.npc[i].GetGlobalNPC<SkillStrikeNPC>().strikeCTRemove = true;
				}
			}
			SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
			/*
			for (double i = 0; i < 7.2; i += 0.2)
			{
				
				if (Main.rand.NextFloat() <= 0.3f)
				{
					Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, new Vector2((float)Math.Sin(i) * 2.6f, (float)Math.Cos(i)) * 2.6f); //cos2.4
					Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, new Vector2((float)Math.Sin(i) * 2.4f, (float)Math.Cos(i)).RotatedBy(Math.PI / 2) * 2.4f); //cos2.4

					dust.noGravity = true;
					dust2.noGravity = true;
					SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
				}
			}
			*/
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, TextureAssets.Projectile[Projectile.type].Value.Height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}

			Texture2D texture2 = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
			Main.spriteBatch.Draw(texture2, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle(0, 0, texture2.Width, texture2.Height), Color.CornflowerBlue * 0.8f, Projectile.rotation, texture2.Size() / 2, 2f * Projectile.scale, SpriteEffects.None, 0f);

			Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle(0, 0, TextureAssets.Projectile[Projectile.type].Value.Width, TextureAssets.Projectile[Projectile.type].Value.Height), Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;

		}
	}

	public class GaussExplosion : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gauss Explosion");
		}

		public override void SetDefaults()
        {
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.hostile = false;
			Projectile.penetrate = -1;

			Projectile.scale = 0.1f;

			Projectile.timeLeft = 30;
			Projectile.tileCollide = false; //false;
			Projectile.width = 20;
			Projectile.height = 20;
		}

        int timer = 0;
        public override void AI()
        {
			if (timer == 0)
            {
				Projectile.rotation = Main.rand.NextFloat(6.28f);
				//SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
			}

			Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.51f, 0.2f); //1.51
			timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Player Player = Main.player[Projectile.owner];
			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/GaussExplosion").Value;
			//Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/muzzle_05").Value;

			//ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/Solsear_Glow").Value

			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value;
			
			//myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/Solsear_Glow").Value);
			myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GaussianStar").Value);


			myEffect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);

			myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GaussianStar").Value);
			//myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/Solsear_Glow").Value);
			myEffect.Parameters["uTime"].SetValue(timer * 0.08f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
			myEffect.CurrentTechnique.Passes[0].Apply();

			int height1 = texture.Height;
			Vector2 origin1 = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
			
			return false;
        }
    }
}