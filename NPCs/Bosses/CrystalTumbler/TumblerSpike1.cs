using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
	public class TumblerSpike1 : ModProjectile
	{
		int t;
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			projectile.width = 44;
			projectile.height = 44;
			projectile.alpha =  0;
			projectile.damage = 12;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}
		public override void AI()
		{
			t++;
			if (Main.expertMode)
			{
				projectile.damage = 16;
			}
			projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 0, Color.Black, 1);
			Main.dust[dust1].velocity /= 2f;
			if (t > 100)
			{
				projectile.tileCollide = true;
			}
		}
	}
}