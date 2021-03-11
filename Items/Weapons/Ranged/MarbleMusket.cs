using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class MarbleMusket : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marble Musket");
            Tooltip.SetDefault("Left right click zoom in and see the trajectory of the bullet" +
                               "\nRight click to fire a chunk of slow-travelling marble");
        }

        public bool HasUsedTrajectory = false;
        public bool Zooming = false;
        public override void SetDefaults()
        {
            item.crit = 6;
            item.damage = 0;
            item.ranged = true;
            item.width = 58;
            item.height = 18;
            item.useTime = 1;
            item.useAnimation = 1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 55, 40);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MarbleBullet>();
            item.ammo = AmmoID.None;
            item.shootSpeed = 100;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (HasUsedTrajectory == true)
            {
                Main.NewText("True");
                Vector2 muzzleOffset = new Vector2(-7, -8);
                if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                {
                    position += muzzleOffset;
                }
            }
            else if (HasUsedTrajectory == false)
            {
                Main.NewText("False");
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(70));
                speedX = perturbedSpeed.X;
                speedY = perturbedSpeed.Y;
                Vector2 muzzleOffset = new Vector2(-7, -8);
                if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                {
                    position += muzzleOffset;
                }
            }
            Collision.CanHit(position, -5, 30, position, -5, 30);

            if (type == ProjectileID.Bullet)
            {
                type = ModContent.ProjectileType<MarbleBullet>();
            }
            return true;
        }
        #region altfuction
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                HasUsedTrajectory = !HasUsedTrajectory;
                Zooming = true;
                Main.NewText("Turning to True");
                item.useStyle = ItemUseStyleID.HoldingOut;
                item.useTime = 5;
                item.useAnimation = 5;
                item.damage = 10;
                item.shoot = ModContent.ProjectileType<MarbleShotTrajectory>();
                item.shootSpeed = 12f;
                
            }
            else
            {
                Zooming = false;
                Main.NewText("Turning to False");
                item.useStyle = ItemUseStyleID.HoldingOut;
                item.useTime = 50;
                item.useAnimation = 50;
                item.damage = 55;
                item.shoot = ModContent.ProjectileType<MarbleBullet>();
                item.shootSpeed = 30f;
            }
            return base.CanUseItem(player);
        }

        public override void HoldItem(Player player)
        {
            if (Zooming)
            {
                player.GetModPlayer<AeroPlayer>().zooming = true;
            }
        }

        #endregion
    }
}

namespace AerovelenceMod.Items.Weapons.Ranged
{
	public class MarbleBullet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Marble Bullet");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}
		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ranged = true;
			projectile.penetrate = 5;
			projectile.timeLeft = 600;
			projectile.alpha = 255;
			projectile.light = 0.5f;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.extraUpdates = 1;
			aiType = ProjectileID.Bullet;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.Kill();
			return true;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{ 
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
		public override void Kill(int timeLeft)
		{
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}
	}
}

namespace AerovelenceMod.Projectiles
{
    public class MarbleShotTrajectory : ModProjectile
    {
        public override bool? CanCutTiles() => false;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marble Trajectory");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;

            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;

            projectile.ranged = true;
            projectile.friendly = false;
            projectile.penetrate = -1;
            projectile.damage = 0;
            projectile.alpha = 255;
            projectile.extraUpdates = 16;
            projectile.light = 0;
            projectile.ignoreWater = true;
        }
        public override bool PreAI()
        {
            if (++projectile.localAI[0] > 4.8f)
            {
                for (int i = 0; i < 4; i++)
                {

                    int num = 5;
                    int index2 = Dust.NewDust(projectile.position, 1, 1, 206, 0.0f, 0.0f, 0, new Color(204, 181, 72), 1.3f);
                    Main.dust[index2].position = projectile.Center - projectile.velocity / num;
                    Main.dust[index2].velocity *= 0f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                }    
            }
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.Kill();
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
        }
    }
}