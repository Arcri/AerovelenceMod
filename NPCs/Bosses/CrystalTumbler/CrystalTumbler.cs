using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AerovelenceMod.Dusts;
using AerovelenceMod.Items.BossBags;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
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
			Jump = 5,
			Teleport
		}

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 3300;  //boss life scale in expertmode
            npc.damage = 20;  //boss damage increase in expermode
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
		public bool Teleport = false;
		int i;
		int t;
		public int counter = 0;

		public override void SetDefaults()
		{
			npc.width = 120;
			npc.height = 128;
			npc.value = Item.buyPrice(0, 5, 60, 45);

			npc.damage = 12;
			npc.defense = 8;
			npc.lifeMax = 2200;
			npc.knockBackResist = 0f;
			npc.alpha = 0;

			npc.boss = true;
			npc.lavaImmune = true;
			npc.noGravity = false;
			npc.noTileCollide = false;

			bossBag = ModContent.ItemType<CrystalTumblerBag>();

			npc.HitSound = SoundID.NPCHit41;
			npc.DeathSound = SoundID.NPCDeath44;
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalTumbler");
		}
		public override bool PreAI()
		{
			npc.TargetClosest(true);
			Player target = Main.player[npc.target];

			Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);

			if (Main.dayTime || target.dead)
			{
				npc.velocity.Y -= 0.09f;
				npc.timeLeft = 300;
				if (npc.position.Y <= 16 * 35) //checking for top of the world practically
				{
					npc.active = false;
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
				npc.Opacity *= 1.1f;
				if (++AttackTimer >= 200)
				{
					AttackTimer = 0;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if ((target.Center.Y < npc.Center.Y - 50 || target.Center.Y > npc.Center.Y + 50) && Main.rand.NextBool(2))
						{
							if (target.Center.Y > npc.Center.Y + 50 || Main.rand.NextBool(2))
							{
								State = CrystalTumblerState.Teleport;
							}
							else
							{
								State = CrystalTumblerState.Jump;
							}
						}
						else if (Math.Abs(target.Center.Y - npc.Center.Y) <= 50 && Main.rand.NextBool(2))
						{
							State = CrystalTumblerState.SuperDash;
						}
						else
						{
							int randomState = Main.rand.Next(3);
							if (randomState == 0)
							{
								State = CrystalTumblerState.ProjectileSpawn;
							}
							else if (randomState == 1)
							{
								State = CrystalTumblerState.Electricity;
							}
							else
							{
								State = CrystalTumblerState.RockRain;
							}
						}
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
					AttackTimer = 0;
					State = CrystalTumblerState.IdleRoll;
				}
				RollingMove(target);
			}
			// Attack state. Starts speedy roll in place. After a set amount of ticks, releases/dashes with a high velocity and frequent jumps/bounces.
			else if (State == CrystalTumblerState.Jump)
			{
				if (npc.velocity.Y == 0)
				{
					npc.velocity.Y -= Main.rand.Next(12) + 7;
					for (int num325 = 0; num325 < 20; num325++)
					{
						Dust.NewDust(npc.position, npc.width, npc.height, DustID.Electric, npc.velocity.X, npc.velocity.Y, 0, default, 1);
					}
				}
				State = CrystalTumblerState.IdleRoll;
			}
			else if (State == CrystalTumblerState.SuperDash)
			{
				t++;
				if (t % 100 == 0)
				{
					if (npc.velocity.Y == 0 && npc.velocity.X != 0 && Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<CrystalCaverns.Tumblerock1>());
					}
				}
				CheckPlatform(target);
				// Rotating in place state.
				if (++AttackTimer <= 180)
				{
					npc.velocity.X *= 0.95f;
					npc.rotation += AttackTimer / 180 * npc.direction;

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
				npc.velocity = Vector2.Clamp(npc.velocity, Vector2.One * -16, Vector2.One * 16);
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
					int damage = Main.expertMode ? 20 : 10;// if u want to change this, 15 is for expert mode, 10 is for normal mod
					int type = mod.ProjectileType("TumblerOrb");
					float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
					Projectile.NewProjectile(vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1), (float)(Math.Sin(rotation) * Speed * -1), type, damage, 0f, 0);
				}
				AttackTimer = 0;
				State = CrystalTumblerState.IdleRoll;
				RollingMove(target);
			}
			else if (State == CrystalTumblerState.Teleport)
			{
				AttackTimer++;
				if(AttackTimer >= 100)
                {
					npc.Opacity *= 0.97f;
				}
				if (AttackTimer % 100 == 0)
				{
					Projectile.NewProjectile(target.position, default, ModContent.ProjectileType<TeleportCharge>(), 12, 1f, Main.myPlayer, 1);
				}
				if (AttackTimer >= 200)
				{
					npc.position.Y = target.position.Y - 370;
					npc.position.X = target.position.X;
					AttackTimer = 0;
					State = CrystalTumblerState.IdleRoll;
				}
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
			return false;
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
				Color.White * npc.Opacity,
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
}