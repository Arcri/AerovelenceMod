using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Projectiles
{
	public class DarkLaser : ModProjectile
	{
		int t;
		public override void SetDefaults()
		{
			projectile.width = 5;
			projectile.height = 5;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.alpha = 255;
			projectile.extraUpdates = 2;
			projectile.scale = 1f;
			projectile.timeLeft = 600;
			projectile.magic = true;
			projectile.ignoreWater = true;
		}
	}
}