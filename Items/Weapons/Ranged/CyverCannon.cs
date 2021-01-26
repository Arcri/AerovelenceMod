using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class CyverCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyver-Cannon");
            Tooltip.SetDefault("Let's just ping everyone all at once");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 20;
            item.useTime = 20;
            item.shootSpeed = 20f;
            item.knockBack = 2f;
            item.width = 20;
            item.height = 12;
            item.damage = 60;
            item.shoot = mod.ProjectileType("CyverCannonProj");
            item.mana = 6;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 5, 20, 0);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.magic = true;
            item.channel = true;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Ranged
{
	public class CyverCannonProj : ModProjectile
	{
		int i;
		public int Timer;
		public float shootSpeed = 0.5f;

        public override void SetStaticDefaults()
        {
			Main.projFrames[projectile.type] = 6;
		}

        public override void SetDefaults()
		{
			projectile.width = 38;
			projectile.aiStyle = 2;
			projectile.alpha = 0;
			projectile.height = 70;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.hide = true;
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


			Timer++;
			if (Timer > 3)
			{
				projectile.velocity *= shootSpeed;
				shootSpeed *= 0.99f;
				Projectile.NewProjectile(projectile.Center, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<DarkLaser>(), 6, 1f, Main.myPlayer);
				Timer = 0;
			}
			int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1.9f);
			Main.dust[dust].noGravity = true;
			Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0.25f, 0.4f, 0.7f);
			{
				int num294 = Main.rand.Next(3, 7);
				for (int num295 = 0; num295 < num294; num295++)
				{
					int num296 = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, 135, 0f, 0f, 100, default(Color), 2.1f);
					Dust dust105 = Main.dust[num296];
					Dust dust2 = dust105;
					dust2.velocity *= 2f;
					Main.dust[num296].noGravity = true;
				}
			}

		}
	}
}