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
        }

        bool tick = false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int fg = 0; fg < 20; fg++)
            {
                Vector2 randomStart = Main.rand.NextVector2Circular(1f,1f) * 3f;
                Dust gd = Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<Dusts.GlowDusts.GlowPixelInnerCore>(), randomStart * Main.rand.NextFloat(0.3f, 1.35f) * 1.5f, newColor: Color.DodgerBlue, Scale: Main.rand.NextFloat(1f, 1.4f) * 0.45f);

            }
            return false;
            float rotationSpeed = 0.03f;

            for (int lm = 0; lm < 4; lm++)
            {
                //Assign where bots will start at
                Vector2 spawnPos = new Vector2(400, 0).RotatedBy(MathHelper.ToRadians(360 / 4) * lm);

                Vector2 trueSpawnPos = (spawnPos * 2.5f) + player.Center;

                int index = NPC.NewNPC(null, (int)trueSpawnPos.X, (int)trueSpawnPos.Y, ModContent.NPCType<CyverBot>(), player.whoAmI);
                NPC thisBot = Main.npc[index];
                thisBot.damage = 0;
                if (thisBot.ModNPC is CyverBot bot)
                {
                    int version = false ? (int)(CyverBot.Behavior.PrimeLaserLong) : (int)(CyverBot.Behavior.PrimeLaser);

                    bot.State = version;
                    bot.rotIntensity = rotationSpeed;
                    bot.setGoalLocation(spawnPos);


                    //if (i == 0 && (newBotsReps == totalReps)) //Makes the ball occur so only do so on the last wave
                        //bot.Leader = true;
                }
            }


            //int Muraa = Projectile.NewProjectile(null, position + new Vector2(0, 0), velocity * 2.3f, ModContent.ProjectileType<Weapons.BossDrops.Cyvercry.NewDarknessDischargeStar>(), 10, 0, player.whoAmI, 0f, 0f);
            return false;
            
            int explosion = Projectile.NewProjectile(null, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<FadeExplosionHandler>(), 0, 0, Main.myPlayer);

            if (Main.projectile[explosion].ModProjectile is FadeExplosionHandler feh)
            {
                feh.color = Color.OrangeRed;
                feh.colorIntensity = 0.75f;
                feh.fadeSpeed = 0.035f;
                for (int m = 0; m < 10; m++)
                {
                    FadeExplosionClass newSmoke = new FadeExplosionClass(Main.projectile[explosion].Center, new Vector2(3f, 0).RotatedByRandom(1.3f) * Main.rand.NextFloat(0.5f, 2f));

                    newSmoke.size = 0.4f + Main.rand.NextFloat(-0.15f, 0.15f);
                    feh.Smokes.Add(newSmoke);

                }
            }
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

            for (int i2 = 0; i2 < 0; i2++)
            {
                int a2 = Projectile.NewProjectile(null, position + new Vector2(0, -15).RotatedBy(velocity.ToRotation()), velocity, ModContent.ProjectileType<BeaconShot>(), 3, 0, player.whoAmI);
                int b2 = Projectile.NewProjectile(null, position + new Vector2(0, 15).RotatedBy(velocity.ToRotation()), velocity, ModContent.ProjectileType<BeaconShot>(), 3, 0, player.whoAmI);
                int c2 = Projectile.NewProjectile(null, position, velocity, ModContent.ProjectileType<BeaconShot>(), 3, 0, player.whoAmI);
            }


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