using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CursedMachine
{
    public class CursedMonster : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Monster");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 210; //Change me
            Projectile.height = 216; //Change me
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 9999999;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.25f, 0.95f, 0.25f);
            int num192 = (int)Player.FindClosest(Projectile.Center, 1, 1);
            var dist = Main.player[num192].Center - Projectile.Center;
            float len = dist.Length();
            dist.Normalize();
            if (len >= 100)
                Projectile.velocity = dist * 6;
            Projectile.rotation = 0;
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 4 == 0)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 3)
                    Projectile.ai[1] = 0;
                Projectile.frame = (int)Projectile.ai[1];
            }
            if (NPC.CountNPCS(Mod.Find<ModNPC>("CursedMachine").Type) < 1)
                Projectile.active = false;
        }
    }
}