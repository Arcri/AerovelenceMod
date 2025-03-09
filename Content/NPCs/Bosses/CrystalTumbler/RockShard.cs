using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class RockShard : ModProjectile
    {
        int t;
        public override void SetDefaults()
        {
            Projectile.aiStyle = 1;
            Projectile.penetrate = -1;
            Projectile.width = 14;
            Projectile.height = 18;
            Projectile.alpha = 0;
            Projectile.damage = 6;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            t++;
            Projectile.velocity.Y += 0.3f;
            int dust1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.Blue, 1);
            Main.dust[dust1].velocity /= 2f;
            if (t > 25)
            {
                Projectile.tileCollide = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item23, Projectile.Center);
        }
    }
}