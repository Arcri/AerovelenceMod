using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
    public class ShadowClone : ModProjectile
    {
        public int timer = 0;
        Vector2 GoalPoint = new Vector2(0, 350);//Vector2.Zero;
        float storedRotation = 0;

        public float dashSpeed = 20;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Clone");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.scale = 0.75f;
            Projectile.timeLeft = 200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;

        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * 0.2f);
            Player player = Main.player[(int)Projectile.ai[0]];

            if (timer < 65)
            {
                float scalespeed = (Math.Abs(Projectile.Distance(GoalPoint + player.Center)) < 275 ? 0.6f * 10f : 0.25f * 10);

                Vector2 move = player.Center - Projectile.Center;

                Vector2 destination = (GoalPoint + player.Center);
                //
                Projectile.velocity.X = (((Projectile.velocity.X + move.X + GoalPoint.X) / 20f)) * scalespeed;
                Projectile.velocity.Y = (((Projectile.velocity.Y + move.Y + GoalPoint.Y) / 20f)) * scalespeed;

                Projectile.rotation = Projectile.AngleTo(player.Center) + MathHelper.Pi;
                Projectile.spriteDirection = Projectile.direction;
            }
            
            if (timer == 65)
            {
                storedRotation = (player.Center - GoalPoint).ToRotation();
                //SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = .15f, MaxInstances = -1, Volume = 0.4f };
                //SoundEngine.PlaySound(style, Projectile.Center);

            }

            if (timer >= 65)
            {
                if (timer == 75)
                {
                    SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/TetraSlide") with { Volume = 0.10f, Pitch = 0.2f, MaxInstances = 4 }, Projectile.Center);


                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .15f, Pitch = 1f, MaxInstances = 4 };
                    SoundEngine.PlaySound(style2, Projectile.Center);
                }

                Projectile.velocity = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.Pi) * dashSpeed;
                
                if (timer % 3 == 0)
                {
                    Dust.NewDust(Projectile.Center, 12, Projectile.height, ModContent.DustType<DashTrailDust>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, new Color(0, 255, 255), 1f);

                }


            }

            Projectile.rotation = player.Center.AngleTo(player.Center + GoalPoint);
            Projectile.spriteDirection = Projectile.direction;
            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            var Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/ShadowClone").Value;

            Color drawingCol = Color.SkyBlue;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition;
                drawingCol = Color.SkyBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(Tex, drawPos + new Vector2(22,22), Tex.Frame(1,1,0,0), drawingCol * 0.5f, Projectile.rotation, Tex.Size() / 2, 0.75f, SpriteEffects.None, 0);
            }


            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, 0.75f, SpriteEffects.None, 0f);
            return false;
        }

        public void SetGoalPoint(Vector2 input)
        {
            GoalPoint = input;
        }
    }
} 