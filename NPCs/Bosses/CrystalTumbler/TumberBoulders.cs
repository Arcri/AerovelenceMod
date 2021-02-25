using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
<<<<<<< Updated upstream
using static Terraria.ModLoader.ModContent;
=======

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
>>>>>>> Stashed changes

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
	public class TumblerBoulder1 : ModProjectile
	{
<<<<<<< Updated upstream
		int t;
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			projectile.width = 44;
			projectile.height = 44;
=======
		private NPC owner => Main.npc[NPC.FindFirstNPC(ModContent.NPCType<CrystalTumbler>())];
        public override void SetStaticDefaults()
        {
			Main.projFrames[projectile.type] = 3;
		}
		bool isntThrown = true;
        public override void SetDefaults()
		{
			projectile.width = 60;
			projectile.height = 46;

>>>>>>> Stashed changes
			projectile.alpha =  255;
			projectile.damage = 12;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}
<<<<<<< Updated upstream
		public override void AI()
		{
			projectile.alpha -= 10;
			projectile.velocity.Y *= 99.8f;
			t++;
			projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 0, Color.Black, 1);
			Main.dust[dust1].velocity /= 2f;
			if (t > 25)
			{
				projectile.tileCollide = true;
=======
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (isntThrown == true)
			{
				Texture2D texture = ModContent.GetTexture(Texture + "Glow");
				Rectangle frame = texture.Frame(1, 3, 0, projectile.frame);
				spriteBatch.Draw(
					texture,
					projectile.Center - Main.screenPosition,
					frame,
					Color.White,
					projectile.rotation,
					frame.Size() / 2,
					projectile.scale + 0.1f,
					SpriteEffects.None,
					0f
				);
			}
			return true;
		}
		public override bool PreAI()
		{
			projectile.alpha -= 10;
			Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 0, Color.Black, 1);

			NPC ownerNPC = owner;
			if(projectile.ai[1] == 0)
            {
				projectile.frame = Main.rand.Next(3);
            }
			if (++projectile.ai[1] >= 120)
			{
				projectile.tileCollide = true;

				if (projectile.ai[1] == 120)
				{
					Vector2 desiredVelocity = Vector2.Normalize(Main.player[ownerNPC.target].Center - projectile.Center) * 16f;
					isntThrown = false;
					projectile.velocity = desiredVelocity;

					projectile.netUpdate = true;
				}
>>>>>>> Stashed changes
			}
		}
	}
}

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
	public class TumblerBoulder2 : ModProjectile
	{
		int t;
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			projectile.width = 44;
			projectile.height = 44;
			projectile.alpha = 255;
			projectile.damage = 12;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}
		public override void AI()
		{
			projectile.alpha -= 10;
			projectile.velocity.Y *= 99.8f;
			t++;
			projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 0, Color.Black, 1);
			Main.dust[dust1].velocity /= 2f;
			if (t > 25)
			{
				projectile.tileCollide = true;
			}
		}
	}
<<<<<<< Updated upstream
}

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
	public class TumblerBoulder3 : ModProjectile
	{
		int t;
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			projectile.width = 44;
			projectile.height = 44;
			projectile.damage = 12;
			projectile.alpha = 255;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}
		public override void AI()
		{
			projectile.alpha -= 10;
			projectile.velocity.Y *= 99.8f;
			t++;
			projectile.velocity *= 1.01f;
			int dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 0, Color.Black, 1);
			Main.dust[dust1].velocity /= 2f;
			if (t > 25)
			{
				projectile.tileCollide = true;
			}
		}
	}
=======
>>>>>>> Stashed changes
}