using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.Event.BloodMoon
{
    public class CrimsonReachBall : ModProjectile
    {
        // Set this to your mod's empty texture.
        // Since I dont have any in my test mod, I'm just using a vanilla one and setting the proj alpha to 255.
        public override string Texture => "Terraria/Images/Item_" + ItemID.CrimtaneOre;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.width = Projectile.height = 4;

            Projectile.alpha = 1;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = 0;
            Projectile.damage = 30;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.2f;

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 20f)
            {
                Projectile.velocity.Y += 0.2f;
            }

            if (Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, -Projectile.velocity.X / 2f, -Projectile.velocity.Y / 2f);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.scale = Main.rand.NextFloat(0.6f, 1f);

                Projectile.netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            return true;
        }
    }
}
