using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Dusts;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Projectiles
{
    public class IcyShardClone : ModProjectile
    {


        public override void SetDefaults()
        {
            projectile.aiStyle = -1;
            projectile.width = 10;
            projectile.height = 22;
            projectile.alpha = 0;
            projectile.damage = 6;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }



        public override void AI()
        {
            {
                for (int k = 0; k < 200; k++)
                {
                    Projectile proj = Main.projectile[k];
                    if (proj.active)
                    {
                        if (proj.type == mod.ProjectileType("FrostHydrasThrowProjectile"))
                        {
                            Vector2 targetProj = proj.Center;
                            projectile.ai[0] -= 2f;
                            float greg = projectile.ai[0];
                            projectile.position = targetProj + new Vector2(-136, 0).RotatedBy(MathHelper.ToRadians(greg));
                        }

                    }
                    else
                    {
                        projectile.timeLeft = 2;
                    }
                }
                projectile.rotation += projectile.velocity.X * 0.1f;
            }
        }
    }
}