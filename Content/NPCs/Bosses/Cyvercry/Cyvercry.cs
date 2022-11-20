using AerovelenceMod.Common.Globals.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry //Change me
{
    [AutoloadBossHead]
    public class Cyvercry : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyvercry"); //DONT Change me
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
        }
        ArmorShaderData dustShader = null; 

        public override bool CheckActive()
        {
            return false;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 37500;
            NPC.damage = 105;
            NPC.defense = 30;
            NPC.knockBackResist = 0f;
            NPC.width = 180;
            NPC.height = 100;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = Item.buyPrice(0, 22, 11, 5);
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Cyvercry");
            }
            dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 41500;
            NPC.damage = 125;
            NPC.defense = 35;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if(NPC.frameCounter >= 10)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 5;
                if (NPC.frame.Y > 4 * frameHeight)
                {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0;
                }  
                if(NPC.frame.Y == 3 * frameHeight && nextAttack != 1) //booster frame
                {
                    Vector2 from = NPC.Center + new Vector2(64, 0).RotatedBy(NPC.rotation);
                    int type = DustID.Electric;
                    if (ai5 == 30)
                        type = 235;
                    for (int i = 0; i < 360; i += 20)
                    {
                        Vector2 circular = new Vector2(24, 0).RotatedBy(MathHelper.ToRadians(i));
                        circular.Y *= 0.7f;
                        circular = circular.RotatedBy(NPC.rotation);
                        Vector2 dustVelo = new Vector2(12, 0).RotatedBy(NPC.rotation);


                        Dust d = GlowDustHelper.DrawGlowDustPerfect(from - new Vector2(5) + circular, ModContent.DustType<GlowCircleSpinner>(), dustVelo, new Color(67, 215, 209), 0.4f, 0.6f, 0f, dustShader);
                        d.scale = 0.4f;
                        
                        //int a = GlowDustHelper.DrawGlowDust(from - new Vector2(5) + circular, 0, 0, ModContent.DustType<GlowCircleQuadStar>(), Color.SkyBlue, 0.6f, 0.6f, 0f, dustShader);
                        //Main.dust[a].velocity *= 0.15f;
                        //Main.dust[a].velocity += dustVelo;
                        //Main.dust[a].scale = 0.6f;

                        //Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, type, 0, 0, NPC.alpha);
                        //dust.velocity *= 0.15f;
                        //dust.velocity += dustVelo;
                        //dust.noGravity = true;
                        //if (type == 235)
                            //dust.scale *= 2.5f;
                    }
                }
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            /*
            var entitySource = NPC.GetSource_Death();

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CyvercryBag>()));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CyvercryTrophy>(), 10));

            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<RimegeistRelic>()));

            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<RimegeistPetItem>(), 4));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CyvercryMask>(), 7));

            npcLoot.Add(notExpertRule);

            if (!Main.expertMode)
            {
                if (Main.rand.NextBool(7))
                {
                    Item.NewItem(entitySource, NPC.getRect(), ModContent.ItemType<CyvercryMask>());
                }
                if (Main.rand.NextBool(10))
                {
                    Item.NewItem(entitySource, NPC.getRect(), ModContent.ItemType<CyvercryTrophy>());
                }
                int rand = Main.rand.Next(4);
                if (rand == 0)
                    Item.NewItem(entitySource, NPC.getRect(), ModContent.ItemType<CyverCannon>());
                if (rand == 1)
                    Item.NewItem(entitySource, NPC.getRect(), ModContent.ItemType<Cyverthrow>());
                if (rand == 2)
                    Item.NewItem(entitySource, NPC.getRect(), ModContent.ItemType<DarknessDischarge>());
                if (rand == 3)
                    Item.NewItem(entitySource, NPC.getRect(), ModContent.ItemType<Oblivion>());
            }
            */
        }


        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;

            DownedWorld.DownedCyvercry = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        float alpha = 1f;
            if (nextAttack == 1)
            {
                alpha = (240 - ai1 + ai2) / 240f;
            }
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Glowmask");
            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, NPC.frame, Color.White * alpha, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            float alpha = 1f;
            float shade = 1f;
            Color color = drawColor;
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
                    texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/CyvercryRed").Value;
                    Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + rotationalPos, NPC.frame, color2 * alpha, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
            }
            texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Cyvercry").Value;
            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, NPC.frame, color * alpha, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            if(shadowTrail)
            {
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY);
                    color = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                    Main.EntitySpriteDraw((Texture2D)TextureAssets.Npc[NPC.type], drawPos, NPC.frame, color * 0.5f, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
            }
            return false;
        }
        private float nextAttack
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        private float ai1
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        private float ai2
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        private float ai3
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
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
            var entitySource = NPC.GetSource_FromAI();

            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.netUpdate = true;
            Player player = Main.player[NPC.target];
            NPC.spriteDirection = -1;
            if (Main.expertMode)
                NPC.defense = 35;
            else
                NPC.defense = 30;
            if (runOnce)
            {
                prevCenter = player.Center;
                nextAttack = -1;
                ai1 = -180;
                runOnce = false;
            }
            if(runOncePhase2 && NPC.life < NPC.lifeMax * 0.5f)
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
                NPC.rotation = (NPC.Center - player.Center).ToRotation();
                NPC.velocity.Y -= 0.09f;
                NPC.timeLeft = 100;
                if(NPC.position.Y <= 16 * 35) //checking for top of the world practically
                    NPC.active = false;
                return;
            }
            if (ai5 > 30)
            {
                NPC.velocity *= 0.8f;
                NPC.dontTakeDamage = false;
                dynamicCounter += 6;
                Vector2 dynamicAddition2 = new Vector2(45 * (ai5 - 30f) / 210f, 0).RotatedBy(MathHelper.ToRadians(dynamicCounter));
                NPC.rotation = (NPC.Center - player.Center).ToRotation() + MathHelper.ToRadians(dynamicAddition2.X);
                ai5--;
                if(ai5 == 30)
                {
                    for (int i = 0; i < 360; i += 5)
                    {
                        Vector2 circular = new Vector2(96, 0).RotatedBy(MathHelper.ToRadians(i));
                        Vector2 dustVelo = circular * 0.4f;
                        Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(5) + circular, 0, 0, 235, 0, 0, NPC.alpha);
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
                        Main.NewText("ai2 >= 180");
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
                    NPC.rotation = (NPC.Center - player.Center).ToRotation();
                }
                float dist = 300;
                if (ai1 <= 10)
                    dist = 650;
                Vector2 dynamicAddition = new Vector2(30, 0).RotatedBy(MathHelper.ToRadians(dynamicCounter));
                Vector2 circular = new Vector2(0, 96).RotatedBy(MathHelper.ToRadians(ai2));
                Vector2 goTo = prevCenter - direction * new Vector2(-dist + circular.X, 0).RotatedBy(MathHelper.ToRadians(dynamicAddition.X));
                goTo -= NPC.Center;
                float distance = goTo.Length();
                goTo = goTo.SafeNormalize(Vector2.Zero);
                if(ai2 > 120)
                {
                    Main.NewText(NPC.velocity);
                    Main.NewText("!");
                    NPC.rotation = MathHelper.ToRadians(180) + goTo.ToRotation();
                }
                if (speed > distance) 
                {
                    speed = distance;
                    ai3 = 0;
                }
                if(ai3 > 0)
                {
                    //SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = 1f, PitchVariance = .04f, Volume = 0.6f, MaxInstances = 1 };
                    //SoundEngine.PlaySound(style, NPC.Center);
                    ai3--;
                    shadowTrail = true;
                    if (((ai3 % 6 == 0 && !Main.expertMode) || (ai3 % 5 == 0 && Main.expertMode)) && !runOncePhase2)
                    {
                        FireLaser(ModContent.ProjectileType<EnergyBall>(), 1, 0, player.whoAmI);
                    }
                }
                else
                    shadowTrail = false;
                NPC.velocity *= 0.8f;
                NPC.velocity += 0.4f * goTo * (speed + distance * 0.01f);
                if(ai1 % 30 == 0 && ai1 >= -90 && ai1 <= 0)
                {
                    SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .56f, PitchVariance = .27f, MaxInstances = -1 };
                    SoundEngine.PlaySound(stylea, NPC.Center);
                    SoundEngine.PlaySound(stylea, NPC.Center);


                    SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .33f, Pitch = .73f, PitchVariance = .27f, MaxInstances = -1 };
                    SoundEngine.PlaySound(styleb, NPC.Center);
                    SoundEngine.PlaySound(styleb, NPC.Center);


                    SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = .38f, Volume = 0.7f }; //1f
                    SoundEngine.PlaySound(stylec, NPC.Center);

                    //SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                    FireLaser(ModContent.ProjectileType<CyverLaser>()); //Death Laser
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
                NPC.rotation = (NPC.Center - player.Center).ToRotation();
                NPC.velocity *= 0.6f;
                Vector2 toPlayer = player.Center - NPC.Center;
                toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
                NPC.velocity += toPlayer * 0.8f;
                if (ai2 > 3)
                {
                    if (ai1 % 5 == 0)
                    {
                        if (Main.expertMode)
                        {
                            SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .56f, PitchVariance = .27f, };
                            SoundEngine.PlaySound(stylea, NPC.Center);
                            SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .33f, Pitch = .73f, PitchVariance = .27f, };
                            SoundEngine.PlaySound(styleb, NPC.Center);
                            FireLaser(ModContent.ProjectileType<CyverLaser>(), 13f, 0.7f);

                        }
                        else
                        {
                            SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .56f, PitchVariance = .27f, };
                            SoundEngine.PlaySound(stylea, NPC.Center);
                            SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .33f, Pitch = .73f, PitchVariance = .27f, };
                            SoundEngine.PlaySound(styleb, NPC.Center);
                            FireLaser(ModContent.ProjectileType<CyverLaser>(), 11f, 0.7f);

                        }
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
                        NPC.defense = 65;
                    else
                        NPC.defense = 60;
                    Vector2 from = NPC.Center + new Vector2(-128, 0).RotatedBy(NPC.rotation);
                    if (ai1 % 50 == 0)
                    {
                        for (int i = 0; i < 360; i += 20)
                        {
                            Vector2 circular = new Vector2(64, 0).RotatedBy(MathHelper.ToRadians(i));
                            circular.X *= 0.6f;
                            circular = circular.RotatedBy(NPC.rotation);
                            Vector2 dustVelo = -circular * 0.1f;

                            Dust b = GlowDustHelper.DrawGlowDustPerfect(from - new Vector2(5) + circular, ModContent.DustType<GlowCircleQuadStar>(), Vector2.Zero, Color.DeepPink, 0.7f, 0.6f, 0f, dustShader);
                            

                            //int a = GlowDustHelper.DrawGlowDust(from - new Vector2(5) + circular, 0, 0, ModContent.DustType<GlowCircleQuadStar>(), Color.DeepPink, 0.7f, 0.6f, 0f, dustShader);
                            //Main.dust[a].velocity *= 0.15f;
                            //Main.dust[a].velocity += dustVelo;

                            /*
                            Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, DustID.Electric, 0, 0, NPC.alpha);
                            dust.velocity *= 0.15f;
                            dust.velocity += dustVelo;
                            dust.scale = 1.2f;
                            dust.noGravity = true;
                            */
                        }
                        ai2++;
                    }
                    for(int j = 0; j < ai2; j++)
                    {
                        Vector2 circular = new Vector2(64, 0).RotatedBy(MathHelper.ToRadians(j * 120 + ai1 * 4));
                        circular.X *= 0.6f;
                        circular = circular.RotatedBy(NPC.rotation);
                        Vector2 dustVelo = -circular * 0.09f;
                        Dust b = GlowDustHelper.DrawGlowDustPerfect(from - new Vector2(5) + circular, ModContent.DustType<GlowCircleQuadStar>(), Vector2.Zero, Color.DeepPink, 0.7f, 0.6f, 0f, dustShader);


                        //int a = GlowDustHelper.DrawGlowDust(from - new Vector2(5) + circular, 0, 0, ModContent.DustType<GlowCircleQuadStar>(), Color.DeepPink, 0.7f, 0.6f, 0f, dustShader);
                        //Main.dust[a].velocity *= 0.1f;

                        //Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, DustID.Electric, 0, 0, NPC.alpha);
                        //dust.velocity *= 0.1f;
                        //dust.scale = 1.4f;
                        //dust.noGravity = true;
                    }
                    
                }
            }
            if(nextAttack == 1)
            {
                ai1 += 6;
                if(ai1 > 60 && ai2 == 0)
                {
                    NPC.dontTakeDamage = true;
                }
                if (ai1 > 240)
                {
                    ai4++;
                    if(ai3 < 10)
                    {
                        if ((ai4 % 25 == 0 && !Main.expertMode) || (ai4 % 20 == 0 && (!runOncePhase2 || Main.expertMode)))
                        {
                            NPC.Center = player.Center + new Vector2(1256, 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360)));
                            ai3++;
                            //SoundEngine.PlaySound(SoundLoader.customSoundType, -1, -1, Mod.GetSoundSlot(SoundType.Custom, "Sounds/Effects/Test"));
                            FireLaser(ModContent.ProjectileType<ShadowCyvercry>(), 20, 0, 0, !runOncePhase2 ? -1 : 0);
                        }
                    }
                    else if(ai2 < 240)
                    {
                        NPC.dontTakeDamage = false;
                        NPC.rotation = (NPC.Center - player.Center).ToRotation();
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
                    NPC.velocity *= 0.1f;
                }
            }
            if(nextAttack == 2)
            {
                ai1++;
                NPC.rotation = (NPC.Center - player.Center).ToRotation();
                NPC.velocity *= 0.6f;
                Vector2 toPlayer = player.Center - NPC.Center;
                toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
                NPC.velocity += toPlayer * 0.8f;
                if (ai2 > 3)
                {
                    SoundEngine.PlaySound(SoundID.Item67, NPC.Center);
                    FireLaser(ModContent.ProjectileType<LaserExplosionBall>(), 3f, 3f);
                    nextAttack = -1;
                    ai1 = 0;
                    ai2 = 0;
                    ai3 = 0;
                    ai4 = 1 + Main.rand.Next(3);
                }
                else
                {
                    Vector2 from = NPC.Center + new Vector2(-128, 0).RotatedBy(NPC.rotation);
                    if (ai1 % 20 == 0)
                    {
                        for (int i = 0; i < 360; i += 20)
                        {
                            Vector2 circular = new Vector2(48, 0).RotatedBy(MathHelper.ToRadians(i));
                            circular.X *= 0.6f;
                            circular = circular.RotatedBy(NPC.rotation);
                            Vector2 dustVelo = -circular * 0.1f;


                            int a = GlowDustHelper.DrawGlowDust(from - new Vector2(5) + circular, 0, 0, ModContent.DustType<GlowCircleQuadStar>(), Color.DeepPink, 0.7f, 0.6f, 0f, dustShader);
                            Main.dust[a].velocity *= 0.15f;
                            Main.dust[a].velocity += dustVelo;

                            //Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, DustID.PinkTorch, 0, 0, NPC.alpha);
                            //dust.velocity *= 0.15f;
                            //dust.velocity += dustVelo;
                            //dust.scale = 1.25f;
                            //dust.noGravity = true;
                        }
                        ai2++;
                    }
                    for (int j = 0; j < ai2; j++)
                    {


                        Vector2 circular = new Vector2(48, 0).RotatedBy(MathHelper.ToRadians(j * 120 + ai1 * 5));
                        circular.X *= 0.6f;
                        circular = circular.RotatedBy(NPC.rotation);
                        Vector2 dustVelo = -circular * 0.09f;


                        int a = GlowDustHelper.DrawGlowDust(from - new Vector2(5) + circular, 0, 0, ModContent.DustType<GlowCircleQuadStar>(), Color.DeepPink, 0.7f, 0.6f, 0f, dustShader);
                        Main.dust[a].velocity *= 0.1f;
                        Main.dust[a].velocity += dustVelo;

                        //Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, DustID.PinkTorch, 0, 0, NPC.alpha);
                        //dust.velocity *= 0.1f;
                        //dust.scale = 1.5f;
                        //dust.noGravity = true;
                    }
                }
            }
            if(nextAttack == 3 || nextAttack == 4)
            {
                ai1++;
                NPC.rotation = (NPC.Center - player.Center).ToRotation();
                NPC.velocity *= 0.7f;
                Vector2 toPlayer = player.Center - NPC.Center;
                toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
                NPC.velocity += toPlayer * 0.8f;
                if (ai2 > 3)
                {
                    SoundEngine.PlaySound(SoundID.Item68, NPC.Center);
                    int type = DustID.Electric;
                    if (ai5 == 30)
                        type = 235;
                    for (int i = 0; i < 360; i += 5)
                    {
                        Vector2 circular = new Vector2(96, 0).RotatedBy(MathHelper.ToRadians(i));
                        Vector2 dustVelo = circular * 0.3f;
                        Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(5) + circular, 0, 0, type, 0, 0, NPC.alpha);
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
                            int num = NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y + 20, ModContent.NPCType<CyverBot>(), 0, player.whoAmI, 0, !runOncePhase2 ? -1 : 0);
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
                    Vector2 from = NPC.Center;
                    if (ai1 % 30 == 0)
                    {
                        for (int i = 0; i < 360; i += 10)
                        {
                            Vector2 circular = new Vector2(96, 0).RotatedBy(MathHelper.ToRadians(i));
                            circular.X *= 0.6f;
                            circular = circular.RotatedBy(NPC.rotation);
                            Vector2 dustVelo = -circular * 0.1f;
                            Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, type, 0, 0, NPC.alpha);
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
                        circular = circular.RotatedBy(NPC.rotation);
                        Vector2 dustVelo = -circular * 0.09f;
                        Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, type, 0, 0, NPC.alpha);
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
                            /*if (Main.expertMode)
                            {
                                damage = (int)(damage / Main.expertDamage);
                            }*/
                            Projectile.NewProjectile(entitySource, toLocation, Vector2.Zero, ModContent.ProjectileType<DarkDagger>(), damage, 0, Main.myPlayer, player.whoAmI);
                        }
                        Vector2 toLocationVelo = toLocation - NPC.Center;
                        Vector2 from = NPC.Center;
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
            var entitySource = NPC.GetSource_FromAI();
            Player player = Main.player[NPC.target];
            Vector2 toPlayer = player.Center - NPC.Center;
            toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
            toPlayer *= speed;
            Vector2 from = NPC.Center - new Vector2(96, 0).RotatedBy(NPC.rotation);
            int damage = 75;
            if (Main.expertMode)
            {
                damage = (int)(damage * 1.5f);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(entitySource, from, toPlayer, type, damage, 3, Main.myPlayer, ai1, ai2);
                if (type == ProjectileID.MartianWalkerLaser)
                {
                    Main.projectile[p].scale = 2f;
                }
            }
            NPC.velocity -= toPlayer * recoilMult;
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
                NPC.netUpdate = true;
        }
    }
    public class EnergyBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Ball");
            Main.projFrames[Projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 48;
            Projectile.timeLeft = 540;
            Projectile.penetrate = -1;
            Projectile.damage = 120;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.9f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.7f / 255f);
            Projectile.rotation = MathHelper.ToRadians(180) + Projectile.velocity.ToRotation();
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            float approaching = ((540f - Projectile.timeLeft) / 540f);
            Lighting.AddLight(Projectile.Center, 0.5f, 0.65f, 0.75f);

            Player player = Main.player[(int)Projectile.ai[0]];
            int dust = Dust.NewDust(Projectile.Center + new Vector2(0, -4), 0, 0, DustID.Electric, 0, 0, Projectile.alpha, default, 0.5f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity += Projectile.velocity;
            Main.dust[dust].velocity *= 0.1f;
            Main.dust[dust].scale *= 0.7f;
            if (player.active)
            {
                float x = Main.rand.Next(-10, 11) * 0.001f * approaching;
                float y = Main.rand.Next(-10, 11) * 0.001f * approaching;
                Vector2 toPlayer = Projectile.Center - player.Center;
                toPlayer = toPlayer.SafeNormalize(Vector2.Zero);
                Projectile.velocity += -toPlayer * (0.155f * Projectile.timeLeft / 540f) + new Vector2(x, y);
            }

            if (Projectile.timeLeft == 300)
                Projectile.Kill();


        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item94 with { Pitch = 0.4f, Volume = 0.45f, PitchVariance = 0.2f }, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0, Main.myPlayer);
            /*
            for (int i = 0; i < 360; i += 5)
            {
                Vector2 circular = new Vector2(12, 0).RotatedBy(MathHelper.ToRadians(i));
                Vector2 dustVelo = circular * 0.5f;
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(5) + circular, 0, 0, DustID.PinkCrystalShard, 0, 0, Projectile.alpha);
                dust.velocity *= 0.15f;
                dust.velocity += dustVelo;
                dust.scale = 1.75f;
                dust.noGravity = true;
            }
            base.Kill(timeLeft);
            */
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 bonus = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5f;
            //Draw the Circlular Glow
            var Tex = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + bonus, Tex.Frame(1, 1, 0, 0), Color.DeepPink * 0.5f, Projectile.rotation, Tex.Size() / 2, 0.91f, SpriteEffects.None, 0f);


            var BallTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/EnergyBall").Value;

            int frameHeight = BallTexture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, BallTexture.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, sourceRectangle, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            return false;

        }
    }
    public class ShadowCyvercry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Cyvercry");
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 124;
            Projectile.height = 75;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.damage = 80;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.75f;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if(Projectile.ai[1] == -1)
                return new Color(255, 0, 0);
            return Color.White;
        }
        Vector2 initialVelocity = Vector2.Zero;
        public override void AI()
        {
            if (initialVelocity == Vector2.Zero)
            {
                initialVelocity = Projectile.velocity;
            }
            Projectile.ai[0]++;
            Projectile.velocity += new Vector2(-0.7f, 0).RotatedBy(MathHelper.ToRadians(Projectile.ai[0] * 10)).RotatedBy(initialVelocity.ToRotation());
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 1.8f / 255f, (255 - Projectile.alpha) * 0.75f / 255f, (255 - Projectile.alpha) * 1.9f / 255f);
            Projectile.rotation = MathHelper.ToRadians(180) + Projectile.velocity.ToRotation();
            if(Projectile.timeLeft <= 25)
                Projectile.alpha += 10;
        }
    }
    public class LaserExplosionBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Ball");
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 42;
            Projectile.timeLeft = 1;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.damage = 54;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.9f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.7f / 255f);
            Projectile.rotation = 0;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
        }
        public override void Kill(int timeLeft)
        {
            var entitySource = Projectile.GetSource_FromAI();
            //SoundEngine.PlaySound(SoundID.Item94 with { Pitch = 0.4f, Volume = 0.75f, PitchVariance = 0.2f }, Projectile.Center);
            //SoundEngine.PlaySound(SoundID.Item91 with { Pitch = 0.4f }, Projectile.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 360; i += 30)
                {
                    Projectile.NewProjectile(entitySource, Projectile.Center, new Vector2(6, 0).RotatedBy(MathHelper.ToRadians(i)), ModContent.ProjectileType<CyverLaser>(), Projectile.damage, 0, Main.myPlayer);
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
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 44;
            Projectile.timeLeft = 560;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.damage = 56;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.timeLeft <= 420;
        }
        public override void AI()
        {
            Projectile.rotation = MathHelper.ToRadians(90) + Projectile.velocity.ToRotation();
            if (Projectile.timeLeft == 420)
            {
                //SoundEngine.PlaySound(SoundID.Item, Projectile.Center);
                for (int i = 0; i < 360; i += 5)
                {
                    Vector2 circular = new Vector2(12, 0).RotatedBy(MathHelper.ToRadians(i));
                    Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(5) + circular, 0, 0, 235, 0, 0, Projectile.alpha);
                    dust.velocity *= 0.15f;
                    dust.velocity += -Projectile.velocity;
                    dust.scale = 2.75f;
                    dust.noGravity = true;
                }
            }
            if(Projectile.timeLeft > 420)
            {
                Player player = Main.player[(int)Projectile.ai[0]];
                if (player.active)
                {
                    Vector2 toPlayer = Projectile.Center - player.Center;
                    toPlayer = toPlayer.SafeNormalize(Vector2.Zero) * 12;
                    Projectile.velocity = -toPlayer;
                }
            }
            else
            {
                Projectile.hostile = true;
                int dust = Dust.NewDust(Projectile.Center + new Vector2(-4, -4), 0, 0, 235, 0, 0, Projectile.alpha, default, 1.25f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].scale *= 0.75f;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 1.8f / 255f, (255 - Projectile.alpha) * 0.0f / 255f, (255 - Projectile.alpha) * 0.0f / 255f);
            if (Projectile.timeLeft <= 25)
                Projectile.alpha += 10;
        }
    }
}
