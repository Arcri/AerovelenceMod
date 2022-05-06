#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class HuntressJavelin : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.MinionShot[Projectile.type] = true;
		}
		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 10;
			
			Projectile.aiStyle = 1;
			
			Projectile.friendly = true;
			Projectile.tileCollide = false;
		}

		public override void PostAI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		public override void Kill(int timeLeft)
		{
			Vector2 startPos = Projectile.position;
			Vector2 oldVelocity = Vector2.Normalize(Projectile.oldVelocity);

			startPos += oldVelocity * 12f;

			for (int i = 0; i < 8; ++i)
			{
				Dust newDust = Dust.NewDustDirect(startPos, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Wood>(), 0, 0, 0, default, 0.8f);
				newDust.position = (newDust.position + Projectile.Center) / 2;
				newDust.velocity += Projectile.oldVelocity * 0.4f;
				newDust.velocity *= 0.5f;
				startPos -= oldVelocity * 8f;
			}
		}
	}
}
