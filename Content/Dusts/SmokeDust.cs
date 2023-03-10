using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Content.Dusts
{
	public class SmokeDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Dusts/SmokeDust").Value;
			dust.frame = new Rectangle(0, texture.Height / 3 * Main.rand.Next(3), texture.Width, texture.Height / 3);
		}

		public override bool Update(Dust dust)
		{

            dust.color = Color.Lerp(dust.color, Color.Gray, 0.02f);
            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.color.R * 0.002f, dust.color.G * 0.002f, dust.color.B * 0.002f);

            dust.noGravity = true;

            dust.position += dust.velocity;
            dust.velocity *= 0.92f;
            dust.scale *= 0.98f;
            dust.alpha += 12;


            if (dust.scale < 0.1f)
            {
                dust.active = false;
            }
            if (dust.alpha >= 250)
                dust.active = false;

            return false;

		}
	}
}