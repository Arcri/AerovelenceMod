#region Using directives

using System;
using System.IO;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus
{
    public sealed class LumberjackAxe : ModProjectile
	{
		private int Target
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		private float AttackTimer
		{
			get => projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		private int AttackDirection
		{
			get => (int)projectile.localAI[1];
			set => projectile.localAI[1] = value;
		}

		private readonly float MaxTargetingDistance = 500f;

		private readonly float AttackWarmup = 50;
		private readonly float AttackDuration = 20;
		private float CompleteAttackTime => AttackWarmup + AttackDuration;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 20;

			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			AeroPlayer ep = owner.GetModPlayer<AeroPlayer>();

			CheckAliveState(ep);

			if (Target == 0)
			{
				float distance = MaxTargetingDistance;

				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					if (!Main.npc[i].CanBeChasedBy(projectile))
						continue;

					float distanceToNPC = projectile.Distance(Main.npc[i].Center);
					if (distanceToNPC < distance && Collision.CanHitLine(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
					{
						Target = i + 1;
						distance = distanceToNPC;
					}
				}

				if (Target != 0)
				{
					AttackTimer = 0;
					projectile.netUpdate = true;
				}
				else
				{
					FollowOwner(owner);
				}
			}

			if (Target != 0)
			{
				NPC npc = Main.npc[Target - 1];

				if (!npc.CanBeChasedBy(projectile) || projectile.Distance(owner.Center) >= 1000)
				{
					Target = 0;
					return (false);
				}

				AttackNPC(npc);
			}

			projectile.velocity *= 0.8f;

			return (false);
		}

		public override bool CanDamage()
			=> Target != 0 && AttackTimer >= AttackWarmup;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			projHitbox.Inflate(15, 15);
			return (projHitbox.Intersects(targetHitbox));
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (Target != 0 && AttackTimer >= AttackWarmup)
			{
				Texture2D texture = Main.projectileTexture[projectile.type];
				Vector2 origin = texture.Size() / 2;

				for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; ++i)
				{
					spriteBatch.Draw(texture, projectile.oldPos[i] + origin / 2 - Main.screenPosition, null, lightColor * (0.8f - 0.1f * i), projectile.oldRot[i], origin,
						projectile.scale, projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
				}
			}

			return (true);
		}

		private void CheckAliveState(AeroPlayer ep)
		{
			if (!ep.lumberjackSetBonus)
			{
				projectile.Kill();
				return;
			}
			projectile.timeLeft = 10;
		}

		private void FollowOwner(Player owner)
		{
			Vector2 targetPosition = owner.position;

			AttackTimer++;

			targetPosition.X -= (25 + owner.width / 2) * owner.direction;
			targetPosition.Y -= 30f + (float)Math.Sin(AttackTimer / 20) * 8f;
			projectile.rotation = (float)Math.Sin(AttackTimer / 35) * 0.33f;

			projectile.Center = Vector2.Lerp(projectile.Center, targetPosition, 0.15f);
			projectile.velocity *= 0.8f;
			projectile.direction = projectile.spriteDirection = owner.direction;
		}

		private void AttackNPC(NPC npc)
		{
			if (AttackTimer == 0)
			{
				projectile.netUpdate = true;
				AttackDirection = Main.rand.Next(2) * 2 - 1;
			}

			float amount = 0.035f;
			Vector2 targetPosition = npc.position;
			Vector2 positionOffset = new Vector2(120f, 200);

			// Attack warmup and positioning near target.
			if (++AttackTimer < AttackWarmup)
			{
				targetPosition.X -= positionOffset.X * AttackDirection;
				targetPosition.Y -= positionOffset.Y + (float)Math.Sin(AttackTimer / 10f) * 8f;
				projectile.rotation = MathHelper.Lerp(projectile.rotation, (projectile.oldPosition.X - projectile.position.X) * 0.01f, 0.1f);

				projectile.spriteDirection = projectile.direction = AttackDirection;
			}
			// Slash attack.
			else
			{
				float currentAttackTime = AttackTimer - AttackWarmup;
				float piFraction = MathHelper.Pi / AttackDuration * currentAttackTime;

				float cos = (float)Math.Cos(piFraction);

				Vector2 offset = positionOffset * new Vector2(-Math.Abs(cos) * AttackDirection, -cos);
				offset.X += projectile.width * 2 * AttackDirection;

				float desiredRotation = ((offset.Y + positionOffset.Y) / (2 * positionOffset.Y)) * MathHelper.Pi * AttackDirection;
				projectile.rotation = projectile.rotation.AngleLerp(desiredRotation, 0.1f);

				amount = 0.2f;
				targetPosition += offset;

				Dust.NewDustPerfect(projectile.Center + (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * projectile.width,
					ModContent.DustType<Dusts.Wood>(), projectile.velocity * 0.2f, 100);

				if (AttackTimer >= CompleteAttackTime)
				{
					AttackTimer = 0;
				}
			}
			projectile.Center = Vector2.Lerp(projectile.Center, targetPosition, amount);
		}

		#region Networking

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(AttackDirection);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			AttackDirection = reader.ReadInt32();
		}

		#endregion
	}
}
