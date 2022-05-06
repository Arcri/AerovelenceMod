using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{

    public class ShockburnDrone : ModProjectile
	{
		int i;

        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Star Drone");
			Main.projFrames[Projectile.type] = 5;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		}
        public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Spazmamini);
			Projectile.width = 56;
			Projectile.height = 22;
			Projectile.minion = true;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.netImportant = true;
			aiType = ProjectileID.Spazmamini;
			Projectile.alpha = 0;
			Projectile.penetrate = -10;
			Projectile.timeLeft = 18000;
			Projectile.minionSlots = 1;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.penetrate == 0)
				Projectile.Kill();

			return false;
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
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.frameCounter++;
			if (Projectile.frameCounter % 10 == 0)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
				if (Projectile.frame >= 5)
					Projectile.frame = 0;
			}

			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 132);
			}
		}
	}
}