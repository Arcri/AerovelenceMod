using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic.FlashLight;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns.Skylight;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using AerovelenceMod.Content.Items.Weapons.Ember;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Items.Weapons.Starglass;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic.CrystalGlade;
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Content.Projectiles.TempVFX;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;
using static AerovelenceMod.Common.Utilities.DustBehaviorUtil;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic.Ceroba;
using AerovelenceMod.Content.NPCs.Bosses.FeatheredFoe;
using Microsoft.CodeAnalysis;

namespace AerovelenceMod.Content.Items
{
    public class DebugItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("DebugItem");
            /* Tooltip.SetDefault("You shouldn't have this...\n" +
                "[i:" + ModContent.ItemType<Emoji>() + "]"); */
        }
        public override void SetDefaults()
        {
            //Item.UseSound = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .86f, };
            Item.crit = 4;
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 7; //7
            Item.useAnimation = 7; //7
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EosSlash>();
            //Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 10f;

            Item.noUseGraphic = true;
        }

        bool tick = false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //int tendril = Projectile.NewProjectile(null, position, velocity * 0f, ModContent.ProjectileType<TendrilTest>(), 10, 0, Main.myPlayer);

            Dust text = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<SkillStrikeText>(), new Vector2(0f, -3f), Scale: 0.5f);

            text.velocity = new Vector2(0f, -10f);
            text.scale = 1f;

            SkillStrikeTextBehavior sstb = new SkillStrikeTextBehavior();
            sstb.isCrit = false;
            sstb.damageNumber = "47";

            text.customData = sstb;

            return false;
            for (int aaaa = 0; aaaa > 3; aaaa++)
            {
                Dust orb = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<PixelGlowOrb>(),
                Vector2.One.RotatedByRandom(1f) * Main.rand.NextFloat(5.5f, 8.25f), newColor: Main.DiscoColor, Scale: Main.rand.NextFloat(0.65f, 0.85f) * 1f);

                orb.customData = AssignBehavior_PGOBase(
                    rotPower: 0.04f, killEarlyTime: -1, timeBeforeSlow: 5, preSlowPower: 0.97f, postSlowPower: 0.89f, velToBeginShrink: 1.25f, fadePower: 0.9f, dontDrawOrb: false, glowIntensity: 0.3f, colorFadePower: 0.9f);
            }


            for (int kl2 = 0; kl2 < 0; kl2++)
            {
                Dust smoke = Dust.NewDustPerfect(Main.MouseWorld + new Vector2(0f, 0f), ModContent.DustType<HighResSmoke>(), Main.rand.NextVector2CircularEdge(1f, 1f), newColor: new Color(255, 100, 15));
                //Projectile.NewProjectile(null, Main.MouseWorld + new Vector2(500f, 0f), Main.rand.NextVector2CircularEdge(1f, 1f), ModContent.ProjectileType<SmokeTest>(), 0, 0, Main.myPlayer);

                smoke.customData = AssignBehavior_HRSBase(5, 25, 1f, 1f, true, 1f);

                //Dust softGlow = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<SoftGlowDust>(), Vector2.Zero, newColor: Color.Orange, Scale: 0.15f);

                //softGlow.customData = DustBehaviorUtil.AssignBehavior_SGDBase(timeToStartFade: 3, timeToChangeScale: 0, fadeSpeed: 0.9f, sizeChangeSpeed: 0.95f, timeToKill: 10,
                //overallAlpha: 0.1f, DrawWhiteCore: false, 1f, 1f);

            }


            float impactScale = 1f;
            for (int j = 0; j < (5 + Main.rand.Next(0, 2)) * impactScale; j++)
            {
                //Dust star = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<GlowStarSharp>(),
                //Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1, 4), newColor: new Color(255, 180, 60), Scale: Main.rand.NextFloat(0.20f, 0.35f) * 1.5f);

                //StarDustDrawInfo info = new StarDustDrawInfo(true, false, true, true, false, 1f);
                //star.customData = AssignBehavior_GSSBase(rotPower: 0.03f, timeBeforeSlow: 8, preSlowPower: 0.94f, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.95f, shouldFadeColor: false, sdci: info);

                Dust star = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<GlowPixelCross>(),
                Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.5f, 3.25f), newColor: new Color(255, 180, 60), Scale: Main.rand.NextFloat(0.35f, 0.50f) * 1f);

                star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                rotPower: 0.15f, preSlowPower: 0.91f, timeBeforeSlow: 15, postSlowPower: 0.90f, velToBeginShrink: 2f, fadePower: 0.93f, shouldFadeColor: false);
            }
            for (int ii = 0; ii < (6 + Main.rand.Next(0, 2)) * impactScale; ii++)
            {
                Dust d = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<MuraLineBasic>(),
                        Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.5f, 3.25f), Alpha: Main.rand.Next(10, 15), new Color(255, 180, 60), 0.35f);
            }


            return false;
            /*
            int[] orbitValues1 = { 20,  80, 140,
                                  40,  100, 160,
                                  60,  120, 180 };

            int[] orbitValues2 = { 20,  60, 40,
                                  100,  40, 100,
                                  60,  80, 80 };

            int[][] orbitValues = { orbitValues1, orbitValues2 };

            int numberOfFeahters = 9;
            for (int ab = 0; ab < 2; ab++)
            {
                for (int index = 1; index <= numberOfFeahters; index++)
                {
                    int orbfeather = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<OrbitingFeatherOld>(), damage, 0, Main.myPlayer);

                    if (Main.projectile[feather].ModProjectile is OrbitingFeatherOld of)
                    {
                        of.timeToOrbit = 60 + (orbitValues[ab][index - 1] * 2) + (180 * ab * 2);  //60 * index;
                        of.orbitVector = new Vector2(355f - (100 * ab), 0f).RotatedBy(MathHelper.TwoPi * ((index - 1f) / numberOfFeahters));
                        of.orbitVal = 355f - (100 * ab);
                        of.rotSpeed = ab == 0 ? 1.85f : 1.5f;
                    }

                }
            }
            
            int barrier = Projectile.NewProjectile(null, player.Center + new Vector2(0f, -245f), Vector2.Zero, ModContent.ProjectileType<WindBarrierTest>(), 0, 0, Main.myPlayer);
            */

            return false;

            int bomb = Projectile.NewProjectile(null, position, velocity.RotatedByRandom(1f) * 1.5f, ModContent.ProjectileType<FeatherProjTest>(), damage, 0, Main.myPlayer);
            (Main.projectile[bomb].ModProjectile as FeatherProjTest).rotGoal = (Main.MouseWorld - (position + velocity * 200f)).ToRotation() + 3.14f;

            for (int i22 = 0; i22 < 8; i22++) //4 //2,2
            {
                Dust p = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<LineSpark>(),
                    velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-2.2f, 2.2f)) * Main.rand.Next(4, 12),
                    newColor: Color.HotPink, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.45f);
                p.velocity += velocity * (1.45f + Main.rand.NextFloat(-0.1f, -0.2f));

                p.customData = AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 80, 
                    1f, 0.75f);

            }
            return false;

            for (int i22 = 0; i22 < 8; i22++) //4 //2,2
            {
                Dust p = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<GlowStarSharp>(),
                    velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)) * Main.rand.Next(4, 12),
                    newColor: Main.DiscoColor, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.85f);
                p.velocity += velocity * (0.45f + Main.rand.NextFloat(-0.1f, -0.2f));

                StarDustDrawInfo info = new StarDustDrawInfo(true, false, true, true, false, 1f);
                p.customData = AssignBehavior_GSSBase(rotPower: 0.04f, timeBeforeSlow: 5, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.8f, shouldFadeColor: false, sdci: info);

            }

            return false;

            
            for (double m = 0; m < 6.28; m += 1)
            {
                Dust dust = Dust.NewDustPerfect(player.Center + new Vector2(200f, 0f), ModContent.DustType<GlowPixelCross>(), new Vector2((float)Math.Sin(m) * 1.3f, (float)Math.Cos(m)) * 2.4f);
                dust.color = Color.DeepSkyBlue;
                dust.velocity *= Main.rand.NextFloat(0.4f, 1.3f);
                dust.scale = 0.8f;


                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 2f, fadePower: 0.87f, shouldFadeColor: false);

            }
            
            for (int i11 = 0; i11 < 10; i11++) //4 //2,2
            {
                Dust p = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<GlowPixelCross>(),
                    velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.Next(2, 10), 
                    newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.3f, 0.5f) * 1.25f);
                p.velocity += velocity * (0.75f + Main.rand.NextFloat(-0.1f, -0.2f));

                p.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.13f, timeBeforeSlow: 10, preSlowPower: 0.94f, postSlowPower: 0.91f, velToBeginShrink: 1.5f, fadePower: 0.92f, shouldFadeColor: false);
            }

            for (int i22 = 0; i22 < 8; i22++) //4 //2,2
            {
                Dust p = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<GlowPixelCross>(), 
                    velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)) * Main.rand.Next(2, 10),
                    newColor: Color.DodgerBlue, Scale: Main.rand.NextFloat(0.3f, 0.5f)  * 1.25f);
                p.velocity += velocity * (0.45f + Main.rand.NextFloat(-0.1f, -0.2f));

                p.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, timeBeforeSlow: 5, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);

            }

            return false;

            for (int fg = 0; fg < 11; fg++)
            {
                Vector2 randomStart = Main.rand.NextVector2Circular(1f,1f) * 6f;
                Dust gd = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<GlowPixelCross>(), randomStart * Main.rand.NextFloat(0.3f, 1.35f) * 1.5f, newColor: Color.DodgerBlue, Scale: Main.rand.NextFloat(1f, 1.4f) * 0.5f);
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, timeBeforeSlow: 5, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }

            //int Muraa = Projectile.NewProjectile(null, position + new Vector2(0, 0), velocity * 2.3f, ModContent.ProjectileType<Weapons.BossDrops.Cyvercry.NewDarknessDischargeStar>(), 10, 0, player.whoAmI, 0f, 0f);
            return false;


            //Common.Systems.FlashSystem.SetFlashEffect(Main.MouseWorld, 4f, 30);
            //int Explo = Projectile.NewProjectile(null, position, Vector2.Zero, ModContent.ProjectileType<FireBlast>(), 0, 0, Main.myPlayer);

            for (int ia = 0; ia < 0; ia++)
            {
                int aa = Projectile.NewProjectile(null, position + velocity * 4f, new Vector2(0.5f, 0).RotatedByRandom(6) * Main.rand.NextFloat(0.7f, 2f), ModContent.ProjectileType<FadeExplosionHighRes>(), 0, 0);
                Main.projectile[aa].rotation = Main.rand.NextFloat(6.28f);
                if (Main.projectile[aa].ModProjectile is FadeExplosionHighRes explo)
                {
                    explo.rise = false;
                    explo.color = ia > 2 ? Color.DeepSkyBlue : Color.DeepPink;
                    explo.size = 0.35f;
                    explo.colorIntensity = 0.75f; //0.5
                }
            }

            return false;

            for (int a2 = 0; a2 < 6; a2++)
            {
                Vector2 spawnPos = new Vector2(400, 0).RotatedBy(MathHelper.ToRadians(360 / 6) * a2);
                int index = NPC.NewNPC(source, (int)position.X, (int)position.Y, ModContent.NPCType<CyverBot>(), player.whoAmI);
                NPC laser = Main.npc[index];
                laser.damage = 0;
                if (laser.ModNPC is CyverBot bot)
                {
                    bot.State = (int)(CyverBot.Behavior.PrimeLaserLong);
                    bot.setGoalLocation(spawnPos);
                    if (a2 == 0)
                        bot.Leader = true;
                }
            }

            return false;
            /*
            for (int ada = 0; ada < 1; ada++)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.PaladinsHammer, new ParticleOrchestraSettings
                {
                    PositionInWorld = player.Center + new Vector2(0,-100),
                    MovementVector = velocity * 0.8f
                }); ;
            }
            */

            //Dust d = Dust.NewDustPerfect(player.Center + new Vector2(0,50), ModContent.DustType<GlowStrong>(), velocity * 0.6f, newColor: Color.Red);
            //return false;

            int Mura = Projectile.NewProjectile(null, position + new Vector2(0,0), velocity * 1.2f, ModContent.ProjectileType<otherHollowPulseTestDearFutureMePleaseRewriteAndMoveThisInsteadOfUsingItInTheFutureDearGod>(), 10, 0, player.whoAmI, 0f, 0f);
            if (Main.projectile[Mura].ModProjectile is otherHollowPulseTestDearFutureMePleaseRewriteAndMoveThisInsteadOfUsingItInTheFutureDearGod vfx)
            {
                vfx.size = 0.5f;
                vfx.color = Color.White;
            }

            tick = !tick;

            return false;
            for (int a2 = 0; a2 < 6; a2++)
            {
                Vector2 spawnPos = new Vector2(400, 0).RotatedBy(MathHelper.ToRadians(360 / 6) * a2);
                int index = NPC.NewNPC(source, (int)position.X, (int)position.Y, ModContent.NPCType<CyverBot>(), player.whoAmI);
                NPC laser = Main.npc[index];
                laser.damage = 0;
                if (laser.ModNPC is CyverBot bot)
                {
                    bot.State = (int)(CyverBot.Behavior.PrimeLaserLong);
                    bot.setGoalLocation(spawnPos);
                    if (a2 == 0)
                        bot.Leader = true;
                }
            }

            return false;

            /*
            int afg = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<DistortProj>(), 0, 0);
            Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);

            if (Main.projectile[afg].ModProjectile is DistortProj distort)
            {
                distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/FlareVortex");
            }
            */
            return false;

            /*
            if (Main.projectile[Mura].ModProjectile is MuraLineHandler mlh)
            {
                for (int m = 0; m < 5; m++)
                {
                    MuraLine newWind = new MuraLine(Main.projectile[Mura].Center, velocity.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)) * 1, 2f);
                    mlh.lines.Add(newWind);
                    mlh.color = Color.OrangeRed;
                }
            }
            */



            //SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_1") with { Pitch = -.53f, PitchVariance = 0.3f, Volume = 0.5f};
            //SoundEngine.PlaySound(style, player.Center);
            return false;
            //SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_explosive_trap_explode_1") with { PitchVariance = 1.16f };
            //SoundEngine.PlaySound(style);

            //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/back_scatter") with { Volume = .14f, Pitch = .6f, PitchVariance = 0.25f };
            //SoundEngine.PlaySound(style);
            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/rescue_ranger_fire") with { Volume = .1f, Pitch = .4f, };
            SoundEngine.PlaySound(style2); 
            //SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Impact_Sword_L_a") with { Volume = .27f, Pitch = .6f, PitchVariance = 0.25f };
            //SoundEngine.PlaySound(style3);

            //SoundStyle style3b = new SoundStyle("Terraria/Sounds/Item_71") with { Pitch = -.51f, PitchVariance = .25f, Volume = 0.4f };
            //SoundEngine.PlaySound(style3b);

            int a = Projectile.NewProjectile(null, position, velocity, ModContent.ProjectileType<BulletTest>(), 10, 0, player.whoAmI);
            //int b3 = Projectile.NewProjectile(null, position, velocity.RotatedByRandom(0.15f) * Main.rand.NextFloat(0.85f, 1f), ModContent.ProjectileType<BulletTest>(), 10, 0, player.whoAmI);
            //int c3 = Projectile.NewProjectile(null, position, velocity.RotatedByRandom(0.15f) * Main.rand.NextFloat(0.85f, 1f), ModContent.ProjectileType<BulletTest>(), 10, 0, player.whoAmI);
            //int d3 = Projectile.NewProjectile(null, position, velocity.RotatedByRandom(0.15f) * Main.rand.NextFloat(0.85f, 1f), ModContent.ProjectileType<BulletTest>(), 10, 0, player.whoAmI);

            //Main.projectile[a].timeLeft = 50;
            return false;

            //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/JuniorShot") with { Pitch = .01f, PitchVariance = .55f, Volume = 0.3f };
            //SoundEngine.PlaySound(style, player.Center);

            if (Main.rand.NextBool(3))
            {
                int b = Projectile.NewProjectile(null, position, velocity.RotatedByRandom(0.1f) * 10f, ModContent.ProjectileType<InkProjTest>(), 10, 0, player.whoAmI);

                if (Main.projectile[b].ModProjectile is InkProjTest m)
                {
                    m.color = Color.Orange;
                    m.overallSize = 0.15f;
                    m.xScale = Main.rand.NextFloat(0.8f, 1.3f);
                    m.yScale = Main.rand.NextFloat(0.9f, 1.2f);
                }
            }

            if (Main.projectile[a].ModProjectile is InkProjTest i)
            {
                i.color = Color.Orange;
                i.overallSize = 0.25f;
                i.xScale = Main.rand.NextFloat(0.8f, 1.3f);
                i.yScale = Main.rand.NextFloat(0.9f, 1.2f);
            }


            //SoundStyle style = new SoundStyle("Terraria/Sounds/Item_100") with { Volume = .5f, Pitch = .73f, PitchVariance = .22f, };
            //SoundEngine.PlaySound(style);

            //SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_116") with { Volume = .41f, Pitch = .27f, PitchVariance = .2f, };
            //SoundEngine.PlaySound(style2);

            //Projectile.NewProjectile(null, position, velocity, ModContent.ProjectileType<StretchLaser>(), 0, 0, player.whoAmI);
            return false;
        }
        
    }
}