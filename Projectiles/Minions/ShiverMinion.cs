using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Projectiles.Minions
{

    public class ShiverMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            projectile.aiStyle = 317;
            projectile.width = 18;
            projectile.height = 28;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
        }


        public override bool? CanCutTiles()
        {
            return true;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        private int timer;

        public override void AI()
        {
            timer++;
            projectile.spriteDirection = projectile.localAI[1] < 0 ? -1 : 1;
            if (projectile.frame < 4)
            {
                projectile.frameCounter++;
                if (projectile.frameCounter >= 8)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    projectile.frame %= 4;
                }
            }
        }
    }
}