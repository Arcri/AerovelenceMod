using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class Bomber : ModNPC
    {

        private int bombFrameVariant;
        private bool initializedBombFrame = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Position = new Vector2(0f, 8f),
                PortraitPositionXOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 120;
            NPC.width = NPC.height = 42;
            NPC.noGravity = true;
            NPC.knockBackResist = 0.25f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            AIType = -1;
            NPC.npcSlots = 1f;
            NPC.value = 0f;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                new FlavorTextBestiaryInfoElement("A specialized crystal moth that has adapted to carry heavy mini-tumblers. It drops these creatures on unsupecting prey below.")
            ]);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;
        public override void ModifyNPCLoot(NPCLoot npcLoot) { }

        private int frame;

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;

            NPC.frameCounter++;

            if (NPC.frameCounter >= 5f)
            {
                frame++;
                NPC.frameCounter = 0f;
            }

            int maxFrame = 4;
            int minFrame = 0;

            if (frame > maxFrame)
            {
                frame = minFrame;
            }

            NPC.frame.Y = frame * frameHeight;
        }

        private float SineProgress
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private float BombState
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        private float LockedPosX
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        private float GlowTimer
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        private const float STATE_APPROACHING = 0f;
        private const float STATE_POSITIONING = 1f;
        private const float STATE_LOCKED = 2f;
        private const float STATE_DROPPING = 3f;
        private const float STATE_RETREATING = 4f;
        private Vector2 targetPosition;
        private bool hasBomb = true;
        private int retreatTimer = 0;
        private int RETREAT_DURATION = 100;

        private int bombForceDropTimer = 0;
        private int FORCE_DROP_TIME = 300;
        public override void AI()
        {
            if (!initializedBombFrame)
            {
                bombFrameVariant = Main.rand.Next(2);
                initializedBombFrame = true;
            }

            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            if (!NPC.HasValidTarget || player.dead)
            {
                NPC.velocity.Y -= 0.1f;
                return;
            }

            bombForceDropTimer++;

            if (bombForceDropTimer >= FORCE_DROP_TIME && BombState != STATE_DROPPING && BombState != STATE_RETREATING)
            {
                BombState = STATE_DROPPING;
                NPC.netUpdate = true;
            }

            NPC.rotation = NPC.velocity.X * 0.1f;

            if (Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GemSapphire);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.scale = Main.rand.NextFloat(0.6f, 1f);
                NPC.netUpdate = true;
            }

            AvoidOtherBombers();

            switch ((int)BombState)
            {
                case (int)STATE_APPROACHING:
                    ApproachPlayerX(player);
                    break;
                case (int)STATE_POSITIONING:
                    PositionAbovePlayer(player);
                    break;
                case (int)STATE_LOCKED:
                    LockPosition(player);
                    break;
                case (int)STATE_DROPPING:
                    DropBomb();
                    break;
                case (int)STATE_RETREATING:
                    RetreatAndDie();
                    break;
            }

            SineProgress++;

            if (BombState == STATE_LOCKED)
            {
                float bobAmplitude = 0.5f;
                float bobSpeed = 0.05f;
                NPC.velocity.Y = (float)Math.Sin(SineProgress * bobSpeed) * bobAmplitude;
            }
            else if (BombState != STATE_DROPPING && BombState != STATE_RETREATING)
            {
                float sine = (float)Math.Sin(SineProgress / 20f) * 0.05f;
                NPC.velocity.Y += sine;
            }
        }


        private void AvoidOtherBombers()
        {
            if (BombState != STATE_APPROACHING && BombState != STATE_POSITIONING)
                return;
            const float AVOIDANCE_DISTANCE = 60f;
            const float AVOIDANCE_STRENGTH = 0.4f;
            Vector2 avoidanceForce = Vector2.Zero;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC otherNPC = Main.npc[i];
                if (!otherNPC.active || otherNPC.type != NPC.type || otherNPC.whoAmI == NPC.whoAmI)
                    continue;
                float distance = Vector2.Distance(NPC.Center, otherNPC.Center);
                if (distance < AVOIDANCE_DISTANCE)
                {
                    Vector2 repulsionDirection = NPC.Center - otherNPC.Center;
                    float repulsionStrength = (AVOIDANCE_DISTANCE - distance) / AVOIDANCE_DISTANCE;
                    if (repulsionDirection.Length() > 0)
                    {
                        repulsionDirection.Normalize();
                        avoidanceForce += repulsionDirection * repulsionStrength * AVOIDANCE_STRENGTH;
                    }
                }
            }
            NPC.velocity.X += avoidanceForce.X;
            NPC.velocity.Y += avoidanceForce.Y * 1.5f;
            if (BombState == STATE_POSITIONING && avoidanceForce != Vector2.Zero)
            {
                targetPosition.X += avoidanceForce.X * 50;
                targetPosition.Y += avoidanceForce.Y * 100;
            }
        }

        private void ApproachPlayerX(Player player)
        {
            float speedVariance = 0.8f + (NPC.whoAmI % 5) * 0.1f;
            float xDifference = player.Center.X - NPC.Center.X;
            float xSpeed = Math.Min(Math.Abs(xDifference) * 0.05f, 4f * speedVariance);

            if (xDifference > 10f)
            {
                NPC.velocity.X = Vector2.Lerp(NPC.velocity, new Vector2(xSpeed, NPC.velocity.Y), 0.05f).X;
            }
            else if (xDifference < -10f)
            {
                NPC.velocity.X = Vector2.Lerp(NPC.velocity, new Vector2(-xSpeed, NPC.velocity.Y), 0.05f).X;
            }
            else
            {
                LockedPosX = player.Center.X;
                targetPosition = new Vector2(LockedPosX, player.Center.Y - 120f);
                float heightVariance = Main.rand.NextFloat(-40f, 40f);
                targetPosition.Y += heightVariance;

                BombState = STATE_POSITIONING;
                NPC.netUpdate = true;
            }
            float baseHeight = 60f + (NPC.whoAmI % 3) * 30f;
            float yDifference = (player.Center.Y - baseHeight) - NPC.Center.Y;
            if (Math.Abs(yDifference) > 30f)
                NPC.velocity.Y = Vector2.Lerp(NPC.velocity, new Vector2(NPC.velocity.X, yDifference * 0.02f), 0.03f).Y;
        }

        private void PositionAbovePlayer(Player player)
        {
            targetPosition.Y = player.Center.Y - 120f;
            if (NPC.localAI[0] == 0)
            {
                targetPosition.X += Main.rand.NextFloat(-50f, 50f);
                targetPosition.Y += Main.rand.NextFloat(-30f, 30f);
                NPC.localAI[0] = 1;
            }

            Vector2 moveDirection = targetPosition - NPC.Center;
            float distance = moveDirection.Length();

            if (distance > 10f)
            {
                moveDirection.Normalize();
                float speed = Math.Min(distance * 0.05f, 5f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, moveDirection * speed, 0.1f);
            }
            else
            {
                BombState = STATE_LOCKED;
                GlowTimer = 0f;
                SineProgress = 0f;
                NPC.localAI[0] = 0;
                NPC.netUpdate = true;
            }
        }

        private void LockPosition(Player player)
        {
            float playerDistance = Math.Abs(player.Center.X - NPC.Center.X);
            if (playerDistance > 300f)
            {
                BombState = STATE_APPROACHING;
                NPC.netUpdate = true;
                return;
            }
            GlowTimer++;
            if (Main.rand.NextBool(5))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GemSapphire);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust.scale = Main.rand.NextFloat(0.8f, 1.5f);
            }
            if (GlowTimer >= 60f)
            {
                BombState = STATE_DROPPING;
                NPC.netUpdate = true;
            }
        }

        private void DropBomb()
        {
            if (hasBomb)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 bombPosition = NPC.Center + new Vector2(0, 20);
                    int projectileType = ModContent.ProjectileType<TumbleBomb>();
                    Projectile proj = Projectile.NewProjectileDirect(
                        NPC.GetSource_FromAI(),
                        bombPosition,
                        new Vector2(0, 3f),
                        projectileType,
                        5,
                        2f,
                        Main.myPlayer);

                    if (proj != null)
                    {
                        proj.frame = bombFrameVariant;
                        proj.ai[0] = bombFrameVariant;
                    }

                    hasBomb = false;
                }
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center, 4, 4, DustID.GemSapphire);
                    dust.noGravity = true;
                    dust.velocity = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-1f, 4f));
                    dust.scale = Main.rand.NextFloat(1f, 1.5f);
                }
                SoundEngine.PlaySound(SoundID.Item9, NPC.Center);

                BombState = STATE_RETREATING;
                retreatTimer = 0;
                NPC.netUpdate = true;
            }

            SineProgress++;
        }



        private void RetreatAndDie()
        {
            retreatTimer++;
            float accelerationCurve = 1f - (float)Math.Pow(1f - (retreatTimer / 40f), 3);
            accelerationCurve = MathHelper.Clamp(accelerationCurve, 0f, 1f);
            float maxSpeed = -8f;
            float currentSpeed = maxSpeed * accelerationCurve;
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, currentSpeed, 0.1f);

            if (retreatTimer < 50)
            {
                float swayAmount = (float)Math.Sin(retreatTimer * 0.1f) * 0.2f;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, swayAmount, 0.05f);
            }
            else
            {
                NPC.velocity.X *= 0.98f;
            }
            if (Main.rand.NextBool(Math.Max(5, 20 - retreatTimer / 5)))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GemSapphire);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
                dust.velocity.Y = -1f;
            }
            if (retreatTimer > 50)
            {
                float fadeAmount = (retreatTimer - 50) / 50f;
                NPC.alpha = (int)(fadeAmount * 255);
            }
            if (retreatTimer >= RETREAT_DURATION)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            drawColor = drawColor * ((255 - NPC.alpha) / 255f);

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/CrystalTumbler/Bomber_Glow").Value;
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawPosition = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);
            Texture2D glowTex = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Texture2D glowTex2 = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
            Color glowColor = Color.DodgerBlue * ((255 - NPC.alpha) / 255f);

            if (BombState == STATE_LOCKED)
            {
                float pulseIntensity = (float)Math.Sin(GlowTimer / 10f) * 0.3f + 0.7f;
                glowColor = Color.Lerp(Color.DodgerBlue, Color.White, GlowTimer / 60f) * pulseIntensity * ((255 - NPC.alpha) / 255f);

                float glowSize = 1.5f + (GlowTimer / 60f) * 1.0f;
                Main.spriteBatch.Draw(glowTex, drawPosition, null, glowColor, 0f, glowTex.Size() / 2, glowSize, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(glowTex2, drawPosition, null, glowColor, 0f, glowTex2.Size() / 2, glowSize * 0.5f, SpriteEffects.None, 0);
            }
            else
            {
                Main.spriteBatch.Draw(glowTex, drawPosition, null, glowColor, 0f, glowTex.Size() / 2, 1.5f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(glowTex2, drawPosition, null, glowColor, 0f, glowTex2.Size() / 2, 0.5f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                float opacity = (0.8f - 0.2f * i) * ((255 - NPC.alpha) / 255f);
                Vector2 trailPosition = NPC.oldPos[i] + NPC.Hitbox.Size() / 2f - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);
                spriteBatch.Draw(texture, trailPosition, NPC.frame, drawColor * opacity, NPC.oldRot[i], NPC.frame.Size() / 2f, NPC.scale * 1.1f, effects, 0f);
            }
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            drawColor = drawColor * ((255 - NPC.alpha) / 255f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPosition = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);
            if (hasBomb && BombState != STATE_DROPPING)
            {
                Texture2D bombTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/TumbleBomb").Value;
                int frameHeight = bombTexture.Height / 2;
                Rectangle bombFrame = new(0, bombFrameVariant * frameHeight, bombTexture.Width, frameHeight);
                Vector2 bombOffset = new(0, 30);
                Vector2 bombOrigin = new(bombTexture.Width / 2, frameHeight / 2);

                spriteBatch.Draw(bombTexture, drawPosition + bombOffset, bombFrame, drawColor, 0f, bombOrigin, 1f, effects, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(texture, drawPosition, NPC.frame, Color.White * ((255 - NPC.alpha) / 255f), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            spriteBatch.Draw(texture, drawPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
        }

    }

    public class TumbleBomb : ModProjectile
    {
        private int frameVariant;
        private bool initializedFrames = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.damage = 20;
            Projectile.light = 0.5f;
        }

        private float RotationSpeed => Projectile.ai[0];
        private float ExplosionTimer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private bool isGrounded = false;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.Y;
            if (!initializedFrames)
            {
                frameVariant = Main.rand.Next(2);
                Projectile.frame = frameVariant;
                initializedFrames = true;
            }
            if (!isGrounded)
            {
                Projectile.velocity.Y += 0.15f;
                if (Projectile.velocity.Y > 8f)
                    Projectile.velocity.Y = 8f;
                Projectile.rotation += Projectile.velocity.X * 0.05f;
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard, 0f, 0f, 150, default, 0.8f);
                    dust.noGravity = true;
                    dust.velocity *= 0.2f;
                }
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
                ExplosionTimer++;
                float pulseRate = 1f - ExplosionTimer / 60f;
                if (Main.rand.NextBool((int)(5 * pulseRate) + 1))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, 0f, -2f, 100, default, 1.2f);
                        dust.noGravity = true;
                        dust.velocity *= 1.2f;
                    }
                }
                Projectile.scale = 1f + (float)Math.Sin(ExplosionTimer * (0.1f + 0.05f * (1f - pulseRate))) * 0.1f;
                if (ExplosionTimer >= 60f)
                    Explode();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!isGrounded)
            {
                isGrounded = true;
                ExplosionTimer = 0;
                Projectile.velocity = Vector2.Zero;
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
                for (int i = 0; i < 10; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        DustID.BlueCrystalShard,
                        0f, -2f, 100, default, 1f);
                    dust.velocity.X *= 0.4f;
                }
                return false;
            }
            return false;
        }

        private void Explode()
        {
            for (int i = 0; i < 50; i++)
            {
                Vector2 velocity = new(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.GemSapphire, velocity.X, velocity.Y, 100, default, Main.rand.NextFloat(1f, 2f));
                dust.noGravity = true;
                if (Main.rand.NextBool(3))
                {
                    dust.noGravity = false;
                    dust.scale *= 0.5f;
                }
            }

            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.WhiteTorch, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 3.5f));
                dust.noGravity = true;
                dust.velocity *= 3f;
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.Damage();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 50), Vector2.Zero, ProjectileID.DD2ExplosiveTrapT2Explosion, 0, Projectile.knockBack, Projectile.owner);
            }
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            if (!isGrounded || ExplosionTimer < 60f)
                Explode();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frameRect = new(0, frameVariant * frameHeight, texture.Width, frameHeight);
            Vector2 drawOrigin = new(frameRect.Width / 2, frameRect.Height / 2);
            if (!isGrounded)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Vector2 drawPos = Projectile.oldPos[i] + new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition;
                    Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(texture, drawPos, frameRect, color * 0.5f, Projectile.oldRot[i], drawOrigin, Projectile.scale - i * 0.05f, SpriteEffects.None, 0f);
                }
            }
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color bombColor = lightColor;
            if (isGrounded)
            {
                float pulse = 0.5f + 0.5f * (float)Math.Sin(ExplosionTimer * 0.2f);
                bombColor = Color.Lerp(lightColor, Color.White, pulse * ExplosionTimer / 60f);
            }
            Main.spriteBatch.Draw(texture, new Vector2(drawPosition.X, drawPosition.Y + 8), frameRect, bombColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            if (isGrounded)
            {
                Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Glow").Value;
                float glowScale = 0.7f + ExplosionTimer / 60f;
                float glowAlpha = (float)Math.Sin(ExplosionTimer * 0.7f) * 0.9f + 0.5f;

                Main.spriteBatch.Draw(glowTexture, drawPosition, null, Color.DodgerBlue * glowAlpha, 0f, glowTexture.Size() / 2f, glowScale, SpriteEffects.None,  0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}