using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Dusts;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns
{   
    public class BunnyShot : ModProjectile
    {
        public int timer = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 500;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {

            if (timer > 5)
                Projectile.velocity.X *= .99f;
            Projectile.velocity.Y += 0.15f;


            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            if (timer % 4 == 0 && Main.rand.NextBool())
            {
                //Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * -6f;
                //int a = Dust.NewDust(Projectile.position, 15, 15, ModContent.DustType<ColorSpark>(), vel.X, vel.Y, 0, Color.OrangeRed, 0.4f);
                

            }

            if (true)
            {
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * -3f;

                Dust c = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), vel.RotatedByRandom(0.25f), 0, Color.Orange, 0.8f);
                c.alpha = 2;
                c.noLight = true;

                if (Main.rand.NextBool())
                {
                    Dust a = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelRise>(), vel.RotatedByRandom(0.5f), 0, Color.Lerp(Color.Orange, Color.OrangeRed, 0.25f), 0.5f);
                    a.alpha = 2;
                    a.noLight = true;
                    //a.fadeIn = 25;
                }

                if (Main.rand.NextBool(2))
                {



                    //Dust b = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), vel.RotatedByRandom(0.15f), 0, Color.OrangeRed, 0.5f);
                    //b.alpha = 2;
                    //Dust.NewDust(Projectile.position, 15, 15, ModContent.DustType<SmokeDustFade>(), vel.X, vel.Y, 0, Color.Orange, 0.65f);
                    //Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SmokeDustFade>(), vel.RotatedByRandom(0.15f), 0, Color.OrangeRed, 0.35f);
                }
            }

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/RainbowRod");
            Texture2D glow2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/DiamondGlow");

            Vector2 scalee = new Vector2(1, 0.5f) * Projectile.scale * 1.5f;
            Vector2 scalee2 = new Vector2(0.25f, 1) * Projectile.scale * 0.5f;

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.spriteBatch.Draw(glow2, Projectile.Center - Main.screenPosition, null, Color.OrangeRed, Projectile.velocity.ToRotation() - MathHelper.PiOver2, glow2.Size() / 2, scalee2, 0, 0f);
            
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 }, Projectile.velocity.ToRotation(), glow.Size() / 2, scalee, 0, 0f);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.Orange with { A = 0 }, Projectile.velocity.ToRotation(), glow.Size() / 2, scalee * 0.8f, 0, 0f);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.velocity.ToRotation(), glow.Size() / 2, scalee * 0.5f, 0, 0f);


            return false;
        }


        public override void Kill(int timeLeft)
        {


        }

    }

    public class VFXTest : ModProjectile
    {
        public int timer = 0;
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 500;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.4f;


            if (timer < 30 && timer > 10)
                Projectile.velocity *= 1.1f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            int simpsons = timer < 20 ? 3 : 1 ;
            if (timer % simpsons == 0)
            {
                Vector2 vel = Projectile.velocity * -0.2f;
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowStrong>(), vel.X, vel.Y, 3, Color.Green, 0.35f);
            }

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Base = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/Tomes/CrystalGladeProj");
            Texture2D GlowMask = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/Tomes/CrystalGladeGlow");
            Texture2D Glow2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/Tomes/CrystalGladeGlow2");
            Texture2D Glow3 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/Tomes/CrystalGladeGlow3");

            Vector2 vscale = new Vector2(1f, 1f - (Projectile.velocity.Length() * 0.00f)) * Projectile.scale;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = 1f - ((float)i / Projectile.oldPos.Length);

                Main.spriteBatch.Draw(Base, Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition, null, lightColor with { A = 0 } * progress * 0.85f, Projectile.rotation, Base.Size() / 2, vscale, 0, 0f);
                Main.spriteBatch.Draw(Glow3, Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition, null, Color.DarkGreen with { A = 0 }, Projectile.oldRot[i], Glow3.Size() / 2f, vscale * 0.8f * progress, 0f, 0f);
            }

            Main.spriteBatch.Draw(Base, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Base.Size() / 2, vscale, 0, 0f);
            Main.spriteBatch.Draw(GlowMask, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, GlowMask.Size() / 2, vscale, 0, 0f);

            Main.spriteBatch.Draw(Glow2, Projectile.Center - Main.screenPosition, null, Color.Green with { A = 0 } * (Projectile.velocity.Length() * 0.08f), Projectile.rotation, Glow2.Size() / 2, vscale, 0, 0f);
            Main.spriteBatch.Draw(Glow3, Projectile.Center - Main.screenPosition, null, Color.Green with { A = 0 } * (Projectile.velocity.Length() * 0.04f), Projectile.rotation, Glow3.Size() / 2, vscale, 0, 0f);


            return false;
        }


        public override void Kill(int timeLeft)
        {


        }

    }

    public class VFXTest2 : ModProjectile
    {
        public int timer = 0;
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 500;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {

            if (timer > 5)
                Projectile.velocity.X *= .98f;
            Projectile.velocity.Y += 0.15f;


            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            if (timer % 4 == 0 && Main.rand.NextBool())
            {
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * -6f;
                int a = Dust.NewDust(Projectile.position, 15, 15, ModContent.DustType<ColorSpark>(), vel.X, vel.Y, 0, Color.OrangeRed, 0.4f);
                ;

            }

            if (true)
            {
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * -3f;

                Dust c = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SmokeDust>(), vel.RotatedByRandom(0.25f), 0, Color.Orange, 0.8f);
                c.alpha = 2;

                Dust a = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SmokeDust>(), vel.RotatedByRandom(0.5f), 0, Color.OrangeRed, 0.5f);
                a.alpha = 2;
                if (Main.rand.NextBool(2))
                {



                    Dust b = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MuraLineDust>(), vel.RotatedByRandom(0.15f), 0, Color.OrangeRed, 0.75f);
                    b.alpha = 2;
                    //Dust.NewDust(Projectile.position, 15, 15, ModContent.DustType<SmokeDustFade>(), vel.X, vel.Y, 0, Color.Orange, 0.65f);
                    //Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SmokeDustFade>(), vel.RotatedByRandom(0.15f), 0, Color.OrangeRed, 0.35f);
                }
            }

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/BunnyShot");

            //Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, star.Size() / 2, Projectile.scale, 0, 0f);

            return false;
        }


        public override void Kill(int timeLeft)
        {


        }

    }

}