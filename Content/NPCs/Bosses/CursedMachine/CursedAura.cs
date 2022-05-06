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
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 200; //Change me
            Projectile.height = 200; //Change me
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.position = Main.npc[(int)Projectile.ai[0]].Center;
            for (int i = 0; i < 6; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 128);
                Main.dust[dust].velocity.X = 11 * (float)new Random().NextDouble() * 2 - 1;
                Main.dust[dust].velocity.Y = 11 * (float)new Random().NextDouble() * 2 - 1;
            }
            if (NPC.CountNPCS(Mod.Find<ModNPC>("CursedMachine").Type) < 1)
                Projectile.active = false;
        }
    }
}