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
            // DisplayName.SetDefault("Shadow Clone");
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
            //Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * 0.5f);
            Player player = Main.player[(int)Projectile.ai[0]];

            if (timer < 65)
            {
                float scalespeed = (Math.Abs(Projectile.Distance(GoalPoint + player.Center)) < 275 ? 0.6f * 6f : 0.25f * 6f);

                Vector2 move = player.Center - Projectile.Center;

                Vector2 destination = (GoalPoint + player.Center);
                //
                Projectile.velocity.X = (Projectile.velocity.X + move.X + GoalPoint.X) / 20f * scalespeed;
                Projectile.velocity.Y = (Projectile.velocity.Y + move.Y + GoalPoint.Y) / 20f * scalespeed;

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

                if (timer == 65)
                {
                    glowVal = 1;
                    for (int m = 0; m < 5 + Main.rand.Next(0, 2); m++)
                    {
                        Color col = Color.SkyBlue * 1f;
                        Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.rotation.ToRotationVector2() * -20, ModContent.DustType<MuraLineDust>(),
                            Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)) * Main.rand.NextFloat(0.75f, 3.5f) * 4.3f, newColor: col, Scale: Main.rand.NextFloat(1.3f, 1.65f));
                        d.fadeIn = 0.65f;
                        d.alpha = 20;
                    }
                }
                if (timer == 75)
                {
                    SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/GloogaSlide") with { Volume = 0.15f, Pitch = 0.5f, MaxInstances = 2 }, Projectile.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .20f, Pitch = 1f, MaxInstances = 2 };
                    SoundEngine.PlaySound(style2, Projectile.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = .1f, Pitch = 1f, }; 
                    SoundEngine.PlaySound(style3, Projectile.Center);


                }

                Projectile.velocity = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.Pi) * dashSpeed;
                
                if (timer % 3 == 0)
                {
                    int a = Dust.NewDust(Projectile.Center, 12, Projectile.height, ModContent.DustType<DashTrailDust>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, new Color(0, 255, 255), 1f);
                    Main.dust[a].noLight = true;
                }

                if (timer > 85 && timer < 185)
                    dashSpeed *= 1.01f;

            }

            glowVal = Math.Clamp(MathHelper.Lerp(glowVal, -0.1f, 0.08f), 0, 1);

            if (Projectile.timeLeft > 30)
                alpha = MathHelper.Lerp(0f, 1f, Easings.easeInCirc(Math.Clamp(timer, 0, 12f) / 12f));
            else
                alpha -= 0.08f;

            //alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.2f, 0.15f), 0, 1);

            Projectile.rotation = player.Center.AngleTo(player.Center + GoalPoint);
            Projectile.spriteDirection = Projectile.direction;
            timer++;

        }

        float glowVal = 0f;
        float alpha = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            var Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/ShadowClone").Value;
            var Tex2 = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/ShadowCloneBorder").Value;

            Color drawingCol = Color.SkyBlue;
            Color drawingCol2 = Color.White;

            Vector2 vec2Scale = new Vector2(1f, 1f - (glowVal * 0.5f)) * 0.75f;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition;
                drawingCol = Color.SkyBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                drawingCol2 = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(Tex, drawPos + new Vector2(22, 22), Tex.Frame(1,1,0,0), drawingCol * 0.5f * alpha, Projectile.rotation, Tex.Size() / 2, vec2Scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(Tex2, drawPos + new Vector2(22, 22), Tex.Frame(1, 1, 0, 0), drawingCol2 * glowVal * alpha, Projectile.rotation, Tex2.Size() / 2, vec2Scale, SpriteEffects.None, 0);

            }


            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), new Color(50,50,50) * alpha, Projectile.rotation, Tex.Size() / 2, vec2Scale, SpriteEffects.None, 0f);
            
            Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), Tex.Frame(1, 1, 0, 0), Color.DeepSkyBlue with { A = 0 } * 0.5f * alpha, Projectile.rotation, Tex2.Size() / 2, vec2Scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.DeepSkyBlue * glowVal * 1f * alpha, Projectile.rotation, Tex2.Size() / 2, vec2Scale, SpriteEffects.None, 0f);

            return false;
        }

        public void SetGoalPoint(Vector2 input)
        {
            GoalPoint = input;
        }

        //!
        public override void OnKill(int timeLeft)
        {
            for (int b = 0; b < 20; b++)
                Dust.NewDust(Projectile.Center, 12, Projectile.height, ModContent.DustType<DashTrailDust>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, new Color(0, 255, 255), 1f);
        }
    }
    public class ShadowClonePink : ModProjectile
    {
        public int timer = 0;
        Vector2 GoalPoint = new Vector2(0, 350);//Vector2.Zero;
        float alpha = 0f;
        public float dashSpeed = 20;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Clone");
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
            Lighting.AddLight(Projectile.Center, Color.HotPink.ToVector3() * 0.2f);
            Player player = Main.player[(int)Projectile.ai[0]];

            float scalespeed = (Math.Abs(Projectile.Distance(GoalPoint + player.Center)) < 275 ? 0.6f * 6f : 0.25f * 6); //wtf past me

            Vector2 move = player.Center - Projectile.Center;

            Projectile.velocity.X = (((Projectile.velocity.X + move.X + GoalPoint.X) / 20f)) * scalespeed;
            Projectile.velocity.Y = (((Projectile.velocity.Y + move.Y + GoalPoint.Y) / 20f)) * scalespeed;

            //Projectile.rotation = Projectile.AngleTo(player.Center) + MathHelper.Pi;
            //Projectile.spriteDirection = Projectile.direction;


            Projectile.rotation = player.Center.AngleTo(player.Center + GoalPoint);
            Projectile.spriteDirection = Projectile.direction;
            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            var Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/ShadowClonePink").Value;
            var Tex2 = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/ShadowCloneBorder").Value;

            Color drawingCol = Color.HotPink;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition;
                drawingCol = Color.HotPink * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(Tex, drawPos + new Vector2(22, 22), Tex.Frame(1, 1, 0, 0), drawingCol with { A = 0 } * 0.5f, Projectile.rotation, Tex.Size() / 2, 0.75f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, 0.75f, SpriteEffects.None, 0f);

            float rot = (float)Main.timeForVisualEffects * 0.12f;
            float offsetVal = MathF.Sin((float)Main.timeForVisualEffects * 0.14f) * 0.5f;

            Vector2 v = new Vector2(1.5f + offsetVal * 2, 0) + Main.rand.NextVector2Circular(2f, 2f);

            float scale1 = (1f + (MathF.Sin((float)Main.timeForVisualEffects * 0.08f) * 0.025f)) * 0.75f;
            float scale2 = (1f + (MathF.Cos((float)Main.timeForVisualEffects * 0.02f) * 0.035f)) * 0.75f;//scale * 1f;

            Main.spriteBatch.Draw(Tex2, Projectile.Center + v.RotatedBy(rot - MathHelper.PiOver2) - Main.screenPosition, null, Color.Pink with { A = 0 } * 0.5f, Projectile.rotation, Tex2.Size() / 2, scale1, 0, 0f);

            Main.spriteBatch.Draw(Tex2, Projectile.Center + v.RotatedBy(rot) - Main.screenPosition, null, Color.DeepPink with { A = 0 } * 0.5f, Projectile.rotation, Tex2.Size() / 2, scale1, 0, 0f);
            Main.spriteBatch.Draw(Tex2, Projectile.Center + v.RotatedBy(rot + MathHelper.PiOver2) - Main.screenPosition, null, Color.Pink with { A = 0 } * 0.5f, Projectile.rotation, Tex2.Size() / 2, scale1, 0, 0f);
            Main.spriteBatch.Draw(Tex2, Projectile.Center + v.RotatedBy(rot + MathHelper.Pi) - Main.screenPosition, null, Color.DeepPink with { A = 0 } * 0.5f, Projectile.rotation, Tex2.Size() / 2, scale1, 0, 0f);




            return false;
        }

        public void SetGoalPoint(Vector2 input)
        {
            GoalPoint = input;
        }
    }
} 