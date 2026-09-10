using System;
using System.Collections.Generic;
using System.IO;
using AerovelenceMod.Common.Globals.Worlds;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public enum TumblerState
    {
        Spawn,
        Idle,
        StarCircuit,
        RecoilRush,
        CrystalRun,
        BoltVolley,
        ConductiveField,
        PhaseTransition,
        ShockDash,
        KnifeCrystals,
        Teleport,
        CrystalCharge,
        Overload,
        Stunned,
        Despawn,
        MagnetClash,
        MagneticCrush,
        LoopSlam,
        GroundRaze,
        CrystalConvergence
    }

    [AutoloadBossHead]
    public partial class CrystalTumbler : ModNPC
    {
        private static readonly TumblerState[] PhaseOnePattern =
        [
            TumblerState.ShockDash,
            TumblerState.ConductiveField,
            TumblerState.StarCircuit,
            TumblerState.MagnetClash,
            TumblerState.MagneticCrush,
            TumblerState.CrystalRun,
            TumblerState.BoltVolley,
            TumblerState.KnifeCrystals
        ];

        private static readonly TumblerState[] PhaseTwoPattern =
        [
            TumblerState.ShockDash,
            TumblerState.BoltVolley,
            TumblerState.MagnetClash,
            TumblerState.KnifeCrystals,
            TumblerState.MagneticCrush,
            TumblerState.CrystalCharge,
            TumblerState.ConductiveField,
            TumblerState.LoopSlam,
            TumblerState.GroundRaze,
            TumblerState.Overload,
            TumblerState.Teleport,
            TumblerState.CrystalConvergence
        ];

        private readonly List<Vector2> afterimagePositions = [];
        private readonly List<float> afterimageRotations = [];
        private float rollVelocity;
        private float visualCharge;
        private float shieldFlash;
        private float auraScale;
        private float? spinTarget;
        private float impactFlash;
        private Vector2 rampStart;
        private Vector2 rampRise;
        private Vector2 teleportDestination;
        private int substate;
        private int repetitions;
        private int shieldHits;
        private int idleDirection;
        private int storedDirection;
        private int stunReturnTimer;
        private int movementCommitTimer;
        private bool phaseTransitionQueued;
        private bool contactDamage;

        private TumblerState State
        {
            get => (TumblerState)(int)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        private int StateTimer
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        private bool PhaseTwo
        {
            get => NPC.ai[2] >= 1f;
            set => NPC.ai[2] = value ? 1f : 0f;
        }

        private int PatternIndex
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        private bool IsServer => Main.netMode != NetmodeID.MultiplayerClient;
        private Player Target => Main.player[NPC.target];
        private float LeftInner => ArenaData.InnerArenaBoundaryLeft.X;
        private float RightInner => ArenaData.InnerArenaBoundaryRight.X;
        private float LeftOuter => ArenaData.OuterArenaBoundaryLeft.X;
        private float RightOuter => ArenaData.OuterArenaBoundaryRight.X;
        private float FloorY => ArenaData.FloorY;
        private Color PhaseColor => PhaseTwo ? new Color(255, 178, 46) : new Color(65, 225, 255);

        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalTumbler";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.TrailingMode[Type] = 0;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 104;
            NPC.height = 104;
            NPC.damage = 28;
            NPC.defense = 10;
            NPC.lifeMax = 3200;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 2, 50, 0);
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = new SoundStyle("AerovelenceMod/Sounds/Effects/RockHit") with { Volume = 0.7f, PitchVariance = 0.18f };
            NPC.DeathSound = SoundID.NPCDeath14;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalTumbler");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.65f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.85f);
        }

        public override void OnSpawn(IEntitySource source)
        {
            State = TumblerState.Spawn;
            idleDirection = Main.rand.NextBool() ? 1 : -1;
            shieldHits = 10;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            if (!ArenaData.Valid)
                ArenaData.TryInitializeFromWorld(NPC.Center);
            if (IsServer)
            {
                ArenaData.ClearEncounterEntities(false);
                if (ArenaData.Valid)
                {
                    NPC.Center = EntranceStart;
                    TumblerArenaGate.SpawnGates(NPC);
                }
            }
            NPC.netUpdate = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                new FlavorTextBestiaryInfoElement("A geode given momentum and purpose, carrying a storm in every crystal seam.")
            ]);
        }

        public override void AI()
        {
            NPC.TargetClosest(false);
            if (!ArenaData.Valid)
                ArenaData.TryInitializeFromWorld(NPC.Center);

            Rectangle encounterBounds = ArenaData.WorldBounds;
            encounterBounds.Inflate(320, 320);
            if (State != TumblerState.Despawn && (!Target.active || Target.dead || !ArenaData.Valid || !encounterBounds.Contains(Target.Center.ToPoint())))
                ChangeState(TumblerState.Despawn);

            if (IsServer && !PhaseTwo && State is not TumblerState.Spawn and not TumblerState.PhaseTransition and not TumblerState.Despawn && NPC.life <= NPC.lifeMax * 0.5f)
            {
                phaseTransitionQueued = true;
                shieldHits = 10;
                ArenaData.ClearEncounterEntities(false, true);
                TumblerMagneticPlatform.Release(NPC);
                ChangeState(TumblerState.PhaseTransition);
            }

            if (State != TumblerState.Despawn)
            {
                NPC.noGravity = false;
                NPC.noTileCollide = false;
            }

            contactDamage = false;
            spinTarget = null;
            impactFlash *= 0.84f;
            shieldFlash *= 0.84f;
            visualCharge = MathHelper.Lerp(visualCharge, 0f, 0.07f);
            auraScale = MathHelper.Lerp(auraScale, 0f, 0.08f);
            RecordAfterimage();

            switch (State)
            {
                case TumblerState.Spawn: SpawnBehavior(); break;
                case TumblerState.Idle: IdleBehavior(); break;
                case TumblerState.StarCircuit: StarCircuit(); break;
                case TumblerState.RecoilRush: RecoilRush(); break;
                case TumblerState.CrystalRun: CrystalRun(); break;
                case TumblerState.BoltVolley: BoltVolley(); break;
                case TumblerState.ConductiveField: ConductiveField(); break;
                case TumblerState.PhaseTransition: PhaseTransition(); break;
                case TumblerState.ShockDash: ShockDash(); break;
                case TumblerState.KnifeCrystals: KnifeCrystals(); break;
                case TumblerState.Teleport: Teleport(); break;
                case TumblerState.CrystalCharge: CrystalCharge(); break;
                case TumblerState.Overload: Overload(); break;
                case TumblerState.Stunned: Stunned(); break;
                case TumblerState.Despawn: Despawn(); break;
                case TumblerState.MagnetClash: MagnetClash(); break;
                case TumblerState.MagneticCrush: MagneticCrush(); break;
                case TumblerState.LoopSlam: LoopSlam(); break;
                case TumblerState.GroundRaze: GroundRaze(); break;
                case TumblerState.CrystalConvergence: CrystalConvergence(); break;
            }

            KeepInsideArena();
            PressureDistantPlayer();
            UpdateRotation();
            if (!Main.dedServ && contactDamage && OnGround() && Math.Abs(NPC.velocity.X) >= 8f && Main.GameUpdateCount % 3 == 0)
                KickUpDust(2);
            if (State != TumblerState.Spawn || StateTimer >= 430)
                Lighting.AddLight(NPC.Center, PhaseColor.ToVector3() * (0.35f + visualCharge * 0.55f));
            StateTimer++;
            if (IsServer && StateTimer % 45 == 0)
                NPC.netUpdate = true;
            if (Target.active && !Target.dead)
                Target.AddBuff(ModContent.BuffType<FearsomeFoe>(), 2);
        }

        private void SpawnBehavior()
        {
            Entrance();
        }

        private void IdleBehavior()
        {
            GroundRoll(PhaseTwo ? 5.7f : 5.1f, 0.16f, 250f);
            if (StateTimer == 80)
                FireShardFan(PhaseTwo ? 3 : 2, 7.5f);
            if (StateTimer >= (PhaseTwo ? 100 : 150))
                SelectNextAttack();
        }

        private void StarCircuit()
        {
            float speed = StateTimer < 55 ? MathHelper.Lerp(5f, 1.4f, StateTimer / 55f) : MathHelper.Lerp(1.4f, 5f, MathHelper.Clamp((StateTimer - 250f) / 100f, 0f, 1f));
            GroundRoll(speed, 0.12f, 250f);
            visualCharge = MathHelper.Clamp(1f - Math.Abs(StateTimer - 55f) / 55f, 0f, 1f);
            if (StateTimer == 55 && IsServer)
            {
                int yellowIndex = 6;
                for (int i = 0; i < 7; i++)
                {
                    Vector2 velocity = (MathHelper.TwoPi * i / 7f).ToRotationVector2() * 7f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(0f, 80f), velocity, ModContent.ProjectileType<TumblerStar>(), ProjectileDamage(15), 0f, Main.myPlayer, i == yellowIndex ? 1f : 0f, NPC.whoAmI);
                }
            }
            if (StateTimer >= 360)
                FinishAttack();
        }

        private void RecoilRush()
        {
            if (substate == 0)
            {
                SpinUp(150, 13.5f);
                if (StateTimer == 85)
                {
                    SpawnStalactites();
                    EnsureConductiveCrystals();
                }
                if (StateTimer >= 150 && OnGround())
                {
                    NPC.velocity = new Vector2(storedDirection * 13.5f, 0f);
                    substate = 1;
                    StateTimer = 0;
                    NPC.netUpdate = true;
                }
            }
            else if (substate == 1)
            {
                contactDamage = true;
                NPC.velocity.X = storedDirection * 13.5f;
                if (ReachedWall(storedDirection))
                {
                    Rebound(storedDirection, 9f, 6f);
                    substate = 2;
                    StateTimer = 0;
                    NPC.netUpdate = true;
                }
            }
            else if (substate == 2)
            {
                float inner = storedDirection < 0 ? LeftInner : RightInner;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, -storedDirection * 3f, 0.04f);
                if ((storedDirection < 0 && NPC.Center.X >= inner) || (storedDirection > 0 && NPC.Center.X <= inner) || StateTimer > 90)
                {
                    NPC.velocity.X *= 0.4f;
                    substate = 3;
                    StateTimer = 0;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                NPC.velocity.X *= 0.91f;
                if (StateTimer < 54 && StateTimer % 9 == 0)
                    SpawnProjectile<CrystalShard>(NPC.Center, new Vector2(Main.rand.NextFloat(-5.5f, 5.5f), Main.rand.NextFloat(-9f, -5.5f)), ProjectileDamage(13));
                if (StateTimer == 58)
                    SpawnProjectile<ElectricBolt>(NPC.Center, (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 9f, ProjectileDamage(16));
                if (StateTimer >= 95)
                    FinishAttack();
            }
        }

        private void CrystalRun()
        {
            if (substate == 0)
            {
                SpinUp(90, 12.5f);
                if (StateTimer >= 90 && OnGround())
                {
                    NPC.velocity.X = storedDirection * 12.5f;
                    substate = 1;
                    StateTimer = 0;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                contactDamage = true;
                NPC.velocity.X = storedDirection * 12.5f;
                if (StateTimer % 22 == 0 && IsServer)
                {
                    float ground = ArenaData.FindGroundWorldY(NPC.Center.X, NPC.Bottom.Y);
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)ground, ModContent.NPCType<TumblerCrystalBud>(), 0, PhaseTwo ? 1f : 0f);
                }
                if (ReachedWall(storedDirection))
                {
                    Rebound(storedDirection, 5.5f, 3.5f);
                    FinishAttack();
                }
            }
        }

        private void BoltVolley()
        {
            GroundRoll(5.4f, 0.14f, 320f);
            if (StateTimer >= 45 && StateTimer % (PhaseTwo ? 60 : 72) == 0)
                SpawnProjectile<TumblerAimLine>(NPC.Center, (Target.Center + Target.velocity * 18f - NPC.Center).SafeNormalize(Vector2.UnitY), ProjectileDamage(17), 0f, PhaseTwo ? 1f : 0f);
            if (StateTimer == 100 || StateTimer == 240)
            {
                if (PhaseTwo)
                    SpawnProjectile<TumblerChargedKnifeBall>(NPC.Center - new Vector2(0f, 60f), new Vector2(-idleDirection * 3f, -4f), ProjectileDamage(16));
                else
                    SpawnProjectile<TumblerKnifeBall>(NPC.Center, new Vector2(-idleDirection * 3f, -4f), ProjectileDamage(16));
            }
            if (StateTimer >= 360)
                FinishAttack();
        }

        private void ConductiveField()
        {
            EnsureConductiveCrystals();
            GroundRoll(1.8f, 0.08f, 180f);
            visualCharge = MathHelper.Clamp(StateTimer / 120f, 0f, 1f);
            if (StateTimer == 35)
                SpawnConductiveLink(false);
            if (StateTimer == 155)
                SpawnProjectile<TumblerBossAura>(NPC.Center, Vector2.Zero, ProjectileDamage(22), 0f, NPC.whoAmI, 195f, 82f);
            if (StateTimer == 170 || StateTimer == 250)
            {
                foreach (NPC crystal in Main.ActiveNPCs)
                {
                    if (crystal.ModNPC is not TumblerConductiveCrystal || (StateTimer == 170) != (crystal.Center.X < NPC.Center.X))
                        continue;
                    Vector2 tip = crystal.Top + new Vector2(0f, 6f);
                    SpawnProjectile<TumblerAimLine>(tip, (Target.Center - tip).SafeNormalize(Vector2.UnitY), ProjectileDamage(15), 0f, PhaseTwo ? 1f : 0f);
                }
            }
            if (StateTimer >= 155 && StateTimer < 350)
            {
                contactDamage = true;
                auraScale = 0.8f;
            }
            if (StateTimer >= 395)
                FinishAttack();
        }

        private void MagnetClash()
        {
            GroundRoll(2.2f, 0.12f, 300f);
            visualCharge = MathHelper.Clamp(StateTimer / 75f, 0f, 1f);
            if (StateTimer == 75 || StateTimer == 93 || StateTimer == 111)
                SpawnMagneticRocks();
            if (StateTimer >= 360)
                FinishAttack();
        }

        private void MagneticCrush()
        {
            GroundRoll(1.6f, 0.12f, 260f);
            visualCharge = MathHelper.Clamp(StateTimer / 90f, 0f, 1f);
            if (StateTimer <= 1)
            {
                EnsureMagneticPlatforms();
                if (IsServer)
                    TumblerMagneticPlatform.BeginCrush(NPC, 90, 45);
            }
            if (StateTimer == 110 || StateTimer == 185)
                SpawnProjectile<TumblerAimLine>(NPC.Center, (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY), ProjectileDamage(16), 0f, PhaseTwo ? 1f : 0f);
            if (StateTimer == 285 && IsServer)
                TumblerMagneticPlatform.Release(NPC);
            if (StateTimer >= 340)
                FinishAttack();
        }

        private void PhaseTransition()
        {
            NPC.dontTakeDamage = false;
            shieldHits = Math.Max(shieldHits, 0);
            auraScale = 0.75f + 0.08f * MathF.Sin(StateTimer * 0.08f);
            GroundRoll(4f, 0.11f, 230f);
            if (StateTimer % 90 == 45)
                FireShardFan(3, 7.5f);
            if (shieldHits <= 0 && IsServer)
            {
                PhaseTwo = true;
                phaseTransitionQueued = false;
                shieldFlash = 1f;
                SoundEngine.PlaySound(SoundID.Shatter with { Pitch = -0.15f }, NPC.Center);
                ScreenShake(10f);
                PatternIndex = 0;
                stunReturnTimer = 90;
                ChangeState(TumblerState.Stunned);
            }
        }

        private void ShockDash()
        {
            bool rampPass = PhaseTwo || repetitions >= 1;
            if (substate == 0)
            {
                if (storedDirection == 0)
                    storedDirection = NPC.Center.X < ArenaData.ArenaCenter.X ? 1 : -1;
                float stagingX = storedDirection > 0 ? LeftInner + 30f : RightInner - 30f;
                MoveHorizontal(stagingX, 8f, 0.08f);
                if (Math.Abs(NPC.Center.X - stagingX) < 24f && Math.Abs(NPC.velocity.X) < 2.5f && OnGround())
                {
                    rampStart = new Vector2(NPC.Center.X + storedDirection * 64f, FloorY);
                    float rise = repetitions switch { 0 => 70f, 1 => 112f, _ => 90f };
                    rampRise = new Vector2(storedDirection * 260f, -rise);
                    if (rampPass)
                        SpawnProjectile<TumblerFilamentRamp>(rampStart, rampRise, 0, 0f, NPC.whoAmI, 320f, PhaseTwo ? 75f : 90f);
                    substate = 1;
                    StateTimer = 0;
                    NPC.netUpdate = true;
                }
            }
            else if (substate == 1)
            {
                int windup = PhaseTwo ? 75 : 90;
                SpinUp(windup, PhaseTwo ? 16f : 13f);
                if (StateTimer >= windup && OnGround())
                {
                    substate = rampPass ? 2 : 5;
                    StateTimer = 0;
                    NPC.velocity = new Vector2(storedDirection * (PhaseTwo ? 16f : 13f), 0f);
                    SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/GiantElectricityShot") with { Volume = 0.45f, Pitch = 0.2f }, NPC.Center);
                    NPC.netUpdate = true;
                }
            }
            else if (substate == 2)
            {
                contactDamage = true;
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                visualCharge = 1f;
                float progress = (NPC.Center.X - rampStart.X) / rampRise.X;
                Vector2 tangent = TumblerFilamentRamp.TangentOnRamp(rampRise, progress);
                float nextX = NPC.Center.X + tangent.X * 16f;
                float nextProgress = (nextX - rampStart.X) / rampRise.X;
                Vector2 surface = TumblerFilamentRamp.PointOnRamp(rampStart, rampRise, nextProgress);
                Vector2 nextCenter = new(nextX, surface.Y - NPC.height * 0.5f);
                NPC.velocity = nextCenter - NPC.Center;
                spinTarget = storedDirection * NPC.velocity.Length() / (NPC.width * 0.5f);
                if (nextProgress >= 1f)
                {
                    NPC.velocity = TumblerFilamentRamp.TangentOnRamp(rampRise, 1f) * 17f;
                    substate = 3;
                    StateTimer = 0;
                    SpawnAuraPulse(90f, 24, false);
                    ScreenShake(3f);
                    NPC.netUpdate = true;
                }
            }
            else if (substate == 3)
            {
                contactDamage = true;
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                visualCharge = 0.9f;
                NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.42f, 16f);
                if (PhaseTwo && StateTimer == 16)
                    SpawnLightningSweep(repetitions == 1 ? 7 : 4, storedDirection, 54);
                if (ReachedWall(storedDirection))
                {
                    Rebound(storedDirection, 7f, 0f);
                    storedDirection *= -1;
                    NPC.netUpdate = true;
                }
                if (NPC.velocity.Y >= 0f && NPC.Bottom.Y + NPC.velocity.Y >= FloorY)
                {
                    NPC.velocity.Y = FloorY - NPC.Bottom.Y;
                    NPC.velocity.X *= 0.6f;
                    impactFlash = 1f;
                    SpawnAuraPulse(135f, 32, false);
                    if (repetitions < 1 || !TumblerMagneticPlatform.CollapseAll(NPC))
                        EnsureMagneticPlatforms();
                    EnsureConductiveCrystals();
                    KickUpDust(12);
                    SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.7f, Pitch = -0.25f }, NPC.Center);
                    ScreenShake(6f);
                    substate = 4;
                    StateTimer = 0;
                    NPC.netUpdate = true;
                }
            }
            else if (substate == 5)
            {
                contactDamage = true;
                NPC.velocity.X = storedDirection * 13f;
                if (ReachedWall(storedDirection))
                {
                    Rebound(storedDirection, 6f, 4f);
                    substate = 4;
                    StateTimer = 0;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                NPC.velocity.X = Approach(NPC.velocity.X, 0f, 0.35f);
                if (StateTimer >= 42)
                {
                    repetitions++;
                    if (repetitions >= 2)
                        FinishAttack();
                    else
                    {
                        storedDirection = NPC.Center.X < ArenaData.ArenaCenter.X ? 1 : -1;
                        substate = 0;
                        StateTimer = 0;
                        NPC.netUpdate = true;
                    }
                }
            }
        }

        private void SpawnLightningSweep(int count, int direction, int warning)
        {
            for (int i = 0; i < count; i++)
            {
                float progress = (i + 0.5f) / count;
                float x = MathHelper.Lerp(LeftInner + 65f, RightInner - 65f, direction > 0 ? progress : 1f - progress);
                Vector2 start = new(x, ArenaData.WorldBounds.Top + 120f);
                SpawnProjectile<TumblerLightningBolt>(start, new Vector2(0f, FloorY - start.Y), ProjectileDamage(19), 0f, warning + i * 12f, PhaseTwo ? 1f : 0f);
            }
        }

        private void KnifeCrystals()
        {
            GroundRoll(2.6f, 0.09f, 240f);
            visualCharge = 0.5f;
            if (StateTimer == 30 || StateTimer == 230 || StateTimer == 430)
                SpawnPylonFields();
            if (StateTimer >= 630)
                FinishAttack();
        }

        private void Teleport()
        {
            NPC.velocity *= 0.9f;
            visualCharge = MathHelper.Clamp(StateTimer / 70f, 0f, 1f);
            if (StateTimer <= 1 && IsServer)
            {
                float x = NPC.Center.X < ArenaData.ArenaCenter.X ? RightInner - 90f : LeftInner + 90f;
                teleportDestination = new Vector2(x, FloorY - NPC.height * 0.5f);
                NPC.netUpdate = true;
            }
            if (StateTimer == 70)
            {
                teleportOrigin = NPC.Center;
                NPC.Center = teleportDestination;
                NPC.velocity = Vector2.Zero;
                afterimagePositions.Clear();
                afterimageRotations.Clear();
                shieldFlash = 1f;
                ScreenShake(5f);
                SpawnAuraPulse(100f, 30, false);
                SpawnProjectile<TumblerChargedKnifeBall>(NPC.Center + new Vector2(0f, -85f), new Vector2(0f, -2f), ProjectileDamage(16));
                NPC.netUpdate = true;
            }
            if (StateTimer >= 170)
                FinishAttack();
        }

        private void CrystalCharge()
        {
            GroundRoll(3.8f, 0.1f, 230f);
            visualCharge = MathHelper.Clamp(StateTimer / 80f, 0f, 1f);
            auraScale = StateTimer >= 80 ? 0.72f : 0f;
            if (StateTimer == 80)
                SpawnProjectile<TumblerBossAura>(NPC.Center, Vector2.Zero, ProjectileDamage(22), 0f, NPC.whoAmI, 160f, 82f);
            if (StateTimer == 95 || StateTimer == 170)
                SpawnProjectile<TumblerChargedKnifeBall>(ArenaData.ClosestCrystal(Target.Center) + new Vector2(0f, 65f), new Vector2(0f, 2f), ProjectileDamage(17));
            if (StateTimer >= 80 && StateTimer < 240)
                contactDamage = true;
            if (StateTimer >= 255)
                FinishAttack();
        }

        private void Overload()
        {
            GroundRoll(0.7f, 0.04f, 200f);
            visualCharge = MathHelper.Clamp(StateTimer / 180f, 0f, 1f);
            auraScale = MathHelper.Lerp(0.65f, 1.15f, visualCharge);
            if (StateTimer == 120)
                SpawnProjectile<TumblerBossAura>(NPC.Center, Vector2.Zero, ProjectileDamage(24), 0f, NPC.whoAmI, 90f, 88f);
            if (StateTimer == 180 || StateTimer == 205)
            {
                int count = StateTimer == 180 ? 10 : 14;
                for (int i = 0; i < count; i++)
                    SpawnProjectile<ElectricBolt>(NPC.Center, (MathHelper.TwoPi * i / count + (StateTimer == 205 ? 0.12f : 0f)).ToRotationVector2() * 8.5f, ProjectileDamage(18), 0f, 1f);
                SpawnAuraPulse(150f, 24, false);
                ScreenShake(9f);
            }
            if (StateTimer >= 280)
                FinishAttack();
        }

        private void Stunned()
        {
            NPC.velocity.X *= 0.86f;
            if (StateTimer >= stunReturnTimer)
                ChangeState(TumblerState.Idle);
        }

        private void Despawn()
        {
            if (StateTimer == 0 && IsServer)
                ArenaData.ClearEncounterEntities();
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0f, -12f), 0.05f);
            if (NPC.timeLeft > 90)
                NPC.timeLeft = 90;
        }

        private void GroundRoll(float maxSpeed, float acceleration, float preferredDistance)
        {
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            float delta = Target.Center.X - NPC.Center.X;
            int towardPlayer = delta >= 0f ? 1 : -1;
            float boundaryPadding = NPC.width * 0.5f + 4f;
            float stoppingDistance = NPC.velocity.X * NPC.velocity.X / (2f * acceleration) + 24f;
            bool nearLeft = NPC.velocity.X < 0f && NPC.Center.X < LeftOuter + boundaryPadding + stoppingDistance;
            bool nearRight = NPC.velocity.X > 0f && NPC.Center.X > RightOuter - boundaryPadding - stoppingDistance;

            if (nearLeft || nearRight)
            {
                idleDirection = nearLeft ? 1 : -1;
                movementCommitTimer = 70;
            }
            else
            {
                if (StateTimer <= 1 && movementCommitTimer <= 0)
                {
                    idleDirection = towardPlayer;
                    movementCommitTimer = 75;
                }
                if (Math.Abs(delta) < 72f && Math.Sign(NPC.velocity.X) == towardPlayer)
                    movementCommitTimer = Math.Max(movementCommitTimer, 42);
                if (movementCommitTimer <= 0)
                {
                    idleDirection = Math.Abs(delta) > preferredDistance ? towardPlayer : -towardPlayer;
                    movementCommitTimer = 68;
                }
                else
                    movementCommitTimer--;
            }

            float desiredSpeed = idleDirection * maxSpeed;
            NPC.velocity.X = Approach(NPC.velocity.X, desiredSpeed, acceleration);
            contactDamage = true;
        }

        private void SpinUp(int duration, float dashSpeed)
        {
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.velocity.X = Approach(NPC.velocity.X, 0f, 0.34f);
            float progress = MathHelper.Clamp(StateTimer / (float)duration, 0f, 1f);
            if (storedDirection == 0)
                storedDirection = Target.Center.X >= NPC.Center.X ? 1 : -1;
            spinTarget = storedDirection * dashSpeed / (NPC.width * 0.5f) * Easings.easeInSine(progress);
            visualCharge = Easings.easeInOutSine(progress);
            if (OnGround() && StateTimer % 4 == 0 && progress > 0.12f)
                KickUpDust(progress > 0.65f ? 2 : 1);
            if (OnGround() && StateTimer > 30 && StateTimer < duration - 18 && StateTimer % Math.Max(14, 32 - (int)(progress * 18f)) == 0)
                NPC.velocity.Y = -MathHelper.Lerp(1.2f, 3f, progress);
            if (OnGround() && Math.Abs(rollVelocity) > 0.12f && StateTimer % 8 == 0)
                SpawnProjectile<TumblerSpark>(NPC.Bottom - new Vector2(storedDirection * 26f, 8f), new Vector2(-storedDirection * Main.rand.NextFloat(2f, 5f), Main.rand.NextFloat(-2.5f, -0.5f)), 0, 0f, PhaseTwo ? 1f : 0f);
        }

        private void MoveHorizontal(float destinationX, float maxSpeed, float inertia)
        {
            float desired = MathHelper.Clamp((destinationX - NPC.Center.X) * 0.08f, -maxSpeed, maxSpeed);
            NPC.velocity.X = Approach(NPC.velocity.X, desired, inertia * maxSpeed);
        }

        private static float Approach(float current, float target, float amount)
        {
            if (current < target)
                return Math.Min(current + amount, target);
            return Math.Max(current - amount, target);
        }

        private bool OnGround()
        {
            return NPC.velocity.Y >= 0f && (NPC.collideY || Collision.SolidCollision(NPC.BottomLeft + new Vector2(5f, 1f), NPC.width - 10, 4));
        }

        private void UpdateRotation()
        {
            float targetRoll = spinTarget ?? NPC.velocity.X / (NPC.width * 0.5f);
            if (!spinTarget.HasValue && !OnGround() && State != TumblerState.Despawn)
                targetRoll = rollVelocity * 0.995f;
            rollVelocity = Approach(rollVelocity, targetRoll, spinTarget.HasValue ? 0.012f : 0.025f);
            NPC.rotation += rollVelocity;
        }

        private bool ReachedWall(int direction)
        {
            float margin = NPC.width * 0.5f + 6f;
            float nextX = NPC.Center.X + NPC.velocity.X;
            bool boundary = direction < 0 ? nextX <= LeftOuter + margin : nextX >= RightOuter - margin;
            return boundary || !NPC.noTileCollide && NPC.collideX && Math.Sign(NPC.oldVelocity.X) == direction;
        }

        private void Rebound(int direction, float speed, float hop)
        {
            NPC.velocity.X = -direction * speed;
            if (hop > 0f)
                NPC.velocity.Y = -hop;
            idleDirection = -direction;
            movementCommitTimer = 65;
            impactFlash = 1f;
            SpawnAuraPulse(100f, 24, false);
            SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.65f, Pitch = -0.35f }, NPC.Center);
            ScreenShake(5f);
            KickUpDust(8);
            NPC.netUpdate = true;
        }

        private void KickUpDust(int count)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < count; i++)
            {
                Vector2 position = NPC.Bottom + new Vector2(Main.rand.NextFloat(-34f, 34f), -7f);
                float speed = spinTarget.HasValue ? spinTarget.Value * NPC.width * 0.5f : NPC.velocity.X;
                Vector2 velocity = new(-speed * 0.18f + Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-2.2f, -0.6f));
                Dust.NewDustPerfect(position, ModContent.DustType<TumblerRollDust>(), velocity, 45, new Color(115, 118, 128), Main.rand.NextFloat(0.16f, 0.25f));
            }
        }

        private void KeepInsideArena()
        {
            if (!ArenaData.Valid || State == TumblerState.Despawn)
                return;
            float margin = NPC.width * 0.5f + 4f;
            float leftEdge = LeftOuter + margin;
            float rightEdge = RightOuter - margin;
            if (NPC.Center.X < leftEdge || NPC.Center.X > rightEdge)
            {
                NPC.Center = new Vector2(MathHelper.Clamp(NPC.Center.X, leftEdge, rightEdge), NPC.Center.Y);
                int direction = NPC.Center.X <= leftEdge ? -1 : 1;
                if (NPC.velocity.X * direction > 0f)
                    Rebound(direction, Math.Max(2f, Math.Abs(NPC.velocity.X) * 0.6f), 0f);
            }
            else if (!NPC.noTileCollide && NPC.collideX && Math.Abs(NPC.oldVelocity.X) > 1f && Math.Sign(NPC.velocity.X) == Math.Sign(NPC.oldVelocity.X))
                Rebound(Math.Sign(NPC.oldVelocity.X), Math.Abs(NPC.oldVelocity.X) * 0.6f, 2f);
        }

        private void SelectNextAttack()
        {
            if (!IsServer)
                return;
            if (phaseTransitionQueued)
            {
                shieldHits = 10;
                ChangeState(TumblerState.PhaseTransition);
                return;
            }
            TumblerState[] pattern = PhaseTwo ? PhaseTwoPattern : PhaseOnePattern;
            TumblerState next = pattern[PatternIndex % pattern.Length];
            PatternIndex = (PatternIndex + 1) % pattern.Length;
            if (!Main.expertMode && next is TumblerState.GroundRaze or TumblerState.CrystalConvergence)
            {
                next = pattern[PatternIndex % pattern.Length];
                PatternIndex = (PatternIndex + 1) % pattern.Length;
            }
            ChangeState(next);
        }

        private void FinishAttack()
        {
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = false;
            if (phaseTransitionQueued)
            {
                shieldHits = 10;
                ChangeState(TumblerState.PhaseTransition);
            }
            else if (PhaseTwo)
            {
                stunReturnTimer = 60;
                ChangeState(TumblerState.Stunned);
            }
            else
                ChangeState(TumblerState.Idle);
        }

        private void ChangeState(TumblerState state)
        {
            if (!IsServer)
                return;
            if (State == state && StateTimer == 0)
                return;
            State = state;
            StateTimer = 0;
            substate = 0;
            repetitions = 0;
            storedDirection = state is TumblerState.CrystalRun or TumblerState.ShockDash
                ? (NPC.Center.X < ArenaData.ArenaCenter.X ? 1 : -1)
                : (Target.Center.X >= NPC.Center.X ? 1 : -1);
            visualCharge = 0f;
            auraScale = 0f;
            contactDamage = false;
            movementCommitTimer = 0;
            NPC.netUpdate = true;
        }

        private void FireShardFan(int count, float speed)
        {
            Vector2 aim = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            for (int i = 0; i < count; i++)
            {
                float spread = count == 1 ? 0f : MathHelper.Lerp(-0.16f, 0.16f, i / (float)(count - 1));
                SpawnProjectile<CrystalShard>(NPC.Center, aim.RotatedBy(spread) * speed + new Vector2(0f, -2f), ProjectileDamage(13));
            }
        }

        private void SpawnStalactites()
        {
            for (int i = 0; i < 7; i++)
            {
                float x = MathHelper.Lerp(LeftInner + 70f, RightInner - 70f, (i + 0.5f) / 7f);
                float y = ArenaData.WorldBounds.Top + 120f;
                SpawnProjectile<Stalactite>(new Vector2(x, y), Vector2.Zero, ProjectileDamage(18), 0f, i * 10f);
            }
        }

        private void EnsureConductiveCrystals()
        {
            if (!IsServer || NPC.CountNPCS(ModContent.NPCType<TumblerConductiveCrystal>()) >= 2)
                return;
            float y = ArenaData.WorldBounds.Top + 100f;
            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(LeftInner + 100f), (int)y, ModContent.NPCType<TumblerConductiveCrystal>(), 0, -1f);
            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(RightInner - 100f), (int)y, ModContent.NPCType<TumblerConductiveCrystal>(), 0, 1f);
        }

        private void SpawnConductiveLink(bool activeImmediately)
        {
            if (!IsServer)
                return;
            List<int> crystals = [];
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<TumblerConductiveCrystal>())
                    crystals.Add(i);
            }
            if (crystals.Count >= 2)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), Vector2.Zero, Vector2.Zero, ModContent.ProjectileType<TumblerConductiveField>(), ProjectileDamage(24), 0f, Main.myPlayer, crystals[0], crystals[1], activeImmediately ? 1f : 0f);
        }

        private void EnsureMagneticPlatforms()
        {
            if (!IsServer)
                return;
            int platformType = ModContent.ProjectileType<TumblerMagneticPlatform>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == platformType && Main.projectile[i].ai[0] == NPC.whoAmI)
                {
                    return;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                float x = MathHelper.Lerp(LeftInner + 140f, RightInner - 140f, i / 2f);
                float y = FloorY - (i == 1 ? 195f : 125f);
                TumblerMagneticPlatform.Spawn(NPC, new Vector2(x, y), i * 16);
            }
            NPC.netUpdate = true;
        }

        private void SpawnPylonFields()
        {
            if (!IsServer)
                return;
            float gapWidth = Main.masterMode ? 180f : Main.expertMode ? 220f : 260f;
            int direction = StateTimer < 200 || StateTimer > 400 ? 1 : -1;
            if (Target.Center.X > RightInner - 340f)
                direction = -1;
            else if (Target.Center.X < LeftInner + 340f)
                direction = 1;
            float gapCenter = MathHelper.Clamp(Target.Center.X + direction * 210f, LeftInner + 48f + gapWidth * 0.5f, RightInner - 48f - gapWidth * 0.5f);
            float gapLeft = gapCenter - gapWidth * 0.5f;
            float gapRight = gapCenter + gapWidth * 0.5f;
            SpawnPylonRange(LeftInner + 48f, RightInner - 48f, FloorY, gapLeft, gapRight);
        }

        private void SpawnPylonRange(float left, float right, float y, float gapLeft, float gapRight)
        {
            float leftEnd = Math.Min(right, gapLeft);
            float rightStart = Math.Max(left, gapRight);
            if (leftEnd - left >= 40f)
                SpawnPylonSegment(left, leftEnd, y);
            if (right - rightStart >= 40f)
                SpawnPylonSegment(rightStart, right, y);
        }

        private void SpawnPylonSegment(float left, float right, float y)
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(left, y), new Vector2(right - left, 0f), ModContent.ProjectileType<TumblerPylonField>(), ProjectileDamage(20), 0f, Main.myPlayer, 100f, 75f, PhaseTwo ? 1f : 0f);
        }

        private void SpawnAuraPulse(float radius, int lifetime, bool hostile)
        {
            SpawnProjectile<TumblerAuraPulse>(NPC.Center, Vector2.Zero, hostile ? ProjectileDamage(22) : 0, 0f, radius, lifetime, PhaseTwo ? 1f : 0f);
        }

        private void SpawnMagneticRocks()
        {
            if (!IsServer)
                return;
            int direction = Target.Center.X >= NPC.Center.X ? 1 : -1;
            SpawnProjectile<TumblerMagneticRock>(NPC.Center + new Vector2(direction * 70f, -40f), new Vector2(direction * (5f + (StateTimer - 75f) / 18f), -9f), ProjectileDamage(20), 0f, NPC.whoAmI, PhaseTwo ? 1f : 0f);
            SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.45f, Pitch = (StateTimer - 75f) / 90f }, NPC.Center);
        }

        private int ProjectileDamage(int normal)
        {
            if (Main.masterMode)
                return (int)(normal * 0.85f);
            if (Main.expertMode)
                return (int)(normal * 0.75f);
            return normal;
        }

        private void SpawnProjectile<T>(Vector2 position, Vector2 velocity, int damage, float knockback = 0f, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f) where T : ModProjectile
        {
            if (IsServer)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<T>(), damage, knockback, Main.myPlayer, ai0, ai1, ai2);
        }

        private void RecordAfterimage()
        {
            if (Main.dedServ || Main.GameUpdateCount % 2 != 0)
                return;
            afterimagePositions.Add(NPC.Center);
            afterimageRotations.Add(NPC.rotation);
            if (afterimagePositions.Count > 9)
            {
                afterimagePositions.RemoveAt(0);
                afterimageRotations.RemoveAt(0);
            }
        }

        private void ScreenShake(float power)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                AeroPlayer player = Main.LocalPlayer.GetModPlayer<AeroPlayer>();
                player.ScreenShakePower = Math.Max(player.ScreenShakePower, power);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return contactDamage;
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (State == TumblerState.PhaseTransition)
            {
                modifiers.SetMaxDamage(1);
                modifiers.HideCombatText();
            }
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (State == TumblerState.Teleport && StateTimer < 70)
                modifiers.FinalDamage *= 0.35f;
            if (PhaseTwo || State == TumblerState.PhaseTransition)
                return;
            int transitionLife = (int)Math.Ceiling(NPC.lifeMax * 0.5f);
            int damageUntilTransition = NPC.life - transitionLife;
            modifiers.SetMaxDamage(Math.Max(1, damageUntilTransition));
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (State == TumblerState.PhaseTransition)
            {
                modifiers.SetMaxDamage(1);
                modifiers.HideCombatText();
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            DamageShield(hit.Damage);
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            if (State == TumblerState.PhaseTransition)
            {
                NPC.lifeRegen = 0;
                NPC.lifeRegenCount = 0;
                damage = 0;
            }
        }

        private void DamageShield(int damageDone)
        {
            if (State != TumblerState.PhaseTransition || damageDone <= 0)
                return;
            NPC.life = Math.Min(NPC.lifeMax, NPC.life + damageDone);
            shieldHits = Math.Max(0, shieldHits - 1);
            shieldFlash = 1f;
            SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.65f, Pitch = 0.35f, PitchVariance = 0.15f }, NPC.Center);
            if (IsServer)
                NPC.netUpdate = true;
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Weapons.BossDrops.CrystalTumbler.DarkCrystalStaff>()));
        }

        public override void OnKill()
        {
            DownedWorld.DownedCrystalTumbler = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            ArenaData.ClearEncounterEntities();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(substate);
            writer.Write(repetitions);
            writer.Write(shieldHits);
            writer.Write(idleDirection);
            writer.Write(storedDirection);
            writer.Write(stunReturnTimer);
            writer.Write(movementCommitTimer);
            writer.Write(phaseTransitionQueued);
            writer.Write(rampStart.X);
            writer.Write(rampStart.Y);
            writer.Write(rampRise.X);
            writer.Write(rampRise.Y);
            writer.Write(teleportDestination.X);
            writer.Write(teleportDestination.Y);
            writer.Write(rollVelocity);
            writer.Write(teleportOrigin.X);
            writer.Write(teleportOrigin.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            substate = reader.ReadInt32();
            repetitions = reader.ReadInt32();
            shieldHits = reader.ReadInt32();
            idleDirection = reader.ReadInt32();
            storedDirection = reader.ReadInt32();
            stunReturnTimer = reader.ReadInt32();
            movementCommitTimer = reader.ReadInt32();
            phaseTransitionQueued = reader.ReadBoolean();
            rampStart = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            rampRise = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            teleportDestination = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            rollVelocity = reader.ReadSingle();
            teleportOrigin = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy && State == TumblerState.Spawn && StateTimer < 430)
                return false;
            Texture2D texture = TextureAssets.Npc[Type].Value;
            bool phaseTwoArt = PhaseTwo || State == TumblerState.PhaseTransition;
            Rectangle frame = texture.Frame(1, 2, 0, phaseTwoArt ? 1 : 0);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 center = NPC.Center - screenPos;
            DrawNewAttackEffects(spriteBatch, screenPos, texture, frame, origin);
            DrawEntrance(spriteBatch, screenPos);
            float trailStrength = MathHelper.Clamp(NPC.velocity.Length() / 15f, 0f, 1f);
            if (State == TumblerState.MagnetClash && StateTimer < 75)
            {
                Texture2D rock = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/RockProjectile").Value;
                Rectangle rockFrame = rock.Frame(1, 3, 0, 0);
                int side = Target.Center.X >= NPC.Center.X ? 1 : -1;
                {
                    Vector2 destination = center + new Vector2(side * 70f, -40f);
                    Main.EntitySpriteDraw(rock, destination, rockFrame, TumblerVFX.Glow(PhaseColor, visualCharge * 0.45f), side * visualCharge, rockFrame.Size() * 0.5f, 48f / rockFrame.Width, SpriteEffects.None);
                    TumblerVFX.DrawCharge(spriteBatch, destination, PhaseColor, visualCharge, 27f, side * StateTimer * 0.025f);
                    TumblerVFX.DrawElectricLine(spriteBatch, center, destination, PhaseColor, visualCharge * 0.55f, 10, side);
                }
            }
            if (State == TumblerState.MagneticCrush)
            {
                foreach (Projectile platform in Main.ActiveProjectiles)
                {
                    if (TumblerMagneticPlatform.IsArenaPlatform(platform) && platform.ai[0] == NPC.whoAmI)
                        TumblerVFX.DrawElectricLine(spriteBatch, center, platform.Center - screenPos, PhaseColor, visualCharge * 0.45f, 22, platform.identity, 1.4f);
                }
            }
            if (State == TumblerState.Teleport && StateTimer < 70 && teleportDestination != Vector2.Zero)
            {
                Vector2 destination = teleportDestination - screenPos;
                TumblerVFX.DrawCorona(spriteBatch, destination, 63f, new Color(65, 225, 255), visualCharge * 0.8f, NPC.whoAmI);
                Main.EntitySpriteDraw(texture, destination, frame, TumblerVFX.Glow(PhaseColor, visualCharge * 0.22f), NPC.rotation, origin, NPC.scale, SpriteEffects.None);
                TumblerVFX.DrawTelegraph(spriteBatch, center, destination, PhaseColor, visualCharge * 0.25f, 70f);
            }
            for (int i = 0; i < afterimagePositions.Count; i++)
            {
                float progress = (i + 1f) / afterimagePositions.Count;
                Color trailColor = TumblerVFX.Glow(Color.Lerp(new Color(35, 115, 255), PhaseColor, progress), progress * progress * 0.28f * trailStrength);
                Main.EntitySpriteDraw(texture, afterimagePositions[i] - screenPos, frame, trailColor, afterimageRotations[i], origin, NPC.scale, SpriteEffects.None);
                if (i > 0 && trailStrength > 0.6f)
                {
                    Vector2 previous = afterimagePositions[i - 1] - screenPos;
                    Vector2 current = afterimagePositions[i] - screenPos;
                    Vector2 normal = (current - previous).SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * 35f;
                    TumblerVFX.DrawElectricLine(spriteBatch, previous + normal, current + normal, PhaseColor, progress * trailStrength * 0.5f, 5, i + NPC.whoAmI, 1.2f);
                    TumblerVFX.DrawElectricLine(spriteBatch, previous - normal, current - normal, new Color(70, 205, 255), progress * trailStrength * 0.3f, 5, i + 7f, 1f);
                }
            }
            float teleportOpacity = State == TumblerState.Teleport ? MathHelper.Clamp(Math.Abs(StateTimer - 70f) / 18f, 0f, 1f) : 1f;
            Main.EntitySpriteDraw(texture, center, frame, Color.Lerp(drawColor, Color.White, Math.Max(shieldFlash, impactFlash * 0.6f)) * teleportOpacity, NPC.rotation, origin, NPC.scale, SpriteEffects.None);
            if (State != TumblerState.Stunned)
            {
                Texture2D eye = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalTumblerEye").Value;
                Color eyeColor = phaseTwoArt ? new Color(255, 201, 84) : Color.White;
                Main.EntitySpriteDraw(eye, center, null, eyeColor * teleportOpacity, NPC.rotation, eye.Size() * 0.5f, NPC.scale, SpriteEffects.None);
            }
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/Glowmask").Value;
            Color glowColor = TumblerVFX.Glow(PhaseColor, 0.25f + visualCharge * 0.5f + impactFlash * 0.2f);
            Main.EntitySpriteDraw(glow, center, null, glowColor * teleportOpacity, NPC.rotation, glow.Size() * 0.5f, NPC.scale, SpriteEffects.None);
            if (State == TumblerState.PhaseTransition || auraScale > 0.05f)
                DrawAura(spriteBatch, screenPos);
            if (visualCharge > 0.05f)
            {
                TumblerVFX.DrawCorona(spriteBatch, center, 59f + visualCharge * 9f, PhaseColor, visualCharge * 0.8f, NPC.whoAmI);
                if (spinTarget.HasValue && State != TumblerState.LoopSlam && (State != TumblerState.ShockDash || !PhaseTwo && repetitions < 1))
                {
                    Vector2 start = new(NPC.Center.X, FloorY - NPC.height * 0.5f);
                    Vector2 end = new(storedDirection > 0 ? RightOuter - 60f : LeftOuter + 60f, start.Y);
                    TumblerVFX.DrawTelegraph(spriteBatch, start - screenPos, end - screenPos, PhaseColor, visualCharge * 0.55f, 62f);
                }
            }
            if (State == TumblerState.CrystalCharge && ArenaData.CrystalPositions.Length > 0)
            {
                Vector2 crystal = ArenaData.ClosestCrystal(NPC.Center) - screenPos;
                TumblerVFX.DrawElectricLine(spriteBatch, crystal, center, PhaseColor, visualCharge * 0.65f, 24, NPC.whoAmI);
                TumblerVFX.DrawCharge(spriteBatch, crystal, PhaseColor, visualCharge, 22f, StateTimer * 0.025f);
            }
            if (State == TumblerState.Overload && ArenaData.CrystalPositions != null)
            {
                for (int i = 0; i < ArenaData.CrystalPositions.Length; i++)
                {
                    Vector2 crystal = ArenaData.CrystalPositions[i] - screenPos;
                    TumblerVFX.DrawElectricLine(spriteBatch, crystal, center, PhaseColor, visualCharge * 0.7f, 24, i * 3f, 2f + visualCharge);
                    TumblerVFX.DrawCharge(spriteBatch, crystal, PhaseColor, visualCharge, 28f, StateTimer * 0.025f + i);
                }
                if (StateTimer < 180)
                    TumblerVFX.DrawCorona(spriteBatch, center, MathHelper.Lerp(165f, 86f, visualCharge), PhaseColor, visualCharge * 0.65f, 5f);
            }
            return false;
        }

        private void DrawAura(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Vector2 center = NPC.Center - screenPos;
            if (State == TumblerState.PhaseTransition)
            {
                Texture2D shard = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/GroundSpike").Value;
                float rotation = StateTimer * 0.008f;
                for (int i = 0; i < 10; i++)
                {
                    float angle = rotation + i * MathHelper.TwoPi / 10f;
                    Vector2 direction = angle.ToRotationVector2();
                    bool intact = i < shieldHits;
                    Color color = intact ? Color.Lerp(new Color(47, 105, 158), Color.White, shieldFlash) : TumblerVFX.Glow(PhaseColor, 0.15f);
                    Vector2 position = center + direction * 69f;
                    Main.EntitySpriteDraw(shard, position, null, color, angle + MathHelper.PiOver2, new Vector2(shard.Width * 0.5f, shard.Height * 0.75f), new Vector2(0.85f, 0.46f), SpriteEffects.None);
                    if (intact)
                        TumblerVFX.DrawElectricLine(spriteBatch, position, center + (angle + MathHelper.TwoPi / 10f).ToRotationVector2() * 69f, PhaseColor, 0.65f + shieldFlash * 0.3f, 5, i);
                }
                TumblerVFX.DrawCorona(spriteBatch, center, 76f, Color.Lerp(PhaseColor, Color.White, shieldFlash), 0.75f, NPC.whoAmI);
                return;
            }
            Texture2D aura = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/whiteFireEye").Value;
            float pulse = 1f + MathF.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.04f;
            float scale = 170f * auraScale / aura.Width;
            Main.EntitySpriteDraw(aura, center, null, TumblerVFX.Glow(PhaseColor, 0.2f + shieldFlash * 0.45f), -NPC.rotation * 0.25f, aura.Size() * 0.5f, scale * pulse, SpriteEffects.None);
            TumblerVFX.DrawCorona(spriteBatch, center, 82f * auraScale, PhaseColor, 0.4f * auraScale, NPC.whoAmI);
        }
    }
}
