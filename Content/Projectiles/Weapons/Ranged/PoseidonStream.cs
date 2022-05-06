using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class PoseidonStream : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poseidon Stream");
        }

        public override void SetDefaults()
        {
            Projectile.width = 7;
            Projectile.height = 7;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 999999;
			Projectile.alpha = 255;
			
        }
		
		public override void AI() {
			for (int i = 0; i < 14; i++) 
			{
				int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 29, Projectile.velocity.X * 0, Projectile.velocity.Y * 0, 120, default(Color), 1f);   //this make so when this projectile disappear will spawn dust, change PinkPlame to what dust you want from Terraria, or add mod.DustType("CustomDustName") for your custom dust
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
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 16; i++) 
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 29, Projectile.velocity.X * 0, Projectile.velocity.Y * 0, 120, default(Color), 2f);   //this make so when this projectile disappear will spawn dust, change PinkPlame to what dust you want from Terraria, or add mod.DustType("CustomDustName") for your custom dust
                Main.dust[dust].noGravity = false;
                Main.dust[dust].velocity *= 2.5f;
            }
        }
    }
}