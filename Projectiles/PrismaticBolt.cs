using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace AerovelenceMod.Projectiles
{
    public class PrismaticBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("CumBolt");
        }
    
        Vector2 storedpos = new Vector2();
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.alpha = 0;
            projectile.timeLeft = 300;
            aiType = ProjectileID.Bullet;
            storedpos.X = projectile.velocity.X;
            storedpos.Y = projectile.velocity.Y;
        }

        float angle = 0;
        int i = 0;
        bool once = true;
        float mouseangleAlpha;
        float direction;

        public override void AI()
        {
            base.AI();
            
            for (int j = 0; j < 10; j++)
            {
                float x = projectile.position.X - projectile.velocity.X / 10f * (float)j;
                float y = projectile.position.Y - projectile.velocity.Y / 10f * (float)j;
                Dust dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 206,0,0,0,Color.DarkBlue, 0.9f);
                dust.position.X = x;
                dust.position.Y = y;
                dust.velocity *= 0f;
                dust.noGravity = true;  
            }

            Vector2 mousepos = new Vector2(Main.MouseScreen.X - 960, Main.MouseScreen.Y - 506);
            float ipo = (float)Math.Sqrt(Math.Pow(mousepos.X, 2) + Math.Pow(mousepos.Y, 2));
         

            if(once)
            {
                once = false;
                mouseangleAlpha = (float)Math.Asin(mousepos.Y / ipo);
                direction = Main.player[Main.myPlayer].direction;
            }

            i += 45;

            angle += (float)Math.Abs((Math.PI * i / 180.0));
            storedpos.Y += (float)(3 * Math.Sin(angle));
            storedpos.X = 15 * direction;
            projectile.velocity = new Vector2((float)(storedpos.X*Math.Cos(mouseangleAlpha) - storedpos.Y*Math.Sin(mouseangleAlpha)), (float)(storedpos.X*Math.Sin(mouseangleAlpha)+storedpos.Y*Math.Cos(mouseangleAlpha)) * direction);
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
        }
    }
}