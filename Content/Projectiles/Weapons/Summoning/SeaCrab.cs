#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
    internal sealed class SeaCrab : ModProjectile
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
			get => (AIState)projectile.ai[0];
			set => projectile.ai[0] = (int)value;
		}

		private int SummonType;

		private readonly int AttackCooldown = 30;
		private readonly int DirectionCooldown = 60;

		private readonly int AnimationFrameDuration = 10;

		private readonly float ShotProjectileSpeed = 12f;

		private readonly float PositionXOffset = 40f;

		public override void SetStaticDefaults()
		{
			Main.projPet[projectile.type] = true;
			Main.projFrames[projectile.type] = 3;

			ProjectileID.Sets.Homing[projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
		}
		public override void SetDefaults()
		{
			projectile.width = 48;
			projectile.height = 30;

			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 1f;

			projectile.minion = true;
			projectile.friendly = false;
			projectile.netImportant = true;
			projectile.manualDirectionChange = true;

			SummonType = 1;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			AeroPlayer ap = owner.GetModPlayer<AeroPlayer>();

			if (SummonType == -1)
			{
				SummonType = (int)projectile.ai[0];
				projectile.ai[0] = 0;
			}

			if (!CheckAliveState(ap))
			{
				return (false);
			}

			SetProjectileFrame();

			HuntressAI(owner);

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.Leaves>());
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity) => false;



		private bool CheckAliveState(AeroPlayer owner)
		{
			if (!owner.player.active)
			{
				projectile.active = false;
				return (false);
			}

			if (owner.player.dead)
			{
				owner.huntressSummon = false;
			}
			if (owner.huntressSummon)
			{
				projectile.timeLeft = 2;
			}

			return (true);
		}

		private void SetProjectileFrame()
		{
			if (projectile.velocity.Y != 0)
			{
				projectile.frame = 1;
			}
			else
			{
				projectile.frameCounter++;
				int frameDuration = AnimationFrameDuration;
				if (projectile.velocity.X != 0)
				{
					if (projectile.frameCounter % 10 == 0)
					{
						projectile.frame++;
						projectile.frameCounter = 0;
						if (projectile.frame >= 3)
							projectile.frame = 0;
					}
				}

				else if(projectile.velocity.X == 0)
				{
					projectile.frame = 1;
				}
				else 
				{
					
					if (projectile.frameCounter % 10 == 0)
					{
						projectile.frame++;
						projectile.frameCounter = 0;
						if (projectile.frame >= 3)
							projectile.frame = 0;
					}
				}
			}
		}

		private void HuntressAI(Player owner)
		{
			MoveDirection moveDirection = MoveDirection.None;

			bool flag3 = false;
			bool flag4 = false;

			if (projectile.lavaWet)
			{
				projectile.ai[1] = 0f;
				State = AIState.FlyingFollow;
			}

			float xOffset = 35 * (projectile.minionPos + 1) * owner.direction;
			if (owner.Center.X < projectile.Center.X - 10 + xOffset)
			{
				moveDirection = MoveDirection.Left;
			}
			else if (owner.Center.X > projectile.Center.X + 10 + xOffset)
			{
				moveDirection = MoveDirection.Right;
			}

			if (projectile.ai[1] == 0f)
			{
				int maxAllowedDistance = 540;

				if (projectile.localAI[0] > 0f)
				{
					maxAllowedDistance += 500;
				}

				Vector2 directionTowardsOwner = owner.Center - projectile.Center;
				float length = directionTowardsOwner.Length();

				if (length > 2000f)
				{
					projectile.position = owner.Center - new Vector2(projectile.width, projectile.height);
				}
				else if (length > maxAllowedDistance || (Math.Abs(directionTowardsOwner.Y) > 300f && projectile.localAI[0] <= 0f))
				{
					if ((directionTowardsOwner.Y > 0f && projectile.velocity.Y < 0f) ||
						(directionTowardsOwner.Y < 0f && projectile.velocity.Y > 0f))
					{
						projectile.velocity.Y = 0f;
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

			if (projectile.ai[1] != 0f)
			{
				moveDirection = MoveDirection.None;
			}
			else if (projectile.localAI[0] == 0f)
			{
				projectile.direction = owner.direction;
			}

			projectile.rotation = 0f;
			projectile.tileCollide = true;

			float speed = 6f;
			float acceleration = 0.2f;

			if (speed < owner.velocity.Length())
			{
				speed = owner.velocity.Length();
				acceleration = 0.3f;
			}

			if (moveDirection == MoveDirection.Left)
			{
				if (projectile.velocity.X > -3.5f)
				{
					projectile.velocity.X -= acceleration;
				}
				else
				{
					projectile.velocity.X -= acceleration * 0.25f;
				}
			}
			else if (moveDirection == MoveDirection.Right)
			{
				if (projectile.velocity.X < 3.5f)
				{
					projectile.velocity.X += acceleration;
				}
				else
				{
					projectile.velocity.X += acceleration * 0.25f;
				}
			}
			else
			{
				projectile.velocity.X *= 0.9f;
				if (Math.Abs(projectile.velocity.X) <= acceleration)
				{
					projectile.velocity.X = 0f;
				}
			}
			
			if (moveDirection != MoveDirection.None)
			{
				int tileX = (int)(projectile.Center.X / 16);
				int tileY = (int)(projectile.Center.Y / 16);

				tileX += (int)moveDirection + (int)projectile.velocity.X;
				if (WorldGen.SolidTile(tileX, tileY))
				{
					flag4 = true;
				}
			}

			if (owner.position.Y + owner.height - 8f > projectile.position.Y + projectile.height)
			{
				flag3 = true;
			}

			Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width,
				projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY);

			if (projectile.velocity.Y == 0f)
			{
				if (!flag3 && projectile.velocity.X != 0f)
				{
					int tileX = (int)(projectile.Center.X / 16);
					int tileY = (int)(projectile.Center.Y / 16) + 1;

					tileX += (int)moveDirection;

					WorldGen.SolidTile(tileX, tileY);
				}

				if (flag4)
				{
					int tileX = (int)(projectile.Center.X / 16);
					int tileY = (int)((projectile.position.Y + projectile.height) / 16) + 1;
					if (WorldGen.SolidTile(tileX, tileY) || Main.tile[tileX, tileY].halfBrick() || Main.tile[tileX, tileY].slope() > 0)
					{
						try
						{
							tileX = (int)(projectile.Center.X / 16);
							tileY = (int)(projectile.Center.Y / 16);

							tileX += (int)moveDirection + (int)projectile.velocity.X;

							tileX += (int)projectile.velocity.X;
							if (!WorldGen.SolidTile(tileX, tileY - 1) && !WorldGen.SolidTile(tileX, tileY - 2))
							{
								projectile.velocity.Y = -5.1f;
							}
							else if (!WorldGen.SolidTile(tileX, tileY - 2))
							{
								projectile.velocity.Y = -7.1f;
							}
							else if (WorldGen.SolidTile(tileX, tileY - 5))
							{
								projectile.velocity.Y = -11.1f;
							}
							else if (WorldGen.SolidTile(tileX, tileY - 4))
							{
								projectile.velocity.Y = -10.1f;
							}
							else
							{
								projectile.velocity.Y = -9.1f;
							}
						}
						catch
						{
							projectile.velocity.Y = -9.1f;
						}
					}
				}
			}

			projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -speed, speed);

			if (projectile.velocity.X > 0 || (projectile.velocity.X > acceleration && moveDirection == MoveDirection.Right))
			{
				projectile.direction = 1;
			}
			else if (projectile.velocity.X < 0 || (projectile.velocity.X < -acceleration && moveDirection == MoveDirection.Left))
			{
				projectile.direction = -1;
			}

			projectile.velocity.Y += 0.4f;
			if (projectile.velocity.Y > 10f)
			{
				projectile.velocity.Y = 10f;
			}

			projectile.spriteDirection = projectile.direction;
		}

		private void OwnerFlyingFollow(Player owner)
		{
			float speed = 12f;
			float acceleration = 0.4f;
			float minAllowedDistance = 100;
			float npcTargetingDistance = 800f;

			Vector2 targetPosition = owner.Center - projectile.Center - new Vector2(40 * owner.direction);

			projectile.tileCollide = false;

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
					if (Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
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

			if (distanceToTarget < minAllowedDistance && owner.velocity.Y == 0f && projectile.position.Y + projectile.height <= owner.position.Y + owner.height &&
				!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
			{
				State = AIState.IdleFollow;
				if (projectile.velocity.Y < -6f)
				{
					projectile.velocity.Y = -6f;
				}
			}

			if (distanceToTarget < 60f)
			{
				targetPosition = projectile.velocity;
			}
			else
			{
				distanceToTarget = speed / distanceToTarget;
				targetPosition.X *= distanceToTarget;
				targetPosition.Y *= distanceToTarget;
			}

			if (projectile.velocity.X < targetPosition.X)
			{
				projectile.velocity.X += acceleration;
				if (projectile.velocity.X < 0f)
				{
					projectile.velocity.X += acceleration * 1.5f;
				}
			}
			else if (projectile.velocity.X > targetPosition.X)
			{
				projectile.velocity.X -= acceleration;
				if (projectile.velocity.X > 0f)
				{
					projectile.velocity.X -= acceleration * 1.5f;
				}
			}

			if (projectile.velocity.Y < targetPosition.Y)
			{
				projectile.velocity.Y += acceleration;
				if (projectile.velocity.Y < 0f)
				{
					projectile.velocity.Y += acceleration * 1.5f;
				}
			}
			else if (projectile.velocity.Y > targetPosition.Y)
			{
				projectile.velocity.Y -= acceleration;
				if (projectile.velocity.Y > 0f)
				{
					projectile.velocity.Y -= acceleration * 1.5f;
				}
			}

			if (projectile.velocity.X > 0.5f)
			{
				projectile.spriteDirection = 1;
			}
			else if (projectile.velocity.X < -0.5f)
			{
				projectile.spriteDirection = -1;
			}

			if (projectile.spriteDirection == 1)
			{
				projectile.rotation = projectile.velocity.ToRotation();
			}
			else
			{
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi;
			}
		}

		private void AcquireAndAttackTarget(ref MoveDirection moveDirection)
		{
			if (--projectile.localAI[0] < 0)
			{
				projectile.localAI[0] = 0;
			}

			if (projectile.ai[1] > 0)
			{
				projectile.ai[1]--;
			}
			else
			{
				float distance = 100000f;
				float attackDistanceLow = 400f;
				float attackDistanceHigh = 800f;
				float firstTargetDistance = distance;
				Vector2 targetPosition = projectile.position;

				int targetIndex = -1;

				// Acquire player-targeted NPC.
				NPC targetNPC = projectile.OwnerMinionAttackTargetNPC;
				if (targetNPC != null && targetNPC.CanBeChasedBy(this))
				{
					Vector2 targetNPCPosition = targetNPC.Center;
					float distanceToTargetNPC = (projectile.Center - targetNPCPosition).Length();

					if (distanceToTargetNPC < distance)
					{
						targetPosition = targetNPCPosition;
						firstTargetDistance = distanceToTargetNPC;

						if (Collision.CanHit(projectile.position, projectile.width, projectile.height,
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
						float distanceToTargetNPC = (projectile.Center - targetNPCPosition).Length();

						if (distanceToTargetNPC < distance)
						{
							if (targetIndex == -1 && distanceToTargetNPC <= firstTargetDistance)
							{
								targetPosition = targetNPCPosition;
								firstTargetDistance = distanceToTargetNPC;
							}
							if (Collision.CanHit(projectile.position, projectile.width, projectile.height,
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

				if (projectile.position.Y > Main.worldSurface * 16f)
				{
					attackDistanceLow = 200f;
				}

				// If there's a target in range, but collision in-between this projectile and the target,
				// Try to see if moving left or right solves the collision block.
				if (distance < attackDistanceLow + PositionXOffset && targetIndex == -1)
				{
					float xDir = targetPosition.X - projectile.Center.X;
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
					projectile.localAI[0] = DirectionCooldown;
					
					float xDir = targetPosition.X - projectile.Center.X;
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
					else if (projectile.owner == Main.myPlayer)
					{
						projectile.ai[1] = AttackCooldown;

						int newProjectileType = ModContent.ProjectileType<SeaCrabProjectile>();

						Vector2 newProjectilePosition = projectile.Center;
						Vector2 newProjectileVelocity = targetPosition - newProjectilePosition;

						// Apply a modifier to the Y velocity of the projectile based on the horizontal distance to the target.
						float newVelocityYModifier = Math.Abs(newProjectileVelocity.X) * 0.1f;
						newVelocityYModifier *= Main.rand.NextFloat() * 0.1f;
						newProjectileVelocity.Y -= newVelocityYModifier;

						newProjectileVelocity = Vector2.Normalize(newProjectileVelocity) * ShotProjectileSpeed;
						Projectile.NewProjectile(newProjectilePosition, newProjectileVelocity, newProjectileType, projectile.damage, projectile.knockBack, Main.myPlayer, 0, projectile.whoAmI);

						if (newProjectileVelocity.X < 0f)
						{
							projectile.direction = -1;
						}
						if (newProjectileVelocity.X > 0f)
						{
							projectile.direction = 1;
						}
						projectile.netUpdate = true;
					}
				}
			}
		}

		partial class SeaCrabProjectile : ModProjectile
        {
			private void ApplyTrailFx()
			{
				Projectile proj = projectile;
				for (int dusts = 0; dusts < 1; dusts++)
				{
					int castAheadDist = 6;
					var pos = new Vector2(
						proj.position.X + castAheadDist,
						proj.position.Y + castAheadDist
					);

					for (int subDusts = 0; subDusts < 3; subDusts++)
					{
						float dustCastAheadX = proj.velocity.X / 3f * subDusts;
						float dustCastAheadY = proj.velocity.Y / 3f * subDusts;

						int dustIdx = Dust.NewDust(
							Position: pos,
							Width: proj.width - castAheadDist * 2,
							Height: proj.height - castAheadDist * 2,
							Type: 59,
							SpeedX: 0f,
							SpeedY: 0f,
							Alpha: 100,
							newColor: default,
							Scale: 1.2f
						);

						Main.dust[dustIdx].noGravity = true;
						Main.dust[dustIdx].velocity *= 0.3f;
						Main.dust[dustIdx].velocity += proj.velocity * 0.5f;

						Dust dust = Main.dust[dustIdx];
						dust.position.X -= dustCastAheadX;
						dust.position.Y -= dustCastAheadY;
					}

					if (Main.rand.Next(8) == 0)
					{
						int dustIdx = Dust.NewDust(
							Position: pos,
							Width: proj.width - castAheadDist * 2,
							Height: proj.height - castAheadDist * 2,
							Type: 60,
							SpeedX: 0f,
							SpeedY: 0f,
							Alpha: 100,
							newColor: default,
							Scale: 0.75f
						);
						Main.dust[dustIdx].velocity *= 0.5f;
						Main.dust[dustIdx].velocity += proj.velocity * 0.5f;
					}
				}
			}
		}
		partial class SeaCrabProjectile : ModProjectile
		{
			

			private static int AquaSceptreAiStyle;
			public override string Texture => "Terraria/Projectile_" + ProjectileID.WaterStream;
			public override void SetStaticDefaults()
			{
				DisplayName.SetDefault("Sea Spray");

				var aquaSceptreProj = new Projectile();
				aquaSceptreProj.SetDefaults(ProjectileID.WaterStream);

				AquaSceptreAiStyle = aquaSceptreProj.aiStyle;
			}
			public override void SetDefaults()
			{
				projectile.width = 12;
				projectile.damage = 10;
				projectile.height = 12;
				projectile.tileCollide = true;
				projectile.friendly = true;
				projectile.hostile = false;
				projectile.melee = true;
				projectile.aiStyle = 2;
				projectile.alpha = 255;
				projectile.timeLeft = 600;
				projectile.ignoreWater = true;
				projectile.tileCollide = true;
				projectile.extraUpdates = 1;
			}

            public override void AI()
            {
				ApplyTrailFx();
            }
        }
	}
}
