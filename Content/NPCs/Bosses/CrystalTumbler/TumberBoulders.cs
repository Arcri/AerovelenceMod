using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerBoulder1 : ModProjectile
	{
		private NPC owner => Main.npc[NPC.FindFirstNPC(NPCType<CrystalTumbler>())];
        public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 3;
		}
		bool isntThrown = true;
        public override void SetDefaults()
		{
			Projectile.width = 60;
			Projectile.height = 46;
			Projectile.alpha =  255;
			Projectile.damage = 12;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}
		public override bool PreDraw(ref Color lightColor)
		{
			if (isntThrown == true)
			{
				Projectile.netUpdate = true;
				Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture + "Glow");
				Rectangle frame = texture.Frame(1, 3, 0, Projectile.frame);
				Main.EntitySpriteDraw(
					texture,
					Projectile.Center - Main.screenPosition,
					frame,
					Color.White,
					Projectile.rotation,
					frame.Size() / 2,
					Projectile.scale + 0.1f,
					SpriteEffects.None,
					0
				);
			}
			return true;
		}
		public override bool PreAI()
		{
			Projectile.alpha -= 10;
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, Color.Black, 1);

			NPC ownerNPC = owner;
			if (Projectile.ai[1] == 0)
			{
				Projectile.frame = Main.rand.Next(3);
				Projectile.netUpdate = true;
			}

			if (++Projectile.ai[1] >= 120)
			{
				Projectile.tileCollide = true;
				Projectile.netUpdate = true;
				if (Projectile.ai[1] == 120)
				{
					Vector2 desiredVelocity = Vector2.Normalize(Main.player[ownerNPC.target].Center - Projectile.Center) * 16f;
					isntThrown = false;
					Projectile.velocity = desiredVelocity;

					Projectile.netUpdate = true;
				}
			}
			else
			{
				Vector2 desiredPosition = ownerNPC.Center + new Vector2(-150 + (150 * Projectile.ai[0]), -150 + 25 * (float)Math.Sin(Projectile.ai[1] / 15));

				Vector2 desiredVelocity = desiredPosition - Projectile.Center;

				float speed = MathHelper.Lerp(0.1f, 12f, desiredVelocity.Length() / 100);
				Projectile.velocity = Vector2.Normalize(desiredVelocity) * speed;
				Projectile.netUpdate = true;
			}

			return (false);
		}
        public override void Kill(int timeLeft)
        {
			SoundEngine.PlaySound(SoundID.Item73, Projectile.Center);
		}
    }
}
