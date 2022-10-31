using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
	public class PinkStar : ModProjectile
	{

		int timer = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Godstar");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 30;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ignoreWater = true;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
			Projectile.scale = 0.5f;
			Projectile.timeLeft = 1000;
		}

        public override void AI()
		{
			ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
			Player owner = Main.player[Projectile.owner];

			if (Projectile.direction == 1)
			{
				Projectile.rotation += 0.1f;
			}
			else
			{
				Projectile.rotation -= 0.1f;
			}

			Projectile.velocity = Vector2.Zero;

			timer++;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			
		}

		Color colToUse = Main.rand.NextBool() ? Color.HotPink : Color.SkyBlue;
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/GreyScaleStar").Value;

			//Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			//myEffect.Parameters["uColor"].SetValue(colToUse.ToVector3() * 2f);
			//myEffect.Parameters["uTime"].SetValue(2);
			//myEffect.Parameters["uOpacity"].SetValue(0.9f);
			//myEffect.Parameters["uSaturation"].SetValue(0f);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			//myEffect.CurrentTechnique.Passes[0].Apply();


			Texture2D texture2 = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
			Main.spriteBatch.Draw(texture2, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle(0, 0, texture2.Width, texture2.Height), colToUse, Projectile.rotation, texture2.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(texture2, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle(0, 0, texture2.Width, texture2.Height), colToUse, Projectile.rotation, texture2.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);


			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle(0, 0, TextureAssets.Projectile[Projectile.type].Value.Width, TextureAssets.Projectile[Projectile.type].Value.Height), colToUse, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle(0, 0, TextureAssets.Projectile[Projectile.type].Value.Width, TextureAssets.Projectile[Projectile.type].Value.Height), colToUse, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;

			return false;
		}
	}
}