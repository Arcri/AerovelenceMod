using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.DataStructures;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Common.Globals.SkillStrikes;
using Terraria.Graphics.Effects;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.FlashLight
{
    public class TheLaserPointerProj : ModProjectile
    {
        public int OFFSET = 15;
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public float lerpVal = 0;

        public Vector2 endPoint;
        public float LaserRotation = 0;
        public float laserWidth = 20;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage()
        {
            return true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        int timer = 0;
        int laserTimer = 0;
        NPC prevLockOn = Main.npc[0];
        public bool lockedOn = false;

        int timeUntilSkillStrike = 180;
        int timeLockedOn = 0;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            #region heldProjStuff

            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            player.itemTime = 2; // Set Item time to 2 frames while we are used
            player.itemAnimation = 2; // Set Item animation time to 2 frames while we are used

            if (player.channel)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (player.Center)).ToRotation();
                }

                direction = Angle.ToRotationVector2();
                player.ChangeDir(direction.X > 0 ? 1 : -1);

            }
            else
            {
                Projectile.active = false;
            }

            lerpVal = Math.Clamp(MathHelper.Lerp(lerpVal, -0.2f, 0.002f), 0, 0.4f);

            direction = Angle.ToRotationVector2().RotatedBy(lerpVal * player.direction * -1f);
            Projectile.Center = player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            player.itemRotation = direction.ToRotation();

            if (player.direction != 1)
                player.itemRotation -= 3.14f;

            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);

            #endregion

            #region Laser

            if (FindNearestNPCMouse(400f, true, false, true, out int index))
            {
                NPC npc = Main.npc[index];
                if (npc != prevLockOn && timer != 0)
                {
                    laserWidth = 0;
                    laserTimer = 0;
                    timer = -1;
                    lockedOn = false;
                }
                else
                {
                    if (!lockedOn)
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Item_108") with { Pitch = .78f, PitchVariance = 0.1f, Volume = 0.3f };
                        SoundEngine.PlaySound(style, Projectile.Center);

                        endPoint = npc.Center;
                        timeLockedOn = 0;
                    }
                    lockedOn = true;

                    Vector2 npcPos = npc.Center;

                    //Manually offset position for Cyvercry so it looks a bit better
                    if (npc.type == ModContent.NPCType<Cyvercry2>())
                        npcPos = npc.Center + new Vector2(-25, 0).RotatedBy(npc.rotation);

                    endPoint = npcPos;

                    LaserRotation = (npcPos - (Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 25))).ToRotation();

                    laserWidth = MathHelper.Clamp(0 + (laserTimer * 0.25f), 0, 20);
                    prevLockOn = npc;

                    if (timer % 2 == 0)
                    {
                        ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                        if (Main.rand.NextBool())
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 randomStart = Main.rand.NextVector2CircularEdge(laserWidth * 0.15f, laserWidth * 0.15f);
                                Dust gd = GlowDustHelper.DrawGlowDustPerfect(npcPos, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.Red, 0.15f, 0.2f, 0f, dustShader2);
                                gd.fadeIn = 52 + Main.rand.NextFloat(-3f, 4f);
                                gd.scale *= Main.rand.NextFloat(0.9f, 1.1f);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 randomStart = Main.rand.NextVector2CircularEdge(laserWidth * 0.25f, laserWidth * 0.25f);
                                Dust gd = GlowDustHelper.DrawGlowDustPerfect(npcPos, ModContent.DustType<GlowCircleDust>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.Red, 0.3f, 0.1f, 0f, dustShader2);
                                gd.fadeIn = 2;
                            }
                        }
                    }


                    if (laserWidth > 15 && timer % 20 == 0)
                    {
                        Vector2 random = Main.rand.NextVector2CircularEdge(5, 5);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), npcPos, random, ModContent.ProjectileType<LaserPointerSpark>(), Projectile.damage / 2, Projectile.knockBack, Main.myPlayer, 0, Main.rand.NextBool() ? 1 : -1);

                        SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/hero_fury_charm_burst") with { Pitch = 1f, PitchVariance = 0.2f, Volume = 0.6f }; 
                        SoundEngine.PlaySound(style, npcPos);

                        SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_91") with { Pitch = 0.9f, PitchVariance = .2f, Volume = 0.5f }; 
                        SoundEngine.PlaySound(style2, npcPos);

                        for (int i = 0; i < 7; i++)
                        {
                            Dust.NewDustPerfect(npcPos, ModContent.DustType<MuraLineBasic>(), random.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.45f, 1.3f),
                                10, Color.Red, Main.rand.NextFloat(0.25f, 0.45f));
                        }
                    }

                    timeLockedOn++;
                }

            }
            else
            {
                lockedOn = false;
                laserWidth = 0;
                laserTimer = 0;
            }

            if (timeLockedOn > timeUntilSkillStrike)
                Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
            else
                Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;

            if (timeLockedOn == timeUntilSkillStrike)
                SSGlowAmount = 2;

            SSGlowAmount = Math.Clamp(MathHelper.Lerp(SSGlowAmount, -0.5f, 0.05f), 0, 1);
            #endregion


            laserTimer++;
            timer++;
        }
        private bool FindNearestNPCMouse(float range, bool scanTiles, bool targetIsFriendly, bool ignoreCritters, out int npcIndex)
        {
            npcIndex = -1;
            bool foundNPC = false;
            double dist = range * range;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                //Make sure NPC is valid anyway
                if (npc.active && npc.life > 0)
                {
                    if (!npc.hide && !npc.dontTakeDamage)
                    {
                        //Target and NPC friendliness are same
                        if (npc.friendly == targetIsFriendly)
                        {
                            //if ignoring critters, make sure lifemax > 10, id is not dummy, and npc does not drop item
                            if ((!(npc.lifeMax < 10 || npc.type == NPCID.TargetDummy || npc.catchItem != 0) && ignoreCritters) || !ignoreCritters)
                            {
                                //cache this
                                float compDist = Main.MouseWorld.DistanceSQ(npc.Center);
                                //Distance is shorter than current distance, but did not overflow (underflow)
                                if (compDist < dist && compDist > 0)
                                {
                                    //ignore tiles, OR scan tiles and can hit anyway
                                    if (!scanTiles || (scanTiles && Collision.CanHit(Projectile, new NPCAimedTarget(npc))))
                                    {
                                        npcIndex = i;
                                        dist = compDist;
                                        foundNPC = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Case: Failed to Find NPC
            if (!foundNPC)
                npcIndex = -1;
            return foundNPC;
        }

        float glowAmount = 0f;
        public float SSGlowAmount = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            #region Flashlight
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/TLPPGlow").Value;
            Texture2D white = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/TLPPGlowWhiteFaded").Value;

            SpriteEffects spriteEffects = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * OFFSET * -1f)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            Vector2 actualPos = new Vector2((int)Projectile.Center.X, (int)Projectile.Center.Y) - new Vector2((int)Main.screenPosition.X, (int)Main.screenPosition.Y);
            Main.spriteBatch.Draw(texture, actualPos, null, lightColor, direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0.0f);
            //Main.spriteBatch.Draw(glowMask, actualPos, null, Color.White * (laserWidth * 0.1f), direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(glowMask, actualPos, null, (Color.White with { A = 0 } * 0.3f) * (laserWidth * 0.1f), direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale + 0.1f, spriteEffects, 0.0f);

            Color glowCol = Color.Lerp(Color.DarkRed, Color.Gold, SSGlowAmount);
            Main.spriteBatch.Draw(glowMask, actualPos, null, glowCol with { A = 0 } * (laserWidth * 0.2f) * 0.5f, direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(white, actualPos, null, glowCol with { A = 0 } * (laserWidth * 0.2f) * 0.4f, direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(white, actualPos, null, glowCol with { A = 0 } * (laserWidth * 0.2f) * SSGlowAmount, direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale + 0.15f, spriteEffects, 0.0f);

            #endregion

            #region Laser

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/CheapScroll", AssetRequestMode.ImmediateLoad).Value;
            #region Shader Params
            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Laser1").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);

            Color c1 = Color.Red;
            Color c2 = Color.Red;
            
            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color1Mult"].SetValue(1f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["totalMult"].SetValue(1f);

            myEffect.Parameters["tex1reps"].SetValue(0.25f);
            myEffect.Parameters["tex2reps"].SetValue(0.25f);
            myEffect.Parameters["satPower"].SetValue(0.8f);
            myEffect.Parameters["time1Mult"].SetValue(1f);
            myEffect.Parameters["time2Mult"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.018f);
            #endregion
            
            Texture2D LaserTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/FlashLightBeam").Value;
            Texture2D flare1 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_1").Value;
            Texture2D flare12 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_12").Value;

            Vector2 origin2 = new Vector2(0, LaserTexture.Height / 2);

            float height = (laserWidth * 4f); //25

            int width = (int)((Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 25)) - endPoint).Length();

            var pos = Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 25) - Main.screenPosition;
            var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.7f));
            var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.65f));
            var target3 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.35f));

            Main.spriteBatch.Draw(LaserTexture, target3, null, Color.Black * 0.5f, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(flare12, Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 25) - Main.screenPosition, flare12.Frame(1, 1, 0, 0), Color.Black * 0.35f, timer * 0.03f, flare12.Size() / 2, 0.2f * laserWidth * 0.025f, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(flare1, endPoint - Main.screenPosition, flare1.Frame(1, 1, 0, 0), Color.Black * 0.5f, timer * 0.07f, flare1.Size() / 2, 0.015f * laserWidth, spriteEffects, 0.0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);

            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(LaserTexture, target2, null, Color.Red * 0.5f, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(LaserTexture, target2, null, Color.Red * 0.25f, LaserRotation, origin2, 0, 0);

            //Flares

            float sinScale = laserWidth < 5 ? 0f : MathF.Sin((float)Main.timeForVisualEffects * 0.06f) * 0.008f;
            Main.spriteBatch.Draw(flare12, Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 25) - Main.screenPosition, flare12.Frame(1, 1, 0, 0), Color.Red, timer * 0.03f, flare12.Size() / 2, 0.2f * laserWidth * 0.025f, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(flare1, endPoint - Main.screenPosition, flare1.Frame(1, 1, 0, 0), Color.Red, timer * 0.07f, flare1.Size() / 2, 0.015f * laserWidth, spriteEffects, 0.0f);

            Main.spriteBatch.Draw(flare12, Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 25) - Main.screenPosition, flare12.Frame(1, 1, 0, 0), Color.White, timer * -0.02f, flare12.Size() / 2, 0.14f * laserWidth * 0.025f + sinScale, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(flare1, endPoint - Main.screenPosition, flare1.Frame(1, 1, 0, 0), Color.White, timer * 0.07f, flare1.Size() / 2, 0.01f * laserWidth + (sinScale * 0.2f), spriteEffects, 0.0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //Reset because tmod still haven't fixed bug
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            #endregion

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle stylees = new SoundStyle("Terraria/Sounds/Item_117") with { Pitch = .72f, PitchVariance = .11f, Volume = 0.01f * (laserWidth) };
            SoundEngine.PlaySound(stylees, target.Center);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = LaserRotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            if (laserWidth > 5)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * 12,
                    endPoint, laserWidth * 0.5f, ref point);
            }

            return false;
        }
    }

    public class LaserPointerSpark : ModProjectile
    {
        public int i;
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.extraUpdates = 2;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool? CanDamage() { return timer > 60; }

        public override bool? CanCutTiles() { return false; }

        float fadeTime = 70f;
        float alpha = 0f;
        public override void AI()
        {
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            if (Projectile.alpha > 30)
            {
                Projectile.alpha -= 15;
                if (Projectile.alpha < 30)
                {
                    Projectile.alpha = 30;
                }
            }

            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                Projectile.localAI[0] = 1f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();


            Vector2 move = Vector2.Zero;
            float distance = 700f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }
            if (target && timer >= 60)
            {
                Projectile.velocity = Projectile.velocity.MoveTowards(move.SafeNormalize(Vector2.UnitX) * 8, 0.06f);
            }
            else if (!target && timer >= 80)
            {
                fadeTime--;

                if (fadeTime == 0)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), Projectile.velocity * 0.75f, 0, Color.Red, 0.4f);
                    Projectile.active = false;
                }
            }

            if (timer < 60)
                Projectile.velocity = Projectile.velocity.RotatedBy(0.04f * Projectile.ai[1]);

            if (Projectile.alpha <= 30 && timer % 4 == 0)
            {
                Dust d = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.Zero,
                    Color.Red, 0.4f, 0.6f, 0f,
                    dustShader);
            }
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D line = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Nightglow").Value;
            Texture2D orb = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;

            Vector2 vec2Scale = new Vector2(1f, 1f - Projectile.velocity.Length() * 0.1f) * Projectile.scale;
            Vector2 vec2ScaleLine = new Vector2(1f - Projectile.velocity.Length() * 0.1f, 1f) * Projectile.scale;
            Vector2 vec2ScaleOrb = new Vector2(1f, 0.5f - Projectile.velocity.Length() * 0.05f) * Projectile.scale;

            Main.spriteBatch.Draw(line, Projectile.Center - Main.screenPosition, null, Color.Black with { A = 0 } * 0.4f, Projectile.rotation - MathHelper.PiOver2, line.Size() / 2, vec2ScaleLine * 1.2f, SpriteEffects.None, 0.0f);

            Main.spriteBatch.Draw(line, Projectile.Center - Main.screenPosition, null, Color.Red with { A = 0 } * 0.75f, Projectile.rotation - MathHelper.PiOver2, line.Size() / 2, vec2ScaleLine * 1.2f, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(line, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 1f, Projectile.rotation - MathHelper.PiOver2, line.Size() / 2, vec2ScaleLine * 0.6f, SpriteEffects.None, 0.0f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation, texture.Size() / 2, vec2Scale, SpriteEffects.None, 0.0f);

            Main.spriteBatch.Draw(orb, Projectile.Center - Main.screenPosition, null, Color.Red with { A = 0 } * 0.75f, Projectile.rotation, orb.Size() / 2, vec2ScaleOrb, SpriteEffects.None, 0.0f);


            return false;
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 3f)
            {
                vector *= 3f / magnitude;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), Projectile.velocity * 0.75f, 0, Color.Red, 0.4f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), Projectile.velocity * 0.75f, 0, Color.Red, 0.4f);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            //ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            //GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleQuadStar>(), Projectile.velocity * 0.75f, Color.Red, 1f, 0.7f, 0, dustShader);

            //Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), Projectile.velocity * 0.75f, 0, Color.Red, 0.4f);
        }
    }
}
