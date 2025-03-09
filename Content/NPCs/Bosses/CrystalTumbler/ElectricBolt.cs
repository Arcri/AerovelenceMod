using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Audio;
using Terraria.ID;
using AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class ElectricBolt : TrailProjBase
    {
        private int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        BaseTrailInfo trail1 = new();
        BaseTrailInfo trail2 = new();

        public override void AI()
        {
            float lifeRatio = (float)Projectile.timeLeft / 240;
            timer++;

            int maxTrailLength1 = 600;
            int maxTrailPoints1 = 600;

            int maxTrailLength2 = 500;
            int maxTrailPoints2 = 500;

            if (Projectile.ai[0] == 1)
            {
                

                if(timer >= 50)
                {
                    Projectile.tileCollide = true;

                    maxTrailLength1 = 1600;
                    maxTrailPoints1 = 1600;

                    maxTrailLength2 = 1500;
                    maxTrailPoints2 = 1500;
                }
            }
            else
            {
                maxTrailLength1 = 600;
                maxTrailPoints1 = 600;

                maxTrailLength2 = 500;
                maxTrailPoints2 = 500;

                Projectile.tileCollide = false;
                Projectile.velocity *= 0.98f;

                trail1.trailMaxLength = (int)(maxTrailLength1 * lifeRatio);
                trail1.trailPointLimit = (int)(maxTrailPoints1 * lifeRatio);

                trail2.trailMaxLength = (int)(maxTrailLength2 * lifeRatio);
                trail2.trailPointLimit = (int)(maxTrailPoints2 * lifeRatio);
            }

            trail1.trailMaxLength = (int)(maxTrailLength1 * lifeRatio);
            trail1.trailPointLimit = (int)(maxTrailPoints1 * lifeRatio);

            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/RealLightning").Value;
            trail1.trailColor = Color.White * 1f;
            trail1.trailWidth = 60;
            trail1.timesToDraw = 1;
            trail1.pinch = true;
            trail1.pinchAmount = 0.55f;

            trail1.trailTime = timer * 0.01f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center;
            trail1.TrailLogic();

            trail2.trailMaxLength = (int)(maxTrailLength2 * lifeRatio);
            trail2.trailPointLimit = (int)(maxTrailPoints2 * lifeRatio);

            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
            trail2.trailColor = Color.Wheat;
            trail2.trailWidth = 30;
            trail2.timesToDraw = 2;
            trail2.pinch = true;
            trail2.pinchAmount = 0.55f;

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/LoopingThunderGrad").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = timer * 0.01f;

            trail2.trailTime = timer * 0.02f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center;
            trail2.TrailLogic();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_hurt_1") with { Pitch = .4f, MaxInstances = -1 };
            SoundEngine.PlaySound(style, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item93 with { Pitch = 0.4f, Volume = 0.2f, MaxInstances = -1 }, Projectile.Center);
            for (int i = 0; i < 6; i++)
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(2.25f, 2.25f) * Main.rand.Next(1, 3);
                dustVel += Projectile.velocity * 0.3f;

                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.2f, 0.35f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, timeBeforeSlow: 5,
                    preSlowPower: 0.94f, postSlowPower: 0.90f, velToBeginShrink: 1f, fadePower: 0.92f, shouldFadeColor: false);
            }
            if(Projectile.ai[0] == 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<ElectricBoltExplosion>(), 19, 1f);
            }
        }
    }

    public class ElectricBoltExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage() { return timer < 4; }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ManaLeech>(), 300);
        }

        public override void AI()
        {
            if (timer == 0)
                Projectile.rotation = Main.rand.NextFloat(6.28f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                if (Projectile.frame == 6)
                    Projectile.active = false;

                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3() * 1f);
            timer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Explo = Mod.Assets.Request<Texture2D>("Assets/HitAnims/BlueFlareDarkGlowPMA").Value;
            int frameHeight = Explo.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Color glowColor = Color.DeepSkyBlue;
            glowColor.A = 0;
            Color glowColor2 = Color.White;
            glowColor2.A = 0;
            Rectangle sourceRectangle = new(0, startY, Explo.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 scale12 = new(1f, 1f);
            Main.spriteBatch.Draw(Explo, Projectile.Center - Main.screenPosition, sourceRectangle, Color.Black * 0.4f, Projectile.rotation, origin, scale12, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Explo, Projectile.Center - Main.screenPosition, sourceRectangle, glowColor2, Projectile.rotation, origin, scale12, SpriteEffects.None, 0f);
            return false;
        }
    }
}
