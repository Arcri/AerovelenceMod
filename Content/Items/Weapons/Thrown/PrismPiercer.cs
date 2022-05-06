using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class PrismPiercer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Prism Piercer");
			Tooltip.SetDefault("Fires knives that electrify enemies");
		}

		public override void SetDefaults()
		{
			Item.UseSound = SoundID.Item1;
			Item.crit = 16;
			Item.damage = 24;
			Item.DamageType = DamageClass.Melee;
			Item.width = 22;
			Item.height = 40;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 4;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.shoot = Mod.Find<ModProjectile>("PrismPiercerProjectile").Type;
			Item.shootSpeed = 16f;
		}
	}
}

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class PrismPiercerProjectile : ModProjectile
	{
		public bool e;
		public float rot = 0.5f;
		public int i;
		public override void SetDefaults()
		{
			Projectile.width = 38;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.height = 38;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 120;
			Projectile.damage = 16;
			Projectile.penetrate = -1;
			Projectile.light = 0.5f;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 132, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
			}
			SoundEngine.PlaySound(SoundID.Item10);
			return true;
		}
		public override void AI()
		{
			i++;
			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 132);
				Main.dust[dust].noGravity = true;
			}
			Projectile.alpha += 2;
			Projectile.rotation += rot;
			rot *= 0.99f;
			if (Projectile.ai[0] == 0f)
			{
				Projectile.ai[0] = Projectile.velocity.X;
				Projectile.ai[1] = Projectile.velocity.Y;
			}
			if (Math.Sqrt(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y) > 2.0)
			{
				Projectile.velocity *= 0.99f;
			}
			int[] array = new int[20];
			int num438 = 0;
			float num439 = 300f;
			bool flag14 = false;
			float num440 = 0f;
			float num441 = 0f;
			for (int num442 = 0; num442 < 200; num442++)
			{
				if (!Main.npc[num442].CanBeChasedBy(this))
				{
					continue;
				}
				float num443 = Main.npc[num442].position.X + Main.npc[num442].width / 2;
				float num444 = Main.npc[num442].position.Y + Main.npc[num442].height / 2;
				float num445 = Math.Abs(Projectile.position.X + Projectile.width / 2 - num443) + Math.Abs(Projectile.position.Y + Projectile.height / 2 - num444);
				if (num445 < num439 && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[num442].Center, 1, 1))
				{
					if (num438 < 20)
					{
						array[num438] = num442;
						num438++;
						num440 = num443;
						num441 = num444;
					}
					flag14 = true;
				}
			}
			if (Projectile.timeLeft < 30)
			{
				flag14 = false;
			}
			if (flag14)
			{
				int num446 = Main.rand.Next(num438);
				num446 = array[num446];
				num440 = Main.npc[num446].position.X + Main.npc[num446].width / 2;
				num441 = Main.npc[num446].position.Y + Main.npc[num446].height / 2;
				Projectile.localAI[0] += 1f;
				if (Projectile.localAI[0] > 8f)
				{
					Projectile.localAI[0] = 0f;
					float num447 = 6f;
					Vector2 vector31 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
					vector31 += Projectile.velocity * 4f;
					float num448 = num440 - vector31.X;
					float num449 = num441 - vector31.Y;
					float num450 = (float)Math.Sqrt(num448 * num448 + num449 * num449);
					float num451 = num450;
					num450 = num447 / num450;
					num448 *= num450;
					num449 *= num450;
					Projectile.NewProjectile(vector31.X, vector31.Y, num448, num449, ModContent.ProjectileType<PrismPiercerProjectile2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
				}
			}
		}
	}
}

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class PrismPiercerProjectile2 : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.damage = 10;
			Projectile.height = 8;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.extraUpdates = 100;
			Projectile.timeLeft = 100;
		}
		public override void AI()
		{
			for (int num452 = 0; num452 < 4; num452++)
			{
				Vector2 position = Projectile.position;
				position -= Projectile.velocity * (num452 * 0.25f);
				Projectile.alpha = 255;
				int num453 = Dust.NewDust(position, 1, 1, 160);
				Main.dust[num453].position = position;
				Main.dust[num453].position.X += Projectile.width / 2;
				Main.dust[num453].position.Y += Projectile.height / 2;
				Main.dust[num453].scale = Main.rand.Next(70, 110) * 0.013f;
				Dust dust77 = Main.dust[num453];
				Dust dust2 = dust77;
				dust2.velocity *= 0.2f;
			}
			return;
		}
	}
}