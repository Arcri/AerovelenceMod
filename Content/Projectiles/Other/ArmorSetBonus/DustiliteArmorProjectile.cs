#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus
{
    internal sealed class DustiliteArmorProjectile : ModProjectile
	{
		private readonly float centerOffset = 32;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dustilite Shield");
		}
		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 20;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player p = Main.player[projectile.owner];
			AeroPlayer ap = p.GetModPlayer<AeroPlayer>();

			// Check to see if the projectile needs to be alive.
			if (!p.HasBuff(ModContent.BuffType<Buffs.DustiliteShield>()))
			{
				projectile.Kill();
				return (false);
			}
			projectile.timeLeft = 10;

			// Positioning of projectile (client/owner side).
			if (Main.myPlayer == projectile.owner)
			{
				Vector2 playerCenter = p.position + new Vector2(0, p.gfxOffY + p.height / 4);
				Vector2 targetDirection = Vector2.Normalize(Main.MouseWorld - playerCenter);

				projectile.position = playerCenter + targetDirection * centerOffset;

				projectile.rotation = targetDirection.ToRotation();
			}

			// Projectile intersection loop.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (i == projectile.whoAmI || !Main.projectile[i].active || Main.projectile[i].friendly)
					continue;

				if (Main.projectile[i].Hitbox.Intersects(projectile.Hitbox))
				{
					projectile.ai[0]++;
					projectile.netUpdate = true;

					Main.projectile[i].Kill();
					Main.projectile[i].netUpdate = true;
				}
			}

			if (projectile.ai[0] >= 5)
			{
				p.AddBuff(ModContent.BuffType<Buffs.DustiliteDefense>(), 420);
				ap.dustiliteSetBonusCooldown = ap.defaultDustiliteSetBonusCooldown;
				projectile.Kill();
			}

			return (false);
		}

		public override bool CanDamage()
			=> false;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.PinkCrystalShard)];
				newDust.velocity *= 1.4f;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[projectile.type];
			Vector2 origin = texture.Size() / 2;

			Vector2 drawPosition = new Vector2((int)(projectile.Center.X - Main.screenPosition.X), (int)(projectile.Center.Y - Main.screenPosition.Y));
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);

			return (false);
		}
	}
}
