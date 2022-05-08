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
    internal sealed class CarbonCadence_Proj : ModProjectile
	{
		public override string Texture => AerovelenceMod.CrystalCavernsAssets + "DiamondCavernCrystal";

		public enum AIState
		{
			Spawn = 0,
			Float = 1,
			Explosion = 2
		}
		private AIState State
		{
			get => (AIState)Projectile.ai[0];
			set => Projectile.ai[0] = (int)value;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Diamond Crystal Shard");
		}
		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 12;

			Projectile.alpha = 100;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;

			Projectile.friendly = true;
			Projectile.tileCollide = false;

			Projectile.localNPCHitCooldown = 30;
			Projectile.usesLocalNPCImmunity = true;
		}

		public override bool PreAI()
		{
			Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.3f / 255f, (255 - Projectile.alpha) * 0.9f / 255f);
			Projectile.velocity *= 0.98f;
			Projectile.rotation = Projectile.velocity.X * 0.1f;

			if (State == AIState.Spawn)
			{
				if (Projectile.velocity.Length() <= 1f)
				{
					State = AIState.Float;
					Projectile.netUpdate = true;
				}
			}
			else if (State == AIState.Float)
			{
				Projectile.ai[1]++;
				Projectile.velocity.Y += (float)Math.Cos(Projectile.ai[1] / 15) * 0.125f;
			}
			else if (State == AIState.Explosion)
			{
				if (Projectile.timeLeft > 3)
				{
					Projectile.timeLeft = 3;
				}

				Projectile.alpha = 255;

				Projectile.position = Projectile.Center;
				Projectile.width = Projectile.height = 128;
				Projectile.Center = Projectile.position;
			}

			return (false);
		}
		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
			=> State = AIState.Explosion;

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}

			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Crystal>());
			}

			if (State == AIState.Explosion)
			{
				for (int i = 0; i < 3; ++i)
				{
					Vector2 velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 8f;

					Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, velocity, ModContent.ProjectileType<DiamondCrystalShard>(), 10, 1f, Projectile.owner);
				}

				for (int i = 0; i < 20; ++i)
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Crystal>());
					dust.scale *= 1.5f;
					dust.velocity *= 2f;
					dust.noGravity = true;
				}
			}
		}

		public override bool PreDraw(ref Color lightColor)
			=> this.DrawProjectileCentered(Main.spriteBatch, lightColor * Projectile.Opacity);
	}
}
