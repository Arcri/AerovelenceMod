using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
	// TODO: Eldrazi - Comments.
	public class TumblerBoulder1 : ModProjectile
	{
		private NPC owner => Main.npc[NPC.FindFirstNPC(ModContent.NPCType<CrystalTumbler>())];

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 44;

			projectile.alpha =  255;

			projectile.hostile = true;
			projectile.friendly = false;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}

		public override bool PreAI()
		{
			projectile.alpha -= 10;
			Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 0, Color.Black, 1);

			NPC ownerNPC = owner;

			if (++projectile.ai[1] >= 120)
			{
				projectile.tileCollide = true;

				if (projectile.ai[1] == 120)
				{
					Vector2 desiredVelocity = Vector2.Normalize(Main.player[ownerNPC.target].Center - projectile.Center) * 16f;

					projectile.velocity = desiredVelocity;

					projectile.netUpdate = true;
				}
			}
			else
			{
				Vector2 desiredPosition = ownerNPC.Center + new Vector2(-150 + (150 * projectile.ai[0]), -150 + 25 * (float)Math.Sin(projectile.ai[1] / 15));

				Vector2 desiredVelocity = desiredPosition - projectile.Center;

				float speed = MathHelper.Lerp(0.1f, 12f, desiredVelocity.Length() / 100);
				projectile.velocity = Vector2.Normalize(desiredVelocity) * speed;
			}

			return (false);
		}
	}
}
