using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{

	public class Minicry : ModProjectile
	{
		int i;

        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Mini-Cry");
			Main.projFrames[Projectile.type] = 6;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		}
        public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Spazmamini);
			Projectile.width = 44;
			Projectile.height = 26;
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
			bool flag64 = Projectile.type == Mod.Find<ModProjectile>("Minicry").Type;
			Player player = Main.player[Projectile.owner];
			AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
			if (flag64)
			{
				if (player.dead)
					modPlayer.Minicry = false;

				if (modPlayer.Minicry)
					Projectile.timeLeft = 2;

			}
			i++;
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.frameCounter++;
			if (Projectile.frameCounter % 10 == 0)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
				if (Projectile.frame >= 6)
					Projectile.frame = 0;
			}

			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 132);
			}
		}
	}
}