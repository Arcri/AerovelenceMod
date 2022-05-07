using System;
using AerovelenceMod.Core.Prim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;

namespace AerovelenceMod.Content.NPCs.Bosses.LightningMoth
{
    //[AutoloadBossHead]
    public class LightningMoth : ModNPC
    {

        float colorLerp = 0f;

        float phase = 0;
        float phase3Glow = 0f;

        float cos1 = 0;

        bool dashing = false;
        float dashTrailLength = 0;
        Vector2 aiVector = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;    //boss frame/animation 
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 12500;   //boss life
            NPC.damage = 32;  //boss damage
            NPC.defense = 9;    //boss defense
            NPC.alpha = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 222;
            NPC.height = 174;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(0, 5, 75, 45);
            NPC.npcSlots = 1f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit44;
            NPC.DeathSound = SoundID.NPCHit46;
            NPC.buffImmune[24] = true;
            NPC.alpha = 150;

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LightningMoth");
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),
				new FlavorTextBestiaryInfoElement("Lightning Moth bestiary text blah blah blah")
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        Main.instance.LoadProjectile(NPC.type);
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D glowTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/LightningMoth/LightningMoth_Glow");
            Texture2D auraTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/LightningMoth/LightningMoth_Aura");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //if (dashing)
            {
                for (int j = 0; j < dashTrailLength; j++)
                {
                    Main.EntitySpriteDraw((Texture2D)TextureAssets.Npc[NPC.type], NPC.oldPos[j] + new Vector2(NPC.width / 2, NPC.height / 2) - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, Color.CornflowerBlue, NPC.rotation, NPC.frame.Size() / 2, 1f, effects, 0);
                }
                for (int i = 0; i <= 4; i++)
                {
                    Main.EntitySpriteDraw((Texture2D)TextureAssets.Npc[NPC.type], NPC.Center + (new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)) * phase3Glow) + new Vector2(0, -5 * phase3Glow).RotatedBy(MathHelper.ToRadians(360 / 4 * i)) - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, Color.CornflowerBlue, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }
            }

            for (int i = 0; i <= 4; i++)
            {
                Main.EntitySpriteDraw(auraTex, NPC.Center + new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)) - Main.screenPosition + new Vector2(0, NPC.gfxOffY), new Rectangle(0, 0, auraTex.Width, auraTex.Height), Color.Lerp(new Color(15, 15, 25), Color.CornflowerBlue, colorLerp), NPC.rotation, auraTex.Size() / 2, ((1f + ((float)Math.Cos(cos1 / 12) * 0.1f)) * phase3Glow) + MathHelper.Lerp(colorLerp, colorLerp / 2, phase3Glow), effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw((Texture2D)TextureAssets.Npc[NPC.type], new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(glowTex, new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        private int cooldownFrames
        {
            get
            {
                return (int)NPC.ai[0];
            }
            set
            {
                NPC.ai[0] = value;
            }
        }
        private CurrentAttack currentAttack
        {
            get
            {
                return (CurrentAttack)(int)NPC.ai[1];
            }
            set
            {
                NPC.ai[1] = (int)value;
            }
        }
        private enum CurrentAttack
        {
            IdleFloat = 0,
            Dash = 1,
            LightningStorm = 2,
            SummonMoths = 3,
            BlastVolleys = 4,
            Divebomb = 5,
            CrystalStomp = 6,
            CrystalSpin = 7,
            JumpAtPlayer = 8,
            SummonJolts = 9,
            Shotgun = 10,
            Teleport = 11,
            IdleRun = 12,
            JumpAndDash = 13
        }
        public bool phaseTwo => NPC.life < NPC.lifeMax / 2;
        public override void AI()
        {
            var entitySource = NPC.GetSource_FromAI();

            if (NPC.life > NPC.lifeMax * 0.8f)
            {
                if (phase == 0)
                {
                    cooldownFrames = 60;
                }
                phase = 1;
            }
            else if (NPC.life > NPC.lifeMax * 0.4f)
            {
                if (phase == 1)
                {
                    cooldownFrames = 2;
                }
                phase = 2;
            }
            else
            {
                if (phase == 2)
                {
                    cooldownFrames = 2;
                    currentAttack = CurrentAttack.LightningStorm;
                }
                phase = 3;
            }

            if (phase == 3)
            {
                phase3Glow = MathHelper.Lerp(phase3Glow, 1f, 0.1f);
            }

            if (cos1 == 0)
            {
                currentAttack = CurrentAttack.IdleRun;
            }
            cos1++;

            Lighting.AddLight(NPC.Center, Color.CornflowerBlue.ToVector3());

            NPC.TargetClosest();
            CheckPlatform(Main.player[NPC.target]);

            dashTrailLength = MathHelper.Lerp(dashTrailLength, dashing || phase == 3 ? 10 : 3, 0.5f);

            if (cooldownFrames <= 0)
            {
                Player player = Main.player[NPC.target];
                if (player.dead)
                {
                    NPC.velocity.Y -= 1;
                    if (NPC.Distance(player.Center) > 1920) NPC.active = false;
                }
                else
                {
                    switch (currentAttack)
                    {
                        case CurrentAttack.IdleFloat:
                            IdleFloat();
                            break;
                        case CurrentAttack.Dash:
                            Dash();
                            break;
                        case CurrentAttack.LightningStorm:
                            LightningStorm();
                            break;
                        case CurrentAttack.SummonMoths:
                            cooldownFrames = 2;
                            break;
                        case CurrentAttack.BlastVolleys:
                            BlastVolleys();
                            break;
                        case CurrentAttack.Divebomb:
                            DiveBomb();
                            break;
                        case CurrentAttack.Shotgun:
                            Shotgun();
                            break;
                        case CurrentAttack.CrystalStomp:
                            CrystalStomp();
                            break;
                        case CurrentAttack.CrystalSpin:
                            cooldownFrames = 2;
                            break;
                        case CurrentAttack.JumpAndDash:
                            JumpAndDash();
                            break;
                        case CurrentAttack.Teleport:
                            Teleport();
                            break;
                        case CurrentAttack.IdleRun:
                            IdleRun();
                            break;
                        default:
                            Main.NewText("Error");
                            cooldownFrames = 2;
                            break;
                    }
                }
            }
            else
            {
                if (cooldownFrames == 1)
                {
                    CurrentAttack attack = CurrentAttack.IdleFloat;
                    if (phase == 1)
                    {
                        attack = CurrentAttack.IdleRun;
                        if (currentAttack == CurrentAttack.IdleRun)
                        {
                            int att = Main.rand.Next(2, 4);
                            switch (att)
                            {
                                case 1:
                                    attack = CurrentAttack.IdleRun;
                                    break;
                                case 2:
                                    attack = CurrentAttack.JumpAndDash;
                                    break;
                                case 3:
                                    attack = CurrentAttack.CrystalStomp;
                                    break;
                                default:
                                    attack = CurrentAttack.IdleRun;
                                    break;

                            }
                        }
                        else
                        {
                            attack = CurrentAttack.IdleRun;
                        }
                    }
                    else
                    {
                        if (currentAttack != CurrentAttack.Divebomb)
                        {
                            NPC.noTileCollide = true;
                        }
                    }
                    if (phase == 2)
                    {
                        int att = Main.rand.Next(6);
                        switch (att)
                        {
                            case 1:
                                attack = CurrentAttack.Dash;
                                break;
                            case 2:
                                attack = CurrentAttack.BlastVolleys;
                                break;
                            case 3:
                                attack = CurrentAttack.Divebomb;
                                break;
                            case 4:
                                attack = CurrentAttack.Shotgun;
                                break;
                            case 5:
                                attack = CurrentAttack.Teleport;
                                break;
                            default:
                                attack = CurrentAttack.IdleFloat;
                                break;

                        }
                    }
                    if (phase == 3)
                    {
                        int att = Main.rand.Next(7);
                        switch (att)
                        {
                            case 1:
                                attack = CurrentAttack.Dash;
                                break;
                            case 2:
                                attack = CurrentAttack.BlastVolleys;
                                break;
                            case 3:
                                attack = CurrentAttack.Divebomb;
                                break;
                            case 4:
                                attack = CurrentAttack.LightningStorm;
                                break;
                            case 5:
                                attack = CurrentAttack.Shotgun;
                                break;
                            case 6:
                                attack = CurrentAttack.Teleport;
                                break;
                            default:
                                attack = CurrentAttack.IdleFloat;
                                break;

                        }
                    }
                    currentAttack = attack;
                }
                attackCounter = 0;
                cooldownFrames--;
            }
        }

        public float trueFrame;
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 268;
            NPC.frame.X = ((int)trueFrame % 4) * NPC.frame.Width;
            NPC.frame.Y = (((int)trueFrame - ((int)trueFrame % 4)) / 4) * NPC.frame.Height;
        }
        internal void UpdateFrame(float speed, int minFrame, int maxFrame)
        {
            trueFrame += speed;
            if (trueFrame < minFrame)
            {
                trueFrame = minFrame;
            }
            if (trueFrame > maxFrame)
            {
                trueFrame = minFrame;
            }
        }

        #region attacks
        int attackCounter;

        private void Shotgun()
        {
            var entitySource = NPC.GetSource_FromAI();
            attackCounter++;
            attackCounter++;
            Player player = Main.player[NPC.target];
            UpdateFrame(0.3f, 0, 5);
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = 0f + MathHelper.ToRadians(NPC.velocity.X / 2);

            if (attackCounter >= 180)
            {
                cooldownFrames = 10;
                return;
            }
            else if (attackCounter >= 130)
            {
                NPC.velocity *= 0.92f;
                if (attackCounter % 20 == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(entitySource, NPC.Center, NPC.DirectionTo(player.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.ToRadians(10), MathHelper.ToRadians(10))) * 10, ModContent.ProjectileType<LightningJoltProjLarge>(), 15, 5, ai1: 1);
                        (proj.ModProjectile as LightningJoltProjLarge).aiVector = player.Center + new Vector2(Main.rand.Next(-50, 50), Main.rand.Next(-50, 50));
                    }

                    SoundEngine.PlaySound(SoundID.Item67, NPC.Center);
                }
            }
            else if (attackCounter > 60)
            {
                colorLerp = MathHelper.Lerp(colorLerp, 0f, 0.1f);
                NPC.velocity *= 0.93f;
            }
            else
            {
                colorLerp = MathHelper.Lerp(colorLerp, 1f, 0.05f);
                Vector2 dir = new Vector2((float)Math.Sign(NPC.Center.X - player.Center.X) * 300, -100);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center + dir) * (NPC.Distance(player.Center + dir) / 15), 0.3f);
            }
        }

        private void CrystalStomp()
        {
            var entitySource = NPC.GetSource_FromAI();
            attackCounter++;
            // frame 19-26. on frame 24 spawn crystals

            if (attackCounter >= 70)
            {
                cooldownFrames = 10;
                NPC.noTileCollide = true;
                return;
            }
            else
            {
                NPC.velocity.X *= 0.92f;
                NPC.noTileCollide = false;
                if (attackCounter <= 35)
                {
                    UpdateFrame(0f, 16, 16);
                }
                else
                {
                    UpdateFrame(0.2f, 17, 24);
                    if (attackCounter == 35 + 25)
                    {
                        SoundEngine.PlaySound(SoundID.Item69, NPC.Center);
                        for (int i = 1; i <= 3; i++)
                        {
                            int np = NPC.NewNPC(entitySource, (int)(NPC.Center.X + NPC.direction * (160 * i)), (int)NPC.Bottom.Y, ModContent.NPCType<LightningCrystal>());
                            (Main.npc[np].ModNPC as LightningCrystal).aiVector = Main.npc[np].Center + new Vector2(0, -80 - (i * 30));
                        }
                    }
                }
            }
        }

        private void Circular()
        {

        }

        private void Teleport()
        {
            var entitySource = NPC.GetSource_FromAI();
            UpdateFrame(0.3f, 0, 5);
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = 0f + MathHelper.ToRadians(NPC.velocity.X / 2);

            Player player = Main.player[NPC.target];
            int distance = Main.rand.Next(350, 550);

            attackCounter++;

            if (attackCounter == 100)
            {

                SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 66);
                Projectile.NewProjectile(entitySource, NPC.position, Vector2.Zero, ModContent.ProjectileType<LightningGem>(), 30, 0f, Main.myPlayer, 0f, 0f);
                int rot = Main.rand.Next(360);
                double anglex = Math.Sin(rot * (Math.PI / 180));
                double angley = Math.Cos(rot * (Math.PI / 180));
                NPC.position.X = player.Center.X + (int)(distance * anglex);
                NPC.position.Y = player.Center.Y + (int)(distance * angley);

                colorLerp = MathHelper.Lerp(colorLerp, 0f, 0.1f);
                NPC.velocity *= 0.95f;

                NPC.netUpdate = true;
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, 226, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-6, 0)));
                }
            }
            if (attackCounter == 120)
            {
                SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 66);
                Projectile.NewProjectile(entitySource, NPC.position, Vector2.Zero, ModContent.ProjectileType<LightningGem>(), 30, 0f, Main.myPlayer, 0f, 0f);
                int rot = Main.rand.Next(360);
                double anglex = Math.Sin(rot * (Math.PI / 180));
                double angley = Math.Cos(rot * (Math.PI / 180));
                NPC.position.X = player.Center.X + (int)(distance * anglex);
                NPC.position.Y = player.Center.Y + (int)(distance * angley);

                colorLerp = MathHelper.Lerp(colorLerp, 0f, 0.1f);
                NPC.velocity *= 0.95f;

                NPC.netUpdate = true;
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, 226, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-6, 0)));
                }
            }
            if (attackCounter == 140)
            {
                SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 66);
                Projectile.NewProjectile(entitySource, NPC.position, Vector2.Zero, ModContent.ProjectileType<LightningGem>(), 30, 0f, Main.myPlayer, 0f, 0f);
                int rot = Main.rand.Next(360);
                double anglex = Math.Sin(rot * (Math.PI / 180));
                double angley = Math.Cos(rot * (Math.PI / 180));
                NPC.position.X = player.Center.X + (int)(distance * anglex);
                NPC.position.Y = player.Center.Y + (int)(distance * angley);

                colorLerp = MathHelper.Lerp(colorLerp, 0f, 0.1f);
                NPC.velocity *= 0.95f;

                NPC.netUpdate = true;
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, 226, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-6, 0)));
                }
            }
            if (attackCounter == 160)
            {
                SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 66);
                Projectile.NewProjectile(entitySource, NPC.position, Vector2.Zero, ModContent.ProjectileType<LightningGem>(), 30, 0f, Main.myPlayer, 0f, 0f);
                int rot = Main.rand.Next(360);
                double anglex = Math.Sin(rot * (Math.PI / 180));
                double angley = Math.Cos(rot * (Math.PI / 180));
                NPC.position.X = player.Center.X + (int)(distance * anglex);
                NPC.position.Y = player.Center.Y + (int)(distance * angley);

                colorLerp = MathHelper.Lerp(colorLerp, 0f, 0.1f);
                NPC.velocity *= 0.95f;

                NPC.netUpdate = true;
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, 226, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-6, 0)));
                }
            }
            if (attackCounter >= 165)
            {
                IdleFloat();
            }
        }

        private void LightningStorm()
        {
            var entitySource = NPC.GetSource_FromAI();
            NPC.dontTakeDamage = true;
            UpdateFrame(0.3f, 0, 5);
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = 0f + MathHelper.ToRadians(NPC.velocity.X / 2);
            attackCounter++;
            Player player = Main.player[NPC.target];

            if (attackCounter >= 240)
            {
                cooldownFrames = 50;
                NPC.dontTakeDamage = false;
                return;
            }
            else
            {
                NPC.velocity *= 0.92f;

                if (attackCounter < 120)
                {
                    if (attackCounter % 4 == 0 && attackCounter < 60)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(entitySource, NPC.Center + new Vector2(0, Main.rand.Next(80, 170)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), Vector2.Zero, ModContent.ProjectileType<LightningChargeProj>(), 15, 5);
                        (proj.ModProjectile as LightningChargeProj).parent = NPC;
                        proj.ai[0] = Main.rand.NextFloat(1, 6);
                        if (Main.rand.NextBool(2)) proj.ai[0] = -proj.ai[0];
                    }
                }
                else
                {
                    if (attackCounter == 120)
                    {
                        for (int i = 0; i <= 16; i++)
                        {
                            for (double b = 0; b < 6.28; b += Main.rand.NextFloat(1f, 2f))
                            {
                                Projectile proj = Projectile.NewProjectileDirect(entitySource, NPC.Center, new Vector2((float)Math.Sin(b), (float)Math.Cos(b)) * 2.5f, ModContent.ProjectileType<LightningJoltProj>(), 8, 5);
                                proj.ai[1] = 1;
                            }
                        }
                        if (phaseTwo)
                        {
                            for (double b = 0; b < 6.28; b += Main.rand.NextFloat(1f, 2f))
                            {
                                int lightningproj = Projectile.NewProjectile(entitySource, NPC.Center, new Vector2((float)Math.Sin(b), (float)Math.Cos(b)) * 2.5f, ModContent.ProjectileType<ElectrapulseProj>(), NPC.damage, 15);
                                if (Main.netMode != NetmodeID.Server)
                                {
                                    AerovelenceMod.primitives.CreateTrail(new CanisterPrimTrail(Main.projectile[lightningproj]));
                                }
                            }
                        }
                    }
                    else if (attackCounter % 4 == 0)
                    {
                        Projectile.NewProjectile(entitySource, player.Center + new Vector2((-Main.screenWidth / 2) + Main.rand.Next(Main.screenWidth), -Main.screenHeight * 0.6f), new Vector2((float)Math.Sin(Main.rand.NextFloat(1f, 2f)), Main.rand.NextFloat(15, 19)), ModContent.ProjectileType<LightningJoltProj>(), 6, 5);
                    }

                }
            }
        }

        bool balls = true;

        private void BlastVolleys()
        {
            var entitySource = NPC.GetSource_FromAI();
            UpdateFrame(0.3f, 0, 5);
            attackCounter++;
            Player player = Main.player[NPC.target];
            Vector2 dir = new Vector2((float)Math.Sign(NPC.Center.X - player.Center.X) * 300, -100);
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = 0f + MathHelper.ToRadians(NPC.velocity.X / 2);
            if (attackCounter >= 90)
            {
                NPC.velocity.X *= 0.95f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center + dir) * (NPC.Distance(player.Center + dir) / 20), 0.1f);
                cooldownFrames = 50;

                return;
            }

            else if (attackCounter >= 30)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center + dir) * (NPC.Distance(player.Center + dir) / 20), 0.2f);
                NPC.velocity.X *= 0.92f;
                colorLerp = MathHelper.Lerp(colorLerp, 0f, 0.1f);
                float count = attackCounter % 30;
                if (count == 0)
                {
                    aiVector = player.Center;
                }
                else
                {
                    if (count % 5 == 0)
                    {
                        float vectorRotation = -count + 15;
                        Projectile proj = Projectile.NewProjectileDirect(entitySource, NPC.Center, NPC.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(vectorRotation * 3)) * 20, ModContent.ProjectileType<LightningJoltProj>(), 10, 5);
                        //proj.ai[1] = 1;

                        SoundEngine.PlaySound(SoundID.Item75, NPC.Center);
                    }
                }

                if (phaseTwo && balls == true)
                {
                    balls = false;
                    for (double i = 0; i < 6.28; i += Main.rand.NextFloat(1f, 2f))
                    {
                        int lightningproj = Projectile.NewProjectile(entitySource, NPC.Center, new Vector2((float)Math.Sin(i), (float)Math.Cos(i)) * 2.5f, ModContent.ProjectileType<ElectrapulseProj>(), NPC.damage, 15);
                        if (Main.netMode != NetmodeID.Server)
                        {
                            AerovelenceMod.primitives.CreateTrail(new CanisterPrimTrail(Main.projectile[lightningproj]));
                        }
                    }
                }
            }
            else
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center + dir) * (NPC.Distance(player.Center + dir) / 15), 0.3f);
                colorLerp = MathHelper.Lerp(colorLerp, 1f, 0.05f);
                if (attackCounter == 2) SoundEngine.PlaySound(SoundID.Item77, NPC.Center);
                balls = true;
            }
        }
        private void CheckPlatform(Player player)
        {
            bool onplatform = true;

            for (int i = (int)NPC.position.X; i < NPC.position.X + NPC.width; i += NPC.width / 4)
            {
                Tile tile = Framing.GetTileSafely(new Point((int)NPC.position.X / 16, (int)(NPC.position.Y + NPC.height + 8) / 16));
                if (!TileID.Sets.Platforms[tile.TileType])
                {
                    onplatform = false;
                }
            }
            if (onplatform && (NPC.position.Y + NPC.height + 20 < player.Center.Y) && !NPC.noTileCollide)
            {
                NPC.position.Y += 2;
                NPC.velocity.Y = NPC.oldVelocity.Y;
            }
        }

        int t;
        int b;

        private void IdleFloat()
        {
            var entitySource = NPC.GetSource_FromAI();
            t++;
            Player player = Main.player[NPC.target];
            int damage;
            float dynamicCounter = 0;

            if (t % 200 == 0)
            {
                for (int i = 0; i < 3 + (Main.expertMode ? 1 : 0); i++)
                {
                    Vector2 toLocation = player.Center + new Vector2(Main.rand.NextFloat(100, 240), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360)));
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        damage = NPC.damage / 2;
                        int lightningproj = Projectile.NewProjectile(entitySource, toLocation, Vector2.Zero, ModContent.ProjectileType<ElectricityProjectile>(), damage, 0, Main.myPlayer, player.whoAmI);
                        AerovelenceMod.primitives.CreateTrail(new CanisterPrimTrail(Main.projectile[lightningproj]));

                    }
                }

                int proj = Projectile.NewProjectile(entitySource, NPC.Center, NPC.DirectionTo(player.Center) * 20, ModContent.ProjectileType<ElectricityProjectile>(), 10, 5);
                Main.projectile[proj].ai[1] = 1;
            }
            if (t % 400 == 0)
            {
                DiveBomb();
            }
                NPC.spriteDirection = NPC.direction;
            NPC.rotation = 0f + MathHelper.ToRadians(NPC.velocity.X / 2);
            UpdateFrame(0.3f, 0, 5);
            attackCounter++;
            if (attackCounter > 200)
            {
                cooldownFrames = 30;
                return;
            }

            Vector2 targetPosition = player.Center + new Vector2(0, -250);
            NPC.velocity += NPC.DirectionTo(targetPosition) * 0.8f;
            if (NPC.velocity.Length() > 18)
            {
                NPC.velocity.Normalize();
                NPC.velocity *= 18;
            }
        }
        Vector2 dashDirection;

        private void GroundAnimation()
        {
            UpdateFrame(MathHelper.Lerp(0f, 0.5f, MathHelper.Clamp(Math.Abs(NPC.velocity.X), 0f, 20) / 20), 8, 18);
        }

        private void JumpAndDash()
        {
            if (NPC.collideY || attackCounter >= 2)
            {
                attackCounter++;
            }
            Player player = Main.player[NPC.target];
            float dashMax = 100;
            if (attackCounter >= dashMax)
            {
                dashing = false;
                cooldownFrames = 1;
                return;
            }
            else
            {
                if (attackCounter == 2)
                {
                    NPC.noTileCollide = false;
                    NPC.velocity = NPC.DirectionTo(player.Center) * 13;
                    NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -999, -10);
                    if (NPC.velocity.Y >= -8)
                    {
                        NPC.velocity.Y -= 8;
                    }
                    NPC.velocity.Y -= 15;
                }
                else if (attackCounter < 80)
                {
                    NPC.noTileCollide = false;
                    if (attackCounter == 50)
                    {
                        aiVector = player.Center;
                    }

                    UpdateFrame(0f, 3, 3);

                    NPC.rotation += MathHelper.ToRadians(NPC.velocity.X);

                    if (attackCounter <= 60)
                    {
                        NPC.velocity.X *= 0.99f;
                        NPC.velocity.Y += 0.68f;
                    }
                    else
                    {
                        NPC.velocity *= 0.9f;
                    }
                }
                else
                {
                    NPC.noTileCollide = false;
                    if (attackCounter == 80)
                    {
                        SoundEngine.PlaySound(SoundID.Item67, NPC.Center);
                        NPC.velocity = NPC.DirectionTo(aiVector) * 30;
                    }
                    else
                    {
                        NPC.velocity *= 0.96f;
                    }
                    UpdateFrame(0.3f, 7, 7);
                    dashing = true;
                    NPC.rotation = NPC.AngleTo(NPC.Center + NPC.velocity) - MathHelper.ToRadians(90f);

                    if (NPC.collideY || NPC.velocity.Y == 0)
                    {
                        NPC.velocity.Y = -NPC.oldVelocity.Y;
                    }
                }
            }
        }

        private void IdleRun()
        {
            NPC.rotation = 0f;
            GroundAnimation();
            Player player = Main.player[NPC.target];
            NPC.spriteDirection = NPC.direction;

            attackCounter++;
            if (attackCounter > 200)
            {
                cooldownFrames = 1;
                return;
            }

            NPC.noTileCollide = false;
            if (NPC.velocity.Y < 0) NPC.noTileCollide = true;

            if (attackCounter < 150)
            {
                if (Math.Abs(NPC.Center.X - player.Center.X) > 500 || !NPC.collideY)
                    NPC.velocity.X += NPC.DirectionTo(player.Center).X * 2;
            }
            else
            {
                if (Math.Abs(NPC.velocity.X) > 0.1f)
                    NPC.velocity.X *= 0.9f;
            }

            if (NPC.velocity.X > 6 || NPC.velocity.X < -6)
            {
                NPC.velocity.X = Math.Sign(NPC.velocity.X) * 6;
            }
            if (!NPC.collideY)
            {
                NPC.velocity.Y += 1f;
            }

            if (NPC.velocity.X == 0 || NPC.collideX || !Collision.CanHit(NPC.position, NPC.width, NPC.height, NPC.position + new Vector2(NPC.velocity.X, 0), NPC.width, NPC.height))
            {
                NPC.velocity.Y = -16;
            }
        }

        private void Dash()
        {
            Player player = Main.player[NPC.target];


            float dashMax = 120;

            attackCounter++;
            if (attackCounter >= dashMax)
            {
                dashing = false;
                cooldownFrames = 1;
                return;
            }
            else
            {
                Vector2 dir = new Vector2((float)Math.Sign(NPC.Center.X - player.Center.X) * 300, 0);

                float count = attackCounter % dashMax;
                if (count == Math.Floor(dashMax * 0.55f))
                {
                    aiVector = player.Center;
                }
                if (count <= Math.Floor(dashMax * 0.55f))
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center + dir) * (NPC.Distance(player.Center + dir) / 20), 0.2f);
                }
                else if (count < Math.Floor(dashMax * 0.75f))
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.4f);

                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 projectilePosition = NPC.DirectionTo(aiVector);
                        projectilePosition -= NPC.velocity * ((float)i * 0.25f);
                        int dust = Dust.NewDust(projectilePosition, 1, 1, 20, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].position = projectilePosition;
                        Main.dust[dust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                        Main.dust[dust].velocity *= 0.2f;
                    }
                }
                if (count == Math.Floor(dashMax * 0.75f))
                {
                    var entitySource = NPC.GetSource_FromAI();
                    Projectile.NewProjectile(entitySource, NPC.position, Vector2.Zero, ModContent.ProjectileType<LightningGem>(), 30, 0f, Main.myPlayer, 0f, 0f);
                    SoundEngine.PlaySound(SoundID.Item67, NPC.Center);
                    NPC.velocity = NPC.DirectionTo(aiVector) * 30;

                    Vector2 startPos = NPC.Center;
                    Vector2 endPos = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center + dir) * (NPC.Distance(player.Center + dir) / 20), 0.2f);

                    for (int i = 0; i < 100; i++)
                    {
                        //int dust2 = Dust.NewDust(startPos, 1, 1, 20, 0f, 0f, 0, default(Color), 1f);
                        Main.NewText("Balls");
                        Vector2 pos = startPos + ((startPos - endPos) * i / 100f);
                        int dust = Dust.NewDust(pos, 1, 1, 6, 0f, 0f, 0, default(Color), 1f);

                        for (int rr = 0; rr < 100; rr++)
                        {
                            int dust4 = Dust.NewDust(endPos, 1, 1, 6, 0f, 0f, 0, default(Color), 1f);
                        }
                        for (int rr4 = 0; rr4 < 100; rr4++)
                        {
                            int dust5 = Dust.NewDust(startPos, 1, 1, 6, 0f, 0f, 0, default(Color), 1f);
                        }
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].position = pos;
                        Main.dust[dust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                        Main.dust[dust].velocity *= 0.2f;
                        //Vector2 pos = (startPos - (startPos + endPos / 2)) * 2;
                    }



                    Projectile.NewProjectile(entitySource, NPC.position, Vector2.Zero, ModContent.ProjectileType<TelegraphLightning>(), NPC.damage / 2, 0f, 1);
                }
                if (count >= Math.Floor(dashMax * 0.75f) && count <= Math.Floor(dashMax * 0.75f) + 10)
                {
                    NPC.position += NPC.velocity;
                }
                if (count >= Math.Floor(dashMax * 0.8f))
                {
                    NPC.velocity *= 0.92f;
                }
                if (count >= Math.Floor(dashMax * 0.9f))
                {
                    NPC.velocity *= 0.94f;
                }

                if (count >= Math.Floor(dashMax * 0.75f))
                {
                    dashing = true;
                }
                else dashing = false;

                if (!dashing)
                {
                    UpdateFrame(0.3f, 0, 5);
                    NPC.rotation = 0f + MathHelper.ToRadians(NPC.velocity.X / 2);
                    NPC.spriteDirection = NPC.direction;
                }
                else
                {
                    UpdateFrame(0.3f, 5, 5);
                    NPC.rotation = NPC.AngleTo(NPC.Center + NPC.velocity) - MathHelper.ToRadians(90f);
                }
            }
        }

        Vector2 posToBe = Vector2.Zero;
        bool diveBombed = false;
        private void DiveBomb()
        {
            NPC.rotation = 0f;
            Player player = Main.player[NPC.target];
            if (attackCounter == 0)
            {
                diveBombed = false;
                attackCounter = 1;
            }
            else
            {
                if (!diveBombed)
                {
                    NPC.spriteDirection = NPC.direction;
                    dashing = false;
                    UpdateFrame(0.3f, 0, 5);
                    if (attackCounter == 1)
                    {
                        posToBe = player.Center - new Vector2(0, 500);
                    }
                    Vector2 direction = posToBe - NPC.Center;
                    float lerpSpeed = (float)Math.Sqrt(direction.Length());
                    direction.Normalize();
                    direction *= lerpSpeed;
                    NPC.velocity = direction;
                    if (lerpSpeed < 10)
                    {
                        attackCounter++;
                        if (attackCounter > 20)
                        {
                            diveBombed = true;
                            NPC.velocity.Y += 5;
                        }
                    }
                }
                else
                {
                    UpdateFrame(0.3f, 6, 6);
                    attackCounter++;
                    NPC.noTileCollide = false;
                    if (NPC.collideY || !Collision.CanHit(NPC.Center, 2, 2, NPC.Bottom + new Vector2(0, 20), 2, 2) || NPC.velocity.Y == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item67, NPC.Center);
                        for (int i = 0; i <= 70; i++)
                        {
                            Dust dust;
                            // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                            Vector2 position = Main.LocalPlayer.Center;
                            dust = Main.dust[Dust.NewDust(NPC.Bottom - new Vector2(130, 10), 260, 10, DustID.Electric, 0f, -4.883721f, 0, new Color(255, 255, 255), 0.9883721f)];
                        }
                        //grounded = true;
                        diveBombed = false;
                        NPC.velocity = new Vector2(NPC.velocity.X, -20);
                        cooldownFrames = 30;
                        dashing = false;
                        return;
                    }
                    else
                        dashing = true;
                    NPC.velocity.Y += 5;
                    NPC.velocity.X *= 0.7f;
                }
            }
        }
        #endregion
    }


    public class LightningJoltProj : ModProjectile
    {
        float start = 0;
        float timer = 0;

        ref float AttackMode => ref Projectile.ai[1];
        ref float AttackTimer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Jolt");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 8;
            Projectile.damage = 1;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 3;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 200;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.CornflowerBlue.ToVector3() / 3);
            start = MathHelper.Lerp(start, 15, 0.14f);
            Projectile.rotation = Projectile.AngleTo(Projectile.Center + Projectile.velocity);
            if (AttackMode == 1)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians((float)Math.Cos(AttackTimer / 3)) * 3);
            }
            else
            {
                if (AttackTimer <= 25) Projectile.velocity *= 0.95f;
                else Projectile.velocity *= 1.05f;
            }
            AttackTimer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1] + (Projectile.oldPos[i] - Projectile.oldPos[i - 1]).SafeNormalize(Vector2.Zero) * MathHelper.Min(Vector2.Distance(Projectile.oldPos[i - 1], Projectile.oldPos[i]), start);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            float vel = MathHelper.Clamp(Projectile.velocity.Length() * 0.02f, 0f, 0.2f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw((Texture2D)TextureAssets.Projectile[Projectile.type], Projectile.oldPos[i] - Main.screenPosition, new Rectangle(0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()), Color.Lerp(Color.White, Color.Black, i / (float)Projectile.oldPos.Length), Projectile.rotation, new Vector2(TextureAssets.Projectile[Projectile.type].Width() / 2, TextureAssets.Projectile[Projectile.type].Height() / 2), new Vector2(Projectile.scale + vel, Projectile.scale - vel) * (1 - ((float)i / Projectile.oldPos.Length)), SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
    public class LightningJoltProjLarge : ModProjectile
    {
        float start = 0;
        float timer = 0;
        public Vector2 aiVector = Vector2.Zero;

        ref float AttackMode => ref Projectile.ai[1];
        ref float AttackTimer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Jolt");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 8;
            Projectile.damage = 1;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 3;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 200;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.CornflowerBlue.ToVector3() / 3);
            start = MathHelper.Lerp(start, 30, 0.14f);
            Projectile.rotation = Projectile.AngleTo(Projectile.Center + Projectile.velocity);

            if (AttackMode == 1)
            {
                Projectile.velocity *= 1.06f;
            }
            else
            {
                if (AttackTimer <= 30)
                {
                    Projectile.velocity += Projectile.DirectionTo(aiVector) * 1.5f;
                    if (Projectile.velocity.Length() <= 10)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 10;
                    }
                }
            }
            AttackTimer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1] + (Projectile.oldPos[i] - Projectile.oldPos[i - 1]).SafeNormalize(Vector2.Zero) * MathHelper.Min(Vector2.Distance(Projectile.oldPos[i - 1], Projectile.oldPos[i]), start);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            float vel = MathHelper.Clamp(Projectile.velocity.Length() * 0.02f, 0f, 0.2f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw((Texture2D)TextureAssets.Projectile[Projectile.type], Projectile.oldPos[i] - Main.screenPosition, new Rectangle(0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()), Color.Lerp(Color.White, Color.Black, i / (float)Projectile.oldPos.Length), Projectile.rotation, new Vector2(TextureAssets.Projectile[Projectile.type].Width() / 2, TextureAssets.Projectile[Projectile.type].Height() / 2), new Vector2(Projectile.scale + vel, Projectile.scale - vel) * (1 - ((float)i / Projectile.oldPos.Length)), SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class LightningCrystal : ModNPC
    {
        public Vector2 aiVector = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.rotation = MathHelper.ToRadians(365);
            NPC.lifeMax = 5;
            NPC.damage = 32;
            NPC.defense = 99999;
            NPC.alpha = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 30;
            NPC.height = 48;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit34;
            NPC.DeathSound = SoundID.NPCHit56;
            NPC.buffImmune[24] = true;
            //bossBag = ModContent.ItemType<SnowriumBag>();
            //music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Snowrium");
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            var entitySource = NPC.GetSource_FromAI();
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            NPC.ai[0]++;

            if (!player.dead)
            {
                if (NPC.ai[0] % 60 == 0 && NPC.Distance(player.Center) < 600)
                {
                    SoundEngine.PlaySound(SoundID.Item75, NPC.Center);
                    int proj = Projectile.NewProjectile(entitySource, NPC.Center, NPC.DirectionTo(player.Center) * 20, ModContent.ProjectileType<LightningJoltProjLarge>(), 10, 5);
                    Main.projectile[proj].ai[1] = 1;
                }
            }
            else
            {
                NPC.velocity.Y += 1f;
                if (NPC.Distance(player.Center) > 1920) NPC.active = false;
            }

            NPC.position = Vector2.Lerp(NPC.position, aiVector, 0.1f);
            NPC.rotation = MathHelper.Lerp(NPC.rotation, (float)Math.Cos(NPC.ai[0] / 20) * MathHelper.ToRadians(10), 0.07f);
        }
    }

    public class LightningChargeProj : ModProjectile
    {
        float start = 0;
        float timer = 0;
        public NPC parent;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Jolt");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 8;
            Projectile.damage = 1;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 70;
            Projectile.scale = 0f;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.CornflowerBlue.ToVector3() / 3);
            start = MathHelper.Lerp(start, 30, 0.14f);
            Projectile.rotation = Projectile.AngleTo(Projectile.Center + Projectile.velocity);

            if (Projectile.timeLeft >= 35)
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.1f);
            }
            else
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, -0.101f);
            }

            if (parent.active)
            {
                Projectile.velocity = parent.velocity + (Projectile.DirectionTo(parent.Center).RotatedBy(MathHelper.ToRadians(90)) * Projectile.ai[0]);
            }
            else Projectile.active = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1] + (Projectile.oldPos[i] - Projectile.oldPos[i - 1]).SafeNormalize(Vector2.Zero) * MathHelper.Min(Vector2.Distance(Projectile.oldPos[i - 1], Projectile.oldPos[i]), start);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            float vel = MathHelper.Clamp(Projectile.velocity.Length() * 0.02f, 0f, 0.2f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw((Texture2D)TextureAssets.Projectile[Projectile.type], Projectile.oldPos[i] - Main.screenPosition, new Rectangle(0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()), Color.Lerp(Color.White, Color.Black, i / (float)Projectile.oldPos.Length), Projectile.rotation, new Vector2(TextureAssets.Projectile[Projectile.type].Width() / 2, TextureAssets.Projectile[Projectile.type].Height() / 2), new Vector2(Projectile.scale + vel, Projectile.scale - vel) * (1 - ((float)i / Projectile.oldPos.Length)), SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw((Texture2D)TextureAssets.Projectile[Projectile.type], Projectile.oldPos[i] - Main.screenPosition, new Rectangle(0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()), Color.Black, Projectile.rotation, new Vector2(TextureAssets.Projectile[Projectile.type].Width() / 2, TextureAssets.Projectile[Projectile.type].Height() / 2), new Vector2(Projectile.scale + vel, Projectile.scale - vel) * MathHelper.Clamp((0.6f - ((float)i / Projectile.oldPos.Length)), 0, 1), SpriteEffects.None, 0);
            }

            return false;
        }
    }
    public class ElectrapulseProj : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Lightning");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.damage = 0;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 5;
        }

        Vector2 initialVelocity = Vector2.Zero;

        private float lerp;
        public Vector2 DrawPos;
        public int boost;
        public override void AI()
        {
            if (initialVelocity == Vector2.Zero)
            {
                initialVelocity = Projectile.velocity;
            }
            if (Projectile.timeLeft % 10 == 0)
            {
                Projectile.velocity = initialVelocity.RotatedBy(Main.rand.NextFloat(-1, 1));
            }
            /* if (projectile.timeLeft % 2 == 0)
             {
                 Dust dust = Dust.NewDustPerfect(projectile.Center, 226);
                 dust.noGravity = true;
                 dust.scale = (float)Math.Sqrt(projectile.timeLeft) / 4;
                 dust.velocity = Vector2.Zero;
             }*/
            DrawPos = Projectile.position;
        }
    }

    public class LightningAnchor : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Storm");
        }

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.damage = 0;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 5;
        }

        Vector2 initialVelocity = Vector2.Zero;

        private float lerp;
        public Vector2 DrawPos;
        public int boost;


        public override void AI()
        {

            Lighting.AddLight(Projectile.Center, Color.CornflowerBlue.ToVector3() / 3);
            for (int k = 0; k < Main.player.Length; k++)
            {
                Projectile.position = Main.player[k].Center;
            }
            /*if (initialVelocity == Vector2.Zero)
            {
                initialVelocity = projectile.velocity;
            }
            if (projectile.timeLeft % 10 == 0)
            {
                projectile.velocity = initialVelocity.RotatedBy(Main.rand.NextFloat(-1, 1));
            }
            /* if (projectile.timeLeft % 2 == 0)
             {
                 Dust dust = Dust.NewDustPerfect(projectile.Center, 226);
                 dust.noGravity = true;
                 dust.scale = (float)Math.Sqrt(projectile.timeLeft) / 4;
                 dust.velocity = Vector2.Zero;
             }*/
            DrawPos = Projectile.position;
        }
    }


    public class LightningGem : ModProjectile
    {

        float colorLerp = 0f;

        float phase = 0;
        float phase3Glow = 0f;
        float cos1 = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Gem");
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.damage = 20;
            Projectile.timeLeft = 500;
            Projectile.extraUpdates = 5;
            Projectile.alpha = 0;
        }

        Vector2 initialVelocity = Vector2.Zero;

        private float lerp;
        public Vector2 DrawPos;
        public int boost;

        public override bool PreDraw(ref Color lightColor)
        {
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D auraTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/LightningMoth/LightningGemAura");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //if (dashing)

            for (int i = 0; i <= 4; i++)
            {
                Main.spriteBatch.Draw(auraTex, Projectile.Center + new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle(0, 0, auraTex.Width, auraTex.Height), Color.Lerp(new Color(15, 15, 25), Color.CornflowerBlue, colorLerp), Projectile.rotation, auraTex.Size() / 2, ((1f + ((float)Math.Cos(cos1 / 12) * 0.1f)) * phase3Glow) + MathHelper.Lerp(colorLerp, colorLerp / 2, phase3Glow), effects, 0);
            }



            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            return true;
        }

        public override void Kill(int timeLeft)
        {
            for (double i = 0; i < 6.28; i += Main.rand.NextFloat(1f, 2f))
            {
                int lightningproj = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2((float)Math.Sin(i), (float)Math.Cos(i)) * 4.5f, ModContent.ProjectileType<ElectrapulseProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                if (Main.netMode != NetmodeID.Server)
                {
                    AerovelenceMod.primitives.CreateTrail(new CanisterPrimTrail(Main.projectile[lightningproj]));
                }
            }
        }

        public override void AI()
        {
            phase3Glow = MathHelper.Lerp(phase3Glow, 1f, 0.1f);
            Lighting.AddLight(Projectile.Center, Color.CornflowerBlue.ToVector3() / 3);
            /*if (initialVelocity == Vector2.Zero)
            {
                initialVelocity = projectile.velocity;
            }
            if (projectile.timeLeft % 10 == 0)
            {
                projectile.velocity = initialVelocity.RotatedBy(Main.rand.NextFloat(-1, 1));
            }
            /* if (projectile.timeLeft % 2 == 0)
             {
                 Dust dust = Dust.NewDustPerfect(projectile.Center, 226);
                 dust.noGravity = true;
                 dust.scale = (float)Math.Sqrt(projectile.timeLeft) / 4;
                 dust.velocity = Vector2.Zero;
             }*/
            DrawPos = Projectile.position;
        }
    }

    public class ElectricityProjectile : ModProjectile
    {

        float colorLerp = 0f;

        float phase = 0;
        float phase3Glow = 0f;
        float cos1 = 0;


        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 44;
            Projectile.timeLeft = 560;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.damage = 56;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 0;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        /*public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft <= 420)
                projectile.alpha = 255;
            if (projectile.timeLeft == 420)
                projectile.alpha = 0;

                var effects = projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D auraTex = ModContent.GetTexture("AerovelenceMod/Content/NPCs/Bosses/LightningMoth/TinyAura");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //if (dashing)

            for (int i = 0; i <= 4; i++)
            {
                spriteBatch.Draw(auraTex, projectile.Center + new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)) - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Rectangle(0, 0, auraTex.Width, auraTex.Height), Color.Lerp(new Color(15, 15, 25) , Color.CornflowerBlue, colorLerp) * (1f - projectile.alpha / 255f), projectile.rotation, auraTex.Size() / 2, ((1f + ((float)Math.Cos(cos1 / 12) * 0.1f)) * phase3Glow) + MathHelper.Lerp(colorLerp, colorLerp / 2, phase3Glow), effects, 0);
            }



            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            return true;
        }*/

        public override bool ShouldUpdatePosition()
        {
            return Projectile.timeLeft <= 420;
        }
        public override void AI()
        {
            Projectile.rotation = MathHelper.ToRadians(180) + Projectile.velocity.ToRotation();
            if (Projectile.timeLeft == 420)
            {
                Projectile.alpha = 255;
                SoundEngine.PlaySound(SoundID.Item, (int)Projectile.Center.X, (int)Projectile.Center.Y, 43, 0.75f);

                for (double i = 0; i < 6.28; i += 0.1)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 226, new Vector2((float)Math.Sin(i) * 1.3f, (float)Math.Cos(i)) * 2.4f);
                    dust.noGravity = true;
                }
            }
            if (Projectile.timeLeft > 420)
            {
                /*for (double i = 0; i < 6.28; i += Main.rand.NextFloat(1f, 2f))
                {
                    
                }*/


                /*if (player.active)
                {
                    Vector2 toPlayer = projectile.Center - player.Center;
                    toPlayer = toPlayer.SafeNormalize(Vector2.Zero) * 12;
                    projectile.velocity = -toPlayer;*/
            }

            else
            {

                Player player = Main.player[(int)Projectile.ai[0]];
                float approaching = ((540f - Projectile.timeLeft) / 540f);
                Lighting.AddLight(Projectile.Center, 0.5f, 0.65f, 0.75f);

                float x = Main.rand.Next(-10, 11) * 0.001f * approaching;
                float y = Main.rand.Next(-10, 11) * 0.001f * approaching;
                Vector2 toPlayer = Projectile.Center - player.Center;
                toPlayer = toPlayer.SafeNormalize(Vector2.Zero);
                Projectile.velocity += -toPlayer * (0.155f * Projectile.timeLeft / 540f) + new Vector2(x, y);

                

                Projectile.hostile = false;
                /*int dust = Dust.NewDust(projectile.Center + new Vector2(-4, -4), 0, 0, 164, 0, 0, projectile.alpha, default, 1.25f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].scale *= 0.75f;*/
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 1.8f / 255f, (255 - Projectile.alpha) * 0.0f / 255f, (255 - Projectile.alpha) * 0.0f / 255f);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 3;
        }
    }
    public class TelegraphLightning : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning");
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }
        public override string Texture { get { return "Terraria/Projectile_" + ProjectileID.ShadowBeamFriendly; } }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8);
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 3f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 projectilePosition = Projectile.position;
                    projectilePosition -= Projectile.velocity * ((float)i * 0.25f);
                    Projectile.alpha = 255;
                    int dust = Dust.NewDust(projectilePosition, 1, 1, DustID.Electric, 0f, 0f, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].position = projectilePosition;
                    Main.dust[dust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[dust].velocity *= 0.2f;
                }
            }
        }
    }
}