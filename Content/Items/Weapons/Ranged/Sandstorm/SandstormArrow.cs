using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.Ranged.Sandstorm
{
	public class SandstormArrow : ModProjectile
	{

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sandstorm Arrow");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 7;
			projectile.height = 7;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.penetrate = 1;
			projectile.aiStyle = 1;
			projectile.damage = 7;
			projectile.tileCollide = true;
			projectile.timeLeft = 700;
			projectile.ignoreWater = false;

		}


		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}

        public override void Kill(int timeLeft)
        {
			for (int i = 0; i < 6; i++)
			{
				int mydust = Dust.NewDust(projectile.Center, 2, 2, DustID.AmberBolt, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f,0, Color.SandyBrown, 0.5f);
				Main.dust[mydust].noGravity = false;
				
			}
			Main.PlaySound(SoundID.Item27.WithVolume(0.6f).WithPitchVariance(-1f), projectile.Center);

		}
    }
}


