using System;
using AerovelenceMod.Common.Globals.Worlds;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.TreasureBags;
using AerovelenceMod.Content.Items.Weapons.Magic;
using AerovelenceMod.Content.Items.Weapons.Melee;
using AerovelenceMod.Content.Items.Weapons.Ranged;
using AerovelenceMod.Content.Items.Weapons.Summoning;
using AerovelenceMod.Content.Items.Weapons.Thrown;
using AerovelenceMod.Content.Projectiles.NPCs.Bosses.CrystalTumbler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using AerovelenceMod.Content.Items.Placeables.Trophies;
using AerovelenceMod.Content.Items.Armor.Vanity;

//using AerovelenceMod.Items.BossBags;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
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
			Teleport = 6
		}

		/// <summary>
		/// Manages the current AI state of the Crystal Tumbler.
		/// Gets and sets npc.ai[0] as tracker.
		/// </summary>
		private CrystalTumblerState State
		{
			get => (CrystalTumblerState)NPC.ai[0];
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

		/// <summary>
		/// Returns a value between 0.0 - 1.0 based on current life.
		/// </summary>
		float LifePercentLeft => (NPC.life / (float)NPC.lifeMax);

		public bool P;
		public int spinTimer;
		public bool Phase2;
		public bool Teleport = false;
		public bool doingDash = false;
		int i;
		bool teleported = false;
		int t;
		public int counter = 0;
		public int counter2 = 0;

		public override void SetDefaults()
		{
			NPC.width = 120;
			NPC.height = 128;
			NPC.value = Item.buyPrice(0, 5, 60, 45);

			NPC.alpha = 0;
			NPC.damage = 30;
			NPC.defense = 20;
			NPC.lifeMax = 4000;
			NPC.knockBackResist = 0f;

			NPC.boss = true;
			NPC.lavaImmune = true;
			NPC.noGravity = false;
			NPC.noTileCollide = false;

			NPC.HitSound = SoundID.NPCHit41;
			NPC.DeathSound = SoundID.NPCDeath44;

			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Rimegeist");
			}
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			NPC.defense = 26;
			NPC.damage = 45;  //boss damage increase in expermode
			NPC.lifeMax = 5500;  //boss life scale in expertmode
		}

		public override bool PreAI()
		{
			var entitySource = NPC.GetSource_FromAI();

			NPC.TargetClosest(true);
			Player target = Main.player[NPC.target];

			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

			if (target.dead)
			{
				NPC.velocity.Y += 0.09f;
				NPC.timeLeft = 300;
				NPC.noTileCollide = true;
			}

			NPC.noTileCollide = NPC.noGravity = false;

			// No second phase implemented yet.
			/*if (npc.life <= npc.lifeMax / 2)
			{
				AttackTimer = 0;
				return;
			}*/

			// Idle roll/follow state. Very basic movement, with a timer for a random attack state.
			//if (State != CrystalTumblerState.IdleRoll) { Main.NewText(State); }
			if (State == CrystalTumblerState.IdleRoll)
			{
				doingDash = false;
				RollingMove(target);
				counter++;
				NPC.Opacity *= 1.1f;

				if (++AttackTimer >= 200)
				{
					AttackTimer = 0;
					NPC.netUpdate = true;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{

						if ((NPC.Center.Y - 50 > target.Center.Y || NPC.Center.Y + 50 < target.Center.Y) && Main.rand.NextBool(2))
						{
							State = CrystalTumblerState.Teleport;
							NPC.netUpdate = true;
							// Jump *only* when below the target, standing on solid ground, and with a bit of randomness.


							if (NPC.velocity.Y == 0 && NPC.Center.Y - 50 > target.Center.Y /*&& Main.rand.NextBool(2)*/)
							{

								State = CrystalTumblerState.Jump;
								NPC.netUpdate = true;
							}
						}
						else if (Math.Abs(target.Center.Y - NPC.Center.Y) <= 50 && Main.rand.NextBool(2))
						{

							State = CrystalTumblerState.SuperDash;
							NPC.netUpdate = true;
						}
						else
						{
							int randomState = Main.rand.Next(3);
							if (randomState == 0)
							{
								State = CrystalTumblerState.ProjectileSpawn;
								NPC.netUpdate = true;
							}
							else if (randomState == 1)
							{
								State = CrystalTumblerState.Electricity;
								NPC.netUpdate = true;
							}
							else
							{
								State = CrystalTumblerState.RockRain;
								NPC.netUpdate = true;
							}
						}
					}
				}
			}

			// Attack state. Spawns three projectiles on the NPC, which hover for a (few) second(s), before shooting towards the target.
			else if (State == CrystalTumblerState.ProjectileSpawn)
			{
				// Spawn a projectile every 10 ticks (and on the first tick this state is active).
				// TODO: Eldrazi - Multiplayer support?
				if (AttackTimer++ == 0)
				{
					SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 69, 0.75f);
					Projectile.NewProjectile(entitySource, NPC.Center, default, ModContent.ProjectileType<TumblerBoulder1>(), 12, 1f, Main.myPlayer, 0);
					NPC.netUpdate = true;
				}
				else if (AttackTimer == 10)
				{
					SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 69, 0.75f);
					Projectile.NewProjectile(entitySource, NPC.Center, default, ModContent.ProjectileType<TumblerBoulder1>(), 12, 1f, Main.myPlayer, 1);
					NPC.netUpdate = true;
				}
				else if (AttackTimer == 20)
				{
					SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 69, 0.75f);
					Projectile.NewProjectile(entitySource, NPC.Center, default, ModContent.ProjectileType<TumblerBoulder1>(), 12, 1f, Main.myPlayer, 2);
					NPC.netUpdate = true;
					AttackTimer = 0;
					State = CrystalTumblerState.IdleRoll;
				}
				RollingMove(target);
			}

			// Attack state. Starts speedy roll in place. After a set amount of ticks, releases/dashes with a high velocity and frequent jumps/bounces.
			else if (State == CrystalTumblerState.Jump)
			{
				/*Main.NewText("Newcheck");
				Main.NewText(target.Center.Y < npc.Center.Y - 60);
				Main.NewText(npc.Center.Y - 60 + "   -60 y");
				Main.NewText(target.Center.Y);*/

				if (target.Center.Y < NPC.Center.Y - 60)
				{
					NPC.velocity.Y -= Main.rand.Next(15) + 12;
					for (int num325 = 0; num325 < 20; num325++)
					{
						Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X, NPC.velocity.Y, 0, default, 1);
						NPC.netUpdate = true;
					}
					State = CrystalTumblerState.IdleRoll;
				}
				else if (target.Center.Y > NPC.Center.Y)
				{
					State = CrystalTumblerState.IdleRoll;
				}
				NPC.netUpdate = true;
			}

			else if (State == CrystalTumblerState.SuperDash)
			{
				t++;
				CheckPlatform(target);
				CheckTilesNextTo(target, 5f);
				// Rotating in place state.
				if (++AttackTimer <= 180)
				{
					doingDash = true;
					NPC.velocity.X *= 0.95f;
					NPC.rotation += AttackTimer / 180 * NPC.direction;
					float speed = 5f;
					Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
					if (t % 25 == 0 && !Main.expertMode == true)
					{
						if (Main.rand.NextBool(2))
						{
							Projectile.NewProjectile(entitySource, NPC.Center, velocity, ModContent.ProjectileType<TumblerSpike1>(), 15, 0f, Main.myPlayer, 0f, 0f);
							NPC.netUpdate = true;
						}
						else
						{
							Projectile.NewProjectile(entitySource, NPC.Center, velocity, ModContent.ProjectileType<TumblerSpike2>(), 15, 0f, Main.myPlayer, 0f, 0f);
							NPC.netUpdate = true;
						}
					}
					else if (t % 15 == 0 && Main.expertMode == true)
					{
						int randomProj = Main.rand.Next(3);
						if (randomProj == 0)
						{
							Projectile.NewProjectile(entitySource, NPC.Center, velocity, ModContent.ProjectileType<TumblerSpike1>(), 20, 10f, Main.myPlayer, 0f, 0f);
							NPC.netUpdate = true;
						}
						else if (randomProj == 1)
						{
							Projectile.NewProjectile(entitySource, NPC.Center, velocity, ModContent.ProjectileType<TumblerSpike2>(), 15, 10f, Main.myPlayer, 0f, 0f);
							NPC.netUpdate = true;
						}
						else
						{
							Projectile.NewProjectile(entitySource, NPC.Center, velocity, ModContent.ProjectileType<TumblerHomingShard>(), 20, 0f, Main.myPlayer, 0f, 0f);
							NPC.netUpdate = true;
						}
					}
					// If the timer is at 180 (or 3 seconds), set the velocity towards the NPCs direction.
					if (AttackTimer == 180)
					{
						NPC.netUpdate = true;
						NPC.velocity.X = NPC.direction * 16f;
						NPC.netUpdate = true;
					}

				}

				// Fast horizontal movement and frequent jumps.
				else
				{
					SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 76, 0.75f);
					if (NPC.velocity.Y == 0 && target.Center.Y < NPC.Center.Y - 100f)
					{
						TryJump(Main.rand.Next(4, 7), 60);
					}
					JumpTimer--;
					var player = Main.player[NPC.target];
					if (player.Center.X > NPC.Center.X)
					{
						NPC.velocity.X += 0.3f;
						NPC.netUpdate = true;
					}
					else if (player.Center.X < NPC.Center.X)
					{
						NPC.velocity.X -= 0.3f;
						NPC.netUpdate = true;
					}
					doingDash = false; //idk
				}



				// After 480 ticks (300 ticks or 5 seconds after the 'Rotating in place' state), go back to idle rolling.
				if (AttackTimer >= 480)
				{
					NPC.netUpdate = true;

					AttackTimer = 0;
					State = CrystalTumblerState.IdleRoll;
				}
				NPC.rotation += NPC.velocity.X * 0.025f;
			}

			else if (State == CrystalTumblerState.Electricity)
			{
				var player = Main.player[NPC.target];
				Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height * 0.5f));
				if (AttackTimer++ == 0)
				{
					SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 93, 0.75f);
					float Speed = 1 * 0.99f;
					int damage = Main.expertMode ? 20 : 10;// if u want to change this, 15 is for expert mode, 10 is for normal mod
					int type = Mod.Find<ModProjectile>("TumblerOrb").Type;
					float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
					Projectile.NewProjectile(entitySource, vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1), (float)(Math.Sin(rotation) * Speed * -1), type, damage, 0f, 0);
					NPC.netUpdate = true;
				}
				AttackTimer = 0;
				State = CrystalTumblerState.IdleRoll;
				RollingMove(target);
			}

			else if (State == CrystalTumblerState.Teleport)
			{
				Vector2 teleportPosition = target.position - Vector2.UnitY * 370;

				if (!Collision.SolidCollision(teleportPosition, NPC.width, NPC.height))
				{
					AttackTimer++;
					counter2++;
					if (AttackTimer >= 100)
					{
						NPC.Opacity *= 0.97f;
						NPC.netUpdate = true;
					}
					if (AttackTimer == 100)
					{
						Projectile.NewProjectile(entitySource, target.position, default, ModContent.ProjectileType<TeleportCharge>(), 12, 1f, Main.myPlayer, 1);
						NPC.netUpdate = true;
					}
					if (teleported == true)
					{
						if (counter2 == 500)
						{
							NPC.noTileCollide = false;
							counter2 = 0;
						}
						teleported = false;
					}
					if (AttackTimer >= 200)
					{
						NPC.position.Y = teleportPosition.Y;
						NPC.position.X = target.position.X;
						NPC.noTileCollide = true;
						teleported = true;
						AttackTimer = 0;
						NPC.netUpdate = true;
						SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 94, 0.75f);
						for (float i = 0; i < 360; i += 0.5f)
						{
							float ang = (float)(i * Math.PI) / 180;

							float x = (float)(Math.Cos(ang) * 150) + NPC.Center.X;
							float y = (float)(Math.Sin(ang) * 150) + NPC.Center.Y;

							Vector2 vel = Vector2.Normalize(new Vector2(x - NPC.Center.X, y - NPC.Center.Y)) * 15;

							int dustIndex = Dust.NewDust(new Vector2(x - 3, y - 3), 6, 6, 54, vel.X, vel.Y);
							Main.dust[dustIndex].noGravity = true;
							NPC.netUpdate = true;
						}
						State = CrystalTumblerState.IdleRoll;
					}
					RollingMove(target);
				}
				else
				{
					NPC.netUpdate = true;
					State = CrystalTumblerState.IdleRoll;
				}
			}

			else if (State == CrystalTumblerState.RockRain)
			{
				SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 74, 0.75f);

				for (int i = -3; i <= 3; i++)
				{
					if (Main.rand.NextBool(2))
					{
						Projectile.NewProjectile(entitySource, target.Center.X + i * 20, target.Center.Y - 380, i * 2, -2, ModContent.ProjectileType<TumblerSpike1>(), 30, 0f, Main.myPlayer, 0f, 0f);
						NPC.netUpdate = true;
					}
					else
					{
						Projectile.NewProjectile(entitySource, target.Center.X + i * 20, target.Center.Y - 380, i * 2, -2, ModContent.ProjectileType<TumblerSpike2>(), 30, 0f, Main.myPlayer, 0f, 0f);
						NPC.netUpdate = true;
					}
				}

				State = CrystalTumblerState.IdleRoll;
			}

			return (false);
		}

		private void CheckPlatform(Player player)
		{
			i++;
			bool onplatform = true;

			for (int i = (int)NPC.position.X; i < NPC.position.X + NPC.width; i += NPC.width / 4)
			{
				Tile tile = Framing.GetTileSafely(new Point((int)NPC.position.X / 16, (int)(NPC.position.Y + NPC.height + 8) / 16));
				if (!TileID.Sets.Platforms[tile.TileType])
				{
					onplatform = false;
				}
			}
			if (onplatform && (NPC.position.Y + NPC.height + 20 < player.Center.Y))
			{
				if (i % 70 == 0)
				{
					NPC.noTileCollide = true;
				}
				else
				{
					NPC.noTileCollide = false;
				}
			}
		}

		private void CheckTilesNextTo(Player player, float desiredspeed)
		{
			bool doJump = false;

			for (int i = (int)NPC.position.X; i < NPC.position.X + NPC.width; i += NPC.width / 4)
			{
				if (NPC.velocity.X < desiredspeed && player.Center.Y < NPC.Center.Y)
				{
					doJump = true;
				}
			}
			if (doJump && !doingDash)
			{
				if (NPC.velocity.Y == 0 && NPC.velocity.X == 0)
				{
					NPC.velocity.Y -= Main.rand.Next(9) + 7;
					for (int num325 = 0; num325 < 20; num325++)
					{
						Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X, NPC.velocity.Y, 0, default, 1);
					}
					State = CrystalTumblerState.IdleRoll;
				}
			}

		}


		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			var entitySource = NPC.GetSource_Death();

			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<RimegeistBag>()));

			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystalTumblerTrophy>(), 10));

			//npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<RimegeistRelic>()));

			//npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<RimegeistPetItem>(), 4));

			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CrystalTumblerMask>(), 7));

			npcLoot.Add(notExpertRule);


			Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("CavernCrystal").Type, Main.rand.Next(10, 20), false, 0, false, false);
			switch (Main.rand.Next(0, 8))
			{
				case 0:
					Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<CavernMauler>());
					break;
				case 1:
					Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<CavernousImpaler>());
					break;
				case 2:
					Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<CrystallineQuadshot>());
					break;
				case 3:
					Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<PrismThrasher>());
					break;
				case 4:
					Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<PrismPiercer>());
					break;
				case 5:
					Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<CarbonCadence>());
					break;
				case 6:
					Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<DarkCrystalStaff>());
					break;
				case 7:
					Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ShiningCrystalCore>());
					break;
			}
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			DownedWorld.DownedCrystalTumbler = true;

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);

			potionType = ItemID.HealingPotion;
		}


		public override void HitEffect(int hitDirection, double damage)
		{
			var entitySource = NPC.GetSource_Death();
			if (NPC.life <= 0)
			{
				for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>(), NPC.velocity.X, NPC.velocity.Y, 0, Color.White, 1);

                for (int i = 0; i < 7; i++)
					Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/TumblerGore" + i).Type);
			}
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/Glowmask");
			Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
		}

        public override void BossHeadRotation(ref float rotation) => rotation = NPC.rotation;

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
			CheckTilesNextTo(player, desiredSpeed);
			if (player.Center.X > NPC.Center.X)
			{
				if (NPC.velocity.X < desiredSpeed)
				{
					NPC.velocity.X += (0.1f * (1 - LifePercentLeft + 1));
				}
			}
			else if (player.Center.X < NPC.Center.X)
			{
				if (NPC.velocity.X > -desiredSpeed)
				{
					NPC.velocity.X -= (0.1f * (1 - LifePercentLeft + 1));
				}
			}
			NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -6, 6);

			// Rotation.
			NPC.rotation += NPC.velocity.X * 0.025f;

			// Jump.
			if (NPC.velocity.Y == 0 && player.Center.Y < NPC.Center.Y - 100f)
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
				NPC.netUpdate = true;
				NPC.velocity.Y -= height;
			}
			for (int num325 = 0; num325 < 20; num325++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X, NPC.velocity.Y, 0, default, 1);

			JumpTimer = cooldown;
		}

		#endregion
	}
}