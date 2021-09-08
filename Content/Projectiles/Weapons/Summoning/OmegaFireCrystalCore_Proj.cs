#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Effects;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class OmegaFireCrystalCore_Proj : ModProjectile
	{
		public override string Texture => AerovelenceMod.CrystalCavernsAssets + "FireCrystal";

		public enum AIState
		{
			OwnerFollow = 0,
			Attack = 1
		}

		private AIState State
		{
			get => (AIState)projectile.ai[0];
			set => projectile.ai[0] = (int)value;
		}

		private readonly float maxSpeed = 8f;
		private readonly float orbitRadius = 60f;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
		}
		public override void SetDefaults()
		{
			projectile.width = 36;
			projectile.height = 52;

			projectile.penetrate = -1;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.tileCollide = false;

			projectile.localNPCHitCooldown = 10;
			projectile.usesLocalNPCImmunity = true;
		}
		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			Vector2 desiredPosition;

			if (State == AIState.OwnerFollow)
			{
				desiredPosition = owner.Center;

				float distance = 600;

				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					if (Main.npc[i].CanBeChasedBy(projectile) && Collision.CanHitLine(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
					{
						float currentDistance = (Main.npc[i].Center - projectile.Center).Length();

						if (currentDistance < distance)
						{
							distance = currentDistance;

							projectile.ai[0] = i + 1;
						}
					}
				}

				if (State != AIState.OwnerFollow)
				{
					projectile.netUpdate = true;
				}
			}
			else
			{
				NPC target = Main.npc[(int)projectile.ai[0] - 1];

				if (!target.CanBeChasedBy(projectile))
				{
					State = AIState.OwnerFollow;
				}

				if (Main.rand.NextBool(10))
				{
					Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.Crystal>(), projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f);
				}

				desiredPosition = target.Center;
			}

			// Whirling movement dependent on desiredPosition.
			projectile.ai[1]++;
			float modifier = projectile.ai[1] * 0.1f;

			desiredPosition += new Vector2((float)Math.Cos(modifier * 0.33f), (float)Math.Cos(modifier)) * orbitRadius;

			Vector2 desiredVelocity = Vector2.Normalize(desiredPosition - projectile.Center) * maxSpeed;

			projectile.velocity = Vector2.Lerp(projectile.velocity, desiredVelocity, 0.05f);

			// Apply offsets to velocity to prevent swarming.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (i != projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == projectile.type)
				{
					Vector2 directionToOther = Main.projectile[i].Center - projectile.Center;
					if (directionToOther.Length() <= 32f)
					{
						projectile.velocity -= directionToOther.SafeNormalize(Vector2.UnitY) * 0.1f;
					}
				}
			}

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			return (false);
		}

		public override bool CanDamage()
			=> State != AIState.OwnerFollow;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.Crystal>());
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(default, BlendState.Additive, default, default, default, Filters.Scene["AerovelenceMod:CavernCrystalShine"].GetShader().Shader);
			Filters.Scene["AerovelenceMod:CavernCrystalShine"].GetShader().ApplyTime((float)Main.time * 0.02f).ApplyOpacity(0.8f);
			this.DrawProjectileTrailCentered(spriteBatch, lightColor, 0.8f, 0.2f, 2);
			return (true);
		}
		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{

			spriteBatch.End();
			spriteBatch.Begin(default, default, default, default, default, default);
		}
	}
}