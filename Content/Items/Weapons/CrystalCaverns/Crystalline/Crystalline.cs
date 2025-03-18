using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.DataStructures;
using static Terraria.NPC;
using static tModPorter.ProgressUpdate;
using ReLogic.Content;
using Terraria.Physics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;
using static System.Formats.Asn1.AsnWriter;
using Terraria.Graphics;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using System.Net;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns.Crystalline
{
    public class Crystalline : ModItem
    {
        public override void SetStaticDefaults()
        {
            // These are all related to gamepad controls and don't seem to affect anything else
            ItemID.Sets.Yoyo[Item.type] = true; // Used to increase the gamepad range when using Strings.
            ItemID.Sets.GamepadExtraRange[Item.type] = 10; // Increases the gamepad range. Some vanilla values: 4 (Wood), 10 (Valor), 13 (Yelets), 18 (The Eye of Cthulhu), 21 (Terrarian).
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true; // Unused, but weapons that require aiming on the screen are in this set.
        }

        public override void SetDefaults()
        {
            Item.damage = 21;
            Item.knockBack = KnockbackTiers.VeryWeak;
            Item.width = 34;
            Item.height = 44;

            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.shootSpeed = 16f;

            Item.shoot = ModContent.ProjectileType<CrystallineProj>();
            Item.useStyle = ItemUseStyleID.Shoot; 
            Item.UseSound = SoundID.Item1; 
            Item.DamageType = DamageClass.MeleeNoSpeed; 
            Item.value = Item.buyPrice(gold: 2);

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
    }

    public class CrystallineProj : ModProjectile
    {
        //mfw example mod code
        public override void SetStaticDefaults()
        {
            // YoyosLifeTimeMultiplier is how long in seconds the yoyo will stay out before automatically returning to the player. 
            // Vanilla values range from 3f (Wood) to 16f (Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f; //11

            // YoyosMaximumRange is the maximum distance the yoyo sleep away from the player. 
            // Vanilla values range from 130f (Wood) to 400f (Terrarian), and defaults to 200f.
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 220f;

            // YoyosTopSpeed is top speed of the yoyo Projectile.
            // Vanilla values range from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 13f;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of the projectile's hitbox.
            Projectile.height = 16; // The height of the projectile's hitbox.

            Projectile.aiStyle = ProjAIStyleID.Yoyo; // The projectile's ai style. Yoyos use aiStyle 99 (ProjAIStyleID.Yoyo). A lot of yoyo code checks for this aiStyle to work properly.

            Projectile.hostile = false;
            Projectile.friendly = true; 
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => hitCounter != 3;


        // notes for aiStyle 99: 
        // localAI[0] is used for timing up to YoyosLifeTimeMultiplier
        // localAI[1] can be used freely by specific types
        // ai[0] and ai[1] usually point towards the x and y world coordinate hover point
        // ai[0] is -1f once YoyosLifeTimeMultiplier is reached, when the player is stoned/frozen, when the yoyo is too far away, or the player is no longer clicking the shoot button.
        // ai[0] being negative makes the yoyo move back towards the player
        // Any AI method can be used for dust, spawning projectiles, etc specific to your yoyo.

        int animTimer = 0;
        public override void PostAI()
        {
            //Start doing the lightning 'animation'
            if (hitCounter == 3)
            {
                //Stop yoyo from retracting during anim
                Projectile.ai[0] = 1;

                //Slow down yoyo
                if (animTimer < 8)
                    Projectile.velocity *= 0.65f;
                else
                    Projectile.velocity *= 0.92f;

                //Release the lightning
                if (animTimer == 8)
                {
                    //Find the three closest enemies
                    NPC[] targets = new NPC[3];

                    for (int i = 0; i < 3; i++)
                    {
                        if (targets[i] == null)
                            targets[i] = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(Projectile.Center) < 300f && !(targets[0] == n || targets[1] == n || targets[2] == n))
                                .OrderBy(n => n.Distance(Projectile.Center)).FirstOrDefault();
                        else if (!targets[i].active || targets[i].Distance(Projectile.Center) > 300f)
                            targets[i] = null;
                    }

                    bool atLeastOneTarget = false;
                    for (int i = 0; i < 3; i++)
                    {
                        if (targets[i] != null)
                        {
                            int a = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CrystallineVFX>(), 0, 0, Projectile.owner);
                            (Main.projectile[a].ModProjectile as CrystallineVFX).startPoint = Projectile.Center;
                            (Main.projectile[a].ModProjectile as CrystallineVFX).endPoint = targets[i].Center;

                            int hitdmg = (int)(Projectile.damage * 1.5);

                            int dir = targets[i].Center.X > Projectile.Center.X ? 1 : -1;

                            HitInfo hit = targets[i].CalculateHitInfo(hitdmg, dir, knockBack: 0, damageType: DamageClass.Melee, damageVariation: true);
                            targets[i].StrikeNPC(hit);

                            atLeastOneTarget = true;
                        }

                    }

                    if (atLeastOneTarget)
                    {
                        SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = 0.04f, Pitch = 0.55f, PitchVariance = 0.15f, MaxInstances = -1, };
                        SoundEngine.PlaySound(style, Projectile.Center);
                        glowPower = 11f;
                    }
                    else
                        hitCounter = 0;
                }

                if (animTimer == 18)
                    hitCounter = 0;
            }

            //Dust
            if (animTimer % 2 == 0 && Main.rand.NextBool())
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(1.5f, 1.5f);

                Dust da = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ElectricSparkGlow>(), dustVel, newColor: Color.DodgerBlue, Scale: Main.rand.NextFloat(0.4f, 0.6f) * 1.75f);
                da.velocity += Projectile.velocity.RotatedByRandom(0.25f) * 0.2f;

                ElectricSparkBehavior esb = new ElectricSparkBehavior(FadeAlphaPower: 0.9f, FadeScalePower: 0.94f, FadeVelPower: 0.9f, Pixelize: true, XScale: 1f, YScale: 0.45f, WhiteLayerPower: 0.5f);
                esb.randomVelRotatePower = 0.15f;
                da.customData = esb;
            }

            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * 0.6f);

            if (hitCounter == 0)
                glowPower = MathHelper.Lerp(glowPower, 0f, 0.3f);
            else if (hitCounter == 1)
                glowPower = MathHelper.Lerp(glowPower, 0.33f, 0.2f);
            else if (hitCounter == 2)
                glowPower = MathHelper.Lerp(glowPower, 0.66f, 0.2f);
            else if (hitCounter == 3)
                glowPower = MathHelper.Lerp(glowPower, 1f, 0.3f);
            animTimer++;
        }

        int hitCounter = 0;
        public override void OnHitNPC(NPC target, HitInfo hit, int damageDone)
        {
            animTimer = 0;
            hitCounter++;
            base.OnHitNPC(target, hit, damageDone);
        }

        Effect myEffect = null;
        float effectRot = 0f;
        float glowPower = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glowTex = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle128PMA").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float scale = Projectile.scale * 0.25f + (0.03f * glowPower);

            Color col = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.4f);
            Main.EntitySpriteDraw(glowTex, drawPos, null, col with { A = 0 } * 0.4f, 0f, glowTex.Size() / 2f, scale * 1.15f, 0);

            #region shader
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/NewRadialScroll", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/foam_mask_bloom").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/ThunderGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/sparkNoiseloop").Value);
            myEffect.Parameters["flowSpeed"].SetValue(0.15f);
            myEffect.Parameters["distortStrength"].SetValue(0.1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.04f);

            myEffect.Parameters["vignetteSize"].SetValue(0.25f);
            myEffect.Parameters["vignetteBlend"].SetValue(1f);
            myEffect.Parameters["colorIntensity"].SetValue(2f * glowPower);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(glowTex, drawPos, null, Color.White, 0f, glowTex.Size() / 2f, scale, 0);
            Main.EntitySpriteDraw(glowTex, drawPos, null, Color.White, 0f, glowTex.Size() / 2f, scale, 0);
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            #endregion

            return base.PreDraw(ref lightColor);
        }
    }

    //This doesn't actually do any damage. The damage is handled by a StrikeNPC call in CrystallineProj
    public class CrystallineVFX : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 22900;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;

        public Vector2 startPoint;
        public Vector2 endPoint;
        public float direction;

        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
            {
                direction = (endPoint - Projectile.Center).ToRotation();

                Dust p2 = Dust.NewDustPerfect(endPoint, ModContent.DustType<SoftGlowDust>(), Vector2.Zero, newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.5f, 0.65f) * 0.3f);
                p2.customData = DustBehaviorUtil.AssignBehavior_SGDBase(overallAlpha: 0.02f);

                Dust p3 = Dust.NewDustPerfect(endPoint, ModContent.DustType<SoftGlowDust>(), Vector2.Zero, newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.5f, 0.65f) * 0.15f);
                p3.customData = DustBehaviorUtil.AssignBehavior_SGDBase(overallAlpha: 0.03f);

                //Sound
                int soundVar = Main.rand.Next(0, 3);
                SoundStyle style5 = new SoundStyle("Terraria/Sounds/Custom/dd2_lightning_bug_zap_" + soundVar) with { Volume = 0.35f, Pitch = 0.51f, PitchVariance = 0.15f, MaxInstances = -1 };
                SoundEngine.PlaySound(style5, endPoint);

                SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/lightning_flash_01") with { Volume = 0.08f, Pitch = 1f, PitchVariance = 0.2f, MaxInstances = -1 };
                SoundEngine.PlaySound(stylea, endPoint);
            }

            Projectile.spriteDirection = direction.ToRotationVector2().X > 0 ? 1 : -1;

            if (timer == 0)
            {
                float length = (endPoint - startPoint).Length();
                int relativeMidpoints = 1 + (int)(2 * (length / 400f));

                if (length < 100)
                    relativeMidpoints = 0;

                int numberOfMidpoints = relativeMidpoints;// 3 + (Main.rand.NextBool() ? 1 : 0); //Make relative to distance between start and end later

                //Add the start point
                trailPositions.Add(startPoint);

                //Add the midpoints
                float distance = startPoint.Distance(endPoint);
                for (int i = 1; i <= numberOfMidpoints; i++)
                {
                    float distanceBetweenMidpoints = distance * (1f / (numberOfMidpoints + 1f));

                    Vector2 newMidPointBasePosition = startPoint + direction.ToRotationVector2() * (distanceBetweenMidpoints * i);

                    //Offset the position vertically by a random amount (rotated by direction)
                    float verticalOffset = Main.rand.NextFloat(-25f, 25f);
                    float horizontalOffset = Main.rand.NextFloat(-15f, 15f);

                    newMidPointBasePosition += new Vector2(horizontalOffset, verticalOffset).RotatedBy(direction);

                    trailPositions.Add(newMidPointBasePosition);
                }

                //Add the end point
                trailPositions.Add(endPoint);

                //Calculate point rotations
                for (int i = 0; i < trailPositions.Count - 1; i++)
                {
                    Vector2 thisPoint = trailPositions[i];
                    Vector2 nextPoint = trailPositions[i + 1];

                    float rot = (nextPoint - thisPoint).ToRotation();
                    trailRotations.Add(rot);
                }

                //Add final rotation
                trailRotations.Add(trailRotations[trailRotations.Count - 1]);
                originalPoints = trailPositions;

                //Do dust
                //Dust from orb
                for (int i = 0; i < 3 + Main.rand.Next(0, 3); i++) //4 //2,2
                {
                    Vector2 vel = Main.rand.NextVector2Circular(5f, 5f) * 1f;
                    vel += trailRotations[0].ToRotationVector2() * 9f;

                    Dust p = Dust.NewDustPerfect(startPoint, ModContent.DustType<PixelatedLineSpark>(), vel, newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.9f, 1.15f) * 0.4f);

                    p.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.9f, preShrinkPower: 0.99f, postShrinkPower: 0.89f, timeToStartShrink: 8 + Main.rand.Next(-5, 5), killEarlyTime: 40,
                        0.75f, 0.5f, shouldFadeColor: false);
                }

                //End dust
                for (int i = 0; i < 6 + Main.rand.Next(0, 4); i++) //4 //2,2
                {
                    Vector2 vel = Main.rand.NextVector2Circular(7f, 7f) * 1f;

                    Dust p = Dust.NewDustPerfect(endPoint, ModContent.DustType<PixelatedLineSpark>(), vel,
                        newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.9f, 1.15f) * 0.4f);

                    p.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.89f, preShrinkPower: 0.99f, postShrinkPower: 0.89f, timeToStartShrink: 4 + Main.rand.Next(-5, 5), killEarlyTime: 40,
                        0.75f, 0.5f, shouldFadeColor: false);

                }
            }

            if (timer % 1 == 0 && timer != 0)
            {
                for (int i = 1; i < trailRotations.Count - 1; i++)
                {
                    trailPositions[i] = originalPoints[i] + Main.rand.NextVector2Circular(10f, 10f);

                }
            }

            if (timer < 9)
                Lighting.AddLight(endPoint, Color.DeepSkyBlue.ToVector3() * 0.9f);

            if (timer == 15) //10
                Projectile.active = false;

            timer++;
        }

        public List<float> trailRotations = new List<float>();
        public List<Vector2> trailPositions = new List<Vector2>();

        public List<Vector2> originalPoints = new List<Vector2>();


        public override bool PreDraw(ref Color lightColor)
        {
            if (trailPositions.Count == 0)
                return false;

            Texture2D flare = CommonTextures.CrispStarPMA.Value;

            Vector2 startPos = trailPositions[0] - Main.screenPosition;
            Vector2 endPos = trailPositions[trailPositions.Count - 1] - Main.screenPosition;
            float startRot = trailRotations[0];
            float endRot = trailRotations[trailPositions.Count - 1];


            float vfxTime = (float)Main.timeForVisualEffects;
            float elboost = Easings.easeInOutBack(Utils.GetLerpValue(0, 7, timer, true), 0f, 5f) * Utils.GetLerpValue(15, 10, timer, true); //8
            elboost = Math.Clamp(elboost, 0.2f, 10f);

            Vector2 vec2Scale = new Vector2(1f, 0.85f) * 1.5f * elboost;


            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                Color betweenBlue = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.7f);

                Main.EntitySpriteDraw(flare, startPos, null, betweenBlue with { A = 0 }, startRot, flare.Size() / 2f, vec2Scale * 0.5f, SpriteEffects.None);
                Main.EntitySpriteDraw(flare, startPos, null, Color.White with { A = 0 }, startRot, flare.Size() / 2f, vec2Scale * 0.25f, SpriteEffects.None);

                Main.EntitySpriteDraw(flare, endPos, null, betweenBlue with { A = 0 }, endRot + (vfxTime * 0.2f * Projectile.spriteDirection), flare.Size() / 2f, vec2Scale * 0.5f, SpriteEffects.None);
                Main.EntitySpriteDraw(flare, endPos, null, Color.White with { A = 0 }, endRot, flare.Size() / 2f, vec2Scale * 0.25f, SpriteEffects.None);

                DrawTrail();
            });

            return false;
        }

        Effect myEffect = null;
        public void DrawTrail()
        {
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;

            Color betweenBlue = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.75f);

            #region shaderPrep
            Texture2D trailTexture = CommonTextures.Extra_196_Black.Value;
            Texture2D trailTexture2 = CommonTextures.ThinGlowLine.Value;

            Vector2[] pos_arr = trailPositions.ToArray();
            float[] rot_arr = trailRotations.ToArray();

            float sineWidthMult = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.3f) * 0.15f;
            float elboost = Easings.easeInOutBack(Utils.GetLerpValue(0, 7, timer, true), 0f, 8f) * Utils.GetLerpValue(15, 10, timer, true);
            elboost = Math.Clamp(elboost, 0.2f, 10f);

            Color StripColor(float progress) => Color.White * 1f;
            float StripWidth(float progress) => 35f * 1f * sineWidthMult * 0.5f * elboost;
            float StripWidth2(float progress) => 80f * 1f * sineWidthMult * 0.5f * elboost;

            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStrip2 = new VertexStrip();
            vertexStrip2.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth2, -Main.screenPosition, includeBacksides: true);
            #endregion

            #region Shader
            float dist = (endPoint - startPoint).Length();
            float repValue = dist / 400f;
            myEffect.Parameters["reps"].SetValue(repValue * 0.8f);

            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            myEffect.Parameters["progress"].SetValue((float)Main.timeForVisualEffects * -0.02f);

            //UnderLayer
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture2);
            myEffect.Parameters["glowThreshold"].SetValue(1f);
            myEffect.Parameters["glowIntensity"].SetValue(1f);
            myEffect.Parameters["ColorOne"].SetValue(betweenBlue.ToVector3() * 1f);
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip2.DrawTrail();

            //Over layer
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            myEffect.Parameters["ColorOne"].SetValue(Color.SkyBlue.ToVector3() * 2f);
            myEffect.Parameters["glowThreshold"].SetValue(0.8f);
            myEffect.Parameters["glowIntensity"].SetValue(1.27f);
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            #endregion
        }
    }
}