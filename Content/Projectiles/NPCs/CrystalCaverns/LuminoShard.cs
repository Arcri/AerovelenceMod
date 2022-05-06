using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Projectiles.NPCs.CrystalCaverns
{
	public class LuminoShard : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.aiStyle = 1;
			Projectile.width = 10;
			Projectile.height = 22;
			Projectile.alpha =  0;
			Projectile.damage = 6;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{
			Projectile.velocity *= 1.01f;
			Dust dust1 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType<Sparkle>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.White, 1);
			dust1.velocity /= 4f;
		}
	}
}