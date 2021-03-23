using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.Dusts
{
	public class WispDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= Main.rand.NextFloat(0.9f, 1.1f);
			dust.velocity.X += Main.rand.Next(-10, 11) * 0.3f;
			dust.velocity.Y += Main.rand.Next(-10, 11) * 0.3f;
			dust.noGravity = true;
			dust.scale *= Main.rand.NextFloat(0.7f, 1.5f);
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity *= 0.9f;
			dust.rotation += dust.velocity.X * 0.15f;
			dust.scale *= 0.93f;
			Lighting.AddLight(dust.position, 0, 1f * (255 - dust.alpha) / 255f, 1f * (255 - dust.alpha) / 255f);
			if (dust.scale <= 0.3f)
			{
				dust.active = false;
			}
			return false;
		}
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
	}
}