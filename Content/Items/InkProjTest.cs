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
using AerovelenceMod.Common.Utilities;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items
{
    public class InkProjTest : ModProjectile
    {
		float timer = 0;
		public Color color = Color.White;
		public float overallSize = 1f;
		public int lineWidth = 3;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ink Test");
		}

        public override void SetDefaults()
		{
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 400;
			Projectile.tileCollide = true;
			Projectile.scale = 1f;

		}


		public float xScale = 1f;
		public float yScale = 1f;
		
		public override void AI()
        {
			Player player = Main.player[Projectile.owner];

			Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

			Projectile.velocity.X *= 0.99f;

			if (timer < 20)
				Projectile.velocity.Y += 0.2f;
			else
				Projectile.velocity.Y += 0.4f;

			if (Main.rand.NextBool(20) && Main.rand.NextBool())
			{
				ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
				int a = GlowDustHelper.DrawGlowDust(Projectile.Center, 1, 1, ModContent.DustType<Dusts.GlowDusts.GlowSpark>(), Projectile.velocity.X,
					Projectile.velocity.Y, color, Main.rand.NextFloat(.05f, .12f), 1f, 0, dustShader);
				Main.dust[a].noLight = true;
				Main.dust[a].velocity *= 0.5f;
				Main.dust[a].scale *= 0.5f;
				Main.dust[a].fadeIn = 30;


				//Main.dust[a].velocity = Projectile.velocity;
			}

			//float widthIntensity = MathHelper.Clamp(1 - (timer / 100), 0.1f, 1);
			timer++;
		}

		public float widthIntensity = 0;
		//public List<Projectile> InkProj = new List<Projectile>();
		public override bool PreDraw(ref Color lightColor)
		{
			/*
			foreach (Projectile p in Main.projectile)
            {
				if (p.type == Projectile.type)
                {
					if (p.Distance(Projectile.Center) <= 20 && p.active && p.whoAmI != Projectile.whoAmI)
                    {
						//Main.NewText(40 / p.Distance(Projectile.Center));
						//float distanceIntensity = 1 - (40 / p.Distance(Projectile.Center));
						Utils.DrawLine(Main.spriteBatch, Projectile.Center, p.Center, color * 0.2f, color * 1f, 1.5f);
                    }
                }
            }
			*/

			Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Items/InkProjTest").Value;
			Vector2 scale = new Vector2(xScale * overallSize , yScale * overallSize + (Projectile.velocity.Length() * 0.01f)) * 0.25f;

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1,1,0,0), color, Projectile.rotation, Tex.Size() / 2, scale, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * (10f * xScale), Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, scale * 0.3f, SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            return base.OnTileCollide(oldVelocity);
        }
    }
}
