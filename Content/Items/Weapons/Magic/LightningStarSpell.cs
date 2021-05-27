using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class LightningStarSpell : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Lightning star spell");
            Tooltip.SetDefault("Fires a burst of shock stars that home on to targets");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 30;
            item.magic = true;
            item.mana = 7;
            item.width = 48;
            item.height = 48;
            item.useTime = 40;
            item.useAnimation = 17;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LightningSpellProj>();
            item.shootSpeed = 5f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<LightningSpellOrbitProj>(), damage, knockBack, player.whoAmI);
            return true;
        }
    }

    public class LightningSpellProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.aiStyle = -1;
            projectile.width = 12;
            projectile.height = 12;
            projectile.alpha = 0;
            projectile.penetrate = 4;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
        }

        public override void AI()
        {
            {
                if (projectile.alpha > 30)
                {
                    projectile.alpha -= 15;
                    if (projectile.alpha < 30)
                    {
                        projectile.alpha = 30;
                    }
                }
                if (projectile.localAI[0] == 0f)
                {
                    AdjustMagnitude(ref projectile.velocity);
                    projectile.localAI[0] = 1f;
                }
                Vector2 move = Vector2.Zero;
                float distance = 400f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && !Main.npc[k].immortal)
                    {
                        Vector2 newMove = Main.npc[k].Center - projectile.Center;
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
                    projectile.velocity = (5 * projectile.velocity + move) / 6f;
                    AdjustMagnitude(ref projectile.velocity);
                }
            }

            projectile.rotation = 2.35619f;
            if (projectile.ai[0] >= 45f)
            {
                projectile.velocity *= 0.98f;
            }
            if (projectile.velocity.Length() < 4.1f)
            {
                projectile.velocity.Normalize();
                projectile.velocity *= 3.2f;
            }
            projectile.ai[0]++;

            if (projectile.ai[0] >= 360f)
            {
                projectile.Kill();
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
            Main.PlaySound(SoundID.Item10, projectile.position);
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.AncientLight);
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
            projectile.width = 0;
            projectile.height = 0;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.penetrate = 4;
        }
        
        public override void AI()
        {   //Homing code from above
            if(projectile.alpha > 30)
                {
                projectile.alpha -= 15;
                if (projectile.alpha < 30)
                {
                    projectile.alpha = 30;
                }
            }
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 400f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && !Main.npc[k].immortal)
                {
                    Vector2 newMove = Main.npc[k].Center - projectile.Center;
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
                projectile.velocity = (5 * projectile.velocity + move) / 6f;
                AdjustMagnitude(ref projectile.velocity);
            }

            //Dust and orbits
            for (int i = 0; i<16; i++) {            
                float x = projectile.Center.X - k*1.5f/smol*(float)(2.7f* Math.Cos(k + 3.14f)*Math.Cos(4.71f) - Math.Sin(k + 3.14f)*Math.Sin(4.71f));
                float y = projectile.Center.Y - k*1.5f/smol*(float)(3f* Math.Cos(k + 3.14f)*Math.Sin(4.71f) + Math.Sin(k + 3.14f)*Math.Cos(4.71f));
                Dust Dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 206);
                Dust.position.RotatedBy(-Math.PI);
                Dust.velocity *= 0f;
                Dust.noGravity = true; 
                alpha.Center = Dust.position - alpha.Size / 2;
                alpha.penetrate = 4;
                alpha.tileCollide = false;
                alpha.alpha = 255;
               
                float x2 = projectile.Center.X - k*1.5f/smol*(float)(2.7f* Math.Cos(k + 3.14f)*Math.Cos(0.48f) - Math.Sin(k + 3.14f)*Math.Sin(0.48f));
                float y2 = projectile.Center.Y - k*1.5f/smol*(float)(3f* Math.Cos(k + 3.14f)*Math.Sin(0.48f) + Math.Sin(k + 3.14f)*Math.Cos(0.48f));
                Dust Dust2 = Dust.NewDustDirect(new Vector2(x2, y2), 1, 1, 206, 0, 0, 0, Color.White);
                Dust2.position.RotatedBy(-Math.PI);
                Dust2.velocity *= 0f;
                Dust2.noGravity = true; 
                beta.Center = Dust2.position - beta.Size / 2;
                beta.penetrate = 4;
                beta.tileCollide = false;
                beta.alpha = 255;
                
                float x3 = projectile.Center.X - k*1.5f/smol*(float)(2.7f* Math.Cos(k + 3.14f)*Math.Cos(2.67f) - Math.Sin(k + 3.14f)*Math.Sin(2.67f));
                float y3 = projectile.Center.Y - k*1.5f/smol*(float)(3f* Math.Cos(k + 3.14f)*Math.Sin(2.67f) + Math.Sin(k + 3.14f)*Math.Cos(2.67f));
                Dust Dust3 = Dust.NewDustDirect(new Vector2(x3, y3), 1, 1, 206, 0, 0, 0, Color.White);
                Dust3.position.RotatedBy(-Math.PI);
                Dust3.velocity *= 0f;
                Dust3.noGravity = true; 
                gamma.Center = Dust3.position-gamma.Size/2;
                gamma.penetrate = 4;
                gamma.tileCollide = false;
                gamma.alpha = 255;

                if (projectile.ai[0] >= 200f)
                    k += 0.014f;
                else
                    k += 0.008f;
            }
            //Slows down the projectile
            if (projectile.ai[0] >= 45f)
                projectile.velocity *= 0.98f;
            
            if (projectile.velocity.Length() < 4.1f)
            {
                projectile.velocity.Normalize();
                projectile.velocity *= 3.2f;
            }
            
            projectile.ai[0]++;
            
            //Starts to resize the orbits and then kill the proj
            if (projectile.ai[0] > 360)
                smol += 0.5f;

            if (projectile.ai[0] == 398)
            {
                projectile.Size = new Vector2(150, 150);
                projectile.Center = projectile.Center - projectile.Size / 2;
            }
            if (projectile.ai[0] >=400f)
                projectile.Kill();
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