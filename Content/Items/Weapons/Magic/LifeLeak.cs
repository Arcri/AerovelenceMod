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
			DisplayName.SetDefault("Lifeleak");
			Tooltip.SetDefault("'You feel numb holding it'\nUses your life as ammo\nProjectiles shot from this weapon heal you");
		}
		public override void SetDefaults()
		{
			item.width = 34;
			item.height = 26;
			item.magic = true;
			item.damage = 26;
			item.mana = 8;
			item.noMelee = true;
			item.useStyle = 5;
			item.useTime = 20;
			item.useAnimation = 20;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<LifeSyphon>();
			item.shootSpeed = 11.4f;
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile p = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, Main.rand.Next(new int[] { 0, 0, 0, 1 }));
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
			projectile.width = 16;
			projectile.height = 16;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.magic = true;
		}
		public override void AI()
		{
			if (projectile.ai[0] == 0)
			{
				projectile.velocity.Y += .014f;
				if (Main.rand.NextFloat() < .8f)
				{
					Dust d = Dust.NewDustDirect(projectile.Center, 1, 1, DustID.Blood);
					d.scale = Main.rand.NextFloat(.9f, 1.2f);
					d.velocity = Vector2.Zero;
					d.alpha = 127;
				}
			}
			if (projectile.ai[0] == 1)
			{
				projectile.velocity *= .979f;

				//Dusts
				for (int i = 0; i < 9; i++)
				{
					if (Main.rand.NextFloat() < .8f)
					{
						Dust d = Dust.NewDustDirect(projectile.Center + (Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * projectile.Size * .5f), 1, 1, DustID.Blood);
						d.scale = Main.rand.NextFloat(.9f, 1.2f);
						d.velocity = Vector2.Zero;
						d.alpha = 127;
					}
				}
			}
			if (projectile.ai[0] == 2)
			{
				Player player = Main.player[projectile.owner];
				projectile.velocity = projectile.DirectionTo(player.Center) * projectile.ai[1];
				projectile.ai[1] += .14f;
				if (projectile.DistanceSQ(player.Center) < 36864)
				{
					player.HealEffect(projectile.damage);
					player.statLife += projectile.damage;
					projectile.Kill();
				}
				Dust d = Dust.NewDustDirect(projectile.Center, 1, 1, DustID.Blood);
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
			if (projectile.ai[0] == 1)
			{
				Projectile p = Projectile.NewProjectileDirect(projectile.position, Vector2.Zero, ModContent.ProjectileType<LifeSyphon>(), projectile.damage / 3, 0f, projectile.owner, 2);
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