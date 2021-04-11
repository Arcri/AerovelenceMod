using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Woodpecker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Woodpecker");
            Tooltip.SetDefault("Converts wooden arrows into flying arrows that home in on enemies");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item5;
            item.crit = 4;
            item.damage = 17;
            item.ranged = true;
            item.width = 30;
            item.height = 54;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
			item.shoot = AmmoID.Arrow;
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 2f;
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (type == ProjectileID.WoodenArrowFriendly)
			{
				type = ModContent.ProjectileType<WoodpeckerProj>();
			}
			return true;
		}
	}
}

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class WoodpeckerProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.Homing[projectile.type] = true;
		}
		public override void SetDefaults()
		{
			projectile.width = 22;
			projectile.height = 22;
			projectile.alpha = 175;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.ranged = true;
			projectile.extraUpdates = 2;
		}
		int counter = 0;
		public override void AI()
		{
			projectile.velocity.Y += 0.03f;
			projectile.rotation = projectile.velocity.ToRotation();
			if (projectile.alpha > 30)
			{
				projectile.alpha -= 15;
				if (projectile.alpha < 30)
				{
					projectile.alpha = 30;
				}
			}
			if (projectile.alpha <= 30)
			{
				int dust = Dust.NewDust(projectile.Center - new Vector2(5), 0, 0, ModContent.DustType<Leaves>());
				Main.dust[dust].velocity *= 1f;
				counter++;
				if (counter >= 10)
				{
					float minDist = 480;
					int target2 = -1;
                    float speed = 1.4f;
                    if (projectile.friendly == true && projectile.hostile == false)
					{
                        float distance;
                        float dY;
                        float dX;
                        for (int i = 0; i < Main.npc.Length; i++)
                        {
                            NPC target = Main.npc[i];
                            if (!target.friendly && target.dontTakeDamage == false && target.lifeMax > 5 && target.active && target.CanBeChasedBy())
                            {
                                dX = target.Center.X - projectile.Center.X;
                                dY = target.Center.Y - projectile.Center.Y;
                                distance = (float)Math.Sqrt(dX * dX + dY * dY);
                                if (distance < minDist)
                                {
                                    bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);
                                    if (lineOfSight)
                                    {
                                        minDist = distance;
                                        target2 = i;
                                    }
                                }
                            }
                        }
                        if (target2 != -1)
						{
							NPC toHit = Main.npc[target2];
							if (toHit.active == true)
							{
								dX = toHit.Center.X - projectile.Center.X;
								dY = toHit.Center.Y - projectile.Center.Y;
								distance = (float)Math.Sqrt((double)(dX * dX + dY * dY));
								speed /= distance;
								projectile.velocity *= 0.85f;
								projectile.velocity += new Vector2(dX * speed, dY * speed);
							}
						}
					}
				}
			}
		}
		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(projectile.Center - new Vector2(5), 0, 0, ModContent.DustType<Wood>(), 0, 0, projectile.alpha);
				dust.velocity *= 0.55f;
				dust.velocity += projectile.velocity * 0.5f;
				dust.scale *= 1.75f;
				dust.noGravity = true;
			}
		}
	}
}