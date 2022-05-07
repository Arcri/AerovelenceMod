#region Using directives

using System;
using System.IO;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

#endregion

namespace AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus
{
    public sealed class LumberjackAxe : ModProjectile
	{
		private int Target
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private float AttackTimer
		{
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		private int AttackDirection
		{
			get => (int)Projectile.localAI[1];
			set => Projectile.localAI[1] = value;
		}

		private readonly float MaxTargetingDistance = 500f;

		private readonly float AttackWarmup = 50;
		private readonly float AttackDuration = 20;
		private float CompleteAttackTime => AttackWarmup + AttackDuration;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 20;

			Projectile.penetrate = -1;

			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[Projectile.owner];
			AeroPlayer ep = owner.GetModPlayer<AeroPlayer>();

			CheckAliveState(ep);

			if (Target == 0)
			{
				float distance = MaxTargetingDistance;

				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					if (!Main.npc[i].CanBeChasedBy(Projectile))
						continue;

					float distanceToNPC = Projectile.Distance(Main.npc[i].Center);
					if (distanceToNPC < distance && Collision.CanHitLine(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
					{
						Target = i + 1;
						distance = distanceToNPC;
					}
				}

				if (Target != 0)
				{
					AttackTimer = 0;
					Projectile.netUpdate = true;
				}
				else
				{
					FollowOwner(owner);
				}
			}

			if (Target != 0)
			{
				NPC npc = Main.npc[Target - 1];

				if (!npc.CanBeChasedBy(Projectile) || Projectile.Distance(owner.Center) >= 1000)
				{
					Target = 0;
					return (false);
				}

				AttackNPC(npc);
			}

			Projectile.velocity *= 0.8f;

			return (false);
		}

		public override bool? CanDamage()
			=> Target != 0 && AttackTimer >= AttackWarmup;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			projHitbox.Inflate(15, 15);
			return (projHitbox.Intersects(targetHitbox));
		}

        public override bool PreDraw(ref Color lightColor)
        {
			if (Target != 0 && AttackTimer >= AttackWarmup)
			{
				Texture2D texture = (Texture2D)TextureAssets.Projectile[Projectile.type];
				Vector2 origin = texture.Size() / 2;

				for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; ++i)
				{
					Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + origin / 2 - Main.screenPosition, null, lightColor * (0.8f - 0.1f * i), Projectile.oldRot[i], origin,
						Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
				}
			}

			return (true);
		}

		private void CheckAliveState(AeroPlayer ep)
		{
			if (!ep.lumberjackSetBonus)
			{
				Projectile.Kill();
				return;
			}
			Projectile.timeLeft = 10;
		}

		private void FollowOwner(Player owner)
		{
			Vector2 targetPosition = owner.position;

			AttackTimer++;

			targetPosition.X -= (25 + owner.width / 2) * owner.direction;
			targetPosition.Y -= 30f + (float)Math.Sin(AttackTimer / 20) * 8f;
			Projectile.rotation = (float)Math.Sin(AttackTimer / 35) * 0.33f;

			Projectile.Center = Vector2.Lerp(Projectile.Center, targetPosition, 0.15f);
			Projectile.velocity *= 0.8f;
			Projectile.direction = Projectile.spriteDirection = owner.direction;
		}

		private void AttackNPC(NPC npc)
		{
			if (AttackTimer == 0)
			{
				Projectile.netUpdate = true;
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
				Projectile.rotation = MathHelper.Lerp(Projectile.rotation, (Projectile.oldPosition.X - Projectile.position.X) * 0.01f, 0.1f);

				Projectile.spriteDirection = Projectile.direction = AttackDirection;
			}
			// Slash attack.
			else
			{
				float currentAttackTime = AttackTimer - AttackWarmup;
				float piFraction = MathHelper.Pi / AttackDuration * currentAttackTime;

				float cos = (float)Math.Cos(piFraction);

				Vector2 offset = positionOffset * new Vector2(-Math.Abs(cos) * AttackDirection, -cos);
				offset.X += Projectile.width * 2 * AttackDirection;

				float desiredRotation = ((offset.Y + positionOffset.Y) / (2 * positionOffset.Y)) * MathHelper.Pi * AttackDirection;
				Projectile.rotation = Projectile.rotation.AngleLerp(desiredRotation, 0.1f);

				amount = 0.2f;
				targetPosition += offset;

				Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * Projectile.width,
					ModContent.DustType<Dusts.Wood>(), Projectile.velocity * 0.2f, 100);

				if (AttackTimer >= CompleteAttackTime)
				{
					AttackTimer = 0;
				}
			}
			Projectile.Center = Vector2.Lerp(Projectile.Center, targetPosition, amount);
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
