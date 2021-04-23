
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class DiamondBlastProjectile : ModProjectile
    {

        public override void SetStaticDefaults() => DisplayName.SetDefault("Diamond Blast Projectile");
        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;


        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = true;
        }

        int Timer = 0;
        public override void AI()
        {

            Timer++;
            if (Timer >= 100)
            {
                projectile.Kill();
            }
            projectile.velocity.X *= 0.984f;
            projectile.velocity.Y += 0.03f;


            // Dust
            for (int j = 0; j < 10; j++)
            {
                float x = projectile.position.X - projectile.velocity.X / 10f * (float)j;
                float y = projectile.position.Y - projectile.velocity.Y / 10f * (float)j;
                Dust dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 20, 0, 0, 0, Color.LightBlue, 0.9f);
                dust.position.X = x;
                dust.position.Y = y;
                dust.velocity *= 0f;
                dust.noGravity = true;
                dust.scale = 0.9f;
            }
        }
    }
}
