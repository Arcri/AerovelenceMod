using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Dusts;
using AerovelenceMod.Projectiles.NPCs.CrystalCaverns;

namespace AerovelenceMod.NPCs.Bosses.TheFallen
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
            get => (TheFallenState)npc.ai[0];
            set => npc.ai[0] = (float)value;
        }

        /// <summary>
        /// Manages several AI state attack timers.
        /// Gets and sets npc.ai[1] as tracker.
        /// </summary>
        private float AttackTimer
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        /// <summary>
        /// Boss-specific jump timer manager, to disallow frequent jumping.
        /// Gets and sets npc.ai[2] as tracker.
        /// </summary>
        private float JumpTimer
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.lifeMax = 1600;
            npc.damage = 12;
            npc.defense = 8;
            npc.knockBackResist = 0f;
            npc.width = 94;
            npc.height = 82;
            npc.value = Item.buyPrice(0, 5, 60, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalTumbler");
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 3200;  //boss life scale in expertmode
            npc.damage = 20;  //boss damage increase in expermode
        }


        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White, 1);
                }
                NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, ModContent.NPCType<TheFallenSpirit>());
            }
        }

        public float rotationIncrease;

        public override void AI()
        {
            npc.TargetClosest(true);
            Player target = Main.player[npc.target];
            Vector2 distanceNorm = target.position - npc.position;
            distanceNorm.Normalize();
            if (target.dead)
            {
                npc.velocity.Y += 0.09f;
                npc.timeLeft = 300;
                npc.noTileCollide = true;
            }
            // Idle flying/follow state. Very basic movement, with a timer for a random attack state.
            //if (State != CrystalTumblerState.IdleRoll) { Main.NewText(State); }
            t++;
            if (State == TheFallenState.IdleFly)
            {
                Main.NewText("Idle Fly");
                Move(target);
                npc.noTileCollide = true;
                if (++AttackTimer >= 200)
                {
                    AttackTimer = 0;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = (TheFallenState)Main.rand.Next(1, 4);
                    }
                    npc.netUpdate = true;
                }
            }
            else if (State == TheFallenState.GroundPound)
            {
                if (++AttackTimer <= 100)
                {
                    Main.NewText("Ground Pound");
                    npc.noTileCollide = false;
                    npc.noGravity = true;
                    npc.velocity.Y += 2f;
                    npc.velocity.X = 0;
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
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<PinkSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                }
                else if (AttackTimer == 5)
                {
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<PinkSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                }
                else if (AttackTimer == 10)
                {
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<BlueSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                }
                else if (AttackTimer == 13)
                {
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<PinkSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                }
                else if (AttackTimer == 15)
                {
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<BlueSpiritBlast>(), 30, 0f, Main.myPlayer, 0f, 0f);
                }
                else if (AttackTimer == 17)
                {
                    AttackTimer = 0;
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<BlueSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                    State = TheFallenState.IdleFly;
                }

            }

            else if (State == TheFallenState.GhastlyRotation)
            {
                // neither of these states are going to conflict, so return when you're done
                if (++AttackTimer <= 180)
                {
                    rotationIncrease += 2f;
                    npc.position = Vector2.Lerp(npc.position, target.position + new Vector2(400, 0).RotatedBy(MathHelper.ToRadians(rotationIncrease)), 0.05f);
                    npc.ai[1]++;
                    if (npc.ai[1] >= 30)
                    {
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<BlueSpiritBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);
                        npc.ai[1] = 0;
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
            if (player.Center.X > npc.Center.X)
            {
                if (npc.velocity.X < 6)
                {
                    npc.velocity.X += 0.15f;
                }
            }
            if (player.Center.X < npc.Center.X)
            {
                if (npc.velocity.X > -6)
                {
                    npc.velocity.X -= 0.15f;
                }
            }

            if (!(t % 400 >= 0 && t % 500 <= 50))
            {
                if (player.Center.Y > npc.Center.Y + 250)
                {
                    if (npc.velocity.Y < 4)
                    {
                        npc.velocity.Y += 0.2f;
                    }
                }
            }
            else
            {
                if (player.Center.Y > npc.Center.Y)
                {
                    if (npc.velocity.Y < 4)
                    {
                        npc.velocity.Y += 0.2f;
                    }
                }
            }
            if (!(t % 400 >= 0 && t % 500 <= 50))
            {
                if (player.Center.Y < npc.Center.Y + 250)
                {
                    if (npc.velocity.Y > -4)
                    {
                        npc.velocity.Y -= 0.2f;
                    }
                }
            }
            else
            {
                if (player.Center.Y < npc.Center.Y)
                {
                    if (npc.velocity.Y > -4)
                    {
                        npc.velocity.Y -= 0.2f;
                    }
                }
            }
            if (t % 300 == 0)
            {
                npc.velocity.Y -= 0.2f;
                npc.velocity.X -= 0.2f;
            }
            npc.rotation = npc.velocity.X * 0.1f;
        }
    }
}