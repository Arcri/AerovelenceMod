using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
	public class LifeLeak : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Life-Leak");
			Tooltip.SetDefault("'You feel numb holding it'\nUses your life as ammo\nProjectiles shot from this weapon heal you");
		}
		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 26;
			Item.DamageType = DamageClass.Magic;
			Item.damage = 26;
			Item.mana = 8;
			Item.noMelee = true;
			Item.useStyle = 5;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<LifeSyphon>();
			Item.shootSpeed = 11.4f;
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
        Projectile p = Projectile.NewProjectileDirect(source, position, new Vector2(velocity.X, velocity.Y), type, damage, 3f, player.whoAmI, Main.rand.Next(new int[] { 0, 0, 0, 1 }));
			if (p.ai[0] == 0)
				p.penetrate = 1;
			p.netUpdate = true;
			return false;
		}
	}

	public class LifeSyphon : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Magic;
		}
		public override void AI()
		{
			if (Projectile.ai[0] == 0)
			{
				Projectile.velocity.Y += .014f;
				if (Main.rand.NextFloat() < .8f)
				{
					Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Blood);
					d.scale = Main.rand.NextFloat(.9f, 1.2f);
					d.velocity = Vector2.Zero;
					d.alpha = 127;
				}
			}
			if (Projectile.ai[0] == 1)
			{
				Projectile.velocity *= .979f;

				//Dusts
				for (int i = 0; i < 9; i++)
				{
					if (Main.rand.NextFloat() < .8f)
					{
						Dust d = Dust.NewDustDirect(Projectile.Center + (Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Projectile.Size * .5f), 1, 1, DustID.Blood);
						d.scale = Main.rand.NextFloat(.9f, 1.2f);
						d.velocity = Vector2.Zero;
						d.alpha = 127;
					}
				}
			}
			if (Projectile.ai[0] == 2)
			{
				Player player = Main.player[Projectile.owner];
				Projectile.velocity = Projectile.DirectionTo(player.Center) * Projectile.ai[1];
				Projectile.ai[1] += .14f;
				if (Projectile.DistanceSQ(player.Center) < 36864)
				{
					player.HealEffect(Projectile.damage);
					player.statLife += Projectile.damage;
					Projectile.Kill();
				}
				Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Blood);
				d.scale = Main.rand.NextFloat(.9f, 1.2f);
				d.velocity = Vector2.Zero;
				d.alpha = 127;
				d.noLight = true;
				d.noGravity = true;
			}
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.GetGlobalNPC<LifeLeakNPC>().AdjustSpeed(.6f);
			if (Projectile.ai[0] == 1)
			{
				Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<LifeSyphon>(), Projectile.damage / 3, 0f, Projectile.owner, 2);
				p.friendly = false;
				p.netUpdate = true;
			}
		}
	}

	public class LifeLeakNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public float spdMult = 1f;
		public override bool PreAI(NPC npc)
		{
			ResetSpeed(npc, spdMult);
			return base.PreAI(npc);
		}
		public override void PostAI(NPC npc)
		{
			SetSpeed(npc, spdMult);
			spdMult = MathHelper.Lerp(spdMult, 1, .003f);
			base.PostAI(npc);
		}

		public void AdjustSpeed(float aimMult)
		{
			spdMult = MathHelper.Lerp(spdMult, aimMult, .5f);
		}

		/// <summary>
		/// Call this in <seealso cref="PostAI(NPC)"/>
		/// </summary>
		/// <param name="npc"></param>
		/// <param name="mult"></param>
		/// <param name="ignoreGrav"></param>
		private void SetSpeed(NPC npc, float mult, bool ignoreGrav = false)
		{
			npc.velocity.X *= mult;
			if (npc.noGravity || ignoreGrav)
				npc.velocity.Y *= mult;
		}
		/// <summary>
		/// Call this in <seealso cref="PreAI(NPC)"/>
		/// </summary>
		/// <param name="npc"></param>
		/// <param name="mult"></param>
		/// <param name="ignoreGrav"></param>
		private void ResetSpeed(NPC npc, float mult, bool ignoreGrav = false)
		{
			npc.velocity.X /= mult;
			if (npc.noGravity || ignoreGrav)
				npc.velocity.Y /= mult;
		}
	}
}