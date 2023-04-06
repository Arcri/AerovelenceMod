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
            // DisplayName.SetDefault("Laser Pointer");
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


        int laserDamage = 0;
        int timer = 0;
        int laserTimer = 0;
        NPC prevLockOn = Main.npc[0];
        public bool lockedOn = false;


        bool hasDamageBeenStored = false;
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

            } else
            {
                Projectile.active = false;
            }

            lerpVal = Math.Clamp(MathHelper.Lerp(lerpVal, -0.2f, 0.002f), 0, 0.4f);

            direction = Angle.ToRotationVector2().RotatedBy(lerpVal * player.direction * -1f);
            Projectile.Center = player.Center + (direction * OFFSET);
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

            if (FindNearestNPCMouse(300f, true, false, true, out int index))
            {
                NPC npc = Main.npc[index];
                if (npc != prevLockOn && timer != 0)
                {
                    laserWidth = 0;
                    laserTimer = 0;
                    timer = -1;
                    lockedOn = false;
                } else
                {
                    if (!lockedOn)
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Item_108") with { Pitch = .78f, PitchVariance = 0.1f, Volume = 0.3f };
                        SoundEngine.PlaySound(style, Projectile.Center);

                        


                    }
                    lockedOn = true;
                    endPoint = npc.Center;
                    LaserRotation = (npc.Center - (Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 25))).ToRotation();

                    laserWidth = MathHelper.Clamp(0 + (laserTimer * 0.25f), 0, 30);
                    prevLockOn = npc;

                    if (timer % 2 == 0)
                    {
                        ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                        
                        if (Main.rand.NextBool())
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 randomStart = Main.rand.NextVector2CircularEdge(laserWidth * 0.15f, laserWidth * 0.15f);
                                Dust gd = GlowDustHelper.DrawGlowDustPerfect(npc.Center, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.Red, 0.15f, 0.2f, 0f, dustShader2);
                                gd.fadeIn = 52 + Main.rand.NextFloat(-3f, 4f);
                                gd.scale *= Main.rand.NextFloat(0.9f, 1.1f);
                            }
                        } 
                        else
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 randomStart = Main.rand.NextVector2CircularEdge(laserWidth * 0.25f, laserWidth * 0.25f);
                                Dust gd = GlowDustHelper.DrawGlowDustPerfect(npc.Center, ModContent.DustType<GlowCircleDust>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.Red, 0.3f, 0.1f, 0f, dustShader2);
                                gd.fadeIn = 2;
                            }
                        }
                    }

                }

            } else
            {
                lockedOn = false;
                laserWidth = 0;
                laserTimer = 0;
            }
            #endregion

            laserTimer++;
            timer++;
        }
        private bool FindNearestNPCMouse(float range, bool scanTiles, bool targetIsFriendly, bool ignoreCritters, out int npcIndex)
        {
            npcIndex = -1;
            bool foundNPC = false;
            double dist = range * range;
            for(int i = 0; i < Main.maxNPCs; i++)
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

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            #region Flashlight
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/TLPPGlow").Value;

            SpriteEffects spriteEffects = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * OFFSET * -1f)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            Vector2 actualPos = new Vector2((int)Projectile.Center.X, (int)Projectile.Center.Y) - new Vector2((int)Main.screenPosition.X, (int)Main.screenPosition.Y);
            Main.spriteBatch.Draw(texture, actualPos, null, lightColor, direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(glowMask, actualPos, null, Color.White * (laserWidth * 0.1f), direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(glowMask, actualPos, null, (Color.White * 0.1f) * (laserWidth * 0.1f), direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale + 0.1f, spriteEffects, 0.0f);

            #endregion

            #region Laser


            Texture2D LaserTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/FlashLightBeam").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/RimeLaser", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["uColor"].SetValue(Color.Red.ToVector3() * 0.52f);
            myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
            myEffect.Parameters["uTime"].SetValue(timer * -0.01f); //0.006
            myEffect.Parameters["uSaturation"].SetValue(2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            Vector2 origin2 = new Vector2(0, LaserTexture.Height / 2);

            float height = (laserWidth); //25

            int width = (int)((Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 25)) - endPoint).Length();

            var pos = Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 25) - Main.screenPosition;
            var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.7f));

            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);

            //Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.8f));
            var target3 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.8f));

            Main.spriteBatch.Draw(LaserTexture, target2, null, Color.Red * 0.5f, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(LaserTexture, target3, null, Color.Red * 0.25f, LaserRotation, origin2, 0, 0);

            //Flares

            Effect myEffect2 = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            myEffect2.Parameters["uColor"].SetValue(Color.Red.ToVector3() * 3f);
            myEffect2.Parameters["uTime"].SetValue(2);
            myEffect2.Parameters["uOpacity"].SetValue(0.7f);
            myEffect2.Parameters["uSaturation"].SetValue(0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect2, Main.GameViewMatrix.TransformationMatrix);

            myEffect2.CurrentTechnique.Passes[0].Apply();
            Texture2D flare1 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_1").Value;
            Texture2D flare12 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_12").Value;

            Main.spriteBatch.Draw(flare12, Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 25) - Main.screenPosition, flare12.Frame(1,1,0,0), Color.Red, timer * 0.03f, flare12.Size() / 2, 0.2f * laserWidth * 0.02f, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(flare1, endPoint - Main.screenPosition, flare1.Frame(1, 1, 0, 0), Color.Red, timer * 0.07f, flare1.Size() / 2, 0.01f * laserWidth, spriteEffects, 0.0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
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
}
