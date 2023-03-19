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
    public class AdamantitePulseShot : ModProjectile
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
        }

        public const int MAX_PENETRATION = 7;
        public int enemiesHit = 0;
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 3;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 400;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            if (timer < 180)
                Projectile.velocity *= 1.03f;

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
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX)), Tex.Frame(1, 1, 0, 0), Color.Red * 0.85f, Projectile.rotation, Tex.Size() / 2, new Vector2(0.5f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition , softGlow.Frame(1, 1, 0, 0), Color.Red, Projectile.rotation, softGlow.Size() / 2, new Vector2(1f, Projectile.velocity.Length() * 0.06f), SpriteEffects.None, 0f);

            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.Crimson.ToVector3() * 3.2f); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX), Tex.Frame(1, 1, 0, 0), Color.Crimson, Projectile.rotation, Tex.Size() / 2, new Vector2(0.25f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -70), Tex.Frame(1, 1, 0, 0), Color.Crimson, Projectile.rotation, Tex.Size() / 2, new Vector2(0.1f, Projectile.velocity.Length() * 0.05f), SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -30), Tex.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, Tex.Size() / 2, 0.2f, SpriteEffects.None, 0f);


            //Set up Shader
            //Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            //myEffect.Parameters["uColor"].SetValue(Color.LawnGreen.ToVector3() * 1.5f);
            //myEffect.Parameters["uTime"].SetValue(2);
            //myEffect.Parameters["uOpacity"].SetValue(0.6f); //0.6
            //myEffect.Parameters["uSaturation"].SetValue(1.2f);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            //myEffect.CurrentTechnique.Passes[0].Apply();




            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }




        public override void Kill(int timeLeft)
        {    

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            for (int i = 0; i < 5; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleDust>(),
                    (Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-1, 1)) * Main.rand.Next(1, 3)),
                    Color.Red, Main.rand.NextFloat(0.2f, 0.4f), 0.3f, 0f, dustShader);
                p.alpha = 0;
                //p.rotation = Main.rand.NextFloat(6.28f);
            }

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            enemiesHit++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 30;
            Projectile.velocity = Vector2.Zero;
            if (Projectile.timeLeft > 0)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_10") with { Pitch = .42f, PitchVariance = .38f, MaxInstances = -1, Volume = 0.5f };
                SoundEngine.PlaySound(style, Projectile.Center);
            }
            return false;
        }

        public override bool? CanDamage()
        {
            if (enemiesHit >= MAX_PENETRATION)
                return false;
            return true;
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

    public class AdamSmallShot : ModProjectile
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 4;
            Projectile.width = 20;
            Projectile.height = 20;
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

            //Projectile.velocity *= 1.02f;

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {

            //Draw the Circlular Glow
            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulseShot").Value;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -40, Tex.Frame(1, 1, 0, 0), Color.Red * 0.85f, Projectile.rotation, Tex.Size() / 2, new Vector2(0.35f, 0.3f), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX), softGlow.Frame(1, 1, 0, 0), Color.Red, Projectile.rotation, softGlow.Size() / 2, new Vector2(0.9f, 2f), SpriteEffects.None, 0f);

            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.Crimson.ToVector3() * 3.2f); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -40, Tex.Frame(1, 1, 0, 0), Color.Red, Projectile.rotation, Tex.Size() / 2, new Vector2(0.2f, 0.2f), SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];

            target.GetGlobalNPC<AdamShotNPC>().AdamShotHitCounter += 1;

            if (target.GetGlobalNPC<AdamShotNPC>().AdamShotHitCounter >= 4)
            {
                Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
                Projectile.GetGlobalProjectile<SkillStrikeGProj>().travelDust = (int)SkillStrikeGProj.TravelDustType.None;
                Projectile.GetGlobalProjectile<SkillStrikeGProj>().critImpact = (int)SkillStrikeGProj.CritImpactType.None;
                Projectile.GetGlobalProjectile<SkillStrikeGProj>().impactScale = 0f;
                Projectile.GetGlobalProjectile<SkillStrikeGProj>().hitSoundVolume = 0f;

                int a = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<H3Impact>(), 0, 0, Main.myPlayer);
                Main.projectile[a].rotation = Projectile.rotation;
                if (Main.projectile[a].ModProjectile is H3Impact h3)
                {
                    h3.color = Color.Red;
                    h3.size = 1f;
                }
                //SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_1") with { Pitch = .66f, PitchVariance = .12f, MaxInstances = -1, Volume = 0.8f };
                //SoundEngine.PlaySound(style2, target.Center);

                SoundStyle stylecs = new SoundStyle("Terraria/Sounds/Item_109") with { Pitch = .82f, PitchVariance = .11f, Volume = 0.7f };
                SoundEngine.PlaySound(stylecs, target.Center);

                SoundStyle BlasterDirect = new SoundStyle("AerovelenceMod/Sounds/Effects/SplatoonDirect") with { Pitch = .20f, PitchVariance = .1f, Volume = 0.3f }; 
                SoundEngine.PlaySound(BlasterDirect, target.Center);


                target.GetGlobalNPC<AdamShotNPC>().AdamShotHitCounter = 0;
            }

        }

        public override void Kill(int timeLeft)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = 1f, PitchVariance = .14f, Volume = 0.1f, MaxInstances = -1 };
            SoundEngine.PlaySound(style, Projectile.Center);

            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_10") with { Pitch = .92f, PitchVariance = .28f, MaxInstances = -1, Volume = 0.6f };
            SoundEngine.PlaySound(style2, Projectile.Center);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            for (int i = 0; i < 4; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleDust>(),
                    (Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-5, 5)) * Main.rand.Next(1, 4)),
                    Color.Crimson, Main.rand.NextFloat(0.25f, 0.45f), 0.3f, 0f, dustShader);
                p.alpha = 0;
                //p.rotation = Main.rand.NextFloat(6.28f);
            }

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

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (projectile.type == ModContent.ProjectileType<AdamSmallShot>())
            {
                AdamShotHitCounter++;
                AdamShotTimer = 0;
            }

            base.OnHitByProjectile(npc, projectile, damage, knockback, crit);
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