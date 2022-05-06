using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
	public class ThunderLanceProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thunder Lance");
		}

		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.aiStyle = 19;
			Projectile.penetrate = -1;
			Projectile.alpha = 0;

			Projectile.hide = true;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
		}



		public float MovementFactor
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI()
		{
			Player projOwner = Main.player[Projectile.owner];
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Projectile.direction = projOwner.direction;
			projOwner.heldProj = Projectile.whoAmI;
			projOwner.itemTime = projOwner.itemAnimation;
			Projectile.position.X = ownerMountedCenter.X - Projectile.width / 2;
			Projectile.position.Y = ownerMountedCenter.Y - Projectile.height / 2;
			if (!projOwner.frozen)
			{
				if (MovementFactor == 0f)
				{
					MovementFactor = 3f;
					Projectile.netUpdate = true;
				}
				if (projOwner.itemAnimation < projOwner.itemAnimationMax / 3)
				{
					MovementFactor -= 2.4f;
				}
				else
				{
					MovementFactor += 2.1f;
				}
			}
			Projectile.position += Projectile.velocity * MovementFactor;
			if (projOwner.itemAnimation == 0)
			{
				Projectile.Kill();
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
			if (Projectile.spriteDirection == -1)
			{
				Projectile.rotation -= MathHelper.ToRadians(90f);
			}

			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustType<Sparkle>(),
					Projectile.velocity.X * .2f, Projectile.velocity.Y * .2f, 200, Scale: 1.2f);
				dust.velocity += Projectile.velocity * 0.3f;
				dust.velocity *= 0.2f;
			}
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustType<Sparkle>(),
					0, 0, 254, Scale: 0.3f);
				dust.velocity += Projectile.velocity * 0.5f;
				dust.velocity *= 0.5f;
			}
			if (projOwner.itemAnimation == projOwner.itemAnimationMax - 1)
				Projectile.NewProjectile(Projectile.Center.X + Projectile.velocity.X, Projectile.Center.Y + Projectile.velocity.Y, Projectile.velocity.X * 2f, Projectile.velocity.Y * 2, ProjectileType<ElectricitySpark>(), Projectile.damage, Projectile.knockBack * 0.85f, Projectile.owner, 0f, 0f);
			}
	}
}