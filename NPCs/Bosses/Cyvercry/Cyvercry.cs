using AerovelenceMod.Items.Armor.Vanity;
using AerovelenceMod.Items.BossBags;
using AerovelenceMod.Items.Placeable.Trophies;
using AerovelenceMod.Items.Weapons.Magic;
using AerovelenceMod.Items.Weapons.Melee;
using AerovelenceMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AerovelenceMod.NPCs.Bosses.Cyvercry //Change me
{
    [AutoloadBossHead]
    public class Cyvercry : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyvercry"); //DONT Change me
            Main.npcFrameCount[npc.type] = 5;
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 0;
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 37500;
            npc.damage = 105;
            npc.defense = 30;
            npc.knockBackResist = 0f;
            npc.width = 180;
            npc.height = 100;
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            bossBag = ModContent.ItemType<CyvercryBag>();
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Cyvercry");
            npc.value = Item.buyPrice(0, 22, 11, 5);
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 41500;
            npc.damage = 125;
            npc.defense = 35;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if(npc.frameCounter >= 10)
            {
                npc.frame.Y = npc.frame.Y + frameHeight;
                npc.frameCounter = 5;
                if (npc.frame.Y > 4 * frameHeight)
                {
                    npc.frame.Y = 0;
                    npc.frameCounter = 0;
                }
                if(npc.frame.Y == 3 * frameHeight && nextAttack != 1) //booster frame
                {
                    Vector2 from = npc.Center + new Vector2(64, 0).RotatedBy(npc.rotation);
                    int type = DustID.Electric;
                    if (ai5 == 30)
                        type = 235;
                    for (int i = 0; i < 360; i += 20)
                    {
                        Vector2 circular = new Vector2(24, 0).RotatedBy(MathHelper.ToRadians(i));
                        circular.Y *= 0.7f;
                        circular = circular.RotatedBy(npc.rotation);
                        Vector2 dustVelo = new Vector2(12, 0).RotatedBy(npc.rotation);
                        Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, type, 0, 0, npc.alpha);
                        dust.velocity *= 0.15f;
                        dust.velocity += dustVelo;
                        dust.noGravity = true;
                        if (type == 235)
                            dust.scale *= 2.5f;
                    }
                }
            }
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
            if (!AeroWorld.downedCyvercry)
            {
                AeroWorld.downedCyvercry = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (Main.expertMode)
            {
                npc.DropBossBags();
                return;
            }
            if (!Main.expertMode)
            {
                if (Main.rand.NextBool(7))
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<CyvercryMask>());
                }
                if (Main.rand.NextBool(10))
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<CyvercryTrophy>());
                }
                int rand = Main.rand.Next(4);
                if (rand == 0)
                    Item.NewItem(npc.getRect(), ModContent.ItemType<CyverCannon>());
                if (rand == 1)
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Cyverthrow>());
                if (rand == 2)
                    Item.NewItem(npc.getRect(), ModContent.ItemType<DarknessDischarge>());
                if (rand == 3)
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Oblivion>());
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            float alpha = 1f;
            if (nextAttack == 1)
            {
                alpha = (240 - ai1 + ai2) / 240f;
            }
            Texture2D texture = mod.GetTexture("NPCs/Bosses/Cyvercry/Glowmask");
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, Color.White * alpha, npc.rotation, npc.frame.Size() / 2f, npc.scale, SpriteEffects.None, 0);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
            float alpha = 1f;
            float shade = 1f;
            Color color = lightColor;
            Color color2 = new Color(100, 100, 100, 0);
            if (nextAttack == 1)
            {
                shade = (120 - ai1 + ai2) / 120f;
                alpha = (240 - ai1 + ai2) / 240f;
                if (shade < 0)
                {
                    shade = 0;
                }
                if (alpha < 0)
                {
                    alpha = 0;
                }
                color.R = (byte)(color.R * shade);
                color.G = (byte)(color.G * shade);
                color.B = (byte)(color.B * shade);
                color2.R = (byte)(color2.R * shade);
                color2.G = (byte)(color2.G * shade);
                color2.B = (byte)(color2.B * shade);
            }
            Texture2D texture;
            if (ai5 == 30)
            {
                for(int i = 0; i < 360; i += 30)
                {
                    Vector2 rotationalPos = new Vector2(Main.rand.NextFloat(2, 3), 0).RotatedBy(MathHelper.ToRadians(i));
                    texture = mod.GetTexture("NPCs/Bosses/Cyvercry/CyvercryRed");
                    spriteBatch.Draw(texture, npc.Center - Main.screenPosition + rotationalPos, npc.frame, color2 * alpha, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0);
                }
            }
            texture = mod.GetTexture("NPCs/Bosses/Cyvercry/Cyvercry");
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, color * alpha, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0);
            if(shadowTrail)
            {
                for (int k = 0; k < npc.oldPos.Length; k++)
                {
                    Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, npc.gfxOffY);
                    color = npc.GetAlpha(lightColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                    spriteBatch.Draw(Main.npcTexture[npc.type], drawPos, npc.frame, color * 0.5f, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0f);
                }
            }
            return false;
        }
        private float nextAttack
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
        }
        private float ai1
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }
        private float ai2
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
        }
        private float ai3
        {
            get => npc.ai[3];
            set => npc.ai[3] = value;
        }
        private float ai4 = 0;
        private float ai5 = 0;
        private float ai6 = 0;
        bool shadowTrail = false;
        Vector2 prevCenter;
        int direction = 1;
        float dynamicCounter = 0;
        bool runOnce = true;
        bool runOncePhase2 = true;
        float speed = 12;
        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                npc.netUpdate = true;
            Player player = Main.player[npc.target];
            npc.spriteDirection = -1;
            if (Main.expertMode)
                npc.defense = 35;
            else
                npc.defense = 30;
            if (runOnce)
            {
                prevCenter = player.Center;
                nextAttack = -1;
                ai1 = -180;
                runOnce = false;
            }
            if(runOncePhase2 && npc.life < npc.lifeMax * 0.5f)
            {
                ai1 = 0;
                ai2 = 0;
                ai3 = 0;
                ai4 = 0;
                ai5 = 240;
                nextAttack = -1;
                runOncePhase2 = false;
            }
            if(Main.dayTime || player.dead)
            {
                npc.rotation = (npc.Center - player.Center).ToRotation();
                npc.velocity.Y -= 0.09f;
                npc.timeLeft = 300;
                if(npc.position.Y <= 16 * 35) //checking for top of the world practically
                    npc.active = false;
                return;
            }
            if(ai5 > 30)
            {
                npc.velocity *= 0.8f;
                npc.dontTakeDamage = false;
                dynamicCounter += 6;
                Vector2 dynamicAddition2 = new Vector2(45 * (ai5 - 30f) / 210f, 0).RotatedBy(MathHelper.ToRadians(dynamicCounter));
                npc.rotation = (npc.Center - player.Center).ToRotation() + MathHelper.ToRadians(dynamicAddition2.X);
                ai5--;
                if(ai5 == 30)
                {
                    for (int i = 0; i < 360; i += 5)
                    {
                        Vector2 circular = new Vector2(96, 0).RotatedBy(MathHelper.ToRadians(i));
                        Vector2 dustVelo = circular * 0.4f;
                        Dust dust = Dust.NewDustDirect(npc.Center - new Vector2(5) + circular, 0, 0, 235, 0, 0, npc.alpha);
                        dust.velocity *= 0.15f;
                        dust.velocity += dustVelo;
                        dust.scale = 1.75f;
                        dust.noGravity = true;
                    }
                }
                return;
            }
            if(nextAttack == -2)
            {
                FindNextAttack();
            }
            if (nextAttack == -1)
            {
                speed = 22f;
                if (!runOncePhase2)
                    speed = 30f;
                if (ai1 > 120)
                {
                    ai2 += 5;
                    if (ai2 >= 180)
                    {
                        direction = -direction;
                        ai2 = 0;
                        ai1 = -120;
                        ai3 += 2700;
                        ai4++;
                    }
                }
                else if (ai3 <= 0)
                {
                    speed = 6f;
                    ai1++;
                    dynamicCounter++;
                    prevCenter = player.Center;
                    npc.rotation = (npc.Center - player.Center).ToRotation();
                }
                float dist = 300;
                if (ai1 <= 10)
                    dist = 650;
                Vector2 dynamicAddition = new Vector2(30, 0).RotatedBy(MathHelper.ToRadians(dynamicCounter));
                Vector2 circular = new Vector2(0, 96).RotatedBy(MathHelper.ToRadians(ai2));
                Vector2 goTo = prevCenter - direction * new Vector2(-dist + circular.X, 0).RotatedBy(MathHelper.ToRadians(dynamicAddition.X));
                goTo -= npc.Center;
                float distance = goTo.Length();
                goTo = goTo.SafeNormalize(Vector2.Zero);
                if(ai2 > 120)
                {
                    npc.rotation = MathHelper.ToRadians(180) + goTo.ToRotation();
                }
                if (speed > distance) 
                {
                    speed = distance;
                    ai3 = 0;
                }
                if(ai3 > 0)
                {
                    ai3--;
                    shadowTrail = true;
                    if (((ai3 % 6 == 0 && !Main.expertMode) || (ai3 % 5 == 0 && Main.expertMode)) && !runOncePhase2)
                    {
                        FireLaser(ModContent.ProjectileType<EnergyBall>(), 1, 0, player.whoAmI);
                    }
                }
                else
                    shadowTrail = false;
                npc.velocity *= 0.8f;
                npc.velocity += 0.4f * goTo * (speed + distance * 0.01f);
                if(ai1 % 30 == 0 && ai1 >= -90 && ai1 <= 0)
                {
                    Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 12, 0.75f);
                    FireLaser(ProjectileID.DeathLaser);
                }

                if (ai4 >= 3 && ai1 > 0)
                {
                    nextAttack = -2;
                    return;
                }
            } //idle attack

            if(nextAttack == 0)
            {
                ai1++;
                npc.rotation = (npc.Center - player.Center).ToRotation();
                npc.velocity *= 0.6f;
                Vector2 toPlayer = player.Center - npc.Center;
                toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
                npc.velocity += toPlayer * 0.8f;
                if (ai2 > 3)
                {
                    if (ai1 % 5 == 0)
                    {
                        if (Main.expertMode)
                            FireLaser(ProjectileID.DeathLaser, 13f, 0.7f);
                        else
                            FireLaser(ProjectileID.DeathLaser, 11f, 0.7f);
                        ai3++;
                    }
                    if(ai3 >= 30)
                    {
                        nextAttack = -1;
                        ai1 = 0;
                        ai2 = 0;
                        ai3 = 0;
                        ai4 = 2 + Main.rand.Next(2);
                    }
                }
                else
                {
                    if (Main.expertMode) //increase defense during charge period
                        npc.defense = 65;
                    else
                        npc.defense = 60;
                    Vector2 from = npc.Center + new Vector2(-128, 0).RotatedBy(npc.rotation);
                    if (ai1 % 50 == 0)
                    {
                        for (int i = 0; i < 360; i += 20)
                        {
                            Vector2 circular = new Vector2(64, 0).RotatedBy(MathHelper.ToRadians(i));
                            circular.X *= 0.6f;
                            circular = circular.RotatedBy(npc.rotation);
                            Vector2 dustVelo = -circular * 0.1f;
                            Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, DustID.Electric, 0, 0, npc.alpha);
                            dust.velocity *= 0.15f;
                            dust.velocity += dustVelo;
                            dust.scale = 1.2f;
                            dust.noGravity = true;
                        }
                        ai2++;
                    }
                    for(int j = 0; j < ai2; j++)
                    {
                        Vector2 circular = new Vector2(64, 0).RotatedBy(MathHelper.ToRadians(j * 120 + ai1 * 4));
                        circular.X *= 0.6f;
                        circular = circular.RotatedBy(npc.rotation);
                        Vector2 dustVelo = -circular * 0.09f;
                        Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, DustID.Electric, 0, 0, npc.alpha);
                        dust.velocity *= 0.1f;
                        dust.scale = 1.4f;
                        dust.noGravity = true;
                    }
                    
                }
            }
            if(nextAttack == 1)
            {
                ai1 += 6;
                if(ai1 > 60 && ai2 == 0)
                {
                    npc.dontTakeDamage = true;
                }
                if (ai1 > 240)
                {
                    ai4++;
                    if(ai3 < 10)
                    {
                        if ((ai4 % 25 == 0 && !Main.expertMode) || (ai4 % 20 == 0 && (!runOncePhase2 || Main.expertMode)))
                        {
                            npc.Center = player.Center + new Vector2(1256, 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360)));
                            ai3++;
                            Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Effects/Test"));
                            FireLaser(ModContent.ProjectileType<ShadowCyvercry>(), 20, 0, 0, !runOncePhase2 ? -1 : 0);
                        }
                    }
                    else if(ai2 < 240)
                    {
                        npc.dontTakeDamage = false;
                        npc.rotation = (npc.Center - player.Center).ToRotation();
                        ai2 += 6;
                    }
                    else
                    {
                        nextAttack = -1;
                        ai1 = 0;
                        ai2 = 0;
                        ai3 = 0;
                        ai4 = 2;
                    }
                }
                else
                {
                    npc.velocity *= 0.1f;
                }
            }
            if(nextAttack == 2)
            {
                ai1++;
                npc.rotation = (npc.Center - player.Center).ToRotation();
                npc.velocity *= 0.6f;
                Vector2 toPlayer = player.Center - npc.Center;
                toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
                npc.velocity += toPlayer * 0.8f;
                if (ai2 > 3)
                {
                    Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 12, 0.75f);
                    FireLaser(ModContent.ProjectileType<LaserExplosionBall>(), 3f, 3f);
                    nextAttack = -1;
                    ai1 = 0;
                    ai2 = 0;
                    ai3 = 0;
                    ai4 = 1 + Main.rand.Next(3);
                }
                else
                {
                    Vector2 from = npc.Center + new Vector2(-128, 0).RotatedBy(npc.rotation);
                    if (ai1 % 20 == 0)
                    {
                        for (int i = 0; i < 360; i += 20)
                        {
                            Vector2 circular = new Vector2(48, 0).RotatedBy(MathHelper.ToRadians(i));
                            circular.X *= 0.6f;
                            circular = circular.RotatedBy(npc.rotation);
                            Vector2 dustVelo = -circular * 0.1f;
                            Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, DustID.PinkFlame, 0, 0, npc.alpha);
                            dust.velocity *= 0.15f;
                            dust.velocity += dustVelo;
                            dust.scale = 1.25f;
                            dust.noGravity = true;
                        }
                        ai2++;
                    }
                    for (int j = 0; j < ai2; j++)
                    {
                        Vector2 circular = new Vector2(48, 0).RotatedBy(MathHelper.ToRadians(j * 120 + ai1 * 5));
                        circular.X *= 0.6f;
                        circular = circular.RotatedBy(npc.rotation);
                        Vector2 dustVelo = -circular * 0.09f;
                        Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, DustID.PinkFlame, 0, 0, npc.alpha);
                        dust.velocity *= 0.1f;
                        dust.scale = 1.5f;
                        dust.noGravity = true;
                    }
                }
            }
            if(nextAttack == 3 || nextAttack == 4)
            {
                ai1++;
                npc.rotation = (npc.Center - player.Center).ToRotation();
                npc.velocity *= 0.7f;
                Vector2 toPlayer = player.Center - npc.Center;
                toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
                npc.velocity += toPlayer * 0.8f;
                if (ai2 > 3)
                {
                    Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 44, 1.25f);
                    int type = DustID.Electric;
                    if (ai5 == 30)
                        type = 235;
                    for (int i = 0; i < 360; i += 5)
                    {
                        Vector2 circular = new Vector2(96, 0).RotatedBy(MathHelper.ToRadians(i));
                        Vector2 dustVelo = circular * 0.3f;
                        Dust dust = Dust.NewDustDirect(npc.Center - new Vector2(5) + circular, 0, 0, type, 0, 0, npc.alpha);
                        dust.velocity *= 0.15f;
                        dust.velocity += dustVelo;
                        if (type == 235)
                            dust.scale = 1.75f;
                        dust.noGravity = true;
                    }
                    for(int i = 0; i < 1 + (Main.expertMode ? 1 : 0) + (!runOncePhase2 ? 1 : 0); i++)
                    {
                        if(Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int num = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 20, ModContent.NPCType<CyverBot>(), 0, player.whoAmI, 0, !runOncePhase2 ? -1 : 0);
                            NPC bot = Main.npc[num];
                            bot.netUpdate = true;
                        }
                    }
                    nextAttack = -1;
                    ai1 = 0;
                    ai2 = 0;
                    ai3 = 0;
                    ai4 = 3;
                }
                else
                {
                    int type = DustID.Electric;
                    if (ai5 == 30)
                        type = 235;
                    Vector2 from = npc.Center;
                    if (ai1 % 30 == 0)
                    {
                        for (int i = 0; i < 360; i += 10)
                        {
                            Vector2 circular = new Vector2(96, 0).RotatedBy(MathHelper.ToRadians(i));
                            circular.X *= 0.6f;
                            circular = circular.RotatedBy(npc.rotation);
                            Vector2 dustVelo = -circular * 0.1f;
                            Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, type, 0, 0, npc.alpha);
                            dust.velocity *= 0.15f;
                            dust.velocity += dustVelo;
                            if (type == 235)
                                dust.scale = 1.75f;
                            dust.noGravity = true;
                        }
                        ai2++;
                    }
                    for (int j = 0; j < ai2; j++)
                    {
                        Vector2 circular = new Vector2(216 - ai1, 0).RotatedBy(MathHelper.ToRadians(j * 120 + ai1 * 5));
                        circular.X *= 0.6f;
                        circular = circular.RotatedBy(npc.rotation);
                        Vector2 dustVelo = -circular * 0.09f;
                        Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, type, 0, 0, npc.alpha);
                        dust.velocity *= 0.1f;
                        if (type == 235)
                            dust.scale = 2.25f;
                        dust.noGravity = true;
                    }
                }
            }
            if(nextAttack != 1 && !runOncePhase2)
            {
                ai6++;
                if(ai6 % 360 == 0)
                {
                    for (int i = 0; i < 3 + (Main.expertMode ? 1 : 0); i++)
                    {
                        Vector2 toLocation = player.Center + new Vector2(Main.rand.NextFloat(240, 600), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360)));
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int damage = 70;
                            if (Main.expertMode)
                            {
                                damage = (int)(damage / Main.expertDamage);
                            }
                            Projectile.NewProjectile(toLocation, Vector2.Zero, ModContent.ProjectileType<DarkDagger>(), damage, 0, Main.myPlayer, player.whoAmI);
                        }
                        Vector2 toLocationVelo = toLocation - npc.Center;
                        Vector2 from = npc.Center;
                        for (int j = 0; j < 300; j++)
                        {
                            Vector2 velo = toLocationVelo.SafeNormalize(Vector2.Zero);
                            from += velo * 12;
                            Vector2 circularLocation = new Vector2(10, 0).RotatedBy(MathHelper.ToRadians(j * 12 + dynamicCounter));

                            int dust = Dust.NewDust(from + new Vector2(-4, -4) + circularLocation, 0, 0, 235, 0, 0, 0, default, 1.25f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 0.1f;
                            Main.dust[dust].scale = 1.8f;
                            
                            if ((from - toLocation).Length() < 24)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        public void FireLaser(int type, float speed = 6f, float recoilMult = 2f, float ai1 = 0, float ai2 = 0)
        {
            Player player = Main.player[npc.target];
            Vector2 toPlayer = player.Center - npc.Center;
            toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
            toPlayer *= speed;
            Vector2 from = npc.Center - new Vector2(96, 0).RotatedBy(npc.rotation);
            int damage = 75;
            if (Main.expertMode)
            {
                damage = (int)(damage / Main.expertDamage);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(from, toPlayer, type, damage, 3, Main.myPlayer, ai1, ai2);
            }
            npc.velocity -= toPlayer * recoilMult;
        }
        bool hasDoneDrop = false;
        public void FindNextAttack()
        {
            if(hasDoneDrop)
            {
                nextAttack = Main.rand.Next(3);
                if (Main.rand.NextBool(4 + (!runOncePhase2 ? 1 : 0)))
                    nextAttack = 3;
            }
            else
            {
                nextAttack = 0;
            }
            //nextAttack = Main.rand.Next(3);
            ai1 = 0;
            ai2 = 0;
            ai3 = 0;
            ai4 = 0;
            hasDoneDrop = true;
            if(Main.netMode != NetmodeID.MultiplayerClient)
                npc.netUpdate = true;
        }
    }
    public class EnergyBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Ball");
            Main.projFrames[projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            projectile.width = 62;
            projectile.height = 48;
            projectile.timeLeft = 540;
            projectile.penetrate = -1;
            projectile.damage = 120;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.9f / 255f, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.7f / 255f);
            projectile.rotation = MathHelper.ToRadians(180) + projectile.velocity.ToRotation();
            projectile.frameCounter++;
            if (projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
            }
            float approaching = ((540f - projectile.timeLeft) / 540f);
            Lighting.AddLight(projectile.Center, 0.5f, 0.65f, 0.75f);

            Player player = Main.player[(int)projectile.ai[0]];
            int dust = Dust.NewDust(projectile.Center + new Vector2(-4, -4), 0, 0, DustID.Electric, 0, 0, projectile.alpha, default, 1.25f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.1f;
            Main.dust[dust].scale *= 0.7f;
            if (player.active)
            {
                float x = Main.rand.Next(-10, 11) * 0.001f * approaching;
                float y = Main.rand.Next(-10, 11) * 0.001f * approaching;
                Vector2 toPlayer = projectile.Center - player.Center;
                toPlayer = toPlayer.SafeNormalize(Vector2.Zero);
                projectile.velocity += -toPlayer * (0.155f * projectile.timeLeft / 540f) + new Vector2(x, y);
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 360; i += 5)
            {
                Vector2 circular = new Vector2(12, 0).RotatedBy(MathHelper.ToRadians(i));
                Vector2 dustVelo = circular * 0.5f;
                Dust dust = Dust.NewDustDirect(projectile.Center - new Vector2(5) + circular, 0, 0, DustID.PinkFlame, 0, 0, projectile.alpha);
                dust.velocity *= 0.15f;
                dust.velocity += dustVelo;
                dust.scale = 1.75f;
                dust.noGravity = true;
            }
            base.Kill(timeLeft);
        }
    }
    public class ShadowCyvercry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Cyvercry");
            Main.projFrames[projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            projectile.width = 124;
            projectile.height = 75;
            projectile.timeLeft = 240;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.damage = 80;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 0.75f;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if(projectile.ai[1] == -1)
                return new Color(255, 0, 0);
            return Color.White;
        }
        Vector2 initialVelocity = Vector2.Zero;
        public override void AI()
        {
            if (initialVelocity == Vector2.Zero)
            {
                initialVelocity = projectile.velocity;
            }
            projectile.ai[0]++;
            projectile.velocity += new Vector2(-0.7f, 0).RotatedBy(MathHelper.ToRadians(projectile.ai[0] * 10)).RotatedBy(initialVelocity.ToRotation());
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 1.8f / 255f, (255 - projectile.alpha) * 0.75f / 255f, (255 - projectile.alpha) * 1.9f / 255f);
            projectile.rotation = MathHelper.ToRadians(180) + projectile.velocity.ToRotation();
            if(projectile.timeLeft <= 25)
                projectile.alpha += 10;
        }
    }
    public class LaserExplosionBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Ball");
            Main.projFrames[projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 42;
            projectile.timeLeft = 120;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.damage = 54;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.9f / 255f, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.7f / 255f);
            projectile.rotation = 0;
            projectile.frameCounter++;
            if (projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
            }
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 12, 1f);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 360; i += 20)
                {
                    Projectile.NewProjectile(projectile.Center, new Vector2(4, 0).RotatedBy(MathHelper.ToRadians(i)), ProjectileID.RayGunnerLaser, projectile.damage, 0, Main.myPlayer);
                }
            }
            base.Kill(timeLeft);
        }
    }
    public class DarkDagger : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Secret Shadow Blade"); // ;) Vortex was here
            Main.projFrames[projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 44;
            projectile.timeLeft = 560;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.damage = 56;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool ShouldUpdatePosition()
        {
            return projectile.timeLeft <= 420;
        }
        public override void AI()
        {
            projectile.rotation = MathHelper.ToRadians(90) + projectile.velocity.ToRotation();
            if (projectile.timeLeft == 420)
            {
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 71, 0.75f);
                for (int i = 0; i < 360; i += 5)
                {
                    Vector2 circular = new Vector2(12, 0).RotatedBy(MathHelper.ToRadians(i));
                    Dust dust = Dust.NewDustDirect(projectile.Center - new Vector2(5) + circular, 0, 0, 235, 0, 0, projectile.alpha);
                    dust.velocity *= 0.15f;
                    dust.velocity += -projectile.velocity;
                    dust.scale = 2.75f;
                    dust.noGravity = true;
                }
            }
            if(projectile.timeLeft > 420)
            {
                Player player = Main.player[(int)projectile.ai[0]];
                if (player.active)
                {
                    Vector2 toPlayer = projectile.Center - player.Center;
                    toPlayer = toPlayer.SafeNormalize(Vector2.Zero) * 12;
                    projectile.velocity = -toPlayer;
                }
            }
            else
            {
                projectile.hostile = true;
                int dust = Dust.NewDust(projectile.Center + new Vector2(-4, -4), 0, 0, 235, 0, 0, projectile.alpha, default, 1.25f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].scale *= 0.75f;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 1.8f / 255f, (255 - projectile.alpha) * 0.0f / 255f, (255 - projectile.alpha) * 0.0f / 255f);
            if (projectile.timeLeft <= 25)
                projectile.alpha += 10;
        }
    }
}