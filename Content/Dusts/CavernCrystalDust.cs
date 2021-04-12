#region Using directives

using Terraria;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Dusts
{
	public sealed class CavernCrystalDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noLight = false;
			dust.noGravity = true;

			dust.alpha = 100;
			dust.fadeIn = 1f;
			dust.scale = 0.25f;
			dust.velocity *= 0;
			dust.color = new Color(200, 200, 200);
			dust.frame = new Rectangle(0, Main.rand.Next(3) * 6, 6, 6);
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
			=> dust.color;

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * 0.1f;

			dust.velocity.X += (float)System.Math.Sin(Main.time / 20) * 0.01f;
			dust.velocity.X = MathHelper.Clamp(dust.velocity.X, -0.5f, 0.5f);

			dust.velocity.Y -= 0.05f;
			if (dust.velocity.Y < 1f)
			{
				dust.velocity.Y = -1f;
			}

			if (dust.fadeIn > dust.scale)
			{
				dust.scale += 0.01f;
			}
			else
			{
				dust.fadeIn = 0;
				
				if ((dust.scale -= 0.005f) < 0.1f)
				{
					dust.active = false;
				}
			}

			Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);

			return (false);
		}
	}
}
