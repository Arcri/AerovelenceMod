using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Buffs;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Projectiles
{
	public class SpiralExplosion : ModProjectile
	{
        public override void SetDefaults()
		{
			projectile.width = 0;
			projectile.height = 0;
			projectile.alpha = 255;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false; 
			projectile.ignoreWater = true;
			projectile.magic = true;
		}

		public Vector2 spiralCenter;
		float t = 7f;
		int dust = DustType<WispDustPurple>();

		public override void AI()
		{
			if(projectile.ai[0] == 1)
            {
				dust = DustType<WispDustAlt>();
			}

			if (t==7f)
            {
				spiralCenter = projectile.position;
				projectile.velocity = Vector2.Zero;
            }

			if (t > 0)
			{
				for (int j = 0; j < 4; j++)
				{
					Dust.NewDustDirect(spiralCenter + new Vector2(8*t, 8*t).RotatedBy(t-Math.PI), 1, 1, dust, 0, 0, 0, default, 0.75f);
					Dust.NewDustDirect(spiralCenter + new Vector2(8*-t, 8*-t).RotatedBy(t-Math.PI), 1, 1, dust, 0, 0, 0, default, 0.75f);
					t -= 0.1f;
				}
			}

			if (t<=0 && projectile.ai[1]<40)
            {
				projectile.Size = new Vector2(130, 130);
				projectile.position = spiralCenter-projectile.Size/2;
				for (int i = 0; i < 90; i++)
				{
					Vector2 position = spiralCenter + new Vector2(0f, -(2*projectile.ai[1])).RotatedBy(MathHelper.ToRadians(360f/90*i)); //credit Pyroknight#5804 for the formula
					Dust.NewDustDirect(position, 1, 1, dust, 0, 0, 0, default, 0.5f);
				}
				projectile.ai[1] += 5f;
			}

			if (projectile.ai[1] >= 40)
            {
				projectile.Kill();
            }
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.NextBool())
			{
				target.AddBuff(BuffType<SoulFire>(), 300);
			}
		}

		public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
	}
}
