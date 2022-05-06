
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
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
        }

        int Timer = 0;
        public override void AI()
        {

            Timer++;
            if (Timer >= 100)
            {
                Projectile.Kill();
            }
            Projectile.velocity.X *= 0.984f;
            Projectile.velocity.Y += 0.03f;


            // Dust
            for (int j = 0; j < 10; j++)
            {
                float x = Projectile.position.X - Projectile.velocity.X / 10f * (float)j;
                float y = Projectile.position.Y - Projectile.velocity.Y / 10f * (float)j;
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
