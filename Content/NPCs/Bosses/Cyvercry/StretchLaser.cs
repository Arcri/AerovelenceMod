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
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.SkillStrikes;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class StretchLaser : ModProjectile
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
        }
        public int enemiesHit = 0;
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 2;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

        }
        public int accelerateTime = 60;
        public float accelerateStrength = 1.038f;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            if (timer < accelerateTime)
                Projectile.velocity *= accelerateStrength;

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            
            if (timer == 0)
                return false;

            //Draw the Circlular Glow
            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulseShot").Value;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX)), Tex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation, Tex.Size() / 2, new Vector2(0.5f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition , softGlow.Frame(1, 1, 0, 0), Color.Pink, Projectile.rotation, softGlow.Size() / 2, new Vector2(0.5f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);

            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 2.2f); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX), Tex.Frame(1, 1, 0, 0), Color.Crimson, Projectile.rotation, Tex.Size() / 2, new Vector2(0.25f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);





            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //return base.Colliding(projHitbox, targetHitbox);
            Player Player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + Projectile.velocity * 30, 10, ref point);
        }
    }

    public class FocusedLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyver Laser");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 78;
        }
        public float LaserRotation = 0;
        public float laserWidth = 70;
        int timer = 0;
        public float direction = 0;
        public Vector2 endPoint;
        public int parentIndex = 0;

        public override void AI()
        {
            NPC parent = Main.npc[parentIndex];

            if (!parent.active)
            {
                Projectile.active = false;
            }

            direction = parent.rotation;
            Projectile.Center = parent.Center + (parent.rotation.ToRotationVector2() * -96);
            endPoint = Projectile.Center + (parent.rotation.ToRotationVector2() * -1200);
            LaserRotation = direction + MathHelper.Pi;

            //Dust.NewDustPerfect(Projectile.Center, DustID.AmberBolt);
            //Dust.NewDustPerfect(endPoint, DustID.AmberBolt);

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D LaserTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/FlashLightBeam").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/RimeLaser", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * 0.4f);
            myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
            myEffect.Parameters["uTime"].SetValue(timer * -0.01f); //0.006
            myEffect.Parameters["uSaturation"].SetValue(2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            Vector2 origin2 = new Vector2(0, LaserTexture.Height / 2);

            float height = (laserWidth * 1.8f); //25

            int width = (int)(Projectile.Center - endPoint).Length();

            var pos = Projectile.Center - Main.screenPosition;
            var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.7f));

            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);

            //Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.8f));
            var target3 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

            Main.spriteBatch.Draw(LaserTexture, target2, null, Color.DeepPink * 0.5f, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(LaserTexture, target3, null, Color.DeepPink * 0.25f, LaserRotation, origin2, 0, 0);

            //Flares

            Effect myEffect2 = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect2.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 1.5f);
            myEffect2.Parameters["uTime"].SetValue(2);
            myEffect2.Parameters["uOpacity"].SetValue(0.7f);
            myEffect2.Parameters["uSaturation"].SetValue(0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect2, Main.GameViewMatrix.TransformationMatrix);

            myEffect2.CurrentTechnique.Passes[0].Apply();
            Texture2D flare1 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_1").Value;
            Texture2D flare12 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_12").Value;

            Main.spriteBatch.Draw(flare12, Projectile.Center - Main.screenPosition, flare12.Frame(1, 1, 0, 0), Color.HotPink, timer * 0.05f, flare12.Size() / 2, 0.2f * laserWidth * 0.02f, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(flare1, endPoint - Main.screenPosition, flare1.Frame(1, 1, 0, 0), Color.HotPink, timer * 0.07f, flare1.Size() / 2, 0.01f * laserWidth, SpriteEffects.None, 0.0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = LaserRotation.ToRotationVector2();
            float point = 0f;

            if (laserWidth > 5)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                    endPoint, 30, ref point);
            }

            return false;
        }
    }

    public class SplittingLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyver Laser");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 2;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 90; //125
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

        }
        int timer = 0;
        public override void AI()
        {
            
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            if (timer < 20) //20
                Projectile.velocity *= 1.088f;


            if (timer > 10 && timer % 10 == 0) // > 10
            {
                int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CyverLaserBomb>(), 20, 0);
                Main.projectile[a].rotation = Projectile.rotation;

                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                for (int i = 0; i < 360; i += 20)
                {
                    Vector2 circular = new Vector2(40, 0).RotatedBy(MathHelper.ToRadians(i));
                    Vector2 dustVelo = -circular * 0.1f;

                    Dust b = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + circular, ModContent.DustType<GlowCircleDust>(), Vector2.Zero, Color.DeepPink, 0.3f, 0.6f, 0f, dustShader);
                }
            }

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {

            if (timer == 0)
                return false;

            //Draw the Circlular Glow
            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulseShot").Value;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX)), Tex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation, Tex.Size() / 2, new Vector2(0.66f, Projectile.velocity.Length() * 0.1f), SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), Color.Pink, Projectile.rotation, softGlow.Size() / 2, new Vector2(0.5f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);

            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 2.7f); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX), Tex.Frame(1, 1, 0, 0), Color.Crimson, Projectile.rotation, Tex.Size() / 2, new Vector2(0.45f, Projectile.velocity.Length() * 0.1f), SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //return base.Colliding(projHitbox, targetHitbox);
            Player Player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + Projectile.velocity * 30, 10, ref point);
        }
    }

}