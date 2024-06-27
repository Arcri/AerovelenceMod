using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Content.Dusts
{
	public class StillDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Dusts/StillDust").Value;
			dust.frame = new Rectangle(0, texture.Height / 5 * Main.rand.Next(5), texture.Width, texture.Height / 5);
		}

		public override bool Update(Dust dust)
		{

            dust.color = Color.Lerp(dust.color, Color.Gray, 0.02f);
            //Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.color.R * 0.005f, dust.color.G * 0.005f, dust.color.B * 0.005f);

            dust.noGravity = true;

            dust.position += dust.velocity;
            dust.velocity *= 0.92f;
            dust.scale *= 0.98f;
            dust.alpha += 12;


            if (dust.scale < 0.5f)
            {
                dust.active = false;
            }
            return false;

		}
	}
}