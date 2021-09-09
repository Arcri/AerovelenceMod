using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            Main.npcFrameCount[npc.type] = 7;    //boss frame/animation 
            NPCID.Sets.TrailCacheLength[npc.type] = 10;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 12500;   //boss life
            npc.damage = 32;  //boss damage
            npc.defense = 9;    //boss defense
            npc.alpha = 0;
            npc.knockBackResist = 0f;
            npc.width = 222;
            npc.height = 174;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(0, 5, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit44;
            npc.DeathSound = SoundID.NPCHit46;
            npc.buffImmune[24] = true;
            //bossBag = ModContent.ItemType<SnowriumBag>();
            //music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Snowrium");
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var effects = npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D glowTex = ModContent.GetTexture("AerovelenceMod/Content/NPCs/Bosses/LightningMoth/LightningMoth_Glow");
            Texture2D auraTex = ModContent.GetTexture("AerovelenceMod/Content/NPCs/Bosses/LightningMoth/LightningMoth_Aura");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //if (dashing)
            {
                for (int j = 0; j < dashTrailLength; j++)
                {
                    spriteBatch.Draw(Main.npcTexture[npc.type], npc.oldPos[j] + new Vector2(npc.width / 2, npc.height / 2) - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, Color.CornflowerBlue, npc.rotation, npc.frame.Size() / 2, 1f, effects, 0);
                }
                for (int i = 0; i <= 4; i++)
                {
                    spriteBatch.Draw(Main.npcTexture[npc.type], npc.Center + (new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)) * phase3Glow) + new Vector2(0, -5 * phase3Glow).RotatedBy(MathHelper.ToRadians(360 / 4 * i)) - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, Color.CornflowerBlue, npc.rotation, npc.frame.Size() / 2, npc.scale, effects, 0);
                }
            }

            for (int i = 0; i <= 4; i++)
            {
                spriteBatch.Draw(auraTex, npc.Center + new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)) - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Rectangle(0, 0, auraTex.Width, auraTex.Height), Color.Lerp(new Color(15, 15, 25), Color.CornflowerBlue, colorLerp), npc.rotation, auraTex.Size() / 2, ((1f + ((float)Math.Cos(cos1 / 12) * 0.1f)) * phase3Glow) + MathHelper.Lerp(colorLerp, colorLerp / 2, phase3Glow), effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(Main.npcTexture[npc.type], new Vector2(npc.Center.X, npc.Center.Y) - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, lightColor, npc.rotation, npc.frame.Size() / 2, npc.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(glowTex, new Vector2(npc.Center.X, npc.Center.Y) - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2, npc.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix); 
            
            return false;
		}
        private int cooldownFrames
        {
            get
            {
                return (int)npc.ai[0];
            }
            set
            {
                npc.ai[0] = value;
            }
        }
        private CurrentAttack currentAttack
        {
            get
            {
                return (CurrentAttack)(int)npc.ai[1];
            }
            set
            {
                npc.ai[1] = (int)value;
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
            IdleRun = 11,
            JumpAndDash = 12
        }
        public bool phaseTwo => npc.life < npc.lifeMax / 2;
        public override void AI()
        {

            if (npc.life > npc.lifeMax * 0.8f)
            {
                if (phase == 0)
                {
                    cooldownFrames = 60;
                }
                phase = 1;
            }
            else if (npc.life > npc.lifeMax * 0.4f)
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

            Lighting.AddLight(npc.Center, Color.CornflowerBlue.ToVector3());

            npc.TargetClosest();
            CheckPlatform(Main.player[npc.target]);

            dashTrailLength = MathHelper.Lerp(dashTrailLength, dashing || phase == 3 ? 10 : 3, 0.5f);

            if (cooldownFrames <= 0)
            {
                Player player = Main.player[npc.target];
                if (player.dead)
                {
                    npc.velocity.Y -= 1;
                    if (npc.Distance(player.Center) > 1920) npc.active = false;
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
                            npc.noTileCollide = true;
                        }
                    }
                    if (phase == 2)
                    {
                        int att = Main.rand.Next(5);
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
                            default:
                                attack = CurrentAttack.IdleFloat;
                                break;

                        }
                    }
                    if (phase == 3)
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
                                attack = CurrentAttack.LightningStorm;
                                break;
                            case 5:
                                attack = CurrentAttack.Shotgun;
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
			npc.frame.Width = 268;
			npc.frame.X = ((int)trueFrame % 4) * npc.frame.Width;
			npc.frame.Y = (((int)trueFrame - ((int)trueFrame % 4)) / 4) * npc.frame.Height;
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
            attackCounter++;
            attackCounter++;
            Player player = Main.player[npc.target];
            UpdateFrame(0.3f, 0, 6);
            npc.spriteDirection = npc.direction;
            npc.rotation = 0f + MathHelper.ToRadians(npc.velocity.X / 2);

            if (attackCounter >= 180)
            {
                cooldownFrames = 10;
                return;
            }
            else if (attackCounter >= 130)
            {
                npc.velocity *= 0.92f;
                if (attackCounter % 20 == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(npc.Center, npc.DirectionTo(player.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.ToRadians(10), MathHelper.ToRadians(10))) * 10, ModContent.ProjectileType<LightningJoltProjLarge>(), 15, 5, ai1: 1);
                        (proj.modProjectile as LightningJoltProjLarge).aiVector = player.Center + new Vector2(Main.rand.Next(-50, 50), Main.rand.Next(-50, 50));
                    }

                    Main.PlaySound(SoundID.Item67, npc.Center);
                }
            }
            else if (attackCounter > 60)
            {
                colorLerp = MathHelper.Lerp(colorLerp, 0f, 0.1f);
                npc.velocity *= 0.93f;
            }
            else
            {
                colorLerp = MathHelper.Lerp(colorLerp, 1f, 0.05f);
                Vector2 dir = new Vector2((float)Math.Sign(npc.Center.X - player.Center.X) * 300, -100);
                npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionTo(player.Center + dir) * (npc.Distance(player.Center + dir) / 15), 0.3f);
            }
        }

        private void CrystalStomp()
        {
            attackCounter++;
            // frame 19-26. on frame 24 spawn crystals

            if (attackCounter >= 70)
            {
                cooldownFrames = 10;
                npc.noTileCollide = true;
                return;
            }
            else
            {
                npc.velocity.X *= 0.92f;
                npc.noTileCollide = false;
                if (attackCounter <= 35)
                {
                    UpdateFrame(0f, 18, 18);
                }
                else
                {
                    UpdateFrame(0.2f, 19, 25);
                    if (attackCounter == 35 + 25)
                    {
                        Main.PlaySound(SoundID.Item69, npc.Center);
                        for (int i = 1; i <= 3; i++)
                        {
                            int np = NPC.NewNPC((int)(npc.Center.X + npc.direction * (160 * i)), (int)npc.Bottom.Y, ModContent.NPCType<LightningCrystal>());
                            (Main.npc[np].modNPC as LightningCrystal).aiVector = Main.npc[np].Center + new Vector2(0, -80 - (i * 30));
                        }
                    }
                }
            }
        }

        private void LightningStorm()
        {
            npc.dontTakeDamage = true;
            UpdateFrame(0.3f, 0, 6);
            npc.spriteDirection = npc.direction;
            npc.rotation = 0f + MathHelper.ToRadians(npc.velocity.X / 2);
            attackCounter++;
            Player player = Main.player[npc.target];

            if (attackCounter >= 240)
            {
                cooldownFrames = 50;
                npc.dontTakeDamage = false;
                return;
            }
            else
            {
                npc.velocity *= 0.92f;
                
                if (attackCounter < 120)
                {
                    if (attackCounter % 4 == 0 && attackCounter < 60)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(npc.Center + new Vector2(0, Main.rand.Next(80, 170)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360))), Vector2.Zero, ModContent.ProjectileType<LightningChargeProj>(), 15, 5);
                        (proj.modProjectile as LightningChargeProj).parent = npc;
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
                            Projectile proj = Projectile.NewProjectileDirect(npc.Center, npc.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(360 / 16 * i)) * 10, ModContent.ProjectileType<LightningJoltProj>(), 8, 5);
                            proj.ai[1] = 1;
                        }
                    } 
                    else if (attackCounter % 4 == 0)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(player.Center + new Vector2((-Main.screenWidth / 2) + Main.rand.Next(Main.screenWidth), -Main.screenHeight * 0.6f), new Vector2(Main.rand.NextFloat(-1, 2), Main.rand.NextFloat(15, 19)), ModContent.ProjectileType<LightningJoltProj>(), 6, 5);
                        //proj.ai[1] = 1;
                    }
                }
            }
        }

        private void BlastVolleys()
        {
            UpdateFrame(0.3f, 0, 6);
            attackCounter++;
            Player player = Main.player[npc.target];
            Vector2 dir = new Vector2((float)Math.Sign(npc.Center.X - player.Center.X) * 300, -100);
            npc.spriteDirection = npc.direction;
            npc.rotation = 0f + MathHelper.ToRadians(npc.velocity.X / 2);
            if (attackCounter >= 90)
            {
                npc.velocity.X *= 0.95f;
                npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionTo(player.Center + dir) * (npc.Distance(player.Center + dir) / 20), 0.1f);
                cooldownFrames = 50;

                return;
            }
            else if (attackCounter >= 30)
            {
                npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionTo(player.Center + dir) * (npc.Distance(player.Center + dir) / 20), 0.2f);
                npc.velocity.X *= 0.92f;
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
                        Projectile proj = Projectile.NewProjectileDirect(npc.Center, npc.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(vectorRotation * 3)) * 20, ModContent.ProjectileType<LightningJoltProj>(), 10, 5);
                        //proj.ai[1] = 1;

                        Main.PlaySound(SoundID.Item75, npc.Center);
                    }
                }
            }
            else
            {
                npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionTo(player.Center + dir) * (npc.Distance(player.Center + dir) / 15), 0.3f);
                colorLerp = MathHelper.Lerp(colorLerp, 1f, 0.05f);
                if (attackCounter == 2) Main.PlaySound(SoundID.Item77, npc.Center);
            }
        }
        private void CheckPlatform(Player player)
        {
            bool onplatform = true;

            for (int i = (int)npc.position.X; i < npc.position.X + npc.width; i += npc.width / 4)
            {
                Tile tile = Framing.GetTileSafely(new Point((int)npc.position.X / 16, (int)(npc.position.Y + npc.height + 8) / 16));
                if (!TileID.Sets.Platforms[tile.type])
                {
                    onplatform = false;
                }
            }
            if (onplatform && (npc.position.Y + npc.height + 20 < player.Center.Y) && !npc.noTileCollide)
            {
                npc.position.Y += 2;
                npc.velocity.Y = npc.oldVelocity.Y;
            }
        }


        private void IdleFloat()
        {
            npc.spriteDirection = npc.direction;
            npc.rotation = 0f + MathHelper.ToRadians(npc.velocity.X / 2);
            UpdateFrame(0.3f, 0, 6);
            Player player = Main.player[npc.target];
            attackCounter++;
            if (attackCounter > 200)
            {
                cooldownFrames = 30;
                return;
            }

            Vector2 targetPosition = player.Center + new Vector2(0, -250);
            npc.velocity += npc.DirectionTo(targetPosition) * 0.8f;
            if (npc.velocity.Length() > 18)
            {
                npc.velocity.Normalize();
                npc.velocity *= 18;
            }
        }
        Vector2 dashDirection;

        private void GroundAnimation()
        {
            UpdateFrame(MathHelper.Lerp(0f, 0.5f, MathHelper.Clamp(Math.Abs(npc.velocity.X), 0f, 20) / 20), 8, 18);
        }

        private void JumpAndDash()
        {
            if (npc.collideY || attackCounter >= 2)
            {
                attackCounter++;
            }
            Player player = Main.player[npc.target];
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
                    npc.noTileCollide = false;
                    npc.velocity = npc.DirectionTo(player.Center) * 13;
                    npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -999, -10);
                    if (npc.velocity.Y >= -8)
                    {
                        npc.velocity.Y -= 8;
                    }
                    npc.velocity.Y -= 15;
                }
                else if (attackCounter < 80)
                {
                    npc.noTileCollide = false;
                    if (attackCounter == 50)
                    {
                        aiVector = player.Center;
                    }

                    UpdateFrame(0f, 3, 3);

                    npc.rotation += MathHelper.ToRadians(npc.velocity.X);

                    if (attackCounter <= 60)
                    {
                        npc.velocity.X *= 0.99f;
                        npc.velocity.Y += 0.68f;
                    }
                    else
                    {
                        npc.velocity *= 0.9f;
                    }
                }
                else
                {
                    npc.noTileCollide = false;
                    if (attackCounter == 80)
                    {
                        Main.PlaySound(SoundID.Item67, npc.Center);
                        npc.velocity = npc.DirectionTo(aiVector) * 30;
                    }
                    else
                    {
                        npc.velocity *= 0.96f;
                    }
                    UpdateFrame(0.3f, 7, 7);
                    dashing = true;
                    npc.rotation = npc.AngleTo(npc.Center + npc.velocity) - MathHelper.ToRadians(90f);

                    if (npc.collideY || npc.velocity.Y == 0)
                    {
                        npc.velocity.Y = -npc.oldVelocity.Y;
                    }
                }
            }
        }

        private void IdleRun()
        {
            npc.rotation = 0f;
            GroundAnimation();
            Player player = Main.player[npc.target];
            npc.spriteDirection = npc.direction;

            attackCounter++;
            if (attackCounter > 200)
            {
                cooldownFrames = 1;
                return;
            }

            npc.noTileCollide = false;
            if (npc.velocity.Y < 0) npc.noTileCollide = true;

            if (attackCounter < 150)
            {
                if (Math.Abs(npc.Center.X - player.Center.X) > 500 || !npc.collideY)
                    npc.velocity.X += npc.DirectionTo(player.Center).X * 2;
            }
            else
            {
                if (Math.Abs(npc.velocity.X) > 0.1f)
                npc.velocity.X *= 0.9f;
            }

            if (npc.velocity.X > 6 || npc.velocity.X < -6)
            {
                npc.velocity.X = Math.Sign(npc.velocity.X) * 6;
            }
            if (!npc.collideY)
            {
                npc.velocity.Y += 1f;
            }
            
            if (npc.velocity.X == 0 || npc.collideX || !Collision.CanHit(npc.position, npc.width, npc.height, npc.position + new Vector2(npc.velocity.X, 0), npc.width, npc.height))
            {
                npc.velocity.Y = -16;
            }
        }

        private void Dash()
        {
            Player player = Main.player[npc.target];

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
                Vector2 dir = new Vector2((float)Math.Sign(npc.Center.X - player.Center.X) * 300, 0);

                float count = attackCounter % dashMax;
                if (count == Math.Floor(dashMax * 0.55f))
                {
                    aiVector = player.Center;
                }
                if (count <= Math.Floor(dashMax * 0.55f))
                {
                    npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionTo(player.Center + dir) * (npc.Distance(player.Center + dir) / 20), 0.2f);
                }
                else if (count < Math.Floor(dashMax * 0.75f))
                {
                    npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Zero, 0.4f);
                }
                if (count == Math.Floor(dashMax * 0.75f))
                {
                    Main.PlaySound(SoundID.Item67, npc.Center);
                    npc.velocity = npc.DirectionTo(aiVector) * 30;
                }
                if (count >= Math.Floor(dashMax * 0.75f) && count <= Math.Floor(dashMax * 0.75f) + 10)
                {
                    npc.position += npc.velocity;
                }
                if (count >= Math.Floor(dashMax * 0.8f))
                {
                    npc.velocity *= 0.92f;
                }
                if (count >= Math.Floor(dashMax * 0.9f))
                {
                    npc.velocity *= 0.94f;
                }

                if (count >= Math.Floor(dashMax * 0.75f))
                {
                    dashing = true;
                }
                else dashing = false;

                if (!dashing)
                {
                    UpdateFrame(0.3f, 0, 6);
                    npc.rotation = 0f + MathHelper.ToRadians(npc.velocity.X / 2);
                    npc.spriteDirection = npc.direction;
                }
                else
                {
                    UpdateFrame(0.3f, 7, 7);
                    npc.rotation = npc.AngleTo(npc.Center + npc.velocity) - MathHelper.ToRadians(90f);
                }
            }
        }

        Vector2 posToBe = Vector2.Zero;
        bool diveBombed = false;
        private void DiveBomb()
        {
            npc.rotation = 0f;
            Player player = Main.player[npc.target];
            if (attackCounter == 0)
            {
                diveBombed = false;
                attackCounter = 1;
            }
            else
            {
                if (!diveBombed)
                {
                    npc.spriteDirection = npc.direction;
                    dashing = false;
                    UpdateFrame(0.3f, 0, 6);
                    if (attackCounter == 1)
                    {
                        posToBe = player.Center - new Vector2(0, 500);
                    }
                    Vector2 direction = posToBe - npc.Center;
                    float lerpSpeed = (float)Math.Sqrt(direction.Length());
                    direction.Normalize();
                    direction *= lerpSpeed;
                    npc.velocity = direction;
                    if (lerpSpeed < 10)
                    {
                        attackCounter++;
                        if (attackCounter > 20)
                        {
                            diveBombed = true;
                            npc.velocity.Y += 5;
                        }
                    }
                }
                else
                {
                    UpdateFrame(0.3f, 7, 7);
                    attackCounter++;
                    npc.noTileCollide = false;
                    if (npc.collideY || !Collision.CanHit(npc.Center, 2, 2, npc.Bottom + new Vector2(0, 20), 2, 2) || npc.velocity.Y == 0)
                    {
                        Main.PlaySound(SoundID.Item67, npc.Center);
                        for (int i = 0; i <= 70; i++)
                        {
                            Dust dust;
                            // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                            Vector2 position = Main.LocalPlayer.Center;
                            dust = Main.dust[Dust.NewDust(npc.Bottom - new Vector2(130, 10), 260, 10, DustID.Electric, 0f, -4.883721f, 0, new Color(255, 255, 255), 0.9883721f)];
                        }
                        //grounded = true;
                        diveBombed = false;
                        npc.velocity = new Vector2(npc.velocity.X, -20);
                        cooldownFrames = 30;
                        dashing = false;
                        return;
                    }
                    else
                        dashing = true;
                    npc.velocity.Y += 5;
                    npc.velocity.X *= 0.7f;
                }
            }
        }
        #endregion
    }


    public class LightningJoltProj : ModProjectile
    {
        float start = 0;
        float timer = 0;

        ref float AttackMode => ref projectile.ai[1];
        ref float AttackTimer => ref projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Jolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 8;
            projectile.damage = 1;
            projectile.aiStyle = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 3;
            projectile.tileCollide = true;
            projectile.timeLeft = 200;
            projectile.scale = 1f;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.CornflowerBlue.ToVector3() / 3);
            start = MathHelper.Lerp(start, 15, 0.14f);
            projectile.rotation = projectile.AngleTo(projectile.Center + projectile.velocity);
            if (AttackMode == 1)
            {
                projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians((float)Math.Cos(AttackTimer / 3)) * 3);
            }
            else
            {
                if (AttackTimer <= 25) projectile.velocity *= 0.95f;
                else projectile.velocity *= 1.05f;
            }
            AttackTimer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            for (int i = 1; i < projectile.oldPos.Length; i++)
            {
                projectile.oldPos[i] = projectile.oldPos[i - 1] + (projectile.oldPos[i] - projectile.oldPos[i - 1]).SafeNormalize(Vector2.Zero) * MathHelper.Min(Vector2.Distance(projectile.oldPos[i - 1], projectile.oldPos[i]), start);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            float vel = MathHelper.Clamp(projectile.velocity.Length() * 0.02f, 0f, 0.2f);

            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.oldPos[i] - Main.screenPosition, new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height), Color.Lerp(Color.White, Color.Black, i / (float)projectile.oldPos.Length), projectile.rotation, new Vector2(Main.projectileTexture[projectile.type].Width / 2, Main.projectileTexture[projectile.type].Height / 2), new Vector2(projectile.scale + vel, projectile.scale - vel) * (1 - ((float)i / projectile.oldPos.Length)), SpriteEffects.None, 0);
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

        ref float AttackMode => ref projectile.ai[1];
        ref float AttackTimer => ref projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Jolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 8;
            projectile.damage = 1;
            projectile.aiStyle = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 3;
            projectile.tileCollide = true;
            projectile.timeLeft = 200;
            projectile.scale = 1f;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.CornflowerBlue.ToVector3() / 3);
            start = MathHelper.Lerp(start, 30, 0.14f);
            projectile.rotation = projectile.AngleTo(projectile.Center + projectile.velocity);

            if (AttackMode == 1)
            {
                projectile.velocity *= 1.06f;
            }
            else
            {
                if (AttackTimer <= 30)
                {
                    projectile.velocity += projectile.DirectionTo(aiVector) * 1.5f;
                    if (projectile.velocity.Length() <= 10)
                    {
                        projectile.velocity.Normalize();
                        projectile.velocity *= 10;
                    }
                }
            }
            AttackTimer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            for (int i = 1; i < projectile.oldPos.Length; i++)
            {
                projectile.oldPos[i] = projectile.oldPos[i - 1] + (projectile.oldPos[i] - projectile.oldPos[i - 1]).SafeNormalize(Vector2.Zero) * MathHelper.Min(Vector2.Distance(projectile.oldPos[i - 1], projectile.oldPos[i]), start);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            float vel = MathHelper.Clamp(projectile.velocity.Length() * 0.02f, 0f, 0.2f);

            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.oldPos[i] - Main.screenPosition, new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height), Color.Lerp(Color.White, Color.Black, i / (float)projectile.oldPos.Length), projectile.rotation, new Vector2(Main.projectileTexture[projectile.type].Width / 2, Main.projectileTexture[projectile.type].Height / 2), new Vector2(projectile.scale + vel, projectile.scale - vel) * (1 - ((float)i / projectile.oldPos.Length)), SpriteEffects.None, 0);
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
            NPCID.Sets.TrailCacheLength[npc.type] = 10;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.rotation = MathHelper.ToRadians(365);
            npc.lifeMax = 5;
            npc.damage = 32;
            npc.defense = 99999;
            npc.alpha = 0;
            npc.knockBackResist = 0f;
            npc.width = 30;
            npc.height = 48;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(0, 0, 0, 0);
            npc.npcSlots = 1f;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit34;
            npc.DeathSound = SoundID.NPCHit56;
            npc.buffImmune[24] = true;
            //bossBag = ModContent.ItemType<SnowriumBag>();
            //music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Snowrium");
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            npc.TargetClosest();
            Player player = Main.player[npc.target];
            npc.ai[0]++;

            if (!player.dead)
            {
                if (npc.ai[0] % 60 == 0 && npc.Distance(player.Center) < 600)
                {
                    Main.PlaySound(SoundID.Item75, npc.Center);
                    int proj = Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center) * 20, ModContent.ProjectileType<LightningJoltProjLarge>(), 10, 5);
                    Main.projectile[proj].ai[1] = 1;
                }
            }
            else
            {
                npc.velocity.Y += 1f;
                if (npc.Distance(player.Center) > 1920) npc.active = false;
            }

            npc.position = Vector2.Lerp(npc.position, aiVector, 0.1f);
            npc.rotation = MathHelper.Lerp(npc.rotation, (float)Math.Cos(npc.ai[0] / 20) * MathHelper.ToRadians(10), 0.07f);
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
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 8;
            projectile.damage = 1;
            projectile.aiStyle = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 3;
            projectile.tileCollide = false;
            projectile.timeLeft = 70;
            projectile.scale = 0f;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.CornflowerBlue.ToVector3() / 3);
            start = MathHelper.Lerp(start, 30, 0.14f);
            projectile.rotation = projectile.AngleTo(projectile.Center + projectile.velocity);

            if (projectile.timeLeft >= 35)
            {
                projectile.scale = MathHelper.Lerp(projectile.scale, 1f, 0.1f);
            }
            else
            {
                projectile.scale = MathHelper.Lerp(projectile.scale, 1f, -0.101f);
            }

            if (parent.active)
            {
                projectile.velocity = parent.velocity + (projectile.DirectionTo(parent.Center).RotatedBy(MathHelper.ToRadians(90)) * projectile.ai[0]);
            }
            else projectile.active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            for (int i = 1; i < projectile.oldPos.Length; i++)
            {
                projectile.oldPos[i] = projectile.oldPos[i - 1] + (projectile.oldPos[i] - projectile.oldPos[i - 1]).SafeNormalize(Vector2.Zero) * MathHelper.Min(Vector2.Distance(projectile.oldPos[i - 1], projectile.oldPos[i]), start);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            float vel = MathHelper.Clamp(projectile.velocity.Length() * 0.02f, 0f, 0.2f);

            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.oldPos[i] - Main.screenPosition, new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height), Color.Lerp(Color.White, Color.Black, i / (float)projectile.oldPos.Length), projectile.rotation, new Vector2(Main.projectileTexture[projectile.type].Width / 2, Main.projectileTexture[projectile.type].Height / 2), new Vector2(projectile.scale + vel, projectile.scale - vel) * (1 - ((float)i / projectile.oldPos.Length)), SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.oldPos[i] - Main.screenPosition, new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height), Color.Black, projectile.rotation, new Vector2(Main.projectileTexture[projectile.type].Width / 2, Main.projectileTexture[projectile.type].Height / 2), new Vector2(projectile.scale + vel, projectile.scale - vel) * MathHelper.Clamp((0.6f - ((float)i / projectile.oldPos.Length)), 0, 1), SpriteEffects.None, 0);
            }

            return false;
        }
    }
}