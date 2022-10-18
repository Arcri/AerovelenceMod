using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace AerovelenceMod.Content.Items.Weapons.Ember
{
	public class MagmaBall : ModProjectile
	{
		private int timer = 0;

		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burning Blaze Ball");
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1;
			Projectile.timeLeft = 200;
			Projectile.tileCollide = false;
			Projectile.width = 80;
			Projectile.height = 80;
			Projectile.alpha = 0;
			Projectile.hide = true;

		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCs.Add(index);

			base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
		}

        public override void AI()
        {
			Projectile.velocity.Y += 0.01f;
			timer++;
			Projectile.scale = 0.1f;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Player Player = Main.player[Projectile.owner];
			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/MagmaBall").Value;
			//Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/circle_03").Value;

			Texture2D texture2 = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/circle_05").Value;


			int height1 = texture.Height;
			Vector2 origin1 = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);

			int height2 = texture.Height;
			Vector2 origin2 = new Vector2((float)texture2.Width / 2f, (float)height2 / 2f);

			//gradientTex
			//time
			//distort
			//caustics tex

			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value;
			myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/FireBallGradient").Value);
			myEffect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);
			myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/FireBallGradient").Value);
			myEffect.Parameters["uTime"].SetValue(timer * 0.02f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
			myEffect.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, 0, origin1, Projectile.scale, SpriteEffects.None, 0.0f);
			Main.spriteBatch.Draw(texture2, Projectile.Center + new Vector2(0, 150) - Main.screenPosition, null, lightColor, 0, origin2, 0.3f, SpriteEffects.None, 0.0f);


			return false;
			
		}

        public override void PostDraw(Color lightColor)
        {
			//Main.pixelShader.CurrentTechnique.Passes[0].Apply();
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
		}

    }
}


