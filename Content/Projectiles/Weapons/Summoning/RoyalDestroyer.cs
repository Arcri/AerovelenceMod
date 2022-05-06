#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
    internal sealed class RoyalDestroyer : ModProjectile
	{
		private enum MoveDirection
		{
			None = 0,
			Left = -1,
			Right = 1
		}
		
		private enum AIState
		{
			IdleFollow = 0,
			FlyingFollow = 1
		}
		private AIState State
		{
			get => (AIState)Projectile.ai[0];
			set => Projectile.ai[0] = (int)value;
		}

		private int SummonType;

		private readonly int AttackCooldown = 30;
		private readonly int DirectionCooldown = 60;

		private readonly int AnimationFrameDuration = 10;

		private readonly float ShotProjectileSpeed = 12f;

		private readonly float PositionXOffset = 40f;

		public override void SetStaticDefaults()
		{
			Main.projPet[Projectile.type] = true;

			Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		}
		public override void SetDefaults()
		{
			Projectile.width = 36;
			Projectile.height = 40;

			Projectile.timeLeft *= 5;
			Projectile.penetrate = -1;
			Projectile.minionSlots = 1f;

			Projectile.minion = true;
			Projectile.friendly = false;
			Projectile.netImportant = true;
			Projectile.manualDirectionChange = true;

			SummonType = -1;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[Projectile.owner];
			AeroPlayer ap = owner.GetModPlayer<AeroPlayer>();

			if (SummonType == -1)
			{
				SummonType = (int)Projectile.ai[0];
				Projectile.ai[0] = 0;
			}

			if (!CheckAliveState(ap))
			{
				return (false);
			}


			HuntressAI(owner);

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Leaves>());
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = (Texture2D)TextureAssets.Projectile[Projectile.type];
			Rectangle frame = texture.Frame(3, 3, SummonType, Projectile.frame);
			Vector2 origin = frame.Size() / 2;
			SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, effects, 0f);

			return (false);
		}

		private bool CheckAliveState(AeroPlayer owner)
		{
			if (!owner.Player.active)
			{
				Projectile.active = false;
				return (false);
			}

			if (owner.Player.dead)
			{
				owner.huntressSummon = false;
			}
			if (owner.huntressSummon)
			{
				Projectile.timeLeft = 2;
			}

			return (true);
		}

		private void SetProjectileFrame()
		{
			if (Projectile.velocity.Y != 0)
			{
				Projectile.frame = 1;
			}
			else
			{
				int frameDuration = AnimationFrameDuration;
				if (Projectile.velocity.X != 0)
				{
					frameDuration /= 2;
				}

				Projectile.frameCounter++;
				if (Projectile.frameCounter <= frameDuration)
				{
					Projectile.frame = 0;
				}
				else if (Projectile.frameCounter <= 2 * frameDuration)
				{
					Projectile.frame = 1;
				}
				else if (Projectile.frameCounter <= 3 * frameDuration)
				{
					Projectile.frame = 2;
				}
				else if (Projectile.frameCounter <= 4 * frameDuration)
				{
					Projectile.frame = 1;
				}
				else if (Projectile.frameCounter >= 5 * frameDuration)
				{
					Projectile.frameCounter = 0;
				}
			}
		}

		private void HuntressAI(Player owner)
		{
			MoveDirection moveDirection = MoveDirection.None;

			bool flag3 = false;
			bool flag4 = false;

			if (Projectile.lavaWet)
			{
				Projectile.ai[1] = 0f;
				State = AIState.FlyingFollow;
			}

			float xOffset = 35 * (Projectile.minionPos + 1) * owner.direction;
			if (owner.Center.X < Projectile.Center.X - 10 + xOffset)
			{
				moveDirection = MoveDirection.Left;
			}
			else if (owner.Center.X > Projectile.Center.X + 10 + xOffset)
			{
				moveDirection = MoveDirection.Right;
			}

			if (Projectile.ai[1] == 0f)
			{
				int maxAllowedDistance = 540;

				if (Projectile.localAI[0] > 0f)
				{
					maxAllowedDistance += 500;
				}

				Vector2 directionTowardsOwner = owner.Center - Projectile.Center;
				float length = directionTowardsOwner.Length();

				if (length > 2000f)
				{
					Projectile.position = owner.Center - new Vector2(Projectile.width, Projectile.height);
				}
				else if (length > maxAllowedDistance || (Math.Abs(directionTowardsOwner.Y) > 300f && Projectile.localAI[0] <= 0f))
				{
					if ((directionTowardsOwner.Y > 0f && Projectile.velocity.Y < 0f) ||
						(directionTowardsOwner.Y < 0f && Projectile.velocity.Y > 0f))
					{
						Projectile.velocity.Y = 0f;
					}
					State = AIState.FlyingFollow;
				}
			}

			if (State == AIState.FlyingFollow)
			{
				OwnerFlyingFollow(owner);
				return;
			}
			
			AcquireAndAttackTarget(ref moveDirection);

			if (Projectile.ai[1] != 0f)
			{
				moveDirection = MoveDirection.None;
			}
			else if (Projectile.localAI[0] == 0f)
			{
				Projectile.direction = owner.direction;
			}

			Projectile.rotation = 0f;
			Projectile.tileCollide = true;

			float speed = 6f;
			float acceleration = 0.2f;

			if (speed < owner.velocity.Length())
			{
				speed = owner.velocity.Length();
				acceleration = 0.3f;
			}

			if (moveDirection == MoveDirection.Left)
			{
				if (Projectile.velocity.X > -3.5f)
				{
					Projectile.velocity.X -= acceleration;
				}
				else
				{
					Projectile.velocity.X -= acceleration * 0.25f;
				}
			}
			else if (moveDirection == MoveDirection.Right)
			{
				if (Projectile.velocity.X < 3.5f)
				{
					Projectile.velocity.X += acceleration;
				}
				else
				{
					Projectile.velocity.X += acceleration * 0.25f;
				}
			}
			else
			{
				Projectile.velocity.X *= 0.9f;
				if (Math.Abs(Projectile.velocity.X) <= acceleration)
				{
					Projectile.velocity.X = 0f;
				}
			}
			
			if (moveDirection != MoveDirection.None)
			{
				int tileX = (int)(Projectile.Center.X / 16);
				int tileY = (int)(Projectile.Center.Y / 16);

				tileX += (int)moveDirection + (int)Projectile.velocity.X;
				if (WorldGen.SolidTile(tileX, tileY))
				{
					flag4 = true;
				}
			}

			if (owner.position.Y + owner.height - 8f > Projectile.position.Y + Projectile.height)
			{
				flag3 = true;
			}

			Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width,
				Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);

			if (Projectile.velocity.Y == 0f)
			{
				if (!flag3 && Projectile.velocity.X != 0f)
				{
					int tileX = (int)(Projectile.Center.X / 16);
					int tileY = (int)(Projectile.Center.Y / 16) + 1;

					tileX += (int)moveDirection;

					WorldGen.SolidTile(tileX, tileY);
				}

				if (flag4)
				{
					int tileX = (int)(Projectile.Center.X / 16);
					int tileY = (int)((Projectile.position.Y + Projectile.height) / 16) + 1;
					if (WorldGen.SolidTile(tileX, tileY) || Main.tile[tileX, tileY].IsHalfBlock || Main.tile[tileX, tileY].slope() > 0)
					{
						try
						{
							tileX = (int)(Projectile.Center.X / 16);
							tileY = (int)(Projectile.Center.Y / 16);

							tileX += (int)moveDirection + (int)Projectile.velocity.X;

							tileX += (int)Projectile.velocity.X;
							if (!WorldGen.SolidTile(tileX, tileY - 1) && !WorldGen.SolidTile(tileX, tileY - 2))
							{
								Projectile.velocity.Y = -5.1f;
							}
							else if (!WorldGen.SolidTile(tileX, tileY - 2))
							{
								Projectile.velocity.Y = -7.1f;
							}
							else if (WorldGen.SolidTile(tileX, tileY - 5))
							{
								Projectile.velocity.Y = -11.1f;
							}
							else if (WorldGen.SolidTile(tileX, tileY - 4))
							{
								Projectile.velocity.Y = -10.1f;
							}
							else
							{
								Projectile.velocity.Y = -9.1f;
							}
						}
						catch
						{
							Projectile.velocity.Y = -9.1f;
						}
					}
				}
			}

			Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X, -speed, speed);

			if (Projectile.velocity.X > 0 || (Projectile.velocity.X > acceleration && moveDirection == MoveDirection.Right))
			{
				Projectile.direction = 1;
			}
			else if (Projectile.velocity.X < 0 || (Projectile.velocity.X < -acceleration && moveDirection == MoveDirection.Left))
			{
				Projectile.direction = -1;
			}

			Projectile.velocity.Y += 0.4f;
			if (Projectile.velocity.Y > 10f)
			{
				Projectile.velocity.Y = 10f;
			}

			Projectile.spriteDirection = Projectile.direction;
		}

		private void OwnerFlyingFollow(Player owner)
		{
			float speed = 12f;
			float acceleration = 0.4f;
			float minAllowedDistance = 100;
			float npcTargetingDistance = 800f;

			Vector2 targetPosition = owner.Center - Projectile.Center - new Vector2(40 * owner.direction);

			Projectile.tileCollide = false;

			int target = -1;
			bool hasTarget = false;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (!Main.npc[i].CanBeChasedBy(this))
				{
					continue;
				}

				NPC npc = Main.npc[i];
				Vector2 npcCenter = npc.Center;

				if (Math.Abs(owner.Center.X - npcCenter.X) + Math.Abs(owner.Center.Y - npcCenter.Y) < npcTargetingDistance)
				{
					if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
					{
						target = i;
					}
					hasTarget = true;
					break;
				}
			}

			if (!hasTarget)
			{
				targetPosition.X -= PositionXOffset * owner.direction;
			}

			if (hasTarget && target >= 0)
			{
				State = AIState.IdleFollow;
			}

			float distanceToTarget = targetPosition.Length();

			if (speed < Math.Abs(owner.velocity.X) + Math.Abs(owner.velocity.Y))
			{
				speed = Math.Abs(owner.velocity.X) + Math.Abs(owner.velocity.Y);
			}

			if (distanceToTarget < minAllowedDistance && owner.velocity.Y == 0f && Projectile.position.Y + Projectile.height <= owner.position.Y + owner.height &&
				!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
			{
				State = AIState.IdleFollow;
				if (Projectile.velocity.Y < -6f)
				{
					Projectile.velocity.Y = -6f;
				}
			}

			if (distanceToTarget < 60f)
			{
				targetPosition = Projectile.velocity;
			}
			else
			{
				distanceToTarget = speed / distanceToTarget;
				targetPosition.X *= distanceToTarget;
				targetPosition.Y *= distanceToTarget;
			}

			if (Projectile.velocity.X < targetPosition.X)
			{
				Projectile.velocity.X += acceleration;
				if (Projectile.velocity.X < 0f)
				{
					Projectile.velocity.X += acceleration * 1.5f;
				}
			}
			else if (Projectile.velocity.X > targetPosition.X)
			{
				Projectile.velocity.X -= acceleration;
				if (Projectile.velocity.X > 0f)
				{
					Projectile.velocity.X -= acceleration * 1.5f;
				}
			}

			if (Projectile.velocity.Y < targetPosition.Y)
			{
				Projectile.velocity.Y += acceleration;
				if (Projectile.velocity.Y < 0f)
				{
					Projectile.velocity.Y += acceleration * 1.5f;
				}
			}
			else if (Projectile.velocity.Y > targetPosition.Y)
			{
				Projectile.velocity.Y -= acceleration;
				if (Projectile.velocity.Y > 0f)
				{
					Projectile.velocity.Y -= acceleration * 1.5f;
				}
			}

			if (Projectile.velocity.X > 0.5f)
			{
				Projectile.spriteDirection = 1;
			}
			else if (Projectile.velocity.X < -0.5f)
			{
				Projectile.spriteDirection = -1;
			}

			if (Projectile.spriteDirection == 1)
			{
				Projectile.rotation = Projectile.velocity.ToRotation();
			}
			else
			{
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
			}
		}

		private void AcquireAndAttackTarget(ref MoveDirection moveDirection)
		{
			if (--Projectile.localAI[0] < 0)
			{
				Projectile.localAI[0] = 0;
			}

			if (Projectile.ai[1] > 0)
			{
				Projectile.ai[1]--;
			}
			else
			{
				float distance = 100000f;
				float attackDistanceLow = 400f;
				float attackDistanceHigh = 800f;
				float firstTargetDistance = distance;
				Vector2 targetPosition = Projectile.position;

				int targetIndex = -1;

				// Acquire player-targeted NPC.
				NPC targetNPC = Projectile.OwnerMinionAttackTargetNPC;
				if (targetNPC != null && targetNPC.CanBeChasedBy(this))
				{
					Vector2 targetNPCPosition = targetNPC.Center;
					float distanceToTargetNPC = (Projectile.Center - targetNPCPosition).Length();

					if (distanceToTargetNPC < distance)
					{
						targetPosition = targetNPCPosition;
						firstTargetDistance = distanceToTargetNPC;

						if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height,
							targetNPC.position, targetNPC.width, targetNPC.height))
						{
							targetIndex = targetNPC.whoAmI;
							distance = distanceToTargetNPC;
							targetPosition = targetNPCPosition;
						}
					}
				}

				// If no valid player targeted NPC, attempt to acquire a valid target.
				if (targetIndex == -1)
				{
					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						if (!Main.npc[i].CanBeChasedBy(this))
						{
							continue;
						}

						targetNPC = Main.npc[i];

						Vector2 targetNPCPosition = targetNPC.Center;
						float distanceToTargetNPC = (Projectile.Center - targetNPCPosition).Length();

						if (distanceToTargetNPC < distance)
						{
							if (targetIndex == -1 && distanceToTargetNPC <= firstTargetDistance)
							{
								targetPosition = targetNPCPosition;
								firstTargetDistance = distanceToTargetNPC;
							}
							if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height,
								targetNPC.position, targetNPC.width, targetNPC.height))
							{
								targetIndex = i;
								distance = distanceToTargetNPC;
								targetPosition = targetNPCPosition;
							}
						}
					}
				}

				if (targetIndex == -1 && firstTargetDistance < distance)
				{
					distance = firstTargetDistance;
				}

				if (Projectile.position.Y > Main.worldSurface * 16f)
				{
					attackDistanceLow = 200f;
				}

				// If there's a target in range, but collision in-between this projectile and the target,
				// Try to see if moving left or right solves the collision block.
				if (distance < attackDistanceLow + PositionXOffset && targetIndex == -1)
				{
					float xDir = targetPosition.X - Projectile.Center.X;
					if (xDir < -5f)
					{
						moveDirection = MoveDirection.Left;
					}
					else if (xDir > 5f)
					{
						moveDirection = MoveDirection.Right;
					}

					return;
				}
				
				if (targetIndex >= 0 && distance < attackDistanceHigh + PositionXOffset)
				{
					Projectile.localAI[0] = DirectionCooldown;
					
					float xDir = targetPosition.X - Projectile.Center.X;
					if (Math.Abs(xDir) > 300)
					{
						if (xDir < -50f)
						{
							moveDirection = MoveDirection.Left;
						}
						else if (xDir > 50f)
						{
							moveDirection = MoveDirection.Right;
						}
					}
					else if (Projectile.owner == Main.myPlayer)
					{
						Projectile.ai[1] = AttackCooldown;

						int newProjectileType = ModContent.ProjectileType<HuntressSpear>();
						if (SummonType == 1)
						{
							newProjectileType = ModContent.ProjectileType<HuntressJavelin>();
						}
						else if (SummonType == 2)
						{
							newProjectileType = ModContent.ProjectileType<HuntressArrow>();
						}

						Vector2 newProjectilePosition = Projectile.Center;
						Vector2 newProjectileVelocity = targetPosition - newProjectilePosition;

						// Apply a modifier to the Y velocity of the projectile based on the horizontal distance to the target.
						float newVelocityYModifier = Math.Abs(newProjectileVelocity.X) * 0.1f;
						newVelocityYModifier *= Main.rand.NextFloat() * 0.1f;
						newProjectileVelocity.Y -= newVelocityYModifier;

						newProjectileVelocity = Vector2.Normalize(newProjectileVelocity) * ShotProjectileSpeed;
						Projectile.NewProjectile(Projectile.InheritSource(Projectile), newProjectilePosition, newProjectileVelocity, newProjectileType, Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, Projectile.whoAmI);

						if (newProjectileVelocity.X < 0f)
						{
							Projectile.direction = -1;
						}
						if (newProjectileVelocity.X > 0f)
						{
							Projectile.direction = 1;
						}
						Projectile.netUpdate = true;
					}
				}
			}
		}
	}
}
