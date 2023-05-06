using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Content.Dusts
{
	public class SmokeDust : ModDust
	{
        public override string Texture => "AerovelenceMod/Content/Dusts/WhiteSmoke";

        public override void OnSpawn(Dust dust)
		{
			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Dusts/WhiteSmoke").Value;
			dust.frame = new Rectangle(0, texture.Height / 5 * Main.rand.Next(5), texture.Width, texture.Height / 5);
		}

		public override bool Update(Dust dust)
		{

            dust.color = Color.Lerp(dust.color, Color.Black, 0.02f);
            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.color.R * 0.002f, dust.color.G * 0.002f, dust.color.B * 0.002f);

            dust.noGravity = true;

            dust.position += dust.velocity;
            dust.velocity *= 0.96f;
            dust.scale *= 0.98f;
            dust.alpha += 8; //12


            if (dust.scale < 0.1f)
            {
                dust.active = false;
            }
            if (dust.alpha >= 250)
                dust.active = false;

            dust.rotation += dust.velocity.X * 0.01f;

            return false;

		}
	}

    public class SmokeDustFade : ModDust
    {
        public override string Texture => "AerovelenceMod/Content/Dusts/WhiteSmoke";

        public override void OnSpawn(Dust dust)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Dusts/WhiteSmoke").Value;
            dust.frame = new Rectangle(0, texture.Height / 5 * Main.rand.Next(5), texture.Width, texture.Height / 5);
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.color.R * 0.002f, dust.color.G * 0.002f, dust.color.B * 0.002f);

            if (!dust.noGravity)
                dust.velocity.Y -= 0.02f;

            dust.position += dust.velocity;
            dust.velocity *= 0.9f;
            dust.scale *= 1.01f;
            dust.alpha += 6;


            if (dust.scale < 0.1f)
            {
                dust.active = false;
            }
            if (dust.alpha >= 250)
                dust.active = false;

            dust.rotation += dust.velocity.X * 0.01f;

            return false;

        }
    }
    
    public class LineSmokeDust : ModDust
    {
        public override string Texture => "AerovelenceMod/Content/Dusts/Lines2";

        public override void OnSpawn(Dust dust)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Dusts/Lines2").Value;
            dust.frame = new Rectangle(0, texture.Height / 7 * Main.rand.Next(7), texture.Width, texture.Height / 7);
        }

        public override bool Update(Dust dust)
        {
            //Makes dust face the direction it is heading correctly
            float plswowk = (float)Math.Atan(dust.velocity.Y / dust.velocity.X);
            dust.rotation = plswowk;

            dust.position += dust.velocity;
            dust.velocity *= 0.98f;
            dust.scale *= 0.98f;
            dust.alpha += 6;

            if (dust.scale < 0.2f) 
            {
                dust.active = false;
            }
            if (dust.alpha >= 250)
                dust.active = false;
            return false;

        }
    }
}