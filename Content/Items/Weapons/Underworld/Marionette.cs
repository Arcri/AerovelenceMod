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

namespace AerovelenceMod.Content.Items.Weapons.Underworld
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
        }

        #region swingPhysicVars
        float barsRot;
        Vector2 stillPos;
        Vector2 barLoc = Vector2.Zero;
        float barSpeed = 10f;
        float acc = 0f;
        float accBack = 0f;
        float accActualBack = 0.002f;
        bool rotStop = false;
        int rememberDir;
        #endregion
        public override void AI()
        {
            Player p = Main.player[Projectile.owner];

            if (barLoc == Vector2.Zero)
            {
                barLoc = Main.MouseWorld;
            }

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
            Vector2 projPos = Projectile.Center += Projectile.velocity * 10;

            #region swingPhysics
            Vector2 dir = (Main.MouseWorld - barLoc).SafeNormalize(Vector2.Zero);

            if (Vector2.Distance(barLoc, Main.MouseWorld) > 100)
            {
                barSpeed = 50f;
            }
            else
                barSpeed = 10f;

            if (Vector2.Distance(barLoc, Main.MouseWorld) > 15)
            {
                accBack = 0f;
                accActualBack = 0.002f;
                rotStop = false;
                barLoc += dir * barSpeed;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        Vector2[] verletEndPos = { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        Vector2[] verletSpeed = { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Mod.Name + "/Content/Items/Weapons/Underworld/Marionette").Value;
            Rectangle sourceRect = new Rectangle(0, 0, tex.Width, tex.Height);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, sourceRect, Color.Gray, Projectile.rotation, sourceRect.Size() / 2, 1f, Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            Vector2 barsLoc = new Vector2(Main.MouseWorld.X, Main.MouseWorld.Y + 16) - Main.screenPosition;

            Vector2 verletPos = new Vector2(barsLoc.X - 25, (barsLoc.Y + barsRot) - 6);
            verletPos.Y += -40 * barsRot;
            for (int i = 0; i < 4; i++)
            {

                if (i == 1)
                {
                    verletPos = new Vector2(barsLoc.X + 25, (barsLoc.Y + barsRot) - 6);
                    verletPos.Y += 40 * barsRot;
                }
                else if (i == 2)
                {
                    verletPos = new Vector2(barsLoc.X - 30, (barsLoc.Y + barsRot) + 12);
                    verletPos.Y += -20 * barsRot;
                }
                else if (i == 3)
                {
                    verletPos = new Vector2(barsLoc.X + 30, (barsLoc.Y + barsRot) + 12);
                    verletPos.Y += 20 * barsRot;
                }


                Vector2 actualVerletEndPos = new Vector2(verletPos.X, verletPos.Y + 300);
                if (verletEndPos[i] == Vector2.Zero)
                    verletEndPos[i] = verletPos;

                Vector2 direction = (actualVerletEndPos - verletEndPos[i]).SafeNormalize(Vector2.Zero);
                verletSpeed[i] += direction / 10;
                verletSpeed[i] *= 0.99f;

                float maxSpeed = 10f;
                if (verletSpeed[i].LengthSquared() > maxSpeed * maxSpeed)
                {
                    verletSpeed[i] = verletSpeed[i].SafeNormalize(Vector2.Zero) * maxSpeed;
                }

                verletEndPos[i] += verletSpeed[i];
                DrawVerlet(verletPos, verletEndPos[i]);
            }

            tex = ModContent.Request<Texture2D>(Mod.Name + "/Content/Items/Weapons/Underworld/MarionetteBar").Value;
            sourceRect = new Rectangle(0, 0, tex.Width, tex.Height);

            Main.EntitySpriteDraw(tex, barsLoc, sourceRect, Color.Gray, 0.6f + barsRot, sourceRect.Size() / 2, 1f, SpriteEffects.None);
            Main.EntitySpriteDraw(tex, barsLoc, sourceRect, Color.White, 0f + barsRot, sourceRect.Size() / 2, 1f, SpriteEffects.None);

            return false;
        }

        #region DrawVerlet

        public void DrawVerlet(Vector2 verletPos1, Vector2 verletPos2)
        {
            Rectangle pixelSource = new Rectangle(0, 0, 2, 2);

            float calcDist = 1f;
            Vector2 dir = (verletPos2 - verletPos1).SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 5000; i++)
            {
                Vector2 stepCalcPos = verletPos1 + (dir * calcDist);
                if (Vector2.Distance(stepCalcPos, verletPos2) < 1)
                {
                    break;
                }
                calcDist += 1f;
            }

            List<Vector2> points = new List<Vector2>();
            points.Add(verletPos1);

            #region MiddlePoints (done manually so it looks nice)
            Vector2 middlePoint = verletPos1 + (dir * (calcDist * 0.15f));
            middlePoint.Y += 16;

            middlePoint = verletPos1 + (dir * (calcDist * 0.25f));
            middlePoint.Y += 24;
            points.Add(middlePoint);

            middlePoint = verletPos1 + (dir * (calcDist * 0.35f));
            middlePoint.Y += 30;
            points.Add(middlePoint);

            middlePoint = verletPos1 + (dir * (calcDist * 0.45f));
            middlePoint.Y += 35;
            points.Add(middlePoint);

            middlePoint = verletPos1 + (dir * (calcDist * 0.5f));
            middlePoint.Y += 35;
            points.Add(middlePoint);

            middlePoint = verletPos1 + (dir * (calcDist * 0.65f));
            middlePoint.Y += 35;
            points.Add(middlePoint);

            middlePoint = verletPos1 + (dir * (calcDist * 0.75f));
            middlePoint.Y += 32;
            points.Add(middlePoint);

            middlePoint = verletPos1 + (dir * (calcDist * 0.85f));
            middlePoint.Y += 24;
            points.Add(middlePoint);

            middlePoint = verletPos1 + (dir * (calcDist * 0.95f));
            middlePoint.Y += 8;
            points.Add(middlePoint);
            #endregion

            points.Add(verletPos2);

            for (int s = 0; s < points.Count - 1; s++)
            {

                float progress = 0f;
                for (int i = 0; i < 5000; i++)
                {
                    Vector2 startPos = points[s];
                    Vector2 endPos = points[(s + 1)];
                    Vector2 direction = (endPos - startPos).SafeNormalize(Vector2.Zero);
                    Vector2 progPoint = startPos + (direction * progress);
                    Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, progPoint, pixelSource, Color.White, 0f, pixelSource.Size() / 2, 1f, SpriteEffects.None);
                    if (Vector2.Distance(progPoint, endPos) < 1)
                    {
                        break;
                    }
                    progress += 1f;
                }

            }

        }
        #endregion
    }
}
