using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Event.BloodMoon
{
    public class CrimsonReachBall : ModProjectile
    {
        // Set this to your mod's empty texture.
        // Since I dont have any in my test mod, I'm just using a vanilla one and setting the proj alpha to 255.
        public override string Texture => "Terraria/Item_" + ItemID.CrimtaneOre;

        public override void SetDefaults()
        {
            projectile.hostile = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;

            projectile.width = projectile.height = 4;

            projectile.alpha = 1;
            projectile.timeLeft = 180;
            projectile.aiStyle = 0;
        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.X * 0.2f;

            projectile.ai[0]++;

            if (projectile.ai[0] > 20f)
            {
                projectile.velocity.Y += 0.2f;
            }

            if (Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Blood, -projectile.velocity.X / 2f, -projectile.velocity.Y / 2f);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.scale = Main.rand.NextFloat(0.6f, 1f);

                projectile.netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Main.PlaySound(SoundID.Dig, projectile.position);
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);

            return true;
        }
    }
}
