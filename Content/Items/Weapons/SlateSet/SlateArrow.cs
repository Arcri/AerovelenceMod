using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using ReLogic.Content;

namespace AerovelenceMod.Content.Items.Weapons.SlateSet
{
    public class SlateArrow : ModProjectile
    {
		int timer;
		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.extraUpdates = 2;
		}
		public override void AI()
		{
			Projectile.velocity.Y += 0.03f;
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (timer % 3 == 0)
            {
				int dust = Dust.NewDust(Projectile.Center - new Vector2(5), 0, 0, DustID.Grass);
				Main.dust[dust].velocity *= 1f;
			}

			timer++;
		}
		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item10);
			for (int i = 0; i < 15; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 0, 0, DustID.WoodFurniture, 0, 0, Projectile.alpha);
				dust.velocity *= 0.55f;
				dust.velocity += Projectile.velocity * 0.5f;
				dust.scale *= 1.25f;
				dust.noGravity = true;
			}
		}
	}
}
