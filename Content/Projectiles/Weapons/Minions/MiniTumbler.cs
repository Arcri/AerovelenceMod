using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Minions
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
			projectile.CloneDefaults(ProjectileID.Spazmamini);
			projectile.width = 56;
			projectile.height = 22;
			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;
			aiType = ProjectileID.Spazmamini;
			projectile.alpha = 0;
			projectile.penetrate = -10;
			projectile.minionSlots = 1;
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
			projectile.rotation *= 0.1f;

			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
			}
		}
	}
}