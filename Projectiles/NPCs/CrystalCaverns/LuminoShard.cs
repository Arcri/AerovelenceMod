using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Dusts;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Projectiles.NPCs.CrystalCaverns
{
	public class LuminoShard : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			projectile.width = 10;
			projectile.height = 22;
			projectile.alpha =  0;
			projectile.damage = 6;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
		}
		public override void AI()
		{
			projectile.velocity *= 1.01f;
			Dust dust1 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustType<Sparkle>(), projectile.velocity.X, projectile.velocity.Y, 0, Color.White, 1);
			dust1.velocity /= 4f;
		}
	}
}