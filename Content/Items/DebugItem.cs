﻿using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;
using Terraria.Audio;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;
using AerovelenceMod.Content.NPCs.Bosses.Rimegeist;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic;
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic.FlashLight;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using AerovelenceMod.Content.Items.Weapons.Ember;
using AerovelenceMod.Content.Items.Weapons.Flares;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Content.Items.Weapons.Misc.Melee;
using AerovelenceMod.Content.Items.Weapons.Frost.DeepFreeze;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Bows;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged;
using AerovelenceMod.Content.Items;
using static Terraria.ModLoader.PlayerDrawLayer;
using AerovelenceMod.Content.Items.Weapons.Aurora;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic.WandOfExploding;
using AerovelenceMod.Content.Projectiles.BulletRework;

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
            Item.shoot = ModContent.ProjectileType<CyverHyperBeam>();
            //Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 10f;
        }

        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int a2 = 0; a2 < 4; a2++)
            {
                Vector2 spawnPos = new Vector2(400, 0).RotatedBy(MathHelper.ToRadians(360 / 4) * a2);
                int index = NPC.NewNPC(source, (int)position.X, (int)position.Y, ModContent.NPCType<CyverBot>(), player.whoAmI);
                NPC laser = Main.npc[index];
                laser.damage = 0;
                if (laser.ModNPC is CyverBot bot)
                {
                    bot.State = (int)(CyverBot.Behavior.PrimeLaser);
                    bot.setGoalLocation(spawnPos);
                    if (a2 == 0)
                        bot.Leader = true;
                }
            }

            //int Mura = Projectile.NewProjectile(null, position, velocity * 0f, ModContent.ProjectileType<NanoReticle>(), 10, 0, player.whoAmI, 0f, 0f);
            return false;

            //int Mura2yeah = Projectile.NewProjectile(null, position, velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(2.0f, 3.1f), ModContent.ProjectileType<WandOfExplodingProj>(), 10, 0, player.whoAmI, 0f, 0f);
            //return false;
            //ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            /*
            for (int fg = 0; fg < 10; fg++)
            {
                Vector2 randomStart = Main.rand.NextVector2CircularEdge(5, 5);
                Dust gd = Dust.NewDustPerfect(player.Center, ModContent.DustType<GlowPixelAlts>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f) * 1.5f, newColor: Color.White, Scale: 1f);

            }
            */
            //new Color(255,85,255)
            for (int h = 0; h < 30; h++)
            {
                int dust = Dust.NewDust(Main.MouseWorld, 30, 30, ModContent.DustType<GlowPixelAlts>(), Scale: 1f, newColor: Color.OrangeRed);
                //Main.dust[dust].velocity *= 3f;
                //Main.dust[dust].position += Main.dust[dust].velocity * 3;

            }

            /*
            int afg = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<DistortProj>(), 0, 0);
            Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);

            if (Main.projectile[afg].ModProjectile is DistortProj distort)
            {
                distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/FlareVortex");
            }
            */
            return false;

            //int Mura2 = Projectile.NewProjectile(null, position + new Vector2(0, 500), velocity, ModContent.ProjectileType<FinaleBeam>(), 10, 0, player.whoAmI, 0f, 0f);

            //int Mura = Projectile.NewProjectile(null, position + new Vector2(0,500), velocity , ModContent.ProjectileType<FinaleBeam>(), 10, 0, player.whoAmI, 0f, 0f);
            int Mura2 = Projectile.NewProjectile(null, position, velocity * 3, ModContent.ProjectileType<WandOfExplodingProj>(), 10, 0, player.whoAmI, 0f, 0f);

            //int Mura = Projectile.NewProjectile(null, position, velocity * 2, ModContent.ProjectileType<LightningBorder>(), 10, 0, player.whoAmI, 0f, 0f);

            //int Mura2 = Projectile.NewProjectile(null, position, velocity * 0, ModContent.ProjectileType<ElementalShiftPulse>(), 0, 0, player.whoAmI, 0f, 0f);
            //Main.projectile[Mura2].scale = 2f;

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

            ///int roA = Projectile.NewProjectile(null, position, velocity * 0.2f, ModContent.ProjectileType<AlibanProj>(), 5, 0, player.whoAmI);

            /*
            int roA = Projectile.NewProjectile(null, position, velocity, ModContent.ProjectileType<RoAHit>(), 5, 0, player.whoAmI);

            if (Main.projectile[roA].ModProjectile is RoAHit hit)
            {
                hit.color = Color.Blue;
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