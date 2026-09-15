using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles.Other;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using static Basic.Reference.Assemblies.Net80;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AerovelenceMod.Content.Items.Weapons.Crimson
{
    public class Marionette : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            this.ModifyLocalization("Marionette", "Continuously drain mana to summon a set of handle bars and puppet strings at the cursor\nThe ends of the strings can attach to enemies and choke their arteries\nIf the handle bars get too far from a string's attach point, or if a string stays attached for too long, it will weaken and snap\nChoke damage gains a small crit chance for every string that is attached")
            .AddName(Language.Default, "The Marionette").AddTooltip(Language.Default, "Continuously drain mana to summon a set of handle bars and puppet strings at the cursor\nThe ends of the strings can attach to enemies and choke their arteries\nIf the handle bars get too far from a string's attach point, or if a string stays attached for too long, it will weaken and snap\nChoke damage gains a small crit chance for every string that is attached")
            .AddSkillStrike(Language.Default, "Skill Strikes when all strings are attached");

            //.AddName(Language.Spanish, "").AddSkillStrike(Language.Spanish, "")
            //.AddName(Language.French, "").AddSkillStrike(Language.French, "")
            //.AddName(Language.German, "").AddSkillStrike(Language.German, "")
            //.AddName(Language.Italian, "").AddSkillStrike(Language.Italian, "")
            //.AddName(Language.Polish, "").AddSkillStrike(Language.Polish, "")
            //.AddName(Language.PortugueseBrazil, "").AddSkillStrike(Language.PortugueseBrazil, "")
            //.AddName(Language.Russian, "").AddSkillStrike(Language.Russian, "");
            //.AddName(Language.ChineseTraditional, "").AddSkillStrike(Language.ChineseTraditional, "")
            //.AddName(Language.ChineseSimplified, "").AddSkillStrike(Language.ChineseSimplified, "")

        }
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
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, Main.MouseWorld.X, Main.MouseWorld.Y);
            return false;
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
            Projectile.extraUpdates = 0; //used to be three, trying to update strings more frequently than the bars but failing :broken_heart:
        }

        private float barsRot;
        private float angularVelocity;
        private Vector2 barLoc;
        private int age;
        private Vector2 previousScreenPosition;
        private Vector2 sentCursor;
        private const int maxDist = 200;
        private Vector2 marionetteLoc;
        private float Appearance => MarionetteMotion.Appearance(age);
        private float AppearScale => 0.65f + Appearance * 0.35f;
        private Vector2 BarsCenter => marionetteLoc + new Vector2(0f, 16f - (1f - Appearance) * 12f);
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Player p = Main.player[Projectile.owner];

            if (!p.active || p.dead || p.CCed || p.noItems || p.HeldItem.type != ModContent.ItemType<Marionette>())
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 cursor = Main.MouseWorld;
                if (age == 0 || (age % 4 == 0 && Vector2.DistanceSquared(cursor, sentCursor) > 4f))
                {
                    sentCursor = cursor;
                    Projectile.netUpdate = true;
                }
                Projectile.ai[1] = cursor.X;
                Projectile.ai[2] = cursor.Y;
            }
            marionetteLoc = new Vector2(Projectile.ai[1], Projectile.ai[2]);
            if (age == 0)
            {
                barLoc = marionetteLoc;
                previousScreenPosition = Main.screenPosition;
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.3f, Pitch = -0.25f }, marionetteLoc);
                    for (int i = 0; i < 18; i++)
                    {
                        Vector2 offset = Main.rand.NextVector2Circular(30f, 22f);
                        Dust dust = Dust.NewDustPerfect(marionetteLoc + offset, DustID.RedTorch,
                            -offset * 0.04f - Vector2.UnitY * 0.4f, 0, new Color(255, 45, 65), Main.rand.NextFloat(0.8f, 1.25f));
                        dust.noGravity = true;
                    }
                }
            }
            Vector2 cameraDelta = previousScreenPosition - Main.screenPosition;
            for (int i = 0; i < 4; i++)
                if (verletEndPos[i] != Vector2.Zero) verletEndPos[i] += cameraDelta;
            previousScreenPosition = Main.screenPosition;
            age++;

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

            p.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.DirectionTo(marionetteLoc).ToRotation() - MathHelper.PiOver2);
            Vector2 ArmPosition = p.RotatedRelativePoint(p.MountedCenter, false, false);
            float RotationOffset = Utils.GetLerpValue(5, 255, Projectile.Distance(marionetteLoc), true);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(marionetteLoc), RotationOffset);
            Projectile.Center = ArmPosition;
            Vector2 projPos = Projectile.Center += Projectile.velocity * 15;

            var plr = p.GetModPlayer<DrawBehindPlayer>();
            plr.DrawVerlet = true;
            plr.Opacity = Appearance;
            plr.pixelSource.Clear();
            plr.col.Clear();
            plr.lerp.Clear();

            #region function
            for (int i = 0; i < 4; i++)
            {
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (age < 20 || !npc.active || npc.life <= 0 || npc.friendly || npc.dontTakeDamage || npc.immortal)
                        continue;
                    if (!npc.boss && !npc.friendly && !npc.dontTakeDamage)
                    {
                        if (verletAttack[i] == 0 && verletStretch[i] <= 0 && Vector2.Distance(npc.Center, verletEndPos[i] + Main.screenPosition) < 30)
                        {
                            int freeToStick = 0;
                            for (int ii = 0; ii < 4; ii++)
                            {
                                if (verletStickedTo[ii] != npc) //only one string per enemy
                                {
                                    freeToStick++;
                                }
                            }

                            if (freeToStick == 4)
                            {
                                verletAttack[i] = 1;
                                verletStickedTo[i] = npc;
                            }
                        }
                    }
                    else if (npc.boss)
                    {
                        Rectangle npcRect = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width - 20, Main.npc[n].height - 20);
                        Rectangle verletTouchRect = new Rectangle((int)(verletEndPos[i].X + Main.screenPosition.X), (int)(verletEndPos[i].Y + Main.screenPosition.Y), 5, 5);

                        if (verletAttack[i] == 0 && verletStretch[i] <= 0 && verletTouchRect.Intersects(npcRect))
                        {
                            verletAttack[i] = 1;
                            verletStickedTo[i] = Main.npc[n];
                            int x = (int)(verletEndPos[i].X + Main.screenPosition.X) - (int)npc.position.X;
                            int y = (int)(verletEndPos[i].Y + Main.screenPosition.Y) - (int)npc.position.Y;
                            verletBossGrabRand[i].X = x + Main.rand.Next(-20, 20);
                            verletBossGrabRand[i].Y = y + Main.rand.Next(-20, 20);
                        }
                    }
                }

                if (verletAttack[i] == 1)
                {
                    if (verletStickedTo[i] == null || !verletStickedTo[i].active || verletStickedTo[i].life <= 0 || verletStickedTo[i].dontTakeDamage)
                    {
                        ResetValues(i);
                        continue;
                    }

                    if (--verletHitCD[i] <= 0 && Projectile.owner == Main.myPlayer)
                    {
                        int howManyAttached = 0;
                        for (int a = 0; a < 4; a++)
                        {
                            if (verletStickedTo[a] != null && verletStickedTo[a].active && verletStickedTo[a].life > 0 && !verletStickedTo[a].dontTakeDamage)
                            {
                                howManyAttached++;
                            }
                        }

                        verletHitCD[i] = 15 * howManyAttached;
                        bool skillStrike = howManyAttached == 4;
                        bool crit = verletChoke[i] > 200 && Main.rand.Next(100) < 4 * howManyAttached;
                        NPC target = verletStickedTo[i];
                        NPC.HitInfo hit = target.CalculateHitInfo(Projectile.damage, 0, crit: crit,
                            damageType: Projectile.DamageType, damageVariation: true, luck: p.luck);

                        if (!skillStrike)
                        {
                            hit.HideCombatText = false;
                        }
                        else
                        {
                            SkillStrikePlayer skillPlayer = p.GetModPlayer<SkillStrikePlayer>();
                            hit.Damage = MarionetteMotion.SkillDamage(hit.Damage, skillPlayer.skillStrikeMultiplier, skillPlayer.superCritMultiplier, hit.Crit);
                            hit.HideCombatText = true;

                            //visuals
                            SkillStrikeUtil.fakeSkillStrike(p, target, target.Center, crit: hit.Crit);
                            Vector2 randomSpawnPos = Main.rand.NextVector2FromRectangle(new Rectangle((int)target.Center.X, (int)target.Center.Y - 20, target.width, (int)(target.height * 0.75f)));
                            Dust text = Dust.NewDustPerfect(randomSpawnPos, ModContent.DustType<SkillStrikeText>(), new Vector2(0f, -12f), Scale: 1f);

                            SkillStrikeTextBehavior sstb = new SkillStrikeTextBehavior();
                            sstb.isCrit = hit.Crit;
                            sstb.damageNumber = "" + hit.Damage;

                            text.customData = sstb;
                        }
                        target.StrikeNPC(hit);
                        target.PlayerInteraction(Projectile.owner);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendStrikeNPC(target, hit);
                        //is there custom behaviour for super crits?

                    }

                    if (Vector2.Distance(marionetteLoc, verletStickedTo[i].Center) > (int)(maxDist * 1.75f))
                    {
                        ResetValues(i);
                        continue;
                    }

                    if (verletStickedTo[i] == null || !verletStickedTo[i].active)
                    {
                        ResetValues(i);
                        continue;
                    }

                    if (verletChoke[i] < 360)
                    {
                        verletChoke[i]++;
                        if (verletChoke[i] > 200)
                        {
                            if (verletTargetLerp[i] < 1f)
                                verletTargetLerp[i] += 0.0035f;
                            verletStickedTo[i].GetGlobalNPC<ColorNPC>().col = Color.Lerp(Color.White, Color.Red, verletTargetLerp[i]);
                            verletStickedTo[i].GetGlobalNPC<ColorNPC>().settingColor = 2;
                        }
                    }
                    else
                    {
                        ResetValues(i);
                    }
                }
            }

            #endregion
            MarionetteMotion.UpdateTilt(ref barsRot, ref angularVelocity, marionetteLoc.X - barLoc.X);
            barLoc = marionetteLoc;

            //-------------------------------- moved here since now it wont lag a frame behind

            Vector2 barsLoc = BarsCenter - Main.screenPosition;

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
                verletPos = barsLoc + (verletPos - barsLoc) * AppearScale;
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

                if (Vector2.Distance(verletEndPos[i], barsLoc) > maxDist * Appearance)
                {
                    Vector2 direction = (verletEndPos[i] - barsLoc).SafeNormalize(Vector2.Zero);
                    verletEndPos[i] = barsLoc + direction * maxDist * Appearance;
                }

                if (verletAttack[i] == 0)
                {
                    if (verletStretch[i] > 0)
                    {
                        DrawVerlet(verletPos, verletEndPos[i], Color.White, i, (float)verletStretch[i] / 500, p: Main.player[Projectile.owner]);
                    }
                    else
                    {
                        DrawVerlet(verletPos, verletEndPos[i], Color.White, i, p: Main.player[Projectile.owner]);
                    }
                }
                else if (verletAttack[i] == 1)
                {
                    float dist = Vector2.Distance(marionetteLoc, verletStickedTo[i].Center);
                    Vector2 stickLoc = new Vector2(verletStickedTo[i].Center.X, verletStickedTo[i].Center.Y);
                    if (!verletStickedTo[i].boss)
                    {
                        stickLoc.Y -= verletStickedTo[i].height / 3;
                    }
                    else
                    {
                        stickLoc = new Vector2(verletStickedTo[i].position.X + verletBossGrabRand[i].X, verletStickedTo[i].position.Y + verletBossGrabRand[i].Y);
                    }

                    DrawVerlet(verletPos, (stickLoc - Main.screenPosition), Color.White, i, (dist / 150000) * dist, verletChoke[i], p: Main.player[Projectile.owner]);
                    verletEndPos[i] = (verletStickedTo[i].Center - Main.screenPosition);
                    verletStretch[i] = (int)(maxDist * 1.75f);
                }
            }
            #endregion
        }

        public override void OnKill(int timeLeft)
        {
            Player p = Main.player[Projectile.owner];
            var plr = p.GetModPlayer<DrawBehindPlayer>();
            plr.DrawVerlet = false;
            plr.pixelSource.Clear();
            plr.col.Clear();
            plr.lerp.Clear();
            for (int i = 0; i < 4; i++)
            {
                ResetValues(i);
                plr.pointCollection[i] = new List<Vector2>();
            }
        }

        Vector2[] verletEndPos = { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        Vector2[] verletSpeed = { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        Vector2[] verletBossGrabRand = { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        NPC[] verletStickedTo = { null, null, null, null };
        int[] verletAttack = { 0, 0, 0, 0 };
        int[] verletHitCD = { 0, 0, 0, 0 };
        int[] verletChoke = { 0, 0, 0, 0 };
        float[] verletTargetLerp = { 0, 0, 0, 0 };
        int[] verletStretch = { 0, 0, 0, 0 };

        #region PreDraw
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Mod.Name + "/Content/Items/Weapons/Crimson/MarionetteStringless").Value;
            Rectangle sourceRect = new Rectangle(0, 0, tex.Width, tex.Height);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, sourceRect, Color.White * Appearance, Projectile.rotation, sourceRect.Size() / 2, AppearScale, Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);


            Vector2 barsLoc = BarsCenter - Main.screenPosition;
            tex = ModContent.Request<Texture2D>(Mod.Name + "/Content/Items/Weapons/Crimson/MarionetteBar").Value;
            sourceRect = new Rectangle(0, 0, tex.Width, tex.Height);
            if (age < 24)
            {
                Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64").Value;
                float flash = MathF.Sin(Appearance * MathHelper.Pi);
                Main.EntitySpriteDraw(glow, barsLoc, null, new Color(255, 100, 130, 0) * flash * 0.45f, 0f, glow.Size() * 0.5f, 1.15f, SpriteEffects.None);
            }
            float rot = MathHelper.Clamp(barsRot, -0.6f, 0.6f);
            Main.EntitySpriteDraw(tex, barsLoc, sourceRect, Color.Gray * Appearance, 0.6f + rot, sourceRect.Size() / 2, AppearScale, SpriteEffects.None);
            Main.EntitySpriteDraw(tex, barsLoc, sourceRect, Color.White * Appearance, 0f + rot, sourceRect.Size() / 2, AppearScale, SpriteEffects.None);

            return false;
        }
        #endregion

        #region DrawVerlet

        public void DrawVerlet(Vector2 verletPos1, Vector2 verletPos2, Color col, int index, float lerp = 0f, int verletChoke = 0, Player p = null)
        {
            Rectangle pixelSource = new Rectangle(0, 0, 2, 2);

            float calcDist = Vector2.Distance(verletPos1, verletPos2);
            Vector2 dir = (verletPos2 - verletPos1).SafeNormalize(Vector2.Zero);

            List<Vector2> points = new List<Vector2>();
            points.Add(verletPos1);
            #region MiddlePoints (done manually so it looks nice)

            Vector2 middlePoint = CreateMiddlePoint(verletPos1, dir, calcDist, 0.15f, 16, verletChoke);
            points.Add(middlePoint);
            middlePoint = CreateMiddlePoint(verletPos1, dir, calcDist, 0.25f, 24, verletChoke);
            points.Add(middlePoint);
            middlePoint = CreateMiddlePoint(verletPos1, dir, calcDist, 0.35f, 30, verletChoke);
            points.Add(middlePoint);
            middlePoint = CreateMiddlePoint(verletPos1, dir, calcDist, 0.45f, 35, verletChoke);
            points.Add(middlePoint);
            middlePoint = CreateMiddlePoint(verletPos1, dir, calcDist, 0.5f, 35, verletChoke);
            points.Add(middlePoint);
            middlePoint = CreateMiddlePoint(verletPos1, dir, calcDist, 0.65f, 35, verletChoke);
            points.Add(middlePoint);
            middlePoint = CreateMiddlePoint(verletPos1, dir, calcDist, 0.75f, 32, verletChoke);
            points.Add(middlePoint);
            middlePoint = CreateMiddlePoint(verletPos1, dir, calcDist, 0.85f, 24, verletChoke);
            points.Add(middlePoint);
            middlePoint = CreateMiddlePoint(verletPos1, dir, calcDist, 0.95f, 8, verletChoke);
            points.Add(middlePoint);
            #endregion
            points.Add(verletPos2);



            var plr = p.GetModPlayer<DrawBehindPlayer>();
            plr.pixelSource.Insert(index, pixelSource);
            plr.pointCollection[index] = points;
            plr.col.Insert(index, col);
            plr.lerp.Insert(index, lerp);

            #region archived points

            /*middlePoint = verletPos1 + dir * (calcDist * 0.25f);
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
            points.Add(middlePoint);*/

            /*for (int s = 0; s < points.Count - 1; s++)
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

                    var plr = p.GetModPlayer<DrawBehindPlayer>();
                    plr.progPoint = progPoint;
                    plr.pixelSource = pixelSource;
                    plr.accCol = accCol;
                    //Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, progPoint, pixelSource, accCol, 0f, pixelSource.Size() / 2, 1f, SpriteEffects.None);


                    if (Vector2.Distance(progPoint, endPos) < 1)
                    {
                        break;
                    }
                    progress += 1f;
                }

            }*/
            #endregion
        }


        public Vector2 CreateMiddlePoint(Vector2 verletPos1, Vector2 dir, float calcDist, float pointPos, int pointYOffset, int verletChoke)
        {
            float tension = MathHelper.Clamp((verletChoke - 250f) / 110f, 0f, 1f);
            float shake = MathF.Sin(age * 0.6f + pointPos * 15f) * tension * 3f * MathF.Sin(pointPos * MathHelper.Pi);

            Vector2 middlePoint = verletPos1 + dir * (calcDist * pointPos);
            middlePoint.Y += pointYOffset * Appearance + shake;
            return middlePoint;

        }
        #endregion

        public void ResetValues(int i, int delay = 60)
        {
            NPC target = verletStickedTo[i];
            if (target != null && !Main.dedServ)
            {
                Vector2 end = target.boss ? target.position + verletBossGrabRand[i] : target.Center - Vector2.UnitY * (target.height / 3f);
                Vector2 direction = (BarsCenter - end).SafeNormalize(-Vector2.UnitY);
                float length = Math.Min(70f, Vector2.Distance(BarsCenter, end) * 0.35f);
                for (int piece = 0; piece < MarionetteMotion.BreakPieces; piece++)
                {
                    Vector2 point = end + direction * (length * (piece + 0.5f) / MarionetteMotion.BreakPieces);
                    Dust dust = Dust.NewDustPerfect(point, ModContent.DustType<MarionetteThreadFragment>(), direction * 1.2f + Main.rand.NextVector2Circular(1.2f, 1.2f));
                    dust.rotation = direction.ToRotation();
                }
            }
            verletBossGrabRand[i] = Vector2.Zero;
            verletTargetLerp[i] = 0f;
            verletChoke[i] = 0;
            verletAttack[i] = 0;
            verletStickedTo[i] = null;
        }
    }

    internal static class MarionetteMotion
    {
        internal const int BreakPieces = 6;
        internal static float Appearance(int age) => MathHelper.SmoothStep(0f, 1f, Math.Clamp(age / 20f, 0f, 1f));
        internal static int SkillDamage(int damage, float skillBonus, float superBonus, bool crit)
            => Math.Max(1, (int)MathF.Round(damage * 1.5f * skillBonus * (crit ? superBonus : 1f)));
        internal static void UpdateTilt(ref float angle, ref float velocity, float horizontalMovement)
        {
            float target = Math.Clamp(horizontalMovement * 0.018f, -0.28f, 0.28f);
            velocity = (velocity + (target - angle) * 0.12f) * 0.72f;
            angle = Math.Clamp(angle + velocity, -0.35f, 0.35f);
            if (Math.Abs(angle) < 0.0001f && Math.Abs(velocity) < 0.0001f && target == 0f)
                angle = velocity = 0f;
        }
    }

    public class MarionetteThreadFragment : ModDust
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = dust.noLight = true;
            dust.customData = 0;
            dust.scale = Main.rand.NextFloat(8f, 14f);
            dust.fadeIn = Main.rand.NextFloat(-0.06f, 0.06f);
        }
        public override bool Update(Dust dust)
        {
            int age = (int)dust.customData + 1;
            dust.customData = age;
            dust.velocity *= 0.94f;
            dust.velocity.Y += 0.025f;
            dust.position += dust.velocity;
            dust.rotation += dust.fadeIn;
            if (age >= 30) dust.active = false;
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            float fade = 1f - (int)dust.customData / 30f;
            Color color = Color.Lerp(new Color(170, 40, 65), new Color(255, 210, 220), fade) * fade;
            Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, dust.position - Main.screenPosition, new Rectangle(0, 0, 1, 1), color,
                dust.rotation, new Vector2(0.5f), new Vector2(dust.scale, 1.5f), SpriteEffects.None);
            return false;
        }
    }

    #region Draw Strings behind npcs
    public class DrawBehindNPC : ModSystem
    {
        public override void Load()
        {
            Terraria.On_Main.DrawNPCs += On_Main_DrawNPCs; ;
        }

        private void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, (Effect)null, Main.Transform);

            for (int players = 0; players < Main.maxPlayers; players++)
            {
                var p = Main.player[players];
                if (p.active)
                {
                    var plr = p.GetModPlayer<DrawBehindPlayer>();
                    if (plr.DrawVerlet)
                    {
                        for (int pp = 0; pp < plr.pointCollection.Length; pp++)
                        {
                            var points = plr.pointCollection[pp];
                            Color colA = Color.DarkRed;
                            Color colB = Color.Lerp(Color.Crimson, Color.White, 0.25f);
                            float gladLerp = 0f;
                            for (int s = 0; s < points.Count - 1; s++)
                            {
                                float progress = 0f;
                                gladLerp += 0.1f;
                                for (int i = 0; i < 5000; i++)
                                {
                                    Color gradColor = Color.Lerp(colA, colB, gladLerp);
                                    Color accCol = Color.Lerp(gradColor, plr.col[pp], plr.lerp[pp]) * plr.Opacity;
                                    Vector2 startPos = points[s];
                                    Vector2 endPos = points[s + 1];
                                    Vector2 direction = (endPos - startPos).SafeNormalize(Vector2.Zero);
                                    Vector2 progPoint = startPos + direction * progress;

                                    Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, progPoint, plr.pixelSource[pp], accCol, 0f, plr.pixelSource[pp].Size() / 2, 1f, SpriteEffects.None);


                                    if (Vector2.Distance(progPoint, endPos) < 1)
                                    {
                                        break;
                                    }
                                    progress += 1f;
                                }

                            }
                        }
                    }
                }
            }

            orig(self, behindTiles);
        }
    }


    public class DrawBehindPlayer : ModPlayer
    {
        public bool DrawVerlet;
        public float Opacity = 1f;

        public List<Rectangle> pixelSource = new List<Rectangle>();
        public List<Vector2>[] pointCollection = { new List<Vector2>(), new List<Vector2>(), new List<Vector2>(), new List<Vector2>() };
        public List<Color> col = new List<Color>();
        public List<float> lerp = new List<float>();

        public override void ResetEffects()
        {
            DrawVerlet = false;
        }
    }
    #endregion

    #region CoolColorDown

    public class ColorNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        float lerp = 0f;
        public Color col;
        public int settingColor;

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (--settingColor > 0)
            {
                lerp = 1f;
                drawColor = col;
            }
            else
            {
                if (lerp > 0f)
                {
                    lerp -= 0.01f;
                    drawColor = Color.Lerp(Color.White, col, lerp);
                }
            }
        }
    }
    #endregion
}