using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
	public class TumblerBoulder1 : ModProjectile
	{
		int t;
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			projectile.width = 44;
			projectile.height = 44;
			projectile.alpha =  255;
			projectile.damage = 12;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}
		public override void AI()
		{
			projectile.alpha -= 10;
			projectile.velocity.Y *= 99.8f;
			t++;
			projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 0, Color.Black, 1);
			Main.dust[dust1].velocity /= 2f;
			if (t > 25)
			{
				projectile.tileCollide = true;
			}
		}
	}
}

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
	public class TumblerBoulder2 : ModProjectile
	{
		int t;
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			projectile.width = 44;
			projectile.height = 44;
			projectile.alpha = 255;
			projectile.damage = 12;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}
		public override void AI()
		{
			projectile.alpha -= 10;
			projectile.velocity.Y *= 99.8f;
			t++;
			projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 0, Color.Black, 1);
			Main.dust[dust1].velocity /= 2f;
			if (t > 25)
			{
				projectile.tileCollide = true;
			}
		}
	}
}

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
	public class TumblerBoulder3 : ModProjectile
	{
		int t;
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			projectile.width = 44;
			projectile.height = 44;
			projectile.damage = 12;
			projectile.alpha = 255;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}
		public override void AI()
		{
			projectile.alpha -= 10;
			projectile.velocity.Y *= 99.8f;
			t++;
			projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 0, Color.Black, 1);
			Main.dust[dust1].velocity /= 2f;
			if (t > 25)
			{
				projectile.tileCollide = true;
			}
		}
	}
}