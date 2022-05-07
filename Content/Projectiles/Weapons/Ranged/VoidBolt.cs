using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 7;
            Projectile.height = 7;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;

        }

        private int TimeLeft = 0;


        private double GetDirection()
        {
            float degreeAngle;

            if (Projectile.direction > 0) degreeAngle = 180f;
            else degreeAngle = 0f;

            return MathHelper.ToRadians(degreeAngle);
        }



        public override void AI()
        {
            {

                float yPos = (float)Math.Sin(GetDirection());
                float xPos = (float)Math.Cos(GetDirection());
                float wobble = WaveAmplitude * (float)Math.Cos(Timer++ * WaveMagnitude) * WaveMagnitude;
                Projectile.velocity.Y += -yPos * Speed + xPos * wobble;
                Projectile.velocity.X += xPos * Speed - yPos * wobble;


                TimeLeft++;
                Projectile.ai[1] += 0.1f;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                float cos = 1 + (float)Math.Cos(Projectile.ai[1]);
                float sin = 1 + (float)Math.Sin(Projectile.ai[1]);
                Color color = new Color(0.5f + cos * 0.10f, 0.1f, 0.1f + sin * 0.10f);
                Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.6f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, 264, -Projectile.velocity * 0.5f, 0, color, 1.4f);
                d.noGravity = true;
                d.rotation = Main.rand.NextFloat(6.50f);
            }
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
                    Projectile.rotation += Projectile.velocity.X * 0.099f;
                }
                if (target)
                {
                    AdjustMagnitude(ref move);
                    Projectile.velocity = (5 * Projectile.velocity + move) / 6f;
                    AdjustMagnitude(ref Projectile.velocity);
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
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            {
                Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(Texture);
                float sin = 1 + (float)Math.Sin(Projectile.ai[1]);
                float cos = 1 + (float)Math.Cos(Projectile.ai[1]);
                Color color = new Color(0.5f + cos * 0.2f, 0.8f, 0.5f + sin * 0.2f);
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(), color, Projectile.rotation, tex.Size() / 2, Projectile.scale, 0, 0);

                return false;
            }
        }
    }
}