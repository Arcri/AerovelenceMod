using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
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
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.alpha = 0;
            Projectile.timeLeft = 300;
            AIType = ProjectileID.Bullet;
            storedpos.X = Projectile.velocity.X;
            storedpos.Y = Projectile.velocity.Y;
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
                float x = Projectile.position.X - Projectile.velocity.X / 10f * (float)j;
                float y = Projectile.position.Y - Projectile.velocity.Y / 10f * (float)j;
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
            Projectile.velocity = new Vector2((float)(storedpos.X*Math.Cos(mouseangleAlpha) - storedpos.Y*Math.Sin(mouseangleAlpha)), (float)(storedpos.X*Math.Sin(mouseangleAlpha)+storedpos.Y*Math.Cos(mouseangleAlpha)) * direction);
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}