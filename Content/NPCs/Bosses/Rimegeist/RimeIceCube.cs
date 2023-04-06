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

namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class RimeIceCube : ModProjectile
    {
        public int timer = 0;
        public int index = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Cube");
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.timeLeft = 4000;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {

            //Projectile.velocity = Vector2.One;
            if (timer == 100)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/deerclops_ice_attack_2") with { Volume = .40f, Pitch = .85f, PitchVariance = 0.2f };
                SoundEngine.PlaySound(style, Projectile.Center);


                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_1") with { Volume = 0.6f, Pitch = .85f, MaxInstances = -1};
                SoundEngine.PlaySound(style2, Projectile.Center);
                //SoundEngine.PlaySound(style2, Projectile.Center);

                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(2, 2).RotatedBy(MathHelper.PiOver2 * i),
                        ModContent.ProjectileType<HomingIceBolt>(), Projectile.damage, 2);
                }
                Projectile.active = false;
            }

            //Projectile.rotation += 0.2f;
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Rimegeist/RimeIceCube").Value;

            Vector2 aeOffset = new Vector2(Tex.Width / 2, Tex.Height / 2);

            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 1f);

            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 1f) * 0.3f + 0.7f;
            Color effectColor = Color.SkyBlue;
            effectColor = effectColor * 0.1f * scale;
            for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
            {
                Main.spriteBatch.Draw(Tex, Projectile.Center + (TwoPi * num5).ToRotationVector2() * (6f + offset * 1.5f) - Main.screenPosition + aeOffset, Tex.Frame(1, 1, 0, 0), effectColor, 0f, Tex.Size(), 1f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, 1f, SpriteEffects.None, 0f);

            /*
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            */
            return false;
        }

    }
    public class HomingIceBolt : ModProjectile
    {
        public int timer = 0;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 200;
        }
        public override void AI()
        {

            if (timer < 70)
            {
                Projectile.velocity *= 0.98f;

                float amountToRot = MathHelper.ToRadians(Projectile.velocity.Length() * 25f);
                if (Projectile.velocity.X > 0)
                {
                    Projectile.rotation += amountToRot;
                } 
                else
                {
                    Projectile.rotation -= amountToRot;
                }
            }

            if (timer >= 70)
            {
                if (timer == 70)
                {

                    Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
                }

                float velMultiplier = MathHelper.Clamp(MathHelper.Lerp((timer - 70), 27, 0.02f), 0, 23);
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * velMultiplier;
            }

            if (Projectile.velocity.Length() > 3)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 255);
                dust.noGravity = true;
                dust.scale = 0.75f;
            }
            timer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Rimegeist.AssetDirectory + "IceBolt_Glowmask");
            Vector2 drawPos = Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition;
            //keep an eye on the width and height when doing this. It matters
            Main.EntitySpriteDraw
            (
                texture,
                drawPos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                Projectile.rotation,
                texture.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None, //adjust this according to the sprite
                0
                );
        }
    }
}