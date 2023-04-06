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
using AerovelenceMod.Common.Globals.SkillStrikes;
using Terraria.Graphics;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class CrystalSpray_Proj : ModProjectile
    {
        int timer = 0;
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 350;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        bool hasHit;

        public override bool? CanDamage()
        {
            return !hasHit;
        }

        public override void AI()
        {
            timer++;
            if (hasHit)
            {
                Projectile.velocity *= 0.6f;
            }
            if(FindNearestNPC(300f, true, false, true, out int index))
            {
                NPC npc = Main.npc[index];
                Projectile.velocity *= .98f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(npc.Center) * 20f, .018f);
                //For some reason, directionTo*length seems to slow down sometimes :shrug:
            }
            else
            {
                Projectile.velocity.Y += .056f;
                Projectile.velocity.X *= .985f;
            }
            if(Main.rand.NextBool(20))
            {
                float randomRot = Main.rand.NextFloat(6.28f);

                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                int a = GlowDustHelper.DrawGlowDust(Projectile.position + (Projectile.rotation.ToRotationVector2() * -35), 1, 1, ModContent.DustType<Dusts.GlowDusts.GlowCircleFlare>(), 0,
                    0, Color.DodgerBlue, Main.rand.NextFloat(.3f, .5f), 0.55f, 0, dustShader);
                Main.dust[a].noLight = true;
                Main.dust[a].velocity = Vector2.Zero;
                Main.dust[a].velocity = Projectile.velocity;

                //a.position += new Vector2(0,1).RotatedBy(Projectile.velocity.ToRotation()) * 32 * Projectile.direction;
                Main.dust[a].rotation = randomRot;
                //a.position += randomRot.ToRotationVector2() * -8;
            }
            else
            {
                //Dust d = Dust.NewDustDirect(Projectile.position, 1, 1, Main.rand.Next(new int[] { DustID.DungeonWater, DustID.SpectreStaff }));
                //d.velocity = Projectile.velocity;
                //d.noGravity = true;
                //d.color = Color.Aqua;
            }

            TrailLogic();
        }
        private bool FindNearestNPC(float range, bool scanTiles, bool targetIsFriendly, bool ignoreCritters, out int npcIndex)
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
                    //Target and NPC friendliness are same
                    if (npc.friendly == targetIsFriendly)
                    {
                        //if ignoring critters, make sure lifemax > 10, id is not dummy, and npc does not drop item
                        if ((!(npc.lifeMax < 10 || npc.type == NPCID.TargetDummy || npc.catchItem != 0) && ignoreCritters) || !ignoreCritters)
                        {
                            //cache this
                            float compDist = Projectile.DistanceSQ(npc.Center);
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
            //Case: Failed to Find NPC
            if (!foundNPC)
                npcIndex = -1;
            return foundNPC;
        }

        public void TrailLogic()
        {
            Projectile.ai[1] = timer * 0.05f;

            Initialize();

            Projectile.rotation = Projectile.velocity.ToRotation();

            trailPositions.Add(Projectile.Center);
            trailRotations.Add(Projectile.velocity.ToRotation());

            //Smoothing
            if (trailPositions.Count > 3)
            {
                for (int i = 3; i < trailPositions.Count - 2; i++)
                {
                    trailPositions[i - 2] = (trailPositions[i - 3] + trailPositions[i - 1]) / 2f;
                    trailRotations[i - 2] = Vector2.Lerp(trailRotations[i - 3].ToRotationVector2(), trailRotations[i - 1].ToRotationVector2(), 0.5f).ToRotation();
                }
            }

            trailCurrentLength = CalculateLength();


            //This could be optimized to not require recomputing the length after each removal
            while (trailPositions.Count > trailPointLimit || trailCurrentLength > trailMaxLength)
            {
                trailPositions.RemoveAt(0);
                trailRotations.RemoveAt(0);
                trailCurrentLength = CalculateLength();
            }
        }

        public void DrawTrail()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, customEffect, Main.GameViewMatrix.TransformationMatrix);

            int width = Main.graphics.GraphicsDevice.Viewport.Width;
            int height = Main.graphics.GraphicsDevice.Viewport.Height;

            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(Main.graphics.GraphicsDevice.Viewport.Width / 2, Main.graphics.GraphicsDevice.Viewport.Height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1f);
            Matrix projection = Matrix.CreateOrthographic(Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height, 0, 1000);

            customEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/IchorMissileExhaust", AssetRequestMode.ImmediateLoad).Value;
            customEffect.Parameters["noiseTexture"].SetValue(Mod.Assets.Request<Texture2D>("Assets/Noise/noise").Value);
            customEffect.Parameters["fadeOut"].SetValue(0.5f);
            customEffect.Parameters["time"].SetValue(1);
            customEffect.Parameters["shaderColor"].SetValue(Color.DodgerBlue.ToVector4());
            customEffect.Parameters["WorldViewProjection"].SetValue(view * projection);
            customEffect.CurrentTechnique.Passes[0].Apply();

            VertexStrip vertexStrip = new VertexStrip();

            if (trailPositions != null)
            {
                vertexStrip.PrepareStrip(trailPositions.ToArray(), trailRotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, includeBacksides: true);
                vertexStrip.DrawTrail();
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }

        #region TrailVariables
        public int trailPointLimit = 60;

        public int trailWidth = 20; //10

        public float trailMaxLength = 200;

        public float trailCurrentLength;

        public float fadeOut = 1;

        public float trailYOffset = 0;

        public Effect customEffect;

        public List<Vector2> trailPositions;

        public List<float> trailRotations;

        private bool initialized = false;
        public float lengthPercent
        {
            get
            {
                return trailCurrentLength / trailMaxLength;
            }
        }
        #endregion
        public void Initialize()
        {
            if (!initialized)
            {
                trailPositions = new List<Vector2>();
                trailRotations = new List<float>();

                initialized = true;
            }
        }

        public float CalculateLength()
        {
            float calculatedLength = 0;
            for (int i = 0; i < trailPositions.Count - 2; i++)
            {
                calculatedLength += Vector2.Distance(trailPositions[i], trailPositions[i + 1]);
            }

            return calculatedLength;
        }

        BasicEffect basicEffect;
        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            return false;
        }

        public virtual float WidthFunction(float progress)
        {
            
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);

            if (hasHit || Projectile.timeLeft <= 20)
            {
                float val = 1 - (Projectile.timeLeft / 20);
                return MathHelper.Lerp(0f, 30f, num) * 0.15f * val;
            }

            return MathHelper.Lerp(0f, 30f, num) * 0.3f; // 0.3f
        }

        public virtual Color ColorFunction(float progress)
        {
            return Color.Gray;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hasHit = true;
            Projectile.timeLeft = 20;
        }
    }

    public class WaterOrb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            
            Projectile.velocity = new Vector2(Projectile.ai[1] * 0.15f, 0);

            Vector2 dustSpawnPos = new Vector2(Main.rand.NextFloat(-100,100), Main.rand.NextFloat(-100, 100));

            //dust pos relative to Center (redundant???)
            Vector2 dustRelPos = (Projectile.Center + dustSpawnPos) - Projectile.Center;

            //dust vel
            float velVal = Main.rand.NextFloat(2, 10);
            Vector2 vel = new Vector2(velVal, 0).RotatedBy(dustSpawnPos.ToRotation() + MathHelper.PiOver2);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            Dust d = GlowDustHelper.DrawGlowDustPerfect(Projectile.position + dustSpawnPos, ModContent.DustType<Dusts.GlowDusts.GlowCircleFlare>(), vel, Color.DodgerBlue,
                Main.rand.NextFloat(.4f, 0.6f), 0.55f, 0, dustShader);

            d.velocity += Projectile.velocity;
            d.fadeIn = 2;
            d.noLight = true;

            //int a = GlowDustHelper.DrawGlowDust(Projectile.Center + new Vector2(-100,-100), 200, 200, ModContent.DustType<Dusts.GlowDusts.GlowCircleFlare>(), 0,
            //0, Color.DodgerBlue, Main.rand.NextFloat(.3f, .5f), 0.55f, 0, dustShader);
            //Main.dust[a].noLight = true;
            //Projectile.scale = 0.24f;
            Projectile.rotation += 0.09f;
            Projectile.ai[1]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D orb = Mod.Assets.Request<Texture2D>("Assets/EnergyBalls/energyball_1").Value;
            Texture2D orb2 = Mod.Assets.Request<Texture2D>("Assets/EnergyBalls/energyball_4").Value;

            Main.spriteBatch.Draw(orb2, Projectile.Center - Main.screenPosition, orb2.Frame(1, 1, 0, 0), Color.Black * 0.5f , Projectile.rotation * 2f, orb2.Size() / 2, Projectile.scale * 1.15f * (1 - Projectile.ai[0]), SpriteEffects.None, 0.0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(orb, Projectile.Center - Main.screenPosition, orb.Frame(1,1,0,0), Color.White, Projectile.rotation, orb.Size() / 2, Projectile.scale, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(orb, Projectile.Center - Main.screenPosition, orb.Frame(1, 1, 0, 0), Color.DodgerBlue * 0.95f, Projectile.rotation * 1.5f, orb.Size() / 2, Projectile.scale * 1.3f, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(orb, Projectile.Center - Main.screenPosition, orb.Frame(1, 1, 0, 0), Color.DeepSkyBlue * 0.55f, Projectile.rotation * 2f, orb.Size() / 2, Projectile.scale * 1.55f * (1 - Projectile.ai[0]), SpriteEffects.None, 0.0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void Kill(int timeLeft)
        {

            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/StampAirSwing2") with { Volume = .46f, Pitch = .35f, };
            SoundEngine.PlaySound(style, Projectile.Center);

            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/CommonWaterFallLight00") with { Volume = .28f, Pitch = -.47f, };
            SoundEngine.PlaySound(style2, Projectile.Center);

            SoundStyle style4 = new SoundStyle("Terraria/Sounds/Item_21") with { Pitch = .69f, PitchVariance = .27f, };
            SoundEngine.PlaySound(style4, Projectile.Center);

            
            for (int i = 1; i < 5; i++)
            {
                for (int m = 1; m < 12; m++)
                {
                    Vector2 dustSpawnPos = new Vector2(Main.rand.NextFloat(-100,100), Main.rand.NextFloat(-100, 100));

                    //dust vel
                    float velVal = Main.rand.NextFloat(m, m);
                    Vector2 vel = new Vector2(velVal, 0).RotatedBy(dustSpawnPos.ToRotation() + MathHelper.PiOver2);

                    ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                    Dust d = GlowDustHelper.DrawGlowDustPerfect(Projectile.position + dustSpawnPos, ModContent.DustType<Dusts.GlowDusts.GlowCircleFlare>(), vel, Color.DodgerBlue,
                        Main.rand.NextFloat(.4f, 0.4f), 0.55f, 0, dustShader);
                }

            }
            
            for (int k = 0; k < 18; k++)
            {
                Vector2 vel;
                if (k < 9)
                    vel = new Vector2(-4,0).RotatedBy(Main.rand.NextFloat(-1.75f, 1.75f)) * Main.rand.NextFloat(0f, 2.5f);
                else
                    vel = new Vector2(4,0).RotatedBy(Main.rand.NextFloat(-1.75f, 1.75f)) * Main.rand.NextFloat(0f, 2.5f);

                //Vector2 vel = Main.rand.NextVector2Circular(5.5f, 2f) * 2.5f;
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                Dust d = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowDusts.GlowCircleFlare>(), vel, Color.DodgerBlue,
                    Main.rand.NextFloat(1f, 1f), 0.4f, 0, dustShader);
            }

            /*
            for (int j = 0; j < 6; j++)
            {
                Vector2 Vecout = new Vector2(30, 0).RotatedBy(j * 45);
                int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Vecout, Vecout * 0.4f, ModContent.ProjectileType<CrystalSpray_Proj>(), Projectile.damage / 4, 0, Main.myPlayer);

                Main.projectile[a].GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
                Main.projectile[a].GetGlobalProjectile<SkillStrikeGProj>().critImpact = (int)SkillStrikeGProj.CritImpactType.glowTargetCenter;
                Main.projectile[a].GetGlobalProjectile<SkillStrikeGProj>().travelDust = (int)SkillStrikeGProj.TravelDustType.glowProjCenter;
                Main.projectile[a].GetGlobalProjectile<SkillStrikeGProj>().hitSoundVolume = 0.15f;
                Main.projectile[a].GetGlobalProjectile<SkillStrikeGProj>().impactScale = 0.25f;
            }
            */
        }
    }

    public class WaterTrailTest : ModProjectile {

        int timer = 0;
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water Trail");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

        public override void SetDefaults()
        {
            Projectile.damage = 10;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 4000;
            Projectile.penetrate = -1;
        }

        public int trailPointLimit = 60;

        public int trailWidth = 20; //10

        public float trailMaxLength = 200;

        public float trailCurrentLength;

        public float fadeOut = 1;

        public float trailYOffset = 0;

        public Effect customEffect;

        public List<Vector2> trailPositions;

        public List<float> trailRotations;

        private bool initialized = false;
        public float lengthPercent
        {
            get
            {
                return trailCurrentLength / trailMaxLength;
            }
        }

        public void Initialize()
        {
            if (!initialized)
            {
                trailPositions = new List<Vector2>();
                trailRotations = new List<float>();

                initialized = true;
            }
        }

        public override void AI()
        {
            timer++;
            Projectile.ai[1] = timer * 0.05f;

            Initialize();

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity.Y += 0.09f;
            //Projectile.velocity *= 0.99f;

            trailPositions.Add(Projectile.Center);
            trailRotations.Add(Projectile.velocity.ToRotation());

            //Smoothing
            if (trailPositions.Count > 3)
            {
                for (int i = 3; i < trailPositions.Count - 2; i++)
                {
                    trailPositions[i - 2] = (trailPositions[i - 3] + trailPositions[i - 1]) / 2f;
                    trailRotations[i - 2] = Vector2.Lerp(trailRotations[i - 3].ToRotationVector2(), trailRotations[i - 1].ToRotationVector2(), 0.5f).ToRotation();
                }
            }

            trailCurrentLength = CalculateLength();


            //This could be optimized to not require recomputing the length after each removal
            while (trailPositions.Count > trailPointLimit || trailCurrentLength > trailMaxLength)
            {
                trailPositions.RemoveAt(0);
                trailRotations.RemoveAt(0);
                trailCurrentLength = CalculateLength();
            }
        }

        public float CalculateLength()
        {
            float calculatedLength = 0;
            for (int i = 0; i < trailPositions.Count - 2; i++)
            {
                calculatedLength += Vector2.Distance(trailPositions[i], trailPositions[i + 1]);
            }

            return calculatedLength;
        }

        BasicEffect basicEffect;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, customEffect, Main.GameViewMatrix.TransformationMatrix);


            //Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(Main.graphics.GraphicsDevice.Viewport.Width / 2, Main.graphics.GraphicsDevice.Viewport.Height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1f);
            //Matrix projection = Matrix.CreateOrthographic(Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height, 0, 1000);

            /*
            customEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/IchorMissileExhaust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            customEffect.Parameters["noiseTexture"].SetValue(Mod.Assets.Request<Texture2D>("Assets/Noise/noise").Value);
            customEffect.Parameters["fadeOut"].SetValue(0.5f);
            customEffect.Parameters["time"].SetValue(1);
            customEffect.Parameters["shaderColor"].SetValue(Color.DodgerBlue.ToVector4());
            customEffect.Parameters["WorldViewProjection"].SetValue(view * projection);
            customEffect.CurrentTechnique.Passes[0].Apply();
            */

            customEffect = AerovelenceMod.BasicTrailShader;
            customEffect.Parameters["TrailTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail7").Value);
            customEffect.Parameters["ColorOne"].SetValue(Color.OrangeRed.ToVector4());

            int width = Main.graphics.GraphicsDevice.Viewport.Width;
            int height = Main.graphics.GraphicsDevice.Viewport.Height;

            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                          Matrix.CreateTranslation(width / 2f, height / -2f, 0) * Matrix.CreateRotationZ(MathHelper.Pi) *
                          Matrix.CreateScale(zoom.X, zoom.Y, 1f);

            Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            customEffect.Parameters["WorldViewProjection"].SetValue(view * projection);
            customEffect.Parameters["progress"].SetValue(Projectile.ai[1]);
            customEffect.CurrentTechnique.Passes["DefaultPass"].Apply();
            customEffect.CurrentTechnique.Passes["MainPS"].Apply();

            VertexStrip vertexStrip = new VertexStrip();
            if (trailPositions != null)
            {
                vertexStrip.PrepareStrip(trailPositions.ToArray(), trailRotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, includeBacksides: true);
                vertexStrip.DrawTrail();
                //vertexStrip.DrawTrail();

            }


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            
            return false;
        }

        public virtual float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num) * 0.3f; // 0.3f
        }

        public virtual Color ColorFunction(float progress)
        {
            return Color.DodgerBlue;
        }

    }

}
