using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class AdamantitePulsarProj : ModProjectile
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Adamantite Pulsar");

        int dustType = 0;
        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.ranged = true;
            switch (new Random().Next(0, 2))
            {
                case 0:
                    dustType = 60;
                    break;
                case 1:
                    dustType = 60;
                    break;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Projectile.NewProjectile(projectile.position, projectile.velocity * 0, ModContent.ProjectileType<SolarWindExplosion>(), projectile.damage, projectile.whoAmI);
			StarExplosion();
            return true;
        }
		int j;
		public override void AI()
		{
			int numDust = 5;
			for (int i = 0; i < numDust; i++)
			{
				Vector2 position = projectile.position;
				position -= projectile.velocity * ((float)i / numDust);
				projectile.alpha = 255;
				int anotherOneBitesThis = Dust.NewDust(position, 1, 1, dustType, 0f, 0f, 100, default, 1f);
				Main.dust[anotherOneBitesThis].position = position;
				Main.dust[anotherOneBitesThis].velocity *= 0.2f;
				Main.dust[anotherOneBitesThis].noGravity = true;
			}
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			Projectile.NewProjectile(projectile.position, projectile.velocity * 0, ModContent.ProjectileType<SolarWindExplosion>(), projectile.damage, projectile.whoAmI);
			StarExplosion();
		}
		private void StarExplosion()
        {
			Vector2 vector16 = ((float)Main.rand.NextDouble() * ((float)Math.PI * 2f)).ToRotationVector2();
			float num651 = Main.rand.Next(4, 8);
			float num652 = Main.rand.Next(4, 8);
			float value16 = Main.rand.Next(4, 8);
			float explosionSize = 10f;
			for (float num654 = 0f; num654 < num651; num654++)
			{
				for (int num655 = 0; num655 < 2; num655++)
				{
					Vector2 value17 = vector16.RotatedBy(((num655 == 0) ? 1f : (-1f)) * ((float)Math.PI * 2f) / (num651 * 2f));
					for (float num656 = 0f; num656 < explosionSize; num656++)
					{
						Vector2 value18 = Vector2.Lerp(vector16, value17, num656 / explosionSize);
						float scaleFactor = MathHelper.Lerp(num652, value16, num656 / explosionSize);
						int num657 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), 6, 6, 60, 0f, 0f, 100, default(Color), 1.3f);
						Dust dust = Main.dust[num657];
						Dust dust2 = dust;
						dust2.velocity *= 0.1f;
						Main.dust[num657].noGravity = true;
						dust = Main.dust[num657];
						dust2 = dust;
						dust2.velocity += value18 * scaleFactor;
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
						Vector2 value20 = Vector2.Lerp(vector16, value19, num660 / explosionSize);
						float scaleFactor2 = MathHelper.Lerp(num652, value16, num660 / explosionSize) / 2f;
						int dust3 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), 6, 6, 60, 0f, 0f, 100, default, 1.3f);
						Dust dust194 = Main.dust[dust3];
						Dust dust2 = dust194;
						dust2.velocity *= 0.1f;
						Main.dust[dust3].noGravity = true;
						dust194 = Main.dust[dust3];
						dust2 = dust194;
						dust2.velocity += value20 * scaleFactor2;
					}
				}
				vector16 = vector16.RotatedBy((float)Math.PI * 2f / num651);
			}
		}
    }
}