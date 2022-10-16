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
        Vector2 GoalPoint = new Vector2(0, 500);//Vector2.Zero;
        float storedRotation = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Clone");

        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 400;
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
            }
            
            if (timer == 65)
            {
                storedRotation = (player.Center - GoalPoint).ToRotation() - MathHelper.PiOver2;
            }

            if (timer >= 65)
            {
                Projectile.velocity = storedRotation.ToRotationVector2() * 20;
                Dust.NewDust(Projectile.Center, 12, Projectile.height, ModContent.DustType<DashTrailDust>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, new Color(0, 255, 255), 1f);

            }


            Projectile.rotation = Projectile.AngleTo(player.Center) + MathHelper.Pi;
            Projectile.spriteDirection = Projectile.direction;

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {

            //Draw the Circlular Glow
            var Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/ShadowClone").Value;
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public void SetGoalPoint(Vector2 input)
        {

        }
    }
} 