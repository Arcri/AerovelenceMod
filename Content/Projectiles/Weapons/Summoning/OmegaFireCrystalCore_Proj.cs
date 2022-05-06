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
			get => (AIState)Projectile.ai[0];
			set => Projectile.ai[0] = (int)value;
		}

		private readonly float maxSpeed = 15f;
		private readonly float orbitRadius = 100f;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
		}
		public override void SetDefaults()
		{
			Projectile.width = 36;
			Projectile.height = 52;

			Projectile.penetrate = -1;

			Projectile.minion = true;
			Projectile.friendly = true;
			Projectile.tileCollide = false;

			Projectile.localNPCHitCooldown = 10;
			Projectile.usesLocalNPCImmunity = true;
		}
		public override bool PreAI()
		{
			Player owner = Main.player[Projectile.owner];

			Vector2 desiredPosition;

			if (State == AIState.OwnerFollow)
			{
				desiredPosition = owner.Center;

				float distance = 600;

				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					if (Main.npc[i].CanBeChasedBy(Projectile) && Collision.CanHitLine(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
					{
						float currentDistance = (Main.npc[i].Center - Projectile.Center).Length();

						if (currentDistance < distance)
						{
							distance = currentDistance;

							Projectile.ai[0] = i + 1;
						}
					}
				}

				if (State != AIState.OwnerFollow)
				{
					Projectile.netUpdate = true;
				}
			}
			else
			{
				NPC target = Main.npc[(int)Projectile.ai[0] - 1];

				if (!target.CanBeChasedBy(Projectile))
				{
					State = AIState.OwnerFollow;
				}

				if (Main.rand.NextBool(10))
				{
					Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Crystal>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
				}

				desiredPosition = target.Center;
			}

			// Whirling movement dependent on desiredPosition.
			Projectile.ai[1]++;
			float modifier = Projectile.ai[1] * 0.1f;

			desiredPosition += new Vector2((float)Math.Cos(modifier * 0.33f), (float)Math.Cos(modifier)) * orbitRadius;

			Vector2 desiredVelocity = Vector2.Normalize(desiredPosition - Projectile.Center) * maxSpeed;

			Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.05f);

			// Apply offsets to velocity to prevent swarming.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type)
				{
					Vector2 directionToOther = Main.projectile[i].Center - Projectile.Center;
					if (directionToOther.Length() <= 32f)
					{
						Projectile.velocity -= directionToOther.SafeNormalize(Vector2.UnitY) * 0.1f;
					}
				}
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			return (false);
		}

		public override bool? CanDamage()
			=> State != AIState.OwnerFollow;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Crystal>());
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(default, BlendState.Additive, default, default, default, Filters.Scene["AerovelenceMod:CavernCrystalShine"].GetShader().Shader);
			Filters.Scene["AerovelenceMod:CavernCrystalShine"].GetShader().ApplyTime((float)Main.time * 0.02f).ApplyOpacity(0.8f);
			this.DrawProjectileTrailCentered(Main.spriteBatch, lightColor, 0.8f, 0.2f, 2);
			return (true);
		}
		public override void PostDraw(Color lightColor)
		{

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(default, default, default, default, default, default);
		}
	}
}