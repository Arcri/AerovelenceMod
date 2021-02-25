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
			SuperDash = 2
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

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = 3200;  //boss life scale in expertmode
			npc.damage = 20;  //boss damage increase in expermode
		}

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

				if (++AttackTimer >= 250)
				{
					AttackTimer = 0;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						State = (CrystalTumblerState)Main.rand.Next(1, 3);
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
					Projectile.NewProjectile(npc.Center, default, ModContent.ProjectileType<TumblerBoulder1>(), 12, 1f, Main.myPlayer, 0);
				}
				else if (AttackTimer == 10)
				{
					Projectile.NewProjectile(npc.Center, default, ModContent.ProjectileType<TumblerBoulder1>(), 12, 1f, Main.myPlayer, 1);
				}
				else if (AttackTimer == 20)
				{
					Projectile.NewProjectile(npc.Center, default, ModContent.ProjectileType<TumblerBoulder1>(), 12, 1f, Main.myPlayer, 2);

					AttackTimer = 0;
					State = CrystalTumblerState.IdleRoll;
				}

				RollingMove(target);
			}
			// Attack state. Starts speedy roll in place. After a set amount of ticks, releases/dashes with a high velocity and frequent jumps/bounces.
			else if (State == CrystalTumblerState.SuperDash)
			{
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
					TryJump(Main.rand.Next(4, 7), 60);
					JumpTimer--;

					if (npc.velocity.X <= 20 * npc.direction)
					{
						npc.velocity.X += 0.3f * npc.direction;

						if (Math.Sign(npc.velocity.X) != npc.direction)
						{
							npc.velocity.X += 0.2f * npc.direction;
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
			}

			/*if ((npc.Center.Y - player.Center.Y) < -150)
			{
				i++;
			}
			else
			{
				i = 0;
			}

            if (i % 100 == 0)
            {
                SuperDash = true;
            }

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