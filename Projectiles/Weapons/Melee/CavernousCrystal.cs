using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Dusts;

namespace AerovelenceMod.Projectiles.Weapons.Melee
{
    public class CavernousCrystal : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cavernous Crystal");
		}
		public override void SetDefaults()
		{
			projectile.width = 52;
			projectile.height = 30;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.melee = true;
			projectile.penetrate = 7;
			projectile.alpha = 65;
			projectile.light = 1f;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.extraUpdates = 1;
		}
		public override void AI()
		{
			projectile.rotation = projectile.velocity.ToRotation();
			{
				projectile.velocity *= 1.00f;
				int dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Sparkle>(), projectile.velocity.X, projectile.velocity.Y, 0, Color.White, 1);
				Main.dust[dust1].velocity /= 2f;
			}
		}
	}
}