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

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns
{   
    //Big Laser
    public class AdamantitePulseShot : ModProjectile
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        { 
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 1000;
        }

        public const int MAX_PENETRATION = 5;
        public int enemiesHit = 0;
        public bool big = false;

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 3;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 110;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            if (timer < 60)
                Projectile.velocity *= 1.03f;

            int modVal = big ? 4 : 6;

            if (timer > 2 && timer % modVal == 0 && Main.rand.NextBool(2))
            {
                //My old shader dust system is yucky and stupid so replace this when i redo it

                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                float scaleBonus = big ? 0.1f : 0f;

                int gd = GlowDustHelper.DrawGlowDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowCircleFlare>(),
                    new Color(255, 30, 30), 0.4f + Main.rand.NextFloat(0.2f) + scaleBonus, 0.55f, 0f, dustShader2);

                Main.dust[gd].velocity += Projectile.velocity.RotateRandom(0.05f);

            }


            //Opacity
            if (timer > 4)
                Projectile.ai[0] = Math.Clamp(MathHelper.Lerp(Projectile.ai[0], big ? 1.3f : 1f, 0.04f), 0, big ? 1.1f : 0.8f); //1.1f

            Lighting.AddLight(Projectile.Center, new Color(255, 20, 20).ToVector3() * 0.8f * Projectile.ai[0]);

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0)
                return false;

            var softGlow = Mod.Assets.Request<Texture2D>("Assets/DiamondGlow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulseShot").Value;

            Vector2 vscale = new Vector2(0.5f, Projectile.velocity.Length() * 0.15f) * Projectile.ai[0];
            Vector2 vscale2 = new Vector2(0.25f, Projectile.velocity.Length() * 0.15f) * Projectile.ai[0];
            Vector2 vscale3 = new Vector2(0.5f, Projectile.velocity.Length() * 0.3f) * Projectile.ai[0];

            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), Color.Black * 0.25f, Projectile.rotation, softGlow.Size() / 2, vscale3 * 0.85f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX)), Tex.Frame(1, 1, 0, 0), Color.Red * 0.85f, Projectile.rotation, Tex.Size() / 2, vscale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), Color.Red * 0.75f, Projectile.rotation, softGlow.Size() / 2, vscale3, SpriteEffects.None, 0f);

            //Set up glowy shader 
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.Red.ToVector3() * 3.2f);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX), Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, vscale2, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            //Inner
            /*
            int maxDust = 15 + Main.rand.Next(2) + (big ? 2 : 0);
            for (int i = 0; i < maxDust; i++)
            {
                float dustScale = 0.6f + Main.rand.NextFloat(0, 0.2f) + (big ? 0.1f : 0f);

                float progress = (i + 1) / maxDust;

                float velMult = progress > 0.8f ? Main.rand.NextFloat(0.55f, 1f) : Main.rand.NextFloat(0.4f, 0.75f);// + (0.2f * progress);

                Vector2 baseVel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * (10f * velMult) * (Easings.easeOutQuad(velMult));

                Vector2 dustVel = baseVel.RotatedBy((1 - velMult) * 0.7f * (Main.rand.NextBool() ? 1 : -1)).RotateRandom(0.01f);

                int gd = GlowDustHelper.DrawGlowDust(target.Center, 1, 1, ModContent.DustType<GlowCircleFlare>(),
                    new Color(255, 30, 30), dustScale, 0.6f, 0f, dustShader);
                Main.dust[gd].velocity = dustVel * Main.rand.NextFloat(0.95f, 1.05f);
                Main.dust[gd].noLight = true;
                Main.dust[gd].alpha = 0;

            }
            */
            float dustScale1 = 0.85f + Main.rand.NextFloat(0, 0.15f) + (big ? 0.1f : 0f);
            float dustScale2 = 0.6f + Main.rand.NextFloat(0, 0.10f) + (big ? 0.1f : 0f);

            int maxDust1 = 3 + (big ? 1 : 0);
            int maxDust2 = 5 + (big ? 2 : 0);

            //help
            for (int i = enemiesHit; i < maxDust1; i++) 
            {
                float side = (i + 1) > maxDust1 / 2 ? 1f : -1f;

                float velVal = Main.rand.NextFloat(6, 9);
                float bonus = 0.1f * (1 - ((velVal - 6) * 0.33f));

                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(0.1f * side) + bonus) * velVal;

                Dust p = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleFlare>(),
                    vel, new Color(255, 30, 30), dustScale1, 0.6f, 0f, dustShader);
                p.fadeIn = Main.rand.NextBool() ? 1 : 0;
                p.noLight = true;
                p.velocity += Projectile.velocity * 0.1f;

            }
            for (int i = enemiesHit; i < maxDust2; i++) 
            {
                float side = (i + 1) > maxDust2 / 2 ? 1f : -1f;

                float velVal = Main.rand.NextFloat(3, 6);
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(0.6f * side) + (0.25f * side)) * velVal;

                Dust p = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleFlare>(),
                    vel.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.75f, 1.1f), new Color(255, 30, 30), dustScale2, 0.6f, 0f, dustShader);
                p.fadeIn = Main.rand.NextBool() ? 1 : 0;
                p.noLight = true;
                p.velocity += Projectile.velocity * 0.1f;

            }

            for (int i = 0; i < 6; i++)
            {
                float dustScale = 0.2f + Main.rand.NextFloat(0.15f);

                ColorSparkBehavior extraInfo = new ColorSparkBehavior();
                Vector2 vel = Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.75f, 1.25f);

                Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<ColorSpark>(), vel, 50 + Main.rand.Next(-2, 5), Color.Red, dustScale);
                extraInfo.gravityIntensity = 0f;
                d.fadeIn = Main.rand.NextFloat(0.5f, 1f);
                d.customData = extraInfo;
            }

            //Only skill strike first two hits
            if (enemiesHit > 1 && Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike == true)
                Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;

            Projectile.damage = (int)(Projectile.damage * 0.8f);
            enemiesHit++;
        }

        public override bool? CanDamage()
        {
            if (enemiesHit >= MAX_PENETRATION)
                return false;
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player Player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;

            Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulseShot").Value;
            float scale = ((Projectile.velocity * (timer > 60 ? 1f : 1.03f)).Length() * 0.15f);

            Vector2 tip = new Vector2(Tex.Height / 3, 0f).RotatedBy(Projectile.velocity.ToRotation()) * scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + tip * -1,
                Projectile.Center + tip, 10, ref point);
        }
    }

    public class AdamSmallShot : ModProjectile
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 4;
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 400;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            if (timer > 2 && timer % 8 == 0 && Main.rand.NextBool(2))
            {
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                int gd = GlowDustHelper.DrawGlowDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowCircleFlare>(),
                new Color(255, 20, 20), 0.2f + Main.rand.NextFloat(0.15f), 0.6f, 0f, dustShader2);

                Main.dust[gd].velocity += Projectile.velocity;

            }

            //Opacity
            if (timer > 0)
                Projectile.ai[0] = Math.Clamp(MathHelper.Lerp(Projectile.ai[0], 1.2f, 0.08f), 0, 1);

            Lighting.AddLight(Projectile.position, new Color(255, 20, 20).ToVector3() * 0.8f * Projectile.ai[0]);

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulseShot").Value;

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -40, Tex.Frame(1, 1, 0, 0), Color.Black * 0.2f * Projectile.ai[0], Projectile.rotation, Tex.Size() / 2, new Vector2(0.3f * Projectile.ai[0], 0.25f), SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -40, Tex.Frame(1, 1, 0, 0), Color.Red * 0.85f * Projectile.ai[0], Projectile.rotation, Tex.Size() / 2, new Vector2(0.35f * Projectile.ai[0], 0.3f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -10), softGlow.Frame(1, 1, 0, 0), Color.Red * Projectile.ai[0], Projectile.rotation, softGlow.Size() / 2, new Vector2(0.6f * Projectile.ai[0], 2f) * 2f, SpriteEffects.None, 0f);

            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.Crimson.ToVector3() * 3.2f * Projectile.ai[0]); 
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -40, Tex.Frame(1, 1, 0, 0), Color.Red, Projectile.rotation, Tex.Size() / 2, new Vector2(0.2f * Projectile.ai[0], 0.2f), SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            target.GetGlobalNPC<AdamShotNPC>().AdamShotHitCounter += 1;

            if (target.GetGlobalNPC<AdamShotNPC>().AdamShotHitCounter >= 4)
            {
                SkillStrikeUtil.setSkillStrike(Projectile, 1.3f);

                int a = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<H3Impact>(), 0, 0, Main.myPlayer);
                Main.projectile[a].rotation = Projectile.rotation;
                if (Main.projectile[a].ModProjectile is H3Impact h3)
                {
                    h3.color = Color.Red;
                    h3.size = 1f;
                }

                SoundStyle stylecs = new SoundStyle("Terraria/Sounds/Item_109") with { Pitch = .82f, PitchVariance = .11f, Volume = 0.7f };
                SoundEngine.PlaySound(stylecs, target.Center);

                SoundStyle BlasterDirect = new SoundStyle("AerovelenceMod/Sounds/Effects/SplatoonDirect") with { Pitch = .20f, PitchVariance = .1f, Volume = 0.3f }; 
                SoundEngine.PlaySound(BlasterDirect, target.Center);


                target.GetGlobalNPC<AdamShotNPC>().AdamShotHitCounter = 0;
            }

        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_10") with { Pitch = .3f, PitchVariance = .28f, MaxInstances = -1, Volume = 0.6f };
            SoundEngine.PlaySound(style2, Projectile.Center);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            for (int i = 0; i < 2 + Main.rand.Next(2); i++)
            {
                int gd = GlowDustHelper.DrawGlowDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowCircleFlare>(), 
                    new Color(255, 30, 30), 0.5f + Main.rand.NextFloat(0, 0.21f), 0.6f, 0f, dustShader);
                Main.dust[gd].velocity += (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * Projectile.velocity.Length() * Main.rand.NextFloat(0.5f, 1f);
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + oldVelocity, oldVelocity, Projectile.width, Projectile.height);
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), oldVelocity * 0.35f, 0, Color.Red, 0.3f);

            return true;
        }

        /*
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            
            //return base.Colliding(projHitbox, targetHitbox);
            Player Player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + unit * -20,
                Projectile.Center + unit * 20, 2, ref point);
            
            return true;
        } */

    }

    public class AdamShotNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int AdamShotHitCounter = 0;
        public int AdamShotTimer = 0;

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ModContent.ProjectileType<AdamSmallShot>())
            {
                AdamShotHitCounter++;
                AdamShotTimer = 0;
            }
        }

        public override void PostAI(NPC npc)
        {
            if (AdamShotHitCounter > 0)
            {
                if (AdamShotTimer >= 35)
                    AdamShotHitCounter = 0;
                AdamShotTimer++;
            }

            base.PostAI(npc);
        }

    }
} 