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
			
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.timeLeft = 30;
			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (projectile.localAI[0] == 0)
			{
				projectile.localAI[0] = 1;

				if (projectile.ai[0] == 0)
				{
					projectile.frame = 3;
				}
				else if (projectile.ai[0] == 10)
				{
					projectile.frame = 0;
				}
				else
				{
					projectile.frame = (int)(projectile.ai[0] % 2) + 1;
				}

				DustSpawnEffect();

				projectile.ai[1] = projectile.velocity.ToRotation();
				projectile.rotation = projectile.ai[1];
				projectile.velocity *= 0f;
			}

			if (projectile.ai[0] > 0)
			{
				if (++projectile.localAI[0] == 6 && Main.myPlayer == projectile.owner)
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

			if (projectile.ai[0] == 0)
			{
				amount = 15;
				velocityModifier = 1.5f;
				dustType = ModContent.DustType<Dusts.PinkPetal>();
			}

			for (int i = 0; i < amount; ++i)
			{
				Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustType);
				dust.velocity *= velocityModifier;
			}
		}

		private void SpawnNextThorn()
		{
			Vector2 currentDirection = projectile.ai[1].ToRotationVector2();
			Vector2 newDirection = currentDirection.RotatedByRandom(MathHelper.PiOver4 * 0.75f);

			Vector2 offset = (newDirection - currentDirection) / 2;

			Projectile.NewProjectile(projectile.Center + currentDirection * 22 + offset * 24, newDirection, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0] - 1);
		}
	}
}
