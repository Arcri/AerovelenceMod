using AerovelenceMod.Buffs;
using AerovelenceMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Projectiles.Minions
{

	public class ShockburnDrone : ModProjectile
	{
		int i;

        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Star Drone");
			Main.projFrames[projectile.type] = 5;
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
			projectile.rotation = projectile.velocity.ToRotation();
			projectile.frameCounter++;
			if (projectile.frameCounter % 10 == 0)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
				if (projectile.frame >= 5)
					projectile.frame = 0;
			}

			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
			}
		}
	}
}