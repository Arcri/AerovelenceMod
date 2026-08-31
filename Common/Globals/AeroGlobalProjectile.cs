using AerovelenceMod.Content.Biomes;
using AerovelenceMod.Content.Items.Tools;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Globals
{
    public class AeroGlobalProjectile : GlobalProjectile
    {
        public override void AI(Projectile projectile)
        {
            /*
			if (projectile.type >= 230 && projectile.type <= 235 && Main.player[projectile.owner].GetModPlayer<AeroPlayer>().UpgradedHooks)
			{
				if (projectile.type == 230)
				{
					Lighting.AddLight(projectile.Center, 0.5f, 0.1f, 0.3f);
				}
				else if (projectile.type == 231)
				{
					Lighting.AddLight(projectile.Center, 0.5f, 0.3f, 0f);
				}
				else if (projectile.type == 232)
				{
					Lighting.AddLight(projectile.Center, 0.2f, 0.3f, 0.5f);
				}
				else if (projectile.type == 233)
				{
					Lighting.AddLight(projectile.Center, 0.2f, 0.5f, 0.05f);
				}
				else if (projectile.type == 234)
				{
					Lighting.AddLight(projectile.Center, 0.5f, 0.1f, 0.1f);
				}
				else if (projectile.type == 235)
				{
					Lighting.AddLight(projectile.Center, 1, 1, 1);
				}
			}
			*/
        }

        public override void PostAI(Projectile projectile)
        {
            //bool isConversionProjectile = projectile.type == ProjectileID.PurificationPowder
            //|| projectile.type == ProjectileID.VilePowder
            //|| projectile.type == ProjectileID.ViciousPowder
            //|| projectile.type == ProjectileID.PureSpray
            //|| projectile.type == ProjectileID.CorruptSpray
            //|| projectile.type == ProjectileID.CrimsonSpray
            //|| projectile.type == ProjectileID.HallowSpray;
            //if (!isConversionProjectile)
            //    return;

            //if (projectile.owner == Main.myPlayer)
            //{
            //    int x = (int)(projectile.Center.X / 16f);
            //    int y = (int)(projectile.Center.Y / 16f);
            //    bool isPowder = projectile.type == ProjectileID.PurificationPowder
            //        || projectile.type == ProjectileID.VilePowder
            //        || projectile.type == ProjectileID.ViciousPowder;

            //    for (int i = x - 1; i <= x + 1; i++)
            //    {
            //        for (int j = y - 1; j <= y + 1; j++)
            //        {
            //            if (projectile.type == ProjectileID.PureSpray || projectile.type == ProjectileID.PurificationPowder)
            //            {
            //                ElectricBlueSolutionProjectile.ConvertFromCrystalCavern(i, j, ConvertType.Pure, !isPowder);
            //            }
            //            if (projectile.type == ProjectileID.CorruptSpray || projectile.type == ProjectileID.VilePowder)
            //            {
            //                ElectricBlueSolutionProjectile.ConvertFromCrystalCavern(i, j, ConvertType.Corrupt, !isPowder);
            //            }
            //            if (projectile.type == ProjectileID.CrimsonSpray || projectile.type == ProjectileID.ViciousPowder)
            //            {
            //                ElectricBlueSolutionProjectile.ConvertFromCrystalCavern(i, j, ConvertType.Crimson, !isPowder);
            //            }
            //            if (projectile.type == ProjectileID.HallowSpray)
            //            {
            //                ElectricBlueSolutionProjectile.ConvertFromCrystalCavern(i, j, ConvertType.Hallow);
            //            }
            //            NetMessage.SendTileSquare(-1, i, j, 1, 1);
            //        }
            //    }
            //}
        }
    }
}