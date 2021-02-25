using AerovelenceMod.Dusts;
using AerovelenceMod.Items.BossBags;
using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
<<<<<<< Updated upstream
    [AutoloadBossHead]
    public class CrystalTumbler : ModNPC
    {
        float LifePercentLeft;
        int t;
        int i;
        public bool P;
        public int spinTimer;
        public bool Phase2;
        bool SuperDash = false;
        bool ThrownRock1 = false;
        public int counter = 0;

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.lifeMax = 1600;
            npc.damage = 12;
            npc.defense = 8;
            npc.knockBackResist = 0f;
            npc.width = 120;
            npc.height = 128;
            npc.value = Item.buyPrice(0, 5, 60, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            bossBag = ModContent.ItemType<CrystalTumblerBag>();
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalTumbler");
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }
=======
	[AutoloadBossHead]
	public class CrystalTumbler : ModNPC
	{
		// AI state management of the Crystal Tumbler.
		private enum CrystalTumblerState
		{
			IdleRoll = 0,
			ProjectileSpawn = 1,
			SuperDash = 2,
			Electricity = 3,
			RockRain = 4,
			Jump = 5
		}

		/// <summary>
		/// Manages the current AI state of the Crystal Tumbler.
		/// Gets and sets npc.ai[0] as tracker.
		/// </summary>
		private CrystalTumblerState State
		{
			get => (CrystalTumblerState)npc.ai[0];
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

		/// <summary>
		/// Returns a value between 0.0 - 1.0 based on current life.
		/// </summary>
		float LifePercentLeft => (npc.life / (float)npc.lifeMax);

		public bool P;
		public int spinTimer;
		public bool Phase2;
		int i;
		public int counter = 0;

		public override void SetDefaults()
		{
			npc.width = 120;
			npc.height = 128;
			npc.value = Item.buyPrice(0, 5, 60, 45);

			npc.damage = 12;
			npc.defense = 8;
			npc.lifeMax = 1600;
			npc.knockBackResist = 0f;

			npc.boss = true;
			npc.lavaImmune = true;
			npc.noGravity = false;
			npc.noTileCollide = false;

			bossBag = ModContent.ItemType<CrystalTumblerBag>();

			npc.HitSound = SoundID.NPCHit41;
			npc.DeathSound = SoundID.NPCDeath44;
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalTumbler");
		}
>>>>>>> Stashed changes

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = 3200;  //boss life scale in expertmode
			npc.damage = 20;  //boss damage increase in expermode
		}

<<<<<<< Updated upstream
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/CrystalTumbler/Glowmask");
            Vector2 drawPos = npc.Center + new Vector2(0, npc.gfxOffY) - Main.screenPosition;
            spriteBatch.Draw
            (
                texture,
                drawPos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                npc.rotation,
                texture.Size() * 0.5f,
                npc.scale,
                npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
                );
        }

        public override void NPCLoot()
        {
            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            if (!Main.expertMode)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.LesserHealingPotion, Main.rand.Next(4, 12), false, 0, false, false);
                switch (Main.rand.Next(5))
                {
                    case 0:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CavernMauler"), 1, false, 0, false, false);
                        break;
                    case 1:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CavernousImpaler"), 1, false, 0, false, false);
                        break;
                    case 2:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrystallineQuadshot"), 1, false, 0, false, false);
                        break;
                    case 3:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PrismThrasher"), 1, false, 0, false, false);
                        break;
                    case 4:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PrismPiercer"), 1, false, 0, false, false);
                        break;
                    case 5:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DiamondDuster"), 1, false, 0, false, false);
                        break;
                }
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White, 1);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore5"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore6"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore7"), 1f);
            }
        }

        public override void AI()
        {
            t++;

            var player = Main.player[npc.target];
            Vector2 delta = player.Center - npc.Center;
            float magnitude = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            Vector2 offset = new Vector2(0, -100);

            Vector2 offsetLightning = new Vector2(0, -500);
            Vector2 LightningTarget = player.Center;
            Vector2 move = player.position - npc.Center;
            LifePercentLeft = -(npc.life / npc.lifeMax) + 1f;
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            if (player.active || !player.dead)
            {
                if (Vector2.Distance(npc.Center, player.Center) <= 500)
                {
                    npc.noTileCollide = npc.noGravity = false;
                    if (npc.life <= npc.lifeMax / 2)
                    {
                        Phase2 = true;
                    }
                    npc.TargetClosest(true);
=======
		public override bool PreAI()
		{
			npc.TargetClosest(true);
			Player target = Main.player[npc.target];

			Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);

			if (!target.active || target.dead)
			{
				npc.noTileCollide = true;
				npc.TargetClosest(false);
				npc.velocity.Y = 20f;
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
				}
			}


			npc.noTileCollide = npc.noGravity = false;

			// No second phase implemented yet.
			/*if (npc.life <= npc.lifeMax / 2)
			{
				AttackTimer = 0;
				return;
			}*/

			// Idle roll/follow state. Very basic movement, with a timer for a random attack state.
			if (State == CrystalTumblerState.IdleRoll)
			{
				RollingMove(target);

				if (++AttackTimer >= 200)
				{
					AttackTimer = 0;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						State = (CrystalTumblerState)Main.rand.Next(1, 6);
					}

					npc.netUpdate = true;
				}
			}
			// Attack state. Spawns three projectiles on the NPC, which hover for a (few) second(s), before shooting towards the target.
			else if (State == CrystalTumblerState.ProjectileSpawn)
			{
				// Spawn a projectile every 10 ticks (and on the first tick this state is active).
				// TODO: Eldrazi - Multiplayer support?
				if (AttackTimer++ == 0)
				{
					Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 69, 0.75f);
					Projectile.NewProjectile(npc.Center, default, ModContent.ProjectileType<TumblerBoulder1>(), 12, 1f, Main.myPlayer, 0);
				}
				else if (AttackTimer == 10)
				{
					Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 69, 0.75f);
					Projectile.NewProjectile(npc.Center, default, ModContent.ProjectileType<TumblerBoulder1>(), 12, 1f, Main.myPlayer, 1);
				}
				else if (AttackTimer == 20)
				{
					Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 69, 0.75f);
					Projectile.NewProjectile(npc.Center, default, ModContent.ProjectileType<TumblerBoulder1>(), 12, 1f, Main.myPlayer, 2);
