using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.CursedMachine
{
    public class CursedMonster : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Monster");
            Main.projFrames[projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            projectile.width = 210; //Change me
            projectile.height = 216; //Change me
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 9999999;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.25f, 0.95f, 0.25f);
            int num192 = (int)Player.FindClosest(projectile.Center, 1, 1);
            var dist = Main.player[num192].Center - projectile.Center;
            float len = dist.Length();
            dist.Normalize();
            if (len >= 100)
                projectile.velocity = dist * 6;
            projectile.rotation = 0;
            projectile.ai[0]++;
            if (projectile.ai[0] % 4 == 0)
            {
                projectile.ai[1]++;
                if (projectile.ai[1] > 3)
                    projectile.ai[1] = 0;
                projectile.frame = (int)projectile.ai[1];
            }
            if (NPC.CountNPCS(mod.NPCType("CursedMachine")) < 1)
                projectile.active = false;
        }
    }
}