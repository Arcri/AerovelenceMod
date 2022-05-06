using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Melee
{
    public class CavernousCrystal : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cavernous Crystal");
		}
		public override void SetDefaults()
		{
			Projectile.width = 52;
			Projectile.height = 30;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = 7;
			Projectile.alpha = 65;
			Projectile.light = 1f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			{
				Projectile.velocity *= 1.00f;
				int dust1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.White, 1);
				Main.dust[dust1].velocity /= 2f;
			}
		}
	}
}