using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Content.Dusts
{
	public class DashTrailDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = false;

			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Dusts/DashTrailDust").Value;

			//Chooses a random frame out of the 12 different Dust lengths, see dust image to better understand
			dust.frame = new Rectangle(0, texture.Height / 12 * Main.rand.Next(12), texture.Width, texture.Height / 12);
		}

		public override bool Update(Dust dust)
		{
			//Makes dust face the direction it is heading correctly
			float plswowk = (float)Math.Atan(dust.velocity.Y / dust.velocity.X);
			dust.rotation = plswowk;


			if (dust.color == Color.Crimson)
            {
				dust.velocity.Y += .2f;
            }
			//dust.velocity.Y += .2f; //Gravity
			dust.position += dust.velocity; //Idk why we have to do this ourselves
			/*
			//So here is the problem, dusts dont have a .SpriteDirection which makes things much harder. Here is how I do it

			Vector2 normalizedVel = dust.velocity.SafeNormalize(Vector2.UnitX); //Normalize velocity to get the raw direction

			/* α = arccos[(xa * xb + ya * yb) / (√(xa^2 + ya^2) * √(xb^2 + yb^2))]
			 * ^This is the formula for finding the angle between 2 vectors, which I just googled
			 * We are going to find the angle between (1,0) and our normalized Vector, in order to find the rotation
			 * Excuse the biggass line 
			*/
			//float rotationangle = (float)Math.Acos((1 * normalizedVel.X + normalizedVel.Y * 0) / (Math.Sqrt(1 * 1 + normalizedVel.X * normalizedVel.X) * Math.Sqrt(0 * 0 + normalizedVel.Y * normalizedVel.Y)));
			//dust.rotation = rotationangle;


			//Main.NewText(rotationangle.ToString());
			

			dust.scale *= 0.98f;
			//float light = 0.35f * dust.scale;

			if (!dust.noLight)
				Lighting.AddLight(dust.position, dust.color.ToVector3() * (0.15f * dust.scale));

			if (dust.scale < 0.5f) //Shrinks dust and deletes it when it is very tiny
			{
				dust.active = false;
			}

			return false; 

		}
	}
}