#region Using directives

using System;

using Terraria;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Utilities;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    internal sealed class DustiliteTome_Proj : ModProjectile
	{
		public override string Texture => AerovelenceMod.CrystalCavernsAssets + "DustiliteCrystal";

		public enum AIState
		{
			Spawn = 0,
			Float = 1,
			Explosion = 2
		}
		private AIState State
		{
			get => (AIState)projectile.ai[0];
			set => projectile.ai[0] = (int)value;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dustilite Shard");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 12;

			projectile.alpha = 100;
			projectile.timeLeft = 600;
			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = false;

			projectile.localNPCHitCooldown = 30;
			projectile.usesLocalNPCImmunity = true;
		}

		public override bool PreAI()
		{
			projectile.velocity *= 0.98f;
			projectile.rotation = projectile.velocity.X * 0.1f;

			if (State == AIState.Spawn)
			{
				if (projectile.velocity.Length() <= 1f)
				{
					State = AIState.Float;
					projectile.netUpdate = true;
				}
			}
			else if (State == AIState.Float)
			{
				projectile.ai[1]++;
				projectile.velocity.Y += (float)Math.Cos(projectile.ai[1] / 15) * 0.125f;
			}
			else if (State == AIState.Explosion)
			{
				if (projectile.timeLeft > 3)
				{
					projectile.timeLeft = 3;
				}

				projectile.alpha = 255;

				projectile.position = projectile.Center;
				projectile.width = projectile.height = 128;
				projectile.Center = projectile.position;
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
			=> State = AIState.Explosion;

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X;
			}

			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y;
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.Dustilite>());
			}

			if (State == AIState.Explosion)
			{
				for (int i = 0; i < 3; ++i)
				{
					Vector2 velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 8f;

					Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<DustiliteShard>(), 10, 1f, projectile.owner);
				}

				for (int i = 0; i < 20; ++i)
				{
					Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.Dustilite>());
					dust.scale *= 1.5f;
					dust.velocity *= 2f;
					dust.noGravity = true;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawProjectileCentered(spriteBatch, lightColor * projectile.Opacity);
	}
}
