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
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
    public class Cyver2EnergyBall : ModProjectile
    {
        public int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Ball");
            Main.projFrames[Projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.damage = 80;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool? CanDamage()
        {
            if (timer < 25)
                return false;
            return true;
        }

        public override void AI()
        {
            //Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.9f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.7f / 255f);
            Lighting.AddLight(Projectile.Center, Color.DeepPink.ToVector3() * 0.02f);
            Projectile.rotation = MathHelper.ToRadians(180) + Projectile.velocity.ToRotation();
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            //float approaching = ((50f - Projectile.timeLeft) / 200);
            Lighting.AddLight(Projectile.Center, 0.5f, 0.65f, 0.75f);

            Player player = Main.player[(int)Projectile.ai[0]];
            //Dust d = Dust.NewDustPerfect(Projectile.position + new Vector2(-4,-4), DustID.Electric, Scale: 0.75f);
            //d.noGravity = true;
            //d.velocity += Projectile.velocity;
            //d.velocity *= 0.1f;
            //d.scale *= 0.7f;

            int dust = Dust.NewDust(Projectile.Center + new Vector2(0, -4), 0, 0, DustID.Electric, 0, 0, Projectile.alpha, default, 0.5f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity += Projectile.velocity * 1.25f;
            Main.dust[dust].velocity *= 0.1f;
            Main.dust[dust].scale *= 0.7f;
            if (player.active)
            {
                //float x = Main.rand.Next(-10, 11) * 0.01f * approaching;
                //float y = Main.rand.Next(-10, 11) * 0.01f * approaching;
                Vector2 toPlayer = Projectile.Center - player.Center;
                toPlayer = toPlayer.SafeNormalize(Vector2.Zero);
                Projectile.velocity += -toPlayer * (0.455f) * (0.6f * MathHelper.Clamp((Projectile.timeLeft / 200), 1, 20));// + new Vector2(x, y);
            }

            if (timer < 30)
                Projectile.velocity *= 0.2f;
            timer++;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item94 with { Pitch = 0.4f, Volume = 0.45f, PitchVariance = 0.2f }, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0, Main.myPlayer);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 bonus = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5f;
            //Draw the Circlular Glow
            var Tex = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + bonus, Tex.Frame(1, 1, 0, 0), Color.DeepPink * 0.5f, Projectile.rotation, Tex.Size() / 2, 0.91f, SpriteEffects.None, 0f);


            var BallTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/EnergyBall").Value;

            int frameHeight = BallTexture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, BallTexture.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, sourceRectangle, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }

    }
} 