using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using AerovelenceMod.Core;
using AerovelenceMod.Backgrounds.Skies;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Common.IL;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.UI;
using ReLogic.Content;
using AerovelenceMod.Content.Skies;
using AerovelenceMod.Common.Globals.SkillStrikes;
using ReLogic.Graphics;
using AerovelenceMod.Common;

namespace AerovelenceMod.Content.Projectiles.Other
{
    public class ScreenShaderTest : ModProjectile
    {
		int timer = 0;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("DistortTest");

		}
        public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.timeLeft = 200;
			Projectile.tileCollide = false;
			Projectile.scale = 0.75f;

		}
		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			timer += 4;

        }

        Effect effect = null;
		public RenderTarget2D ScreenTarget;

		public override bool PreDraw(ref Color lightColor)
		{

			if (ScreenTarget == null)
				ScreenTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);

			/*
			effect = ModContent.Request<Effect>("AerovelenceMod/Effects/Shockwave", AssetRequestMode.ImmediateLoad).Value;
			//effect.Parameters["sampleTexture"].SetValue(Mod.Assets.Request<Texture2D>("Assets/Glorb").Value);

			Filters.Scene.Activate("Shockwave", Projectile.Center).GetShader().UseColor(1, 1, 30).UseTargetPosition(Projectile.Center);
			Filters.Scene["Shockwave"].GetShader().UseProgress(timer * 0.8f);
			Filters.Scene["Shockwave"].GetShader().UseOpacity(0.5f);

			if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive())
			{
				float progress = (180 - Projectile.timeLeft) / 60f;
				Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(100f * (1 - progress / 3f));
			}
			*/

			/*Filters.Scene["PulsarShockwave"].GetShader().UseProgress(progress);
			if (effect == null)
            {
				effect = Filters.Scene["AerovelenceMod:DistortScreen"].GetShader().Shader;


			} else
            {
				effect.Parameters["uTime"].SetValue((float)Main.time * 0.02f);
				Main.NewText("yippee");
			}
			*/

			//Filters.Scene["AerovelenceMod:DistortScreen"].Activate(Projectile.Center);
			///Filters.Scene["AerovelenceMod:DistortScreen"].GetShader().ApplyTime

			//Effect shader = ModContent.Request<Effect>("AerovelenceMod/Effects/DistortScreen", AssetRequestMode.ImmediateLoad).Value;//Filters.Scene["AerovelenceMod:DistortScreen"].GetShader().Shader;

			//Filters.Scene["AerovelenceMod:DistortScreen"].Activate(Projectile.Center);

			//Manifesting this doesn't turn the game into 1fps //it did :(
			//ScreenTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);


			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/DistortMisc", AssetRequestMode.ImmediateLoad).Value;
			myEffect.Parameters["ScreenTarget"].SetValue(ScreenTarget);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
			myEffect.CurrentTechnique.Passes[0].Apply();

			//AerovelenceMod.DistortShader.Parameters["sampleTexture"].SetValue(Mod.Assets.Request<Texture2D>("Assets/Glorb").Value);
			//AerovelenceMod.DistortShader.Parameters["TileTarget"].SetValue(ScreenTarget);

			//AerovelenceMod.DistortShader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 10);
			//AerovelenceMod.DistortShader.Parameters["uOpacity"].SetValue(1);

			//AerovelenceMod.CrystalShine.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 10);
			//AerovelenceMod.CrystalShine.Parameters["uOpacity"].SetValue(1);

			//AerovelenceMod.DistortShader.Parameters["uTime"].SetValue(timer * 10);
			//shader.CurrentTechnique.Passes[0].Apply();
			//shader.Parameters["uImage0"].SetValue("AerovelenceMod/Assets/Noise/noise");
			//shader.Parameters["uTime"].SetValue((float)Main.time * 0.1f);


			Texture2D Tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/icon");
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1,1,0,0), Color.White, Projectile.rotation, Tex.Size() / 2, Projectile.scale * 2, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, Projectile.scale * 0.35f, SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
			return false;
		}

        public override void Kill(int timeLeft)
        {
			Filters.Scene.Deactivate("Shockwave", Main.LocalPlayer.Center);
			Filters.Scene["Shockwave"].GetShader().UseOpacity(1f);
        }

    }
}
