using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Dusts
{
	public class Charge : ModDust
	{
		int i;
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.noLight = true;
			dust.scale *= 0.99f;
			dust.active = true;
		}
		public override bool Update(Dust dust)
		{
			float num112 = dust.scale;
			if (num112 > 1f)
			{
				num112 = 1f;
			}
			if (!dust.noLight)
			{
				Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), num112 * 0.2f, num112 * 0.7f, num112 * 1f);
			}
			if (dust.noGravity)
			{
				dust.velocity *= 0.93f;
				if (dust.fadeIn == 0f)
				{
					dust.scale += 0.0025f;
				}
			}
			dust.velocity *= new Vector2(0.97f, 0.99f);
			if (dust.customData != null && dust.customData is Player)
			{
				Player player7 = (Player)dust.customData;
				dust.position += player7.position - player7.oldPosition;
			}
			dust.scale -= 0.01f;
			if (dust.scale < 1f)
			{
				dust.active = true;
				dust.scale = 1f;
			}
			dust.velocity.Y = -1.5f;
		//	if (i % 2 == 0)
		//	{
				//dust.velocity.Y += Main.rand.NextFloat(-1, 1);
			//}
			i++;
			if (i % 2 == 0)
			{
				dust.velocity.X += Main.rand.NextFloat(-5, 5);
			}
			return true;
		}
	}
}