using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class DayNightProj : ModProjectile
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Solar Wind");

        int DustType = 0;
		public override void SetDefaults()
		{
			Projectile.width = 6;
			Projectile.height = 6;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.DamageType = DamageClass.Ranged;
			if (Main.dayTime)
				DustType = 127;
			if (!Main.dayTime)
				DustType = 135;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0, ModContent.ProjectileType<SolarWindExplosion>(), Projectile.damage, Projectile.whoAmI);
			StarExplosion();
            return true;
        }

		private readonly int oneHelixRevolutionInUpdateTicks = 30;

		public override void AI()
		{
			int numDust = 5;
			for (int i = 0; i < numDust; i++)
			{
				Vector2 position = Projectile.position;
				position -= Projectile.velocity * ((float)i / numDust);
				Projectile.alpha = 255;
				int anotherOneBitesThis = Dust.NewDust(position, 1, 1, DustType, 0f, 0f, 100, default, 1f);
				Main.dust[anotherOneBitesThis].position = position;
				Main.dust[anotherOneBitesThis].velocity *= 0.2f;
				Main.dust[anotherOneBitesThis].noGravity = true;
			}


			// Rotate the projectile towards the direction it's going.
			Projectile.rotation += Projectile.velocity.Length() * 0.1f * Projectile.direction;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			
			StarExplosion();
		}
		private void StarExplosion()
        {
			Vector2 vector16 = ((float)Main.rand.NextDouble() * ((float)Math.PI * 2f)).ToRotationVector2();
			float num651 = Main.rand.Next(5, 9);
			float num652 = Main.rand.Next(12, 17);
			float value16 = Main.rand.Next(3, 7);
			float explosionSize = 20f;
			for (float num654 = 0f; num654 < num651; num654++)
			{
				for (int num655 = 0; num655 < 2; num655++)
				{
					Vector2 value17 = vector16.RotatedBy(((num655 == 0) ? 1f : (-1f)) * ((float)Math.PI * 2f) / (num651 * 2f));
					for (float num656 = 0f; num656 < explosionSize; num656++)
					{
						Vector2 value18 = Vector2.Lerp(vector16, value17, num656 / explosionSize);
						float scaleFactor = MathHelper.Lerp(num652, value16, num656 / explosionSize);
						if (Main.dayTime)
						{
							int num657 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), 6, 6, 127, 0f, 0f, 100, default(Color), 1.3f);
							Dust dust = Main.dust[num657];
							Dust dust2 = dust;
							dust2.velocity *= 0.1f;
							Main.dust[num657].noGravity = true;
							dust = Main.dust[num657];
							dust2 = dust;
							dust2.velocity += value18 * scaleFactor;
						}
						if (!Main.dayTime)
						{
							int num657 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), 6, 6, 135, 0f, 0f, 100, default(Color), 1.3f);
							Dust dust = Main.dust[num657];
							Dust dust2 = dust;
							dust2.velocity *= 0.1f;
							Main.dust[num657].noGravity = true;
							dust = Main.dust[num657];
							dust2 = dust;
							dust2.velocity += value18 * scaleFactor;
						}
					}
				}
				vector16 = vector16.RotatedBy((float)Math.PI * 2f / num651);
			}
			for (float num658 = 0f; num658 < num651; num658++)
			{
				for (int num659 = 0; num659 < 2; num659++)
				{
					Vector2 value19 = vector16.RotatedBy(((num659 == 0) ? 1f : (-1f)) * ((float)Math.PI * 2f) / (num651 * 2f));
					for (float num660 = 0f; num660 < explosionSize; num660++)
					{
						if (Main.dayTime)
						{
							Vector2 value20 = Vector2.Lerp(vector16, value19, num660 / explosionSize);
							float scaleFactor2 = MathHelper.Lerp(num652, value16, num660 / explosionSize) / 2f;
							int dust3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), 6, 6, 127, 0f, 0f, 100, default, 1.3f);
							Dust dust194 = Main.dust[dust3];
							Dust dust2 = dust194;
							dust2.velocity *= 0.1f;
							Main.dust[dust3].noGravity = true;
							dust194 = Main.dust[dust3];
							dust2 = dust194;
							dust2.velocity += value20 * scaleFactor2;
						}
						if (!Main.dayTime)
						{
							Vector2 value20 = Vector2.Lerp(vector16, value19, num660 / explosionSize);
							float scaleFactor2 = MathHelper.Lerp(num652, value16, num660 / explosionSize) / 2f;
							int dust3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), 6, 6, 135, 0f, 0f, 100, default, 1.3f);
							Dust dust194 = Main.dust[dust3];
							Dust dust2 = dust194;
							dust2.velocity *= 0.1f;
							Main.dust[dust3].noGravity = true;
							dust194 = Main.dust[dust3];
							dust2 = dust194;
							dust2.velocity += value20 * scaleFactor2;
						}
					}
				}
				vector16 = vector16.RotatedBy((float)Math.PI * 2f / num651);
			}
		}
    }
}