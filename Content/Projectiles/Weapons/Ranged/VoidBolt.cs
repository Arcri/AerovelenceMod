using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class VoidBolt : ModProjectile
    {
        public float WaveAmplitude = 175f;
        public float WaveMagnitude = .05f;
        private const float Speed = 7f;
        private int Timer = 0;



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Bolt");
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 7;
            projectile.height = 7;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 300;

        }

        private int TimeLeft = 0;


        private double GetDirection()
        {
            float degreeAngle;

            if (projectile.direction > 0) degreeAngle = 180f;
            else degreeAngle = 0f;

            return MathHelper.ToRadians(degreeAngle);
        }



        public override void AI()
        {
            {

                float yPos = (float)Math.Sin(GetDirection());
                float xPos = (float)Math.Cos(GetDirection());
                float wobble = WaveAmplitude * (float)Math.Cos(Timer++ * WaveMagnitude) * WaveMagnitude;
                projectile.velocity.Y += -yPos * Speed + xPos * wobble;
                projectile.velocity.X += xPos * Speed - yPos * wobble;


                TimeLeft++;
                projectile.ai[1] += 0.1f;
                projectile.rotation = projectile.velocity.ToRotation() + 1.57f;
                float cos = 1 + (float)Math.Cos(projectile.ai[1]);
                float sin = 1 + (float)Math.Sin(projectile.ai[1]);
                Color color = new Color(0.5f + cos * 0.10f, 0.1f, 0.1f + sin * 0.10f);
                Lighting.AddLight(projectile.Center, color.ToVector3() * 0.6f);
                Dust d = Dust.NewDustPerfect(projectile.Center, 264, -projectile.velocity * 0.5f, 0, color, 1.4f);
                d.noGravity = true;
                d.rotation = Main.rand.NextFloat(6.50f);
            }
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
                     projectile.rotation += projectile.velocity.X * 0.099f;
                }
                if (target)
                {
                    AdjustMagnitude(ref move);
                    projectile.velocity = (5 * projectile.velocity + move) / 6f;
                    AdjustMagnitude(ref projectile.velocity);
                }
            }
        }




        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 25f / magnitude;
            }
        }




        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.GetTexture(Texture);
            float sin = 1 + (float)Math.Sin(projectile.ai[1]);
            float cos = 1 + (float)Math.Cos(projectile.ai[1]);
            Color color = new Color(0.5f + cos * 0.2f, 0.8f, 0.5f + sin * 0.2f);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, tex.Frame(), color, projectile.rotation, tex.Size() / 2, projectile.scale, 0, 0);

            return false;
        }
    }
}