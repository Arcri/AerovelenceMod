using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;
using System.Collections.Generic;
using System;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class HugeLaserProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 9000;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 500;
            Projectile.penetrate = -1;

            Projectile.extraUpdates = 1;
        }


        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        int timer = 0;

        public override void AI()
        {
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/s06sBloom").Value;
            trail1.trailColor = Color.White * 0.7f;
            trail1.trailPointLimit = 200;
            trail1.trailWidth = 200;
            trail1.trailMaxLength = 10000;
            trail1.timesToDraw = 2;
            trail1.pinch = true;
            trail1.pinchAmount = 0.8f;

            trail1.trailTime = timer * 0.01f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center;
            trail1.TrailLogic();

            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
            trail2.trailColor = Color.DeepSkyBlue;
            trail2.trailPointLimit = 200;
            trail2.trailWidth = 200;
            trail2.trailMaxLength = 10000;
            trail2.timesToDraw = 2;
            trail2.pinch = true;
            trail2.pinchAmount = 0.8f;

            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/LoopingThunderGrad").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = timer * 0.02f;

            trail2.trailTime = timer * 0.02f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center;
            trail2.TrailLogic();

            timer++;

            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * 1.25f);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            trail1.trailTime = (float)Main.timeForVisualEffects * 0.01f;

            trail2.gradientTime = (float)Main.timeForVisualEffects * 0.02f;
            trail2.trailTime = (float)Main.timeForVisualEffects * 0.03f;

            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            return false;
        }
    }

    public class HugeLaserTelegraph : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

        public Vector2 endPoint;
        public float Rotation = 0;

        public bool sweepTell = false;
        public bool sweepDir = false;

        float rotOffset = 0f;

        public bool custom = false;
        public int timeToLast = 0;

        int timer = 0;

        public NPC NPCTetheredTo = null;
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 110 + 5;
            Projectile.hide = true;
        }
        public override bool? CanDamage() { return false; }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void AI()
        {
            if (timer == 0)
            {
                Rotation = Projectile.velocity.ToRotation();
                rotOffset = Rotation;
            }


            if (NPCTetheredTo != null)
            {
                if (NPCTetheredTo.active == false)
                    Projectile.active = false;
                if (sweepTell)
                {
                    uColorIntensity = 0.9f;
                    Projectile.Center = NPCTetheredTo.Center + NPCTetheredTo.rotation.ToRotationVector2() * -30;
                    Rotation += 0.06f * (sweepDir ? 1 : -1);// + (timer * 0.002f);
                }
                else
                {
                    Projectile.Center = NPCTetheredTo.Center;
                    Rotation = NPCTetheredTo.rotation + MathHelper.Pi + rotOffset;
                }


            }
            endPoint = Projectile.Center + Rotation.ToRotationVector2() * 2500f;
            Projectile.velocity = Vector2.Zero;

            Projectile.ai[0] = Math.Clamp(MathHelper.Lerp(0f, 1f, Easings.easeInQuad(timer / 15f)), 0f, 1f);
            timer++;
        }

        public float uColorIntensity = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(new Color(85, 200, 255).ToVector3() * 1.5f);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.3f);
            myEffect.Parameters["uSaturation"].SetValue(1.2f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //myEffect.CurrentTechnique.Passes[0].Apply();

            if (timer > 0)
            {
                var texBeam = Mod.Assets.Request<Texture2D>("Assets/ThinLineGlowClear").Value;

                Vector2 origin2 = new Vector2(0, texBeam.Height / 2);

                float height = 30f * Projectile.scale * (sweepTell ? 0.5f : 1);
                float height2 = 15f * Projectile.scale * (sweepTell ? 0.5f : 1); 

                if (height == 0)
                    Projectile.active = false;

                int width = (int)(Projectile.Center - endPoint).Length() - 24;

                var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(Rotation) * 24;
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1f));
                var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height2 * 1f));



                Main.spriteBatch.Draw(texBeam, target, null, Color.Aqua * Projectile.ai[0], Rotation, origin2, 0, 0);

            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}