using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{

	public class MiniTumbler : ModProjectile
	{
		int i;

        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Mini Tumbler");
		}
        public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Spazmamini);
			Projectile.width = 56;
			Projectile.height = 22;
			Projectile.minion = true;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
			aiType = ProjectileID.Spazmamini;
			Projectile.alpha = 0;
			Projectile.penetrate = -10;
			Projectile.minionSlots = 1;
		}
		public override bool? CanCutTiles()
		{
			return true;
		}
		public override bool MinionContactDamage()
		{
			return true;
		}
		public override void AI()
		{
			i++;
			Projectile.rotation *= 0.1f;

			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 132);
			}
		}
	}
}