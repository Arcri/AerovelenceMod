using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;
using Microsoft.CodeAnalysis;
using AerovelenceMod.Content.Projectiles;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Bows
{
    public class WarBowHeldProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("War Bow");
        }

        public int OFFSET = 10; //15
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public float lerpage = 0.82f;

        public float lerpToStuff = 0;

        int timer = 0;

        int timeBeforeKill = 0; //once the player lets go, this will start ticking up and the proj will die after a certain time

        float percentDrawnBack = 0;

        int skillCritWindow = 0;
        public bool runOnceCharge = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1;
        }


        public override bool? CanDamage()
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public int projToShootID = ProjectileID.WoodenArrowFriendly;

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            Player.itemTime = 2; // Set Item time to 2 frames while we are used
            Player.itemAnimation = 2; // Set Item animation time to 2 frames while we are used

            if (Player.channel)
            {
                //Main.NewText(Player.direction);
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (Player.MountedCenter)).ToRotation();
                }

                direction = Angle.ToRotationVector2();
                Player.ChangeDir(direction.X > 0 ? 1 : -1);

                percentDrawnBack = MathHelper.Clamp(percentDrawnBack + 0.03f, 0f, 1f);
                if (timer < 10)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);
                else if (timer < 20)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2);
                else
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, Projectile.rotation - MathHelper.PiOver2);

                if (percentDrawnBack >= 1f && runOnceCharge)
                {
                    runOnceCharge = false;
                    skillCritWindow = 10;
                }
            }
            else
            {
                Vector2 projPosition = (Projectile.position - (0.5f * (direction * OFFSET * -1)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY).Floor();


                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, Projectile.rotation - MathHelper.PiOver2);

                if (timeBeforeKill == 0)
                {
                    float vel = MathHelper.Clamp(20 * percentDrawnBack, 6, 20);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, direction.SafeNormalize(Vector2.UnitX) * vel, projToShootID, (Projectile.damage / 2 + (int)(Projectile.damage * percentDrawnBack)), 0, Player.whoAmI);

                    if (percentDrawnBack == 1f)
                    {
                        WarBowTrail globalProjectile = proj.GetGlobalProjectile<WarBowTrail>();
                        globalProjectile.trailActive = true;

                        if (skillCritWindow >= 0) globalProjectile.trailColor = Color.Gold * 0.5f;
                        if (skillCritWindow >= 0) globalProjectile.sparkColor = Color.Gold;

                    }


                    if (skillCritWindow >= 0)
                    {
                        ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                        for (int j = 0; j < 6; j++)
                        {
                            Dust dust1 = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + direction.SafeNormalize(Vector2.UnitX) * 16, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 3),
                                Color.Orange, Main.rand.NextFloat(0.3f, 0.5f), 0.6f, 0f, dustShader2);
                            dust1.velocity *= 1f;

                        }
                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(4f, 4f) * 0.75f;
                            Dust gd = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + randomStart + direction.SafeNormalize(Vector2.UnitX) * 16, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.Orange, 0.2f, 0.4f, 0f, dustShader2);
                            gd.fadeIn = 50 + Main.rand.NextFloat(-3f, 4f);
                        }

                        SkillStrikeUtil.setSkillStrike(proj, 1.3f);

                        SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_1") with { Pitch = .54f, };
                        SoundEngine.PlaySound(style3, Player.Center);
                    }
                    else
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_javelin_throwers_attack_1") with { Pitch = .85f, PitchVariance = 0.1f, Volume = 0.5f + (0.5f * percentDrawnBack) };
                        SoundEngine.PlaySound(style, Player.Center);
                    }

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_5") with { Pitch = -.53f, };
                    SoundEngine.PlaySound(style2, Player.Center);

                }

                if (timeBeforeKill >= 40)
                    Projectile.active = false;
                timeBeforeKill++;
            }

            //lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.06f), 0, 0.4f);


            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (Player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);

            if (skillCritWindow >= 0)
            {
                //Main.NewText("starScaleMultiplier - " + starScaleMultiplier);
                starScaleMultiplier = MathHelper.Clamp(skillCritWindow, 2, 20);
                outerStarScale = 0.05f * starScaleMultiplier * 0.1f;
                innerStarScale = 0.035f * starScaleMultiplier * 0.1f;
            }

            skillCritWindow--;
            timer++;
        }

        float starScaleMultiplier = 0;
        float outerStarScale = 0;
        float innerStarScale = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * OFFSET * -1)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            if (Player.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation(), origin, Projectile.scale, effects1, 0.0f);

            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation() - 3.14f, origin, Projectile.scale, effects1, 0.0f);
            }


            //Arrow Drawing
            Texture2D arrowTexture = TextureAssets.Projectile[projToShootID].Value;
            Vector2 origin2 = new Vector2((float)arrowTexture.Width / 2f, (float)arrowTexture.Height / 2f);
            Vector2 position2 = (Projectile.position - (0.5f * (direction * OFFSET * -1.5f)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            Vector2 chargeOffset = new Vector2(-5 * percentDrawnBack, 0).RotatedBy(direction.ToRotation());

            Vector2 lineOffsetRot1 = new Vector2(0f, -15f).RotatedBy(Projectile.rotation);
            Vector2 lineOffsetRot2 = new Vector2(0f, 15f).RotatedBy(Projectile.rotation);

            Vector2 arrowButtPos = position2 + chargeOffset + new Vector2(0, -13).RotatedBy(direction.ToRotation() - MathHelper.PiOver2);
            if (Player.channel)
            {
                Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot1, arrowButtPos + Main.screenPosition, lightColor, lightColor, 1.5f);
                Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot2, arrowButtPos + Main.screenPosition, lightColor, lightColor, 1.5f);
            }
            else
            {
                Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot1, Projectile.Center + lineOffsetRot2, lightColor, lightColor, 1.5f);
                //Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot2, Projectile.Center + lineOffsetRot1, lightColor, lightColor, 1.5f);
            }


            if (Player.channel)
            {
                Main.spriteBatch.Draw(arrowTexture, position2 + chargeOffset, null, skillCritWindow >= 0 ? Color.Gold : lightColor, direction.ToRotation() - MathHelper.PiOver2 + 3.14f, origin2, 1f, Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);

                //SkillStrike
                Main.spriteBatch.Draw(arrowTexture, position2 + chargeOffset, null, skillCritWindow >= 0 ? Color.Gold with { A = 0 } * 0.25f : Color.White * 0f, direction.ToRotation() - MathHelper.PiOver2 + 3.14f, origin2, 1f, Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);
            }

            //Texture2D shineTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
            Texture2D shineTexture = Mod.Assets.Request<Texture2D>("Assets/TrailImages/CrispStar").Value;

            Vector2 arrowTipPos = position2 + chargeOffset + new Vector2(0, 10).RotatedBy(direction.ToRotation() - MathHelper.PiOver2);

            if (skillCritWindow >= 0)
            {
                //outerStarScale = 0.08f;
                //innerStarScale = 0.065f;
                //float scaleMultiplier = skillCritWindow / 10;

                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                Main.spriteBatch.Draw(shineTexture, arrowTipPos, shineTexture.Frame(1, 1, 0, 0), Color.Gold with { A = 0 } * 0.75f, MathHelper.ToRadians(timer * 2),
                    shineTexture.Size() / 2, innerStarScale * 15f, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(shineTexture, arrowTipPos, shineTexture.Frame(1, 1, 0, 0), Color.Orange with { A = 0 } * 0.75f, MathHelper.ToRadians(timer * 2) + (float)Math.PI,
                    shineTexture.Size() / 2, innerStarScale * 15f, SpriteEffects.None, 0f);

                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }

    }

    public class WarBowTrail : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool trailActive = false;
        public float trailType = 1;
        public float trailIntensity = 1f;
        public Color trailColor = Color.Gray * 0.5f;
        public Color sparkColor = Color.White;

        private BaseTrailInfo fireTrail = new BaseTrailInfo();
        public List<Vector2> previousPositions = new List<Vector2>();
        int timer = 0;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!trailActive)
            {
                base.OnHitNPC(projectile, target, hit, damageDone);
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                float arrowVel = Math.Clamp(projectile.velocity.Length(), 5f, 11f);
                Vector2 randomStart = Main.rand.NextVector2Circular(3f, 3f) * 1f;
                Dust dust = Dust.NewDustPerfect(projectile.Center, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: sparkColor * 0.5f, Scale: Main.rand.NextFloat(0.5f, 0.6f));
                dust.velocity += projectile.velocity.SafeNormalize(Vector2.UnitX) * arrowVel * 0.5f;

                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.97f, timeBeforeSlow: 6, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.85f, shouldFadeColor: true);
            }

            for (int i = 0; i < 1; i++)
            {
                
                int a = Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<WarBowSpike>(), (int)(hit.Damage / 6f), 0f, Main.player[projectile.owner].whoAmI);
                Main.projectile[a].rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                if (Main.projectile[a].ModProjectile is WarBowSpike wbs)
                {
                    wbs.relativeNPCPos = (projectile.Center - target.Center);
                    wbs.IsStickingToTarget = true;
                    wbs.TargetWhoAmI = target.whoAmI;
                }
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (!trailActive) return;
            fireTrail.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value;
            fireTrail.trailColor = Color.Gray * 0.5f;
            fireTrail.trailPointLimit = (int)(200 * projectile.scale);
            fireTrail.trailWidth = (int)(10 * projectile.scale);
            fireTrail.trailMaxLength = (int)(200 * projectile.scale); //225
            fireTrail.timesToDraw = 1;
            fireTrail.pinch = false;
            fireTrail.pinchAmount = 0.4f;

            fireTrail.trailTime = (float)Main.timeForVisualEffects * 0.05f;
            fireTrail.trailRot = projectile.velocity.ToRotation();
            fireTrail.trailPos = projectile.Center + projectile.velocity;
            fireTrail.TrailLogic();

            if (timer % 1 == 0)
            {
                previousPositions.Add(projectile.Center);

                int trailCount = 10;
                if (previousPositions.Count > trailCount)
                {
                    previousPositions.RemoveAt(0);
                }
            }

            timer++;
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (!trailActive) return base.PreDraw(projectile, ref lightColor);

            fireTrail.TrailDrawing(Main.spriteBatch);
            Vector2 scale = new Vector2(0.25f, 0.65f) * projectile.scale;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Flare").Value;
            Main.EntitySpriteDraw(glow, projectile.Center - Main.screenPosition, null, sparkColor with { A = 0 } * 0.25f, projectile.rotation, glow.Size() / 2, scale, SpriteEffects.None);
            Main.EntitySpriteDraw(glow, projectile.Center - Main.screenPosition, null, sparkColor with { A = 0 } * 0.75f, projectile.rotation, glow.Size() / 2, scale * 0.75f, SpriteEffects.None);


            return base.PreDraw(projectile, ref lightColor);
        }

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (!trailActive)
            {
                base.PostDraw(projectile, lightColor);
                return;
            }
            //Vector2 scale = new Vector2(0.25f, 0.65f) * projectile.scale;
            //Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Flare").Value;
            //Main.EntitySpriteDraw(glow, projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.25f, projectile.rotation, glow.Size() / 2, scale, SpriteEffects.None);
            //Main.EntitySpriteDraw(glow, projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.25f, projectile.rotation, glow.Size() / 2, scale * 0.5f, SpriteEffects.None);

            base.PostDraw(projectile, lightColor);
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (trailActive)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 randomStart = Main.rand.NextVector2Circular(2f, 2f) * 1f;
                    Dust dust = Dust.NewDustPerfect(projectile.Center, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: sparkColor, Scale: Main.rand.NextFloat(0.55f, 0.7f));
                    dust.velocity += projectile.velocity * 0.15f;

                    dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                        rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.88f, shouldFadeColor: false);
                }
            }
            base.OnKill(projectile, timeLeft);
        }
    }

    public class WarBowSpike : ModProjectile
    {
        // mfw example mod code
        public bool IsStickingToTarget
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        public int TargetWhoAmI
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public float StickTimer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public Vector2 relativeNPCPos = Vector2.Zero;

        public override void SetDefaults()
        {
            Projectile.width = 16; 
            Projectile.height = 16;
            Projectile.friendly = false; //Deals damage via Striking
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged; 
            Projectile.timeLeft = 150; 
            Projectile.ignoreWater = true; 
            Projectile.tileCollide = true; 
            Projectile.hide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 0.9f;
        }

        public override void AI()
        {
            if (IsStickingToTarget)
            {
                Projectile.tileCollide = false;
                StickTimer += 1f;

                // Hit every 18 ticks
                bool hitEffect = StickTimer % 22f == 0f;

                int npcTarget = TargetWhoAmI;
                if (StickTimer >= StickTime || npcTarget < 0 || npcTarget >= 200)
                { 
                    Projectile.Kill();
                }
                else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage)
                {
                    // Set the projectile's position relative to the target's center
                    Projectile.Center = Main.npc[npcTarget].Center + relativeNPCPos * 0.95f;
                    //Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                    if (hitEffect)
                    {
                        // Perform a hit effect here, causing the npc to react as if hit.
                        // Note that this does NOT damage the NPC, the damage is done through the debuff.
                        //Main.npc[npcTarget].HitEffect(0, 1.0);

                        //We want to strike the npc without hitsounds because it sounds bad when it hits this often
                        //We do this by replacing the npc's hitsound with a sound at 0 volume before the Strike, and set it back after

                        SoundStyle style = new SoundStyle("Terraria/Sounds/Item_1") with { Volume = 0f, MaxInstances = -1 };

                        SoundStyle? storedHitsound = Main.npc[npcTarget].HitSound;

                        Main.npc[npcTarget].HitSound = style;

                        Main.npc[npcTarget].StrikeNPC(Main.npc[npcTarget].CalculateHitInfo(Projectile.damage, 1, false, 0, DamageClass.Ranged, true));

                        Main.npc[npcTarget].HitSound = storedHitsound;


                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 vel = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.25f) * Main.rand.NextFloat(1f, 2.5f);
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), vel, newColor: Color.Crimson, Scale: Main.rand.NextFloat(0.3f, 0.45f));
                            
                            dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.88f, shouldFadeColor: false);
                        }
                        extraScaleMult = 1.25f;
                    }
                }
                else
                { 
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
                Projectile.timeLeft -= 5;
            }

            alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.25f, 0.1f), 0f, 1f);
            extraScaleMult = Math.Clamp(MathHelper.Lerp(extraScaleMult, 0.5f, 0.1f), 1f, 2f);

        }

        private int StickTime = 60 * 7; // 10 seconds
    
        public override void OnKill(int timeLeft)
        {
            //SoundEngine.PlaySound(SoundID.Dig, Projectile.position); // Play a death sound
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.25f) * Main.rand.NextFloat(1f, 3f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), vel, newColor: Color.Gray, Scale: Main.rand.NextFloat(0.4f, 0.55f));

                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.88f, shouldFadeColor: true);
            }
        }

        float alpha = 0f;
        float extraScaleMult = 2f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D spike = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Bows/WarBowSpike").Value;
            Main.EntitySpriteDraw(spike, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.rotation, spike.Size() / 2, Projectile.scale * extraScaleMult, SpriteEffects.None);
            Main.EntitySpriteDraw(spike, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1f, 1f), null, Color.White with { A = 0 } * 0.25f * alpha, Projectile.rotation, spike.Size() / 2, new Vector2(Projectile.scale * 0.5f, Projectile.scale) * extraScaleMult, SpriteEffects.None);

            return false;
        }
    }
}