>>>>>>> Stashed changes

                    if (player.Center.X > npc.Center.X)
                    {
                        if (npc.velocity.X < 2 * (LifePercentLeft + 1))
                        {
                            npc.velocity.X += (0.075f * (LifePercentLeft + 1));
                        }
                    }
                    else if (player.Center.X < npc.Center.X)
                    {
                        if (npc.velocity.X > -2 * (LifePercentLeft + 1))
                        {
                            npc.velocity.X -= (0.075f * (LifePercentLeft + 1));

<<<<<<< Updated upstream
                        }
                    }

                    npc.rotation += npc.velocity.X * 0.025f;
                    if (t % 250 == 0)
                    {

                        npc.velocity.Y -= Main.rand.Next(12) + 7;
                        for (int num325 = 0; num325 < 20; num325++)
                            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Electric, npc.velocity.X, npc.velocity.Y, 0, default, 1);
                    }
                    if (t % 250 == 0)
                    {
                        ThrownRock1 = true;
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
                        counter++;
                        if (counter >= 10)
                        {
                            Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
                        }
                        counter++;
                        if (counter >= 20)
                        {
                            Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
                        }
                    }
                    if (ThrownRock1 == true)
                    {

                    }


                    /*
                    if (t % Main.rand.Next(130, 150) == 10)
                    {
                        Vector2 offset = new Vector2(0, -100);
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard2>(), 5, 1f, Main.myPlayer);
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(-2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
                        npc.netUpdate = true;
                    }
                    if (npc.life < 800 && npc.life > 201)
                        if (t % 70 == 0)
                        {
                            Vector2 offset = new Vector2(0, -100);
                            Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
                            npc.netUpdate = true;
                        }*/

                    if ((npc.Center.Y - player.Center.Y) < -150)
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }


                }
                if (i % 100 == 0)
                {
                    SuperDash = true;
                }
=======
				RollingMove(target);
			}
			// Attack state. Starts speedy roll in place. After a set amount of ticks, releases/dashes with a high velocity and frequent jumps/bounces.
			else if (State == CrystalTumblerState.Jump)
			{

				npc.velocity.Y -= Main.rand.Next(12) + 7;
				for (int num325 = 0; num325 < 20; num325++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, DustID.Electric, npc.velocity.X, npc.velocity.Y, 0, default, 1);
				}
				State = CrystalTumblerState.IdleRoll;
			}
			else if (State == CrystalTumblerState.SuperDash)
			{
				CheckPlatform(target);
				// Rotating in place state.
				if (++AttackTimer <= 180)
				{
					npc.velocity.X *= 0.95f;
					npc.rotation += (AttackTimer / 180) * npc.direction;

					// If the timer is at 180 (or 3 seconds), set the velocity towards the NPCs direction.
					if (AttackTimer == 180)
					{
						npc.netUpdate = true;
						npc.velocity.X = npc.direction * 16f;
					}
				}
				// Fast horizontal movement and frequent jumps.
				else
				{
					Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 76, 0.75f);
					TryJump(Main.rand.Next(4, 7), 60);
					JumpTimer--;
					var player = Main.player[npc.target];
					if (player.Center.X > npc.Center.X)
					{
						npc.velocity.X += 0.3f;
					}
					else if (player.Center.X < npc.Center.X)
					{
						npc.velocity.X -= 0.3f;
					}
				}


				// After 480 ticks (300 ticks or 5 seconds after the 'Rotating in place' state), go back to idle rolling.
				if (AttackTimer >= 480)
				{
					npc.netUpdate = true;

					AttackTimer = 0;
					State = CrystalTumblerState.IdleRoll;
				}

				npc.rotation += npc.velocity.X * 0.025f;
			}

			else if (State == CrystalTumblerState.Electricity)
			{
				var player = Main.player[npc.target];
				Vector2 vector8 = new Vector2(npc.position.X + (npc.width * 0.5f), npc.position.Y + (npc.height * 0.5f));
				if (AttackTimer++ == 0)
				{
					Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 93, 0.75f);
					float Speed = 1 * 0.99f;
					int damage = Main.expertMode ? 50 : 40;// if u want to change this, 15 is for expert mode, 10 is for normal mod
					int type = mod.ProjectileType("TumblerOrb");
					float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
					Projectile.NewProjectile(vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1), (float)(Math.Sin(rotation) * Speed * -1), type, damage, 0f, 0);
				}
				AttackTimer = 0;
				State = CrystalTumblerState.IdleRoll;
				RollingMove(target);
			}
			else if (State == CrystalTumblerState.RockRain)
			{
				Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 74, 0.75f);
				var player = Main.player[npc.target];
				for (int i = -3; i <= 3; i++)
				{
					Projectile.NewProjectile(player.Center.X + i * 20, player.Center.Y - 380, i * 2, -2, ModContent.ProjectileType<TumblerSpike1>(), 30, 0f, Main.myPlayer, 0f, 0f);
					Projectile.NewProjectile(player.Center.X + i * 20, player.Center.Y - 380, i * 2, -2, ModContent.ProjectileType<TumblerSpike2>(), 30, 0f, Main.myPlayer, 0f, 0f);
				}
				AttackTimer = 0;
				State = CrystalTumblerState.Jump;
				RollingMove(target);
			}


			/*if ((npc.Center.Y - player.Center.Y) < -150)
			{
				i++;
			}
			else
			{
				i = 0;
			}
>>>>>>> Stashed changes

                if (SuperDash == true)
                {
                    counter++;
                    if (counter >= 60)
                    {
                        if (player.Center.X > npc.Center.X)
                        {
                            npc.velocity.X += -1.1f;
                        }
                        else if (player.Center.X < npc.Center.X)
                        { 
                            npc.velocity.X += 1.1f;
                        }
                        counter = 0;
                    }
                    else
                    {
                        SuperDash = false;
                    }
                }
                /*if (i % 50 == 0)
                {
                    npc.noTileCollide = true;
                }
                if (i % 50 == 10 || i == 0)
                {
                    npc.noTileCollide = false;
                */
                if (Phase2)
                {
                    if (t % 250 == 0)
                    {
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerHomingShard>(), 12, 1f, Main.myPlayer);
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 6, 1f, Main.myPlayer);
                        npc.netUpdate = true;
                    }
                    if (t % 250 == 0)
                        //  {
                        //     Projectile.NewProjectile(npc.Center.X, npc.Center.Y, delta.X, delta.Y, ModContent.ProjectileType<TumblerShockBlast>(), 10, 3f, Main.myPlayer, BuffID.OnFire, 600f);
                        //    npc.netUpdate = true;
                        // }
                        if (t % 50 == 0)
                        {

                        }
                    if (Main.expertMode)
                    {
                        if (t % 250 == 0)
                        {
                            Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerHomingShard>(), 12, 1f, Main.myPlayer);
                            Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 6, 1f, Main.myPlayer);
                            npc.netUpdate = true;
                        }
                        /*if (t % 275 == 0)
                        {
                            if (player.position.X > npc.position.X)
                            {
                                npc.velocity.X = ((10 * npc.velocity.X + move.X + 10) / 5f);
                            }
                            else if (player.position.X < npc.position.X)
                            {
                                npc.velocity.X = ((10 * npc.velocity.X + move.X - 10) / 5f);
                            }*/
                    }
                }
                /*if (t % 50 == 0)
                {
                    Projectile.NewProjectile(player.Center - new Vector2(0, 16f * 5f), new Vector2(0f, 0f), ProjectileID.CultistBossLightningOrbArc, 10, 3f, Main.myPlayer, BuffID.OnFire, -0f);
                }*/
            }
            /*if (Vector2.Distance(npc.Center, player.Center) >= 500)
            {
                cheeseCheck++;
                npc.rotation += npc.velocity.X * 0.025f;
                if (cheeseCheck >= 180)
                {
                    Time++;
                    // Rise for a bit.
                    if (Time < FlyUpwardTime)
                    {
                        npc.noTileCollide = npc.noGravity = true;
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.UnitY * -12, 0.2f); // Quickly rise upward (but don't stop all movement and rise immediately)
                    }
                    // Redirect towards the target before spinning.
                    if (Time == FlyUpwardTime)
                    {
                        npc.velocity = npc.DirectionTo(player.Center) * rotationalSpeed;
                    }
                    // And spin around.
                    if (Time > FlyUpwardTime && Time < RotationTime + FlyUpwardTime)
                    {
                        npc.velocity = npc.velocity.RotatedBy(MathHelper.TwoPi / RotationTime * TotalRotations);
                    }
                    // After time is >= RotationTime + FlyUpwardTime, do nothing for a bit and let the lunge happen. Maybe decelerate at the end. Be sure to change npc.noTileCollide and npc.noGravity back to normal.
                    if (t % Main.rand.Next(190, 215) == 10)
                    {
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerSpike1>(), 6, 1f, Main.myPlayer);
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerSpike2>(), 8, 1f, Main.myPlayer);
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(-2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerSpike1>(), 6, 1f, Main.myPlayer);
                        npc.netUpdate = true;
                        Time = 0;
                        cheeseCheck = 0;
                    }
                }
            }*/
            if (!player.active || player.dead)
            {
                npc.noTileCollide = true;
                npc.TargetClosest(false);
                npc.velocity.Y = 20f;
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
            }
<<<<<<< Updated upstream
        }
    }
=======

			if (State == CrystalTumblerState.Phase2)
			{
				if (AttackTimer % 250 == 0)
				{
					Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerHomingShard>(), 12, 1f, Main.myPlayer);
					Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 6, 1f, Main.myPlayer);
					npc.netUpdate = true;
				}
				if (AttackTimer % 250 == 0)
					//  {
					//     Projectile.NewProjectile(npc.Center.X, npc.Center.Y, delta.X, delta.Y, ModContent.ProjectileType<TumblerShockBlast>(), 10, 3f, Main.myPlayer, BuffID.OnFire, 600f);
					//    npc.netUpdate = true;
					// }
					if (AttackTimer % 50 == 0)
					{

					}
				if (Main.expertMode)
				{
					if (AttackTimer % 250 == 0)
					{
						Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerHomingShard>(), 12, 1f, Main.myPlayer);
						Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 6, 1f, Main.myPlayer);
						npc.netUpdate = true;
					}
					/*if (t % 275 == 0)
					{
						if (player.position.X > npc.position.X)
						{
							npc.velocity.X = ((10 * npc.velocity.X + move.X + 10) / 5f);
						}
						else if (player.position.X < npc.position.X)
						{
							npc.velocity.X = ((10 * npc.velocity.X + move.X - 10) / 5f);
						}
				}
			}*/

			return (false);
		}
		private void CheckPlatform(Player player)
		{
			i++;
			bool onplatform = true;

			for (int i = (int)npc.position.X; i < npc.position.X + npc.width; i += npc.width / 4)
			{
				Tile tile = Framing.GetTileSafely(new Point((int)npc.position.X / 16, (int)(npc.position.Y + npc.height + 8) / 16));
				if (!TileID.Sets.Platforms[tile.type])
				{
					onplatform = false;
				}
			}
			if (onplatform && (npc.position.Y + npc.height + 20 < player.Center.Y))
			{
				if (i % 70 == 0)
				{
					npc.noTileCollide = true;
				}
				else
				{
					npc.noTileCollide = false;
				}
			}
		}
		public override void NPCLoot()
		{
			if (Main.expertMode)
			{
				npc.DropBossBags();
			}
			if (!Main.expertMode)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.LesserHealingPotion, Main.rand.Next(4, 12), false, 0, false, false);
				switch (Main.rand.Next(5))
				{
					case 0:
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CavernMauler"), 1, false, 0, false, false);
						break;
					case 1:
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CavernousImpaler"), 1, false, 0, false, false);
						break;
					case 2:
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrystallineQuadshot"), 1, false, 0, false, false);
						break;
					case 3:
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PrismThrasher"), 1, false, 0, false, false);
						break;
					case 4:
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PrismPiercer"), 1, false, 0, false, false);
						break;
					case 5:
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DiamondDuster"), 1, false, 0, false, false);
						break;
				}
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White, 1);
				}
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore4"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore5"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore6"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore7"), 1f);
			}
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Texture2D texture = mod.GetTexture("NPCs/Bosses/CrystalTumbler/Glowmask");
			Vector2 drawPos = npc.Center + new Vector2(0, npc.gfxOffY) - Main.screenPosition;
			spriteBatch.Draw
			(
				texture,
				drawPos,
				new Rectangle(0, 0, texture.Width, texture.Height),
				Color.White,
				npc.rotation,
				texture.Size() * 0.5f,
				npc.scale,
				npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				0f
				);
		}

		#region AI Methods

		/// <summary>
		/// Very basic movement state.
		/// Roll speed depends on current NPC life (from 2 to 14 at 0 life).
		/// </summary>
		/// <param name="player"></param>
		private void RollingMove(Player player)
		{
			// Movement.
			float desiredSpeed = 2 + 12 * (1 - LifePercentLeft);
			CheckPlatform(player);
			if (player.Center.X > npc.Center.X)
			{
				if (npc.velocity.X < desiredSpeed)
				{
					npc.velocity.X += (0.2f * (1 - LifePercentLeft + 1));
				}
			}
			else if (player.Center.X < npc.Center.X)
			{
				if (npc.velocity.X > -desiredSpeed)
				{
					npc.velocity.X -= (0.2f * (1 - LifePercentLeft + 1));
				}
			}

			// Rotation.
			npc.rotation += npc.velocity.X * 0.025f;

			// Jump.
			if (npc.velocity.Y == 0 &&
				player.Center.Y < npc.Center.Y - 100f)
			{
				TryJump(Main.rand.Next(12) + 7, 180);
			}
			else if (JumpTimer > 0)
			{
				JumpTimer--;
			}
		}

		/// <summary>
		/// Attempts to jump if the JumpTimer is past its timeout.
		/// </summary>
		/// <param name="height"></param>
		/// <param name="cooldown"></param>
		private void TryJump(float height, int cooldown)
		{
			if (JumpTimer > 0)
			{
				return;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.netUpdate = true;
				npc.velocity.Y -= height;
			}
			for (int num325 = 0; num325 < 20; num325++)
				Dust.NewDust(npc.position, npc.width, npc.height, DustID.Electric, npc.velocity.X, npc.velocity.Y, 0, default, 1);

			JumpTimer = cooldown;
		}

		#endregion
	}
>>>>>>> Stashed changes
}