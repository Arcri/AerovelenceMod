using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.TheFallen
{
    [AutoloadBossHead]
    public class TheFallen : ModNPC
    {
        int t;
        private enum TheFallenState
        {
            IdleFly = 0,
            GroundPound = 1,
            GhastlyBlasts = 2,
            GhastlyRotation = 3
        }

        /// <summary>
        /// Manages the current AI state of the Crystal Tumbler.
        /// Gets and sets npc.ai[0] as tracker.
        /// </summary>
        private TheFallenState State
        {
            get => (TheFallenState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        /// <summary>
        /// Manages several AI state attack timers.
        /// Gets and sets npc.ai[1] as tracker.
        /// </summary>
        private float AttackTimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        /// <summary>
        /// Boss-specific jump timer manager, to disallow frequent jumping.
        /// Gets and sets npc.ai[2] as tracker.
        /// </summary>
        private float JumpTimer
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1600;
            NPC.damage = 12;
            NPC.defense = 8;
            NPC.knockBackResist = 0f;
            NPC.width = 94;
            NPC.height = 82;
            NPC.value = Item.buyPrice(0, 5, 60, 45);
            NPC.npcSlots = 1f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            //music = Mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalTumbler");
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 3200;  //boss life scale in expertmode
            NPC.damage = 20;  //boss damage increase in expermode
        }


        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>(), NPC.velocity.X, NPC.velocity.Y, 0, Color.White, 1);
                }
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<TheFallenSpirit>());
            }
        }

        public float rotationIncrease;

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player target = Main.player[NPC.target];
            Vector2 distanceNorm = target.position - NPC.position;
            distanceNorm.Normalize();
            if (target.dead)
            {
                NPC.velocity.Y += 0.09f;
                NPC.timeLeft = 300;
                NPC.noTileCollide = true;
            }
            // Idle flying/follow state. Very basic movement, with a timer for a random attack state.
            //if (State != CrystalTumblerState.IdleRoll) { Main.NewText(State); }
            t++;
            if (State == TheFallenState.IdleFly)
            {
                Main.NewText("Idle Fly");
                Move(target);
                NPC.noTileCollide = true;
                if (++AttackTimer >= 200)
                {
                    AttackTimer = 0;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = (TheFallenState)Main.rand.Next(1, 4);
                    }
                    NPC.netUpdate = true;
                }
            }
            else if (State == TheFallenState.GroundPound)
            {
                if (++AttackTimer <= 100)
                {
                    Main.NewText("Ground Pound");
                    NPC.noTileCollide = false;
                    NPC.noGravity = true;
                    NPC.velocity.Y += 2f;
                    NPC.velocity.X = 0;
                }
                if (AttackTimer >= 250)
                {
                    AttackTimer = 0;
                    State = TheFallenState.IdleFly;
                }
            }


            else if (State == TheFallenState.GhastlyBlasts)
            {
                Main.NewText("Ghastly Blasts");
                if (AttackTimer++ == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<PinkSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                }
                else if (AttackTimer == 5)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<PinkSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                }
                else if (AttackTimer == 10)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<BlueSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                }
                else if (AttackTimer == 13)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<PinkSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                }
                else if (AttackTimer == 15)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<BlueSpiritBlast>(), 30, 0f, Main.myPlayer, 0f, 0f);
                }
                else if (AttackTimer == 17)
                {
                    AttackTimer = 0;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<BlueSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                    State = TheFallenState.IdleFly;
                }

            }

            else if (State == TheFallenState.GhastlyRotation)
            {
                // neither of these states are going to conflict, so return when you're done
                if (++AttackTimer <= 180)
                {
                    rotationIncrease += 2f;
                    NPC.position = Vector2.Lerp(NPC.position, target.position + new Vector2(400, 0).RotatedBy(MathHelper.ToRadians(rotationIncrease)), 0.05f);
                    NPC.ai[1]++;
                    if (NPC.ai[1] >= 30)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<BlueSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                        NPC.ai[1] = 0;
                    }
                    return;
                }
                if (AttackTimer == 280)
                {
                    AttackTimer = 0;
                    State = TheFallenState.IdleFly;
                    Move(target);
                    return;
                }
            }
        }
        
        private void Move(Player player)
        {
            if (player.Center.X > NPC.Center.X)
            {
                if (NPC.velocity.X < 6)
                {
                    NPC.velocity.X += 0.15f;
                }
            }
            if (player.Center.X < NPC.Center.X)
            {
                if (NPC.velocity.X > -6)
                {
                    NPC.velocity.X -= 0.15f;
                }
            }

            if (!(t % 400 >= 0 && t % 500 <= 50))
            {
                if (player.Center.Y > NPC.Center.Y + 250)
                {
                    if (NPC.velocity.Y < 4)
                    {
                        NPC.velocity.Y += 0.2f;
                    }
                }
            }
            else
            {
                if (player.Center.Y > NPC.Center.Y)
                {
                    if (NPC.velocity.Y < 4)
                    {
                        NPC.velocity.Y += 0.2f;
                    }
                }
            }
            if (!(t % 400 >= 0 && t % 500 <= 50))
            {
                if (player.Center.Y < NPC.Center.Y + 250)
                {
                    if (NPC.velocity.Y > -4)
                    {
                        NPC.velocity.Y -= 0.2f;
                    }
                }
            }
            else
            {
                if (player.Center.Y < NPC.Center.Y)
                {
                    if (NPC.velocity.Y > -4)
                    {
                        NPC.velocity.Y -= 0.2f;
                    }
                }
            }
            if (t % 300 == 0)
            {
                NPC.velocity.Y -= 0.2f;
                NPC.velocity.X -= 0.2f;
            }
            NPC.rotation = NPC.velocity.X * 0.1f;
        }
    }
}