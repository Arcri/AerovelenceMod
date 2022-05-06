#region Using directives

using Terraria;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Utilities;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    internal sealed class ThornsOfAgony : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thorns of Agony");
			
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 16;

			Projectile.timeLeft = 30;
			Projectile.penetrate = -1;

			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (Projectile.localAI[0] == 0)
			{
				Projectile.localAI[0] = 1;

				if (Projectile.ai[0] == 0)
				{
					Projectile.frame = 3;
				}
				else if (Projectile.ai[0] == 10)
				{
					Projectile.frame = 0;
				}
				else
				{
					Projectile.frame = (int)(Projectile.ai[0] % 2) + 1;
				}

				DustSpawnEffect();

				Projectile.ai[1] = Projectile.velocity.ToRotation();
				Projectile.rotation = Projectile.ai[1];
				Projectile.velocity *= 0f;
			}

			if (Projectile.ai[0] > 0)
			{
				if (++Projectile.localAI[0] == 6 && Main.myPlayer == Projectile.owner)
				{
					SpawnNextThorn();
				}
			}

			return (false);
		}

		public override void Kill(int timeLeft)
			=> DustSpawnEffect();

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawProjectileCentered(spriteBatch, lightColor);

		private void DustSpawnEffect()
		{
			int amount = 6;
			int dustType = ModContent.DustType<Dusts.Wood>();
			float velocityModifier = 1f;

			if (Projectile.ai[0] == 0)
			{
				amount = 15;
				velocityModifier = 1.5f;
				dustType = ModContent.DustType<Dusts.PinkPetal>();
			}

			for (int i = 0; i < amount; ++i)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType);
				dust.velocity *= velocityModifier;
			}
		}

		private void SpawnNextThorn()
		{
			Vector2 currentDirection = Projectile.ai[1].ToRotationVector2();
			Vector2 newDirection = currentDirection.RotatedByRandom(MathHelper.PiOver4 * 0.75f);

			Vector2 offset = (newDirection - currentDirection) / 2;

			Projectile.NewProjectile(Projectile.Center + currentDirection * 22 + offset * 24, newDirection, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] - 1);
		}
	}
}
