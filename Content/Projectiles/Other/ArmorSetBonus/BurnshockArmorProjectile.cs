#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

#endregion

namespace AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus
{
    internal sealed class BurnshockArmorProjectile : ModProjectile
	{
		private readonly float centerOffset = 32;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnshock Shield");
		}
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 20;

			Projectile.friendly = true;
			Projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player p = Main.player[Projectile.owner];
			AeroPlayer ap = p.GetModPlayer<AeroPlayer>();

			// Check to see if the projectile needs to be alive.
			if (!p.HasBuff(ModContent.BuffType<Buffs.BurnshockShield>()))
			{
				Projectile.Kill();
				return (false);
			}
			Projectile.timeLeft = 10;

			// Positioning of projectile (client/owner side).
			if (Main.myPlayer == Projectile.owner)
			{
				Vector2 playerCenter = p.position + new Vector2(0, p.gfxOffY + p.height / 4);
				Vector2 targetDirection = Vector2.Normalize(Main.MouseWorld - playerCenter);

				Projectile.position = playerCenter + targetDirection * centerOffset;

				Projectile.rotation = targetDirection.ToRotation();
			}

			// Projectile intersection loop.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (i == Projectile.whoAmI || !Main.projectile[i].active || Main.projectile[i].friendly)
					continue;

				if (Main.projectile[i].Hitbox.Intersects(Projectile.Hitbox))
				{
					Projectile.ai[0]++;
					Projectile.netUpdate = true;

					Main.projectile[i].Kill();
					Main.projectile[i].netUpdate = true;
				}
			}

			if (Projectile.ai[0] >= 5)
			{
				p.AddBuff(ModContent.BuffType<Buffs.BurnshockDefense>(), 420);
				ap.burnshockSetBonusCooldown = ap.defaultBurnshockSetBonusCooldown;
				Projectile.Kill();
			}

			return (false);
		}


		public override bool? CanDamage()
			=> false;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkCrystalShard)];
				newDust.velocity *= 1.4f;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.instance.LoadProjectile(Projectile.type);
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			Vector2 origin = texture.Size() / 2;

			Vector2 drawPosition = new Vector2((int)(Projectile.Center.X - Main.screenPosition.X), (int)(Projectile.Center.Y - Main.screenPosition.Y));
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			return (false);
		}
	}
}
