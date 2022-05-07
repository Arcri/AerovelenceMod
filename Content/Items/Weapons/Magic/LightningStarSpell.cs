using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class LightningStarSpell : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Lightning star spell");
            Tooltip.SetDefault("Fires a burst of shock stars that home on to targets");
        }
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 48;
            Item.height = 48;
            Item.useTime = 40;
            Item.useAnimation = 17;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LightningSpellProj>();
            Item.shootSpeed = 5f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, velocity.Y, ModContent.ProjectileType<LightningSpellOrbitProj>(), damage, knockBack, player.whoAmI);
            return true;
        }
    }

    public class LightningSpellProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.alpha = 0;
            Projectile.penetrate = 4;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            {
                if (Projectile.alpha > 30)
                {
                    Projectile.alpha -= 15;
                    if (Projectile.alpha < 30)
                    {
                        Projectile.alpha = 30;
                    }
                }
                if (Projectile.localAI[0] == 0f)
                {
                    AdjustMagnitude(ref Projectile.velocity);
                    Projectile.localAI[0] = 1f;
                }
                Vector2 move = Vector2.Zero;
                float distance = 400f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && !Main.npc[k].immortal)
                    {
                        Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance)
                        {
                            move = newMove;
                            distance = distanceTo;
                            target = true;
                        }
                    }
                }
                if (target)
                {
                    AdjustMagnitude(ref move);
                    Projectile.velocity = (5 * Projectile.velocity + move) / 6f;
                    AdjustMagnitude(ref Projectile.velocity);
                }
            }

            Projectile.rotation = 2.35619f;
            if (Projectile.ai[0] >= 45f)
            {
                Projectile.velocity *= 0.98f;
            }
            if (Projectile.velocity.Length() < 4.1f)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 3.2f;
            }
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 360f)
            {
                Projectile.Kill();
            }
        }
        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight);
                Main.dust[dust].noGravity = false;
                Main.dust[dust].velocity *= 2.5f;
            }
        }
    }

    public class LightningSpellOrbitProj : ModProjectile
    {
        //Fromula:{ [xCos(a) + ySin(a)]^2 /a^2 + [xSin(a) - yCos(a)]^2 /b^2 = 1 }
        //Single X and Y form:
        //->  X = kCos(x)Cos([rotation angle]) - Sin(x)*sin([rotation angle])
        //->  Y = kCos(x)Sin([rotation angle]) + Sin(x)*Cos([rotation angle])

        private float k; //k multiplier
        private float smol=1; //var to make the orbits smaller
        private Projectile alpha = Projectile.NewProjectileDirect(Vector2.Zero, Vector2.Zero, 166, 50 / 3, 2, 0);
        private Projectile beta = Projectile.NewProjectileDirect(Vector2.Zero, Vector2.Zero, 166, 50 / 3, 2, 0);  //orbits projs
        private Projectile gamma = Projectile.NewProjectileDirect(Vector2.Zero, Vector2.Zero, 166, 50 / 3, 2, 0);

        public override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.penetrate = 4;
        }
        
        public override void AI()
        {   //Homing code from above
            if(Projectile.alpha > 30)
                {
                Projectile.alpha -= 15;
                if (Projectile.alpha < 30)
                {
                    Projectile.alpha = 30;
                }
            }
            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                Projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 400f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && !Main.npc[k].immortal)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }
            if (target)
            {
                AdjustMagnitude(ref move);
                Projectile.velocity = (5 * Projectile.velocity + move) / 6f;
                AdjustMagnitude(ref Projectile.velocity);
            }

            //Dust and orbits
            for (int i = 0; i<16; i++) {            
                float x = Projectile.Center.X - k*1.5f/smol*(float)(2.7f* Math.Cos(k + 3.14f)*Math.Cos(4.71f) - Math.Sin(k + 3.14f)*Math.Sin(4.71f));
                float y = Projectile.Center.Y - k*1.5f/smol*(float)(3f* Math.Cos(k + 3.14f)*Math.Sin(4.71f) + Math.Sin(k + 3.14f)*Math.Cos(4.71f));
                Dust Dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 206);
                Dust.position.RotatedBy(-Math.PI);
                Dust.velocity *= 0f;
                Dust.noGravity = true; 
                alpha.Center = Dust.position - alpha.Size / 2;
                alpha.penetrate = 4;
                alpha.tileCollide = false;
                alpha.alpha = 255;
               
                float x2 = Projectile.Center.X - k*1.5f/smol*(float)(2.7f* Math.Cos(k + 3.14f)*Math.Cos(0.48f) - Math.Sin(k + 3.14f)*Math.Sin(0.48f));
                float y2 = Projectile.Center.Y - k*1.5f/smol*(float)(3f* Math.Cos(k + 3.14f)*Math.Sin(0.48f) + Math.Sin(k + 3.14f)*Math.Cos(0.48f));
                Dust Dust2 = Dust.NewDustDirect(new Vector2(x2, y2), 1, 1, 206, 0, 0, 0, Color.White);
                Dust2.position.RotatedBy(-Math.PI);
                Dust2.velocity *= 0f;
                Dust2.noGravity = true; 
                beta.Center = Dust2.position - beta.Size / 2;
                beta.penetrate = 4;
                beta.tileCollide = false;
                beta.alpha = 255;
                
                float x3 = Projectile.Center.X - k*1.5f/smol*(float)(2.7f* Math.Cos(k + 3.14f)*Math.Cos(2.67f) - Math.Sin(k + 3.14f)*Math.Sin(2.67f));
                float y3 = Projectile.Center.Y - k*1.5f/smol*(float)(3f* Math.Cos(k + 3.14f)*Math.Sin(2.67f) + Math.Sin(k + 3.14f)*Math.Cos(2.67f));
                Dust Dust3 = Dust.NewDustDirect(new Vector2(x3, y3), 1, 1, 206, 0, 0, 0, Color.White);
                Dust3.position.RotatedBy(-Math.PI);
                Dust3.velocity *= 0f;
                Dust3.noGravity = true; 
                gamma.Center = Dust3.position-gamma.Size/2;
                gamma.penetrate = 4;
                gamma.tileCollide = false;
                gamma.alpha = 255;

                if (Projectile.ai[0] >= 200f)
                    k += 0.014f;
                else
                    k += 0.008f;
            }
            //Slows down the projectile
            if (Projectile.ai[0] >= 45f)
                Projectile.velocity *= 0.98f;
            
            if (Projectile.velocity.Length() < 4.1f)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 3.2f;
            }
            
            Projectile.ai[0]++;
            
            //Starts to resize the orbits and then kill the proj
            if (Projectile.ai[0] > 360)
                smol += 0.5f;

            if (Projectile.ai[0] == 398)
            {
                Projectile.Size = new Vector2(150, 150);
                Projectile.Center = Projectile.Center - Projectile.Size / 2;
            }
            if (Projectile.ai[0] >=400f)
                Projectile.Kill();
        }
        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }
        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
            alpha.Kill();
            beta.Kill();
            gamma.Kill();
        }

        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
    }
}