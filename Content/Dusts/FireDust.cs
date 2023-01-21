using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Content.Dusts
{
	public class FireDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.noLight = true;
		}

		public override bool Update(Dust dust)
		{
			Lighting.AddLight(dust.position, Color.Orange.ToVector3() * 0.02f);

			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X;
			dust.scale -= 0.01f;
			if (dust.scale < 0.5f)
				dust.active = false;
			return false;
		}
	}
}