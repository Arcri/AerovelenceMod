using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Basic.Reference.Assemblies.Net80;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AerovelenceMod.Content.Items.Weapons.Crimson
{
    public class Marionette : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.damage = 30;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Summon;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<MarionetteProj>();

            Item.rare = ItemRarityID.Orange;

            Item.channel = true;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
        }
    }

    public class MarionetteProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.hide = true;
        }

        #region swingPhysicVars
        float barsRot;
        Vector2 stillPos;
        Vector2 barLoc = Vector2.Zero;
        float acc = 0f;
        float accBack = 0f;
        float accActualBack = 0.002f;
        bool rotStop = false;
        int rememberDir;
        #endregion
        int maxDist = 200;
        Vector2 marionetteLoc = Vector2.Zero;
        public override void AI()
        {
            Player p = Main.player[Projectile.owner];

            if (barLoc == Vector2.Zero)
            {
                barLoc = Main.MouseWorld;
            }

            marionetteLoc = Main.MouseWorld;

            if (p.channel)
            {
                Projectile.timeLeft = 3;
                p.ChangeDir(Projectile.direction);
                p.heldProj = Projectile.whoAmI;
                p.itemTime = 3;
                p.itemAnimation = 3;
                p.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
                Projectile.spriteDirection = Projectile.direction;
            }

            p.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2);
            Vector2 ArmPosition = p.RotatedRelativePoint(p.MountedCenter, false, false);
            float RotationOffset = Utils.GetLerpValue(5, 255, Projectile.Distance(Main.MouseWorld), true);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld), RotationOffset);
            Projectile.Center = ArmPosition;
            Vector2 projPos = Projectile.Center += Projectile.velocity * 15;

            for (int i = 0; i < 4; i++)
            {
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    if (verletAttack[i] == 0 && verletStretch[i] == 0 && Vector2.Distance(Main.npc[n].Center, verletEndPos[i] + Main.screenPosition) < 30)
                    {
                        int freeToStick = 0;
                        for (int ii = 0; ii < 4; ii++)
                        {
                            if (verletStickedTo[ii] != Main.npc[n]) //only one string per enemy
                            {
                                freeToStick++;
                            }
                            else if (verletStickedTo[ii] == Main.npc[n] && Main.npc[n].boss) //multiple allowed on bosses
                            {
                                freeToStick++;
                            }
                        }

                        if (freeToStick == 4)
                        {
                            verletAttack[i] = 1;
                            verletStickedTo[i] = Main.npc[n];
                        }
                    }
                }

                if (verletAttack[i] == 1)
                {
                    //verletEndPos[i] = verletStickedTo[i].Center - Main.screenPosition;

                    if (--verletHitCD[i] <= 0)
                    {
                        int howManyAttached = 0;
                        for (int a = 0; a < 4; a++)
                        {
                            if (verletStickedTo[a] != null)
                            {
                                howManyAttached++;
                            }
                        }

                        verletHitCD[i] = 15 * howManyAttached;
                        bool skillStrike = false;
                        bool isCrit = false;
                        NPC.HitInfo hit = verletStickedTo[i].CalculateHitInfo(Projectile.damage, 0, isCrit ? true : false);



                        if (howManyAttached == 4)
                        {
                            skillStrike = true;
                        }

                        int critChanceMult = 3 * howManyAttached;

                        if (verletChoke[i] > 200)
                        {
                            if (Main.rand.Next(100) < (10 + critChanceMult))
                            {
                                isCrit = true;
                            }
                        }

                        if (!skillStrike)
                        {
                            hit.HideCombatText = false;
                            verletStickedTo[i].StrikeNPC(hit);
                        }
                        else
                        {
                            var target = verletStickedTo[i];

                            var multiplier = 1.5f;
                            var skillStrikeMultiplier = multiplier * p.GetModPlayer<SkillStrikePlayer>().skillStrikeMultiplier;
                            var superCritMultiplier = multiplier * p.GetModPlayer<SkillStrikePlayer>().superCritMultiplier;

                            hit.Damage *= (int)skillStrikeMultiplier;
                            if (isCrit)
                                hit.Damage *= (int)superCritMultiplier;

                            //visuals
                            SkillStrikeUtil.fakeSkillStrike(p, target, target.Center, crit: hit.Crit);
                            Vector2 randomSpawnPos = Main.rand.NextVector2FromRectangle(new Rectangle((int)target.Center.X, (int)target.Center.Y - 20, target.width, (int)(target.height * 0.75f)));
                            Dust text = Dust.NewDustPerfect(randomSpawnPos, ModContent.DustType<SkillStrikeText>(), new Vector2(0f, -12f), Scale: 1f);

                            SkillStrikeTextBehavior sstb = new SkillStrikeTextBehavior();
                            sstb.isCrit = hit.Crit;
                            sstb.damageNumber = "" + hit.Damage;

                            text.customData = sstb;
                        }
                        //is there custom behaviour for super crits?

                    }

                    if (Vector2.Distance(Main.MouseWorld, verletStickedTo[i].Center) > (int)(maxDist * 1.75f))
                    {
                        ResetValues(i);
                    }

                    if (verletStickedTo[i] != null && !verletStickedTo[i].active)
                    {
                        ResetValues(i);
                    }

                    if (verletChoke[i] < 300)
                    {
                        verletChoke[i]++;
                        //implement color
                    }
                    else
                    {
                        ResetValues(i);
                    }
                }
            }


            #region swingPhysics
            Vector2 dir = (Main.MouseWorld - barLoc).SafeNormalize(Vector2.Zero);

            if (Vector2.Distance(barLoc, Main.MouseWorld) > 15)
            {
                accBack = 0f;
                accActualBack = 0.002f;
                rotStop = false;

                if (acc < 1f)
                    acc += 0.001f;
                if (barLoc.X < Main.MouseWorld.X)
                {
                    if (barsRot < 0.5f)
                    {
                        barsRot += acc;
                        rememberDir = 1;
                    }
                }

                if (barLoc.X > Main.MouseWorld.X)
                {
                    if (barsRot > -0.5f)
                    {
                        barsRot -= acc;
                        rememberDir = -1;
                    }
                }

                barLoc += dir * 10f;

                int maxDist = 150;
                if (Vector2.Distance(barLoc, Main.MouseWorld) > maxDist)
                {
                    dir = (barLoc - Main.MouseWorld).SafeNormalize(Vector2.Zero);
                    barLoc = Main.MouseWorld + dir * maxDist;
                }

            }
            else
            {
                acc = 0f;
                if (accBack < 1f)
                    accBack += 0.001f;

                if (!rotStop)
                {
                    if (rememberDir == -1)
                    {
                        barsRot += accBack;
                        if (barsRot > 0f)
                        {
                            accBack -= accActualBack;
                            if (accActualBack > 0.0009f)
                            {
                                accActualBack -= 0.00001f;
                            }
                            else
                            {
                                rotStop = true;
                                accActualBack = 0f;
                            }
                        }

                    }
                    else
                    {
                        barsRot -= accBack;
                        if (barsRot < 0f)
                        {
                            accBack -= accActualBack;
                            if (accActualBack > 0.0009f)
                            {
                                accActualBack -= 0.00001f;
                            }
                            else
                            {
                                rotStop = true;
                                accActualBack = 0f;
                            }
                        }

                    }
                }
                else
                {

                    accActualBack += 0.001f;
                    if (barsRot > 0f)
                    {
                        barsRot -= accActualBack;
                    }

                    if (barsRot < 0f)
                    {
                        barsRot += accActualBack;
                    }
                }


                barLoc = Main.MouseWorld;
            }
            #endregion
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                ResetValues(i);
            }
        }

        Vector2[] verletEndPos = { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        Vector2[] verletSpeed = { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        NPC[] verletStickedTo = { null, null, null, null };
        int[] verletAttack = { 0, 0, 0, 0 };
        int[] verletHitCD = { 0, 0, 0, 0 };
        int[] verletChoke = { 0, 0, 0, 0 };
        int[] verletStretch = { 0, 0, 0, 0 };

        #region PreDraw
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Mod.Name + "/Content/Items/Weapons/Crimson/MarionetteStringless").Value;
            Rectangle sourceRect = new Rectangle(0, 0, tex.Width, tex.Height);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, sourceRect, Color.White, Projectile.rotation, sourceRect.Size() / 2, 1f, Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);


            Vector2 barsLoc = new Vector2(Main.MouseWorld.X, Main.MouseWorld.Y + 16) - Main.screenPosition;

            #region VerletStuff
            Vector2 verletPos = new Vector2(barsLoc.X - 22, barsLoc.Y - 6);
            verletPos.Y += -40 * barsRot;
            for (int i = 0; i < 4; i++)
            {
                Color col = Color.Red;
                col = Color.White;
                if (i == 1)
                {
                    verletPos = new Vector2(barsLoc.X + 22, barsLoc.Y - 6);
                    verletPos.Y += 40 * barsRot;
                    col = Color.White;
                }
                else if (i == 2)
                {
                    verletPos = new Vector2(barsLoc.X - 30, barsLoc.Y + 12);
                    verletPos.Y += -20 * barsRot;
                    col = Color.White;
                }
                else if (i == 3)
                {
                    verletPos = new Vector2(barsLoc.X + 30, barsLoc.Y + 12);
                    verletPos.Y += 20 * barsRot;
                    col = Color.White;
                }
                Vector2 actualVerletEndPos = new Vector2(verletPos.X, verletPos.Y + maxDist * 1.5f);
                if (verletEndPos[i] == Vector2.Zero)
                    verletEndPos[i] = verletPos;

                if (verletAttack[i] == 0)
                {
                    Vector2 direction = (actualVerletEndPos - verletEndPos[i]).SafeNormalize(Vector2.Zero);
                    verletSpeed[i] += direction / (verletStretch[i] > 300 ? 3 : 5);
                    verletSpeed[i] *= 0.99f;
                }

                float maxSpeed = 10f;
                if (verletSpeed[i].LengthSquared() > maxSpeed * maxSpeed)
                {
                    verletSpeed[i] = verletSpeed[i].SafeNormalize(Vector2.Zero) * maxSpeed;
                }

                verletEndPos[i] += verletSpeed[i];

                if (verletStretch[i] > 0)
                {
                    verletStretch[i] -= 6;
                }

                if (Vector2.Distance(verletEndPos[i], barsLoc) > maxDist)
                {
                    Vector2 direction = (verletEndPos[i] - barsLoc).SafeNormalize(Vector2.Zero);
                    verletEndPos[i] = barsLoc + direction * maxDist;
                }

                if (verletAttack[i] == 0)
                {
                    if (verletStretch[i] > 0)
                    {
                        DrawVerlet(verletPos, verletEndPos[i], Color.White, (float)verletStretch[i] / 500);
                    }
                    else
                    {
                        DrawVerlet(verletPos, verletEndPos[i], Color.White, 0f);
                    }
                }
                else if (verletAttack[i] == 1)
                {
                    float dist = Vector2.Distance(Main.MouseWorld, verletStickedTo[i].Center);

                    DrawVerlet(verletPos, (verletStickedTo[i].Center - Main.screenPosition), Color.White, (dist / 150000) * dist);
                    verletEndPos[i] = (verletStickedTo[i].Center - Main.screenPosition);
                    verletStretch[i] = (int)(maxDist * 1.75f);
                }
            }
            #endregion

            tex = ModContent.Request<Texture2D>(Mod.Name + "/Content/Items/Weapons/Crimson/MarionetteBar").Value;
            sourceRect = new Rectangle(0, 0, tex.Width, tex.Height);
            Main.EntitySpriteDraw(tex, barsLoc, sourceRect, Color.Gray, 0.6f + barsRot, sourceRect.Size() / 2, 1f, SpriteEffects.None);
            Main.EntitySpriteDraw(tex, barsLoc, sourceRect, Color.White, 0f + barsRot, sourceRect.Size() / 2, 1f, SpriteEffects.None);

            return false;
        }
        #endregion

        #region DrawVerlet

        public void DrawVerlet(Vector2 verletPos1, Vector2 verletPos2, Color col, float lerp = 0f)
        {
            Rectangle pixelSource = new Rectangle(0, 0, 2, 2);

            float calcDist = 1f;
            Vector2 dir = (verletPos2 - verletPos1).SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 5000; i++)
            {
                Vector2 stepCalcPos = verletPos1 + dir * calcDist;
                if (Vector2.Distance(stepCalcPos, verletPos2) < 1)
                {
                    break;
                }
                calcDist += 1f;
            }

            List<Vector2> points = new List<Vector2>();
            points.Add(verletPos1);

            #region MiddlePoints (done manually so it looks nice)
            Vector2 middlePoint = verletPos1 + dir * (calcDist * 0.15f);
            middlePoint.Y += 16;

            middlePoint = verletPos1 + dir * (calcDist * 0.25f);
            middlePoint.Y += 24;
            points.Add(middlePoint);

            middlePoint = verletPos1 + dir * (calcDist * 0.35f);
            middlePoint.Y += 30;
            points.Add(middlePoint);

            middlePoint = verletPos1 + dir * (calcDist * 0.45f);
            middlePoint.Y += 35;
            points.Add(middlePoint);

            middlePoint = verletPos1 + dir * (calcDist * 0.5f);
            middlePoint.Y += 35;
            points.Add(middlePoint);

            middlePoint = verletPos1 + dir * (calcDist * 0.65f);
            middlePoint.Y += 35;
            points.Add(middlePoint);

            middlePoint = verletPos1 + dir * (calcDist * 0.75f);
            middlePoint.Y += 32;
            points.Add(middlePoint);

            middlePoint = verletPos1 + dir * (calcDist * 0.85f);
            middlePoint.Y += 24;
            points.Add(middlePoint);

            middlePoint = verletPos1 + dir * (calcDist * 0.95f);
            middlePoint.Y += 8;
            points.Add(middlePoint);
            #endregion

            points.Add(verletPos2);

            Color colA = Color.Crimson;
            Color colB = Color.Lerp(Color.Crimson, Color.White, 0.4f);
            float gradLerp = 0f;
            for (int s = 0; s < points.Count - 1; s++)
            {

                float progress = 0f;
                gradLerp += 0.1f;
                for (int i = 0; i < 5000; i++)
                {
                    Color gradColor = Color.Lerp(colA, colB, gradLerp);
                    Color accCol = Color.Lerp(gradColor, col, lerp);
                    Vector2 startPos = points[s];
                    Vector2 endPos = points[s + 1];
                    Vector2 direction = (endPos - startPos).SafeNormalize(Vector2.Zero);
                    Vector2 progPoint = startPos + direction * progress;
                    Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, progPoint, pixelSource, accCol, 0f, pixelSource.Size() / 2, 1f, SpriteEffects.None);
                    if (Vector2.Distance(progPoint, endPos) < 1)
                    {
                        break;
                    }
                    progress += 1f;
                }

            }

        }
        #endregion

        public void ResetValues(int i)
        {
            verletChoke[i] = 0;
            verletAttack[i] = 0;
            verletStickedTo[i] = null;
            //adding line breaking vfx
        }
    }
}
