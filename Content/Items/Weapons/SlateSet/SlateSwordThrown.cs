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
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.SlateSet
{
    public class SlateSwordThrown : ModProjectile
    {
		int timer;
		bool stuckInEnemy = false;
		bool stuckInGround = false;

		public int stickIndex = -1;
		public Vector2 offset = Vector2.Zero;

		Vector2 randomVec = Vector2.Zero;

		float storedRotation = 0;

		 float intensity = 40f;

		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 200;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Main.NewText(intensity);
			if (timer == 80)
            {
				ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

				for (int i = 0; i < 5 + (int)Main.rand.NextFloat(0f, 3f); i++)
                {
                    //Dust a = Common.Utilities.GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowDusts.GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 4f, Color.ForestGreen, Main.rand.NextFloat(.95f, 1.05f), 0.7f, 0, dustShader);
					//a.velocity *= 0.4f;
				}
			}

			if (timer % 8 == 0)
				randomVec = new Vector2(Main.rand.Next(-1, 2), Main.rand.Next(-1, 2)) * (1 - intensity / 40);
			if (stuckInEnemy || stuckInGround)
            {
				Projectile.tileCollide = false;
				Projectile.velocity = Vector2.Zero;
				Projectile.rotation = storedRotation + MathHelper.ToRadians(Main.rand.NextFloat(-3,4)) * (1 - intensity / 40);

				if (stuckInEnemy)
                {
					NPC enemyStuckInto = Main.npc[stickIndex];
					if (enemyStuckInto != null && enemyStuckInto.active)
                    {
						Projectile.Center = (enemyStuckInto.Center - offset);
					}
					if (enemyStuckInto.active == false)
						Projectile.active = false;

				}
			}
			else
            {
				if (timer >= 20)
				{
					Projectile.velocity.Y += 0.24f;
					Projectile.velocity.X *= 0.98f;

				}

				Projectile.velocity.X *= 0.98f;
				Projectile.rotation = ((float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f) + (float)((Math.PI) * -0.25f);

				if (timer % 6 == 0)
				{
					//int dust = Dust.NewDust(Projectile.Center - new Vector2(5), 0, 0, DustID.Grass);
					//Main.dust[dust].velocity *= 1f;
					//Main.dust[dust].scale = 0.5f;
				}
			}

			if (player.Center.Distance(Projectile.Center) < 800 && (stuckInGround || stuckInEnemy))
            {
				float distance = player.Center.Distance(Projectile.Center);
				intensity = distance / 20;
				//intensity = MathHelper.Lerp(intensity, distance / 20, 0.01f); //0 - 40
				// 40 * ? = 1/40

				if (player.Center.Distance(Projectile.Center) < 30 && timer > 80)
                {
					if (stuckInGround)
                    {
						ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");


						for (int i = 0; i < 10; i++) //4 //2,2
						{
							Vector2 vel = Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.Next(2, 10) * -1f;

							Dust p = Common.Utilities.GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleFlare>(), vel.RotatedBy((float)((Math.PI) * -0.25f)),
								Color.ForestGreen, Main.rand.NextFloat(0.6f, 1.2f), 0.4f, 0f, dustShader);
							p.velocity += Projectile.velocity * (0.4f + Main.rand.NextFloat(-0.1f, -0.2f));
							//p.rotation = Main.rand.NextFloat(6.28f);
						}



						SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, Projectile.Center);
						for (int i = 0; i < 2; i++)
                        {
							Vector2 vel2 = Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * (8f + Main.rand.NextFloat(-3, 4)) * -1f;

							Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel2.RotatedBy((float)((Math.PI) * -0.25f) + Main.rand.NextFloat(-0.1f, 0.1f)), ModContent.ProjectileType<SlateChunk>(), Projectile.damage / 3, 1, Main.myPlayer);
							//Little Ones
							Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel2.RotatedBy((float)((Math.PI) * -0.25f)), ModContent.ProjectileType<SlateChunk>(), Projectile.damage / 4, 1, Main.myPlayer, 0, 2);

						}

					}

					Projectile.active = false;
                }

			}
            else
            {
				intensity = 0;
            }

			timer++;
		}
		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item10);
			for (int i = 0; i < 15; i++)
			{
				//Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 0, 0, DustID.WoodFurniture, 0, 0, Projectile.alpha);
				//dust.velocity *= 0.55f;
				//dust.velocity += Projectile.velocity * 0.5f;
				//dust.scale *= 1.25f;
				//dust.noGravity = true;
			}
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			storedRotation = Projectile.rotation;
			offset = (target.Center - Projectile.Center);
			stickIndex = target.whoAmI;
			stuckInEnemy = true;
			base.OnHitNPC(target, damage, knockback, crit);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			storedRotation = Projectile.rotation;
			stuckInGround = true;
			return false;
            //return base.OnTileCollide(oldVelocity);
        }

        public override bool? CanHitNPC(NPC target)
        {
			if (stickIndex > -1 || stuckInGround)
				return false;
			return base.CanHitNPC(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			
			if (stuckInEnemy || stuckInGround)
            {
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
				for (int i = 0; i <= 4; i++)
				{
					Main.EntitySpriteDraw(texture, Projectile.Center + randomVec + new Vector2(0, -2 * 1).RotatedBy(MathHelper.ToRadians(360 / 4 * i)) - Main.screenPosition, null, Color.ForestGreen * (1 - intensity / 40) * 1.5f, Projectile.rotation, texture.Size() / 2, Projectile.scale + 0.15f, SpriteEffects.None, 0);
				}
				//Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.ForestGreen *  0.75f, Projectile.rotation, texture.Size() / 2, 1.25f, SpriteEffects.None, 0);

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			}
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, 1f, SpriteEffects.None, 0f);

			return false;
        }
    }
}
