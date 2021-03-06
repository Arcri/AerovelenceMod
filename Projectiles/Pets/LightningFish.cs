using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Projectiles.Pets
{
    public class LightningFish : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 4;
			Main.projPet[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.ZephyrFish);
			aiType = ProjectileID.ZephyrFish;
			projectile.width = 28;
			projectile.height = 20;
		}

		public override bool PreAI()
		{
			Player player = Main.player[projectile.owner];
			player.zephyrfish = false;
			return true;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.9f / 255f, (255 - projectile.alpha) * 0.1f / 255f, (255 - projectile.alpha) * 0.3f / 255f);

            #region vectors floats and ints
            Player player = Main.player[projectile.owner];
			AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
			Vector2 idlePosition = player.Center;
			Vector2 vectorToIdlePosition = idlePosition - projectile.Center;
			float distanceToIdlePosition = vectorToIdlePosition.Length();

            #endregion

            if (player.dead)
			{
				modPlayer.FishPartner = false;
			}
			if (modPlayer.FishPartner)
			{
				projectile.timeLeft = 2;
			}

            #region tp to player with dusty

            if (distanceToIdlePosition > 600f)
            {
                projectile.position.X = projectile.position.X + (projectile.width / 2.0f);
                projectile.position.Y = projectile.position.Y + (projectile.height / 2.0f);
                projectile.width = 30;
                projectile.height = 30;
                projectile.position.X = projectile.position.X - (projectile.width / 2.0f);
                projectile.position.Y = projectile.position.Y - (projectile.height / 2.0f);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 226, 0f, 0f, 100, new Color(112, 244, 250), 2f);
                    Main.dust[num622].velocity *= 1f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num622].scale = 0.3f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                projectile.Center = player.velocity * .25f;
                NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
                player.fallStart = (int)(player.position.Y / 16f);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 projectilePosition = projectile.position;
                    projectilePosition -= projectile.velocity * (i * 0.25f);
                    projectile.alpha = 255;
                    int dust = Dust.NewDust(projectilePosition, 1, 1, ModContent.DustType<Sparkle>(), 0f, 0f, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].position = projectilePosition;
                    Main.dust[dust].scale = Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[dust].velocity *= 0.2f;
                }
            }
            #endregion
        }
    }
}