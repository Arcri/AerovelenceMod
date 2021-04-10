#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class HuntressSpear : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.MinionShot[projectile.type] = true;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 10;
			
			projectile.aiStyle = 1;
			
			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override void PostAI()
		{
			projectile.rotation = projectile.velocity.ToRotation();
		}

		public override void Kill(int timeLeft)
		{
			Vector2 startPos = projectile.position;
			Vector2 oldVelocity = Vector2.Normalize(projectile.oldVelocity);

			startPos += oldVelocity * 12f;

			for (int i = 0; i < 8; ++i)
			{
				Dust newDust = Dust.NewDustDirect(startPos, projectile.width, projectile.height, ModContent.DustType<Dusts.Wood>(), 0, 0, 0, default, 0.8f);
				newDust.position = (newDust.position + projectile.Center) / 2;
				newDust.velocity += projectile.oldVelocity * 0.4f;
				newDust.velocity *= 0.5f;
				startPos -= oldVelocity * 8f;
			}
		}
	}
}
