using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class IceArrow : ModProjectile
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Ice Arrow");

        public override void SetDefaults()
        {
            Projectile.arrow = true;
            Projectile.aiStyle = 1;

            Projectile.width = Projectile.height = 18;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.velocity.X *= 0.999f;
            Projectile.velocity.Y += 0.23f;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 109, 0, 0);
            Main.dust[dust].noGravity = true;
        }   
    }
}
