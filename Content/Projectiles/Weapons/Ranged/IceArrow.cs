using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class IceArrow : ModProjectile
    {
        int Timer = 0;
        public override void SetStaticDefaults() => DisplayName.SetDefault("Ice Arrow");

        public override void SetDefaults()
        {
            projectile.arrow = true;
            projectile.aiStyle = 1;

            projectile.width = projectile.height = 18;

            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.friendly = true;
        }

        public override void AI()
        {
            projectile.velocity.X *= 1.015f;
            projectile.velocity.Y += 0.35f;

            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 20, 0, 0);
            Main.dust[dust].noGravity = true;
        }

        
    }
}
