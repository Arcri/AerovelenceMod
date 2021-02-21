using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Dusts
{
	public class Charge : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 0.4f;
			dust.noGravity = true;
			dust.noLight = true;
			dust.scale *= 0.99f;
		}
	}
}