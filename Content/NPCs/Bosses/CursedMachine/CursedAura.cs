using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CursedMachine
{
    public class CursedAura: ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Aura");
            Main.projFrames[projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            projectile.width = 200; //Change me
            projectile.height = 200; //Change me
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            projectile.position = Main.npc[(int)projectile.ai[0]].Center;
            for (int i = 0; i < 6; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 128);
                Main.dust[dust].velocity.X = 11 * (float)new Random().NextDouble() * 2 - 1;
                Main.dust[dust].velocity.Y = 11 * (float)new Random().NextDouble() * 2 - 1;
            }
            if (NPC.CountNPCS(mod.NPCType("CursedMachine")) < 1)
                projectile.active = false;
        }
    }
}