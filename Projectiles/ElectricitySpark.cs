using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Projectiles
{
    public class ElectricitySpark : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electricity Spark");
            
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 14;
            projectile.aiStyle = 0;
            Main.projFrames[projectile.type] = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 600;
            projectile.spriteDirection = projectile.direction;
        }

        int i;

        public override void AI()
        {
            i++;
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.frameCounter++;
            if (projectile.frameCounter % 3 == 0) //does the exact same thing but more elegant
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame >= 4)
                    projectile.frame = 0;
            }
            
            if (i % 2 == 0)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.ai[0] += 0.1f;
            projectile.velocity *= 0.75f;
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
                {
                    projectile.velocity.X = oldVelocity.X * -2;
                }
                if (projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
                {
                    projectile.velocity.Y = oldVelocity.Y * -2;
                }
                projectile.velocity *= 0.75f;
                Main.PlaySound(SoundID.Item10, projectile.position);
            }
            return false;
        }
    }
}