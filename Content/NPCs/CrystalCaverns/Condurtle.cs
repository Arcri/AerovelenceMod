using AerovelenceMod.Content.Biomes;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class Condurtle : ModNPC
    {
        private enum State { Walking, Idle, EnteringShell, Waiting, Shaking, Discharging, ExitingShell }
        private State Current => (State)(int)NPC.ai[0];
        private ref float Timer => ref NPC.ai[1];
        private ref float Duration => ref NPC.ai[2];
        private ref float EruptionTimer => ref NPC.ai[3];
        private int cooldown;
        private int turnCooldown;
        private bool linked;
        private float receivedCharge;
        private bool Authority => Main.netMode != NetmodeID.MultiplayerClient;
        private int ShakeCount => Main.expertMode ? 2 : 3;
        internal Vector2 ShellPoint => NPC.Center + new Vector2(-5f * NPC.direction, -16f);
        private static readonly Vector2[][] Creases =
        {
            new[] { new Vector2(-20, -10), new Vector2(-23, -2), new Vector2(-16, 5), new Vector2(-20, 12) },
            new[] { new Vector2(-5, -10), new Vector2(-8, -1), new Vector2(-3, 6), new Vector2(3, 12) },
            new[] { new Vector2(10, -10), new Vector2(17, -3), new Vector2(11, 5), new Vector2(15, 10) }
        };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 26;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers
            {
                Position = new Vector2(0f, 8f), PortraitPositionXOverride = 0f
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 72;
            NPC.height = 40;
            NPC.damage = 15;
            NPC.defense = 12;
            NPC.lifeMax = 120;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 150f;
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = -1;
            SpawnModBiomes = new[] { ModContent.GetInstance<CrystalCavernsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.Add(new FlavorTextBestiaryInfoElement("An unhurried grazer that sheds crystal splinters from its shell. When startled, it rocks and crackles before discharging. Nearby Condurtles form a living electric fence."));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.InModBiome<CrystalCavernsBiome>()
            ? SpawnCondition.Underground.Chance * 0.5f + SpawnCondition.Cavern.Chance * 0.5f : 0f;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => Current == State.Discharging && Timer < 22f;
        public override void SendExtraAI(BinaryWriter writer) { writer.Write((short)cooldown); writer.Write(linked); }
        public override void ReceiveExtraAI(BinaryReader reader) { cooldown = reader.ReadInt16(); linked = reader.ReadBoolean(); }

        private void Change(State next, int duration = 0)
        {
            NPC.ai[0] = (float)next;
            Timer = 0f;
            Duration = duration;
            NPC.netUpdate = true;
        }

        public override void AI()
        {
            if (Duration == 0f && Current == State.Walking && Authority)
            {
                NPC.direction = Main.rand.NextBool() ? 1 : -1;
                Duration = Main.rand.Next(180, 360);
                EruptionTimer = Main.rand.Next(180, 330);
                NPC.netUpdate = true;
            }
            NPC.spriteDirection = NPC.direction;
            Timer++;
            cooldown = Math.Max(0, cooldown - 1);
            turnCooldown = Math.Max(0, turnCooldown - 1);
            receivedCharge = Math.Max(0f, receivedCharge - 0.035f);
            bool roaming = Current is State.Walking or State.Idle;
            NPC.knockBackResist = roaming ? 0.2f : 0f;
            if (roaming)
            {
                if (Current == State.Walking)
                    Walk();
                else
                    NPC.velocity.X *= 0.8f;
                if (Authority && Timer >= Duration)
                {
                    if (Current == State.Idle)
                    {
                        NPC.direction *= -1;
                        Change(State.Walking, Main.rand.Next(180, 360));
                    }
                    else
                        Change(State.Idle, Main.rand.Next(50, 95));
                }
                if (Authority && cooldown == 0 && PlayerNearby())
                {
                    linked = false;
                    Change(State.EnteringShell, 32);
                }
                else
                {
                    EruptionTimer--;
                    if (EruptionTimer > 0f && EruptionTimer < 28f && !Main.dedServ && (int)EruptionTimer % 4 == 0)
                        CondurtleEffects.Spark(ShellPoint, Main.rand.NextVector2Circular(0.7f, 0.7f) - Vector2.UnitY, 0.16f);
                    if (EruptionTimer <= 0f && Authority)
                    {
                        Erupt();
                        EruptionTimer = Main.rand.Next(270, 420);
                        NPC.netUpdate = true;
                    }
                }
            }
            else
            {
                NPC.velocity.X *= 0.7f;
                if (Math.Abs(NPC.velocity.X) < 0.05f) NPC.velocity.X = 0f;
                if (Current == State.Shaking)
                    ShakeEffects();
                if (Current == State.Discharging && (Timer == 1f || Timer == 10f))
                {
                    CondurtleEffects.Burst(ShellPoint, 12, 3.5f);
                    SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.35f, Pitch = Timer == 1f ? 0.2f : 0.45f, MaxInstances = 4 }, NPC.Center);
                    if (Authority && !linked)
                        for (int direction = -1; direction <= 1; direction += 2)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(direction * 28f, -4f), new Vector2(direction * 5.5f, 0f), ModContent.ProjectileType<CondurtleSpark>(), Math.Max(1, NPC.damage / 2), 0f, Main.myPlayer);
                }
                if (Authority && Timer >= Duration)
                {
                    switch (Current)
                    {
                        case State.EnteringShell: Change(State.Waiting, Main.rand.Next(45, 96)); break;
                        case State.Waiting:
                            int windup = (ShakeCount - 1) * 36 + 22;
                            linked = StartLinks(windup);
                            Change(State.Shaking, windup);
                            break;
                        case State.Shaking: Change(State.Discharging, 54); break;
                        case State.Discharging: Change(State.ExitingShell, 30); break;
                        case State.ExitingShell:
                            cooldown = 150;
                            Change(State.Walking, Main.rand.Next(180, 300));
                            break;
                    }
                }
            }
            float charge = Charge;
            if (charge > 0f)
                Lighting.AddLight(NPC.Center, CondurtleEffects.Blue.ToVector3() * charge * 0.65f);
        }

        private bool PlayerNearby()
        {
            foreach (Player player in Main.ActivePlayers)
                if (!player.dead && Vector2.DistanceSquared(player.Center, NPC.Center) < 220f * 220f && Collision.CanHitLine(NPC.Center, 1, 1, player.Center, 1, 1))
                    return true;
            return false;
        }

        private void Walk()
        {
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 0.6f, 0.12f);
            if (NPC.velocity.Y < 0f) return;
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, true, 1);
            if (!Authority || turnCooldown > 0 || NPC.velocity.Y > 1f) return;
            Vector2 ahead = NPC.Bottom + new Vector2(NPC.direction * (NPC.width * 0.5f + 5f), 3f);
            bool ground = Collision.SolidCollision(ahead - new Vector2(3f, 0f), 6, 22);
            Point tile = ahead.ToTileCoordinates();
            if (WorldGen.InWorld(tile.X, tile.Y, 2))
                ground |= Main.tile[tile.X, tile.Y].HasTile && Main.tileSolidTop[Main.tile[tile.X, tile.Y].TileType];
            bool wall = Collision.SolidCollision(ahead - new Vector2(3f, 29f), 6, 12);
            if (!ground || wall || NPC.collideX)
            {
                NPC.direction *= -1;
                NPC.velocity.X = NPC.direction * 0.3f;
                turnCooldown = 30;
                NPC.netUpdate = true;
            }
        }

        private void Erupt()
        {
            int count = 0;
            foreach (Projectile projectile in Main.ActiveProjectiles)
                if (projectile.ModProjectile is CondurtleShard) count++;
            int amount = Math.Min(5, 60 - count);
            for (int i = 0; i < amount; i++)
            {
                float angle = -MathHelper.PiOver2 + (i - 2) * 0.39f + Main.rand.NextFloat(-0.08f, 0.08f);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), ShellPoint, angle.ToRotationVector2() * Main.rand.NextFloat(6.7f, 8.7f), ModContent.ProjectileType<CondurtleShard>(), Math.Max(1, NPC.damage / 2), 0f, Main.myPlayer);
            }
        }

        private bool StartLinks(int warning)
        {
            List<NPC> neighbors = new();
            foreach (NPC other in Main.ActiveNPCs)
                if (other.whoAmI != NPC.whoAmI && other.ModNPC is Condurtle && Vector2.DistanceSquared(other.Center, NPC.Center) <= 360f * 360f && Collision.CanHitLine(ShellPoint, 1, 1, ((Condurtle)other.ModNPC).ShellPoint, 1, 1))
                    neighbors.Add(other);
            neighbors.Sort((a, b) => Vector2.DistanceSquared(a.Center, NPC.Center).CompareTo(Vector2.DistanceSquared(b.Center, NPC.Center)));
            foreach (NPC other in neighbors)
            {
                int total = 0, sourceLinks = 0, targetLinks = 0;
                bool duplicate = false;
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.ModProjectile is not CondurtleArc) continue;
                    total++;
                    int a = (int)projectile.ai[0], b = (int)projectile.ai[1];
                    if (a == NPC.whoAmI || b == NPC.whoAmI) sourceLinks++;
                    if (a == other.whoAmI || b == other.whoAmI) targetLinks++;
                    if ((a == NPC.whoAmI && b == other.whoAmI) || (b == NPC.whoAmI && a == other.whoAmI)) duplicate = true;
                }
                if (total >= 12 || sourceLinks >= 3) break;
                if (duplicate || targetLinks >= 3) continue;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), ShellPoint, Vector2.Zero, ModContent.ProjectileType<CondurtleArc>(), Math.Max(1, NPC.damage / 2), 0f, Main.myPlayer, NPC.whoAmI, other.whoAmI, warning);
            }
            return neighbors.Count > 0;
        }

        private float Charge => Math.Max(receivedCharge, Current switch
        {
            State.Shaking => 0.2f + 0.65f * MathHelper.Clamp(Timer / Duration, 0f, 1f),
            State.Discharging => MathHelper.Clamp((54f - Timer) / 24f, 0f, 1f),
            _ => 0f
        });

        internal void ReceiveCharge(float intensity) => receivedCharge = Math.Max(receivedCharge, intensity);

        private void ShakeEffects()
        {
            if (Main.dedServ) return;
            int shake = Math.Min(ShakeCount - 1, (int)(Timer - 1f) / 36);
            int beat = ((int)Timer - 1) % 36;
            if (beat == 0)
            {
                SoundEngine.PlaySound(SoundID.Item37 with { Volume = 0.18f + shake * 0.07f, Pitch = 0.1f + shake * 0.18f, MaxInstances = 4 }, NPC.Center);
                CondurtleEffects.Burst(ShellPoint, 3 + shake * 4, 1.2f + shake * 0.6f);
            }
            if (beat < 20 && beat % (5 - shake) == 0)
                CondurtleEffects.Spark(NPC.Center + Main.rand.NextVector2Circular(25f, 10f), Main.rand.NextVector2Circular(1.5f, 1.5f), 0.16f + shake * 0.04f);
        }

        public override void FindFrame(int frameHeight)
        {
            int frame = Current switch
            {
                State.Walking => (int)(Timer / 8f) % 6,
                State.Idle => 6 + (int)(Timer / 22f) % 4,
                State.EnteringShell => 10 + Math.Min(7, (int)(Timer / 4f)),
                State.Waiting => 17,
                State.Shaking => 18 + (int)(Timer / 6f) % 2,
                State.Discharging => 18 + (int)(Timer / 3f) % 2,
                State.ExitingShell => 20 + Math.Min(5, (int)(Timer / 5f)),
                _ => 0
            };
            if (NPC.IsABestiaryIconDummy) frame = (int)(Main.GameUpdateCount / 8) % 6;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            float rock = 0f;
            if (Current == State.Shaking)
            {
                float beat = (Timer - 1f) % 36f;
                float envelope = beat < 22f ? MathF.Sin(beat / 22f * MathHelper.Pi) : 0f;
                rock = MathF.Sin(beat * 0.65f) * envelope * (0.07f + 0.07f * Timer / Duration);
            }
            Vector2 pivot = NPC.Bottom + new Vector2(0f, NPC.gfxOffY);
            Vector2 origin = NPC.frame.Size() * 0.5f + new Vector2(0f, NPC.height * 0.5f);
            SpriteEffects flip = NPC.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, pivot - screenPos, NPC.frame, drawColor, rock, origin, NPC.scale, flip, 0f);
            spriteBatch.Draw(glow, pivot - screenPos, NPC.frame, Color.White * 0.8f, rock, origin, NPC.scale, flip, 0f);
            float charge = Charge;
            if (charge > 0.02f)
            {
                float flicker = 0.8f + 0.2f * MathF.Sin(Main.GlobalTimeWrappedHourly * 43f);
                spriteBatch.Draw(glow, pivot - screenPos, NPC.frame, TumblerVFX.Glow(Color.Lerp(CondurtleEffects.Blue, Color.White, charge), charge * flicker), rock, origin, NPC.scale, flip, 0f);
                foreach (Vector2[] crease in Creases)
                {
                    Vector2[] path = new Vector2[crease.Length];
                    for (int i = 0; i < path.Length; i++)
                    {
                        Vector2 local = new(crease[i].X * NPC.direction, crease[i].Y - NPC.height * 0.5f);
                        local.X += i > 0 && i < path.Length - 1 ? MathF.Sin((float)(Main.GameUpdateCount / 3) * 2.1f + i * 13f) * 1.5f : 0f;
                        path[i] = pivot + local.RotatedBy(rock) * NPC.scale;
                    }
                    TumblerLightningSystem.DrawPath(path, CondurtleEffects.Blue, charge * flicker, 1.2f, false);
                }
            }
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit) => CondurtleEffects.Burst(NPC.Center, NPC.life <= 0 ? 18 : 4, NPC.life <= 0 ? 3.5f : 1.5f);
    }

    public class CondurtleShard : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/NPCs/CrystalCaverns/CondurtleConductor";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 100;
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0f)
            {
                CondurtleEffects.Burst(Projectile.Center, 2, 1.3f);
                SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.12f, Pitch = 0.6f, MaxInstances = 2 }, Projectile.Center);
            }
            Projectile.velocity.Y = Math.Min(12f, Projectile.velocity.Y + 0.38f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.12f, 0.3f, 0.4f);
            if ((int)Projectile.localAI[0] % 7 == 0) CondurtleEffects.Spark(Projectile.Center, -Projectile.velocity * 0.08f, 0.12f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            float fade = Math.Min(1f, Projectile.timeLeft / 15f);
            for (int i = 0; i < 4; i++)
                Main.EntitySpriteDraw(texture, position + (i * MathHelper.PiOver2).ToRotationVector2(), null, TumblerVFX.Glow(Color.White, fade * 0.65f), Projectile.rotation, texture.Size() * 0.5f, 0.65f, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, position, null, Color.White * fade, Projectile.rotation, texture.Size() * 0.5f, 0.65f, SpriteEffects.None);
            return false;
        }
        public override void OnKill(int timeLeft) => CondurtleEffects.Burst(Projectile.Center, 4, 1.8f);
    }

    public class CondurtleSpark : ModProjectile
    {
        private readonly TumblerLightningVisual lightning = new();
        public override string Texture => "AerovelenceMod/Assets/Pixel/CrispStarPMA";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 42;
        }
        public override void AI()
        {
            Projectile.localAI[0]++;
            lightning.Update(Projectile, Projectile.Center - Projectile.velocity * Math.Min(5f, Projectile.localAI[0]), Projectile.Center, 0.3f);
            Lighting.AddLight(Projectile.Center, 0.1f, 0.3f, 0.4f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float fade = Math.Min(1f, Projectile.timeLeft / 10f);
            lightning.Draw(Main.spriteBatch, CondurtleEffects.Blue, fade, 1f);
            Texture2D star = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition, null, TumblerVFX.Glow(Color.White, fade), 0f, star.Size() * 0.5f, new Vector2(0.22f, 0.1f), SpriteEffects.None);
            return false;
        }
        public override void OnKill(int timeLeft) => CondurtleEffects.Burst(Projectile.Center, 5, 1.8f);
    }

    public class CondurtleArc : ModProjectile
    {
        private readonly TumblerLightningVisual lightning = new();
        private Vector2 start;
        private Vector2 end;
        private int age;
        private bool retiring;
        private int Warning => (int)Projectile.ai[2];
        private bool Live => !retiring && age >= Warning && age < Warning + 36;
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 480;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Live ? null : false;
        public override void SendExtraAI(BinaryWriter writer) { writer.Write(age); writer.Write(retiring); }
        public override void ReceiveExtraAI(BinaryReader reader) { age = reader.ReadInt32(); retiring = reader.ReadBoolean(); }
        private Condurtle Turtle(float index)
        {
            int i = (int)index;
            return i >= 0 && i < Main.maxNPCs && Main.npc[i].active ? Main.npc[i].ModNPC as Condurtle : null;
        }
        public override void AI()
        {
            age++;
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, Math.Max(1, Warning + 54 - age));
            Condurtle first = Turtle(Projectile.ai[0]), second = Turtle(Projectile.ai[1]);
            if (!retiring && (first == null || second == null || Vector2.DistanceSquared(first.ShellPoint, second.ShellPoint) > 420f * 420f || !Collision.CanHitLine(first.ShellPoint, 1, 1, second.ShellPoint, 1, 1)))
            {
                retiring = true;
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 18);
                Projectile.netUpdate = true;
            }
            if (!retiring)
            {
                start = first.ShellPoint;
                end = second.ShellPoint;
                Projectile.Center = Vector2.Lerp(start, end, 0.5f);
                float charge = Live ? 1f : age < Warning ? 0.2f + age / (float)Warning * 0.6f : Projectile.timeLeft / 18f;
                first.ReceiveCharge(charge);
                second.ReceiveCharge(charge);
            }
            lightning.Update(Projectile, start, end, Live ? 0.65f : 0.35f);
            if (!retiring && (age == Warning || age == Warning + 9))
            {
                CondurtleEffects.Burst(start, 7, 3f);
                CondurtleEffects.Burst(end, 7, 3f);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!Live) return false;
            float collision = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 10f, ref collision);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (age < Warning && !retiring)
            {
                float progress = age / (float)Warning;
                TumblerLightningSystem.DrawPath(new[] { start, end }, CondurtleEffects.Blue, 0.12f + progress * 0.32f, 1f, false, bloom: 0.3f);
                Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
                for (int i = 0; i < 3; i++)
                {
                    float along = (age / 42f + i / 3f) % 1f;
                    Vector2 point = Vector2.Lerp(start, end, along) - Main.screenPosition;
                    Main.EntitySpriteDraw(star, point, null, TumblerVFX.Glow(CondurtleEffects.Blue, 0.3f + progress * 0.5f), (end - start).ToRotation(), star.Size() * 0.5f, new Vector2(0.15f, 0.06f), SpriteEffects.None);
                }
            }
            else
            {
                float fade = Math.Min(1f, Projectile.timeLeft / 18f) * (age < Warning ? 0.25f : 1f);
                float flash = age == Warning || age == Warning + 9 ? 1f : 0f;
                lightning.Draw(Main.spriteBatch, Color.Lerp(CondurtleEffects.Blue, Color.White, flash), fade, 1.8f + flash);
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 1; i <= 5; i++) CondurtleEffects.Spark(Vector2.Lerp(start, end, i / 6f), Main.rand.NextVector2Circular(1.5f, 1.5f), 0.16f);
        }
    }

    internal static class CondurtleEffects
    {
        internal static readonly Color Blue = new(65, 225, 255);
        internal static void Spark(Vector2 position, Vector2 velocity, float scale) => TumblerVFX.SpawnSpark(position, velocity, Blue, scale);
        internal static void Burst(Vector2 point, int count, float speed)
        {
            if (Main.dedServ) return;
            for (int i = 0; i < count; i++) Spark(point, Main.rand.NextVector2Circular(speed, speed), Main.rand.NextFloat(0.14f, 0.24f));
        }
    }
}
