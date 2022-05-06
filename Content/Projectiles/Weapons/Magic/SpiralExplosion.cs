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
			Projectile.width = 0;
			Projectile.height = 0;
			Projectile.alpha = 255;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false; 
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Magic;
		}

		public Vector2 spiralCenter;
		float t = 7f;
		int dust = DustType<WispDustPurple>();

		public override void AI()
		{
			if(Projectile.ai[0] == 1)
            {
				dust = DustType<WispDustAlt>();
			}

			if (t==7f)
            {
				spiralCenter = Projectile.position;
				Projectile.velocity = Vector2.Zero;
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

			if (t<=0 && Projectile.ai[1]<40)
            {
				Projectile.Size = new Vector2(130, 130);
				Projectile.position = spiralCenter-Projectile.Size/2;
				for (int i = 0; i < 90; i++)
				{
					Vector2 position = spiralCenter + new Vector2(0f, -(2*Projectile.ai[1])).RotatedBy(MathHelper.ToRadians(360f/90*i)); //credit Pyroknight#5804 for the formula
					Dust.NewDustDirect(position, 1, 1, dust, 0, 0, 0, default, 0.5f);
				}
				Projectile.ai[1] += 5f;
			}

			if (Projectile.ai[1] >= 40)
            {
				Projectile.Kill();
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
