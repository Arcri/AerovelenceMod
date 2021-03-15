using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace AerovelenceMod.Projectiles.Weapons.Ranged
{
    public class PoseidonStream : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poseidon Stream");
        }

        public override void SetDefaults()
        {
            projectile.width = 7;
            projectile.height = 7;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 999999;
			projectile.alpha = 255;
			
        }
		
		public override void AI() {
			for (int i = 0; i < 14; i++) 
			{
				int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 29, projectile.velocity.X * 0, projectile.velocity.Y * 0, 120, default(Color), 1f);   //this make so when this projectile disappear will spawn dust, change PinkPlame to what dust you want from Terraria, or add mod.DustType("CustomDustName") for your custom dust
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 2f;
			}	
		}
		
		public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit)
		{
			target.AddBuff(BuffID.Wet, 999999);
			target.AddBuff(BuffID.CursedInferno , 300);
		}
        
        public override void Kill(int timeLeft)
        {
			Main.PlaySound(SoundID.Item10, projectile.position);
            for (int i = 0; i < 16; i++) 
            {
                int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 29, projectile.velocity.X * 0, projectile.velocity.Y * 0, 120, default(Color), 2f);   //this make so when this projectile disappear will spawn dust, change PinkPlame to what dust you want from Terraria, or add mod.DustType("CustomDustName") for your custom dust
                Main.dust[dust].noGravity = false;
                Main.dust[dust].velocity *= 2.5f;
            }
        }
    }
}