using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
	public class PinkStar : TrailProjBase
	{

		int mainTimer = 0;
		private int orbitTimer;
        Color colToUse = Main.rand.NextBool() ? Color.HotPink : Color.SkyBlue;
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
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
			Projectile.scale = 0.5f;
			Projectile.timeLeft = 400;
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


            mainTimer++;
			if (mainTimer <= 130 - Projectile.ai[1])
			{
                Projectile.penetrate = -1;
                orbitTimer += 4;
                /*Vector2 orbitPos = Main.MouseWorld + new Vector2(50, 0).RotatedBy(MathHelper.ToRadians
					(orbitTimer + (Projectile.ai[0] * (360 / Projectile.localAI[1])) + (4 * Projectile.ai[1])));*/
                Vector2 orbitPos = owner.Center + new Vector2(70, 0).RotatedBy(MathHelper.ToRadians
    (orbitTimer + (Projectile.ai[0] * (360 / Projectile.localAI[1])) + (4 * Projectile.ai[1])));
                Projectile.velocity = (10 * Projectile.velocity + orbitPos - Projectile.Center) / 20f;
			}
			if (mainTimer >= 130 - Projectile.ai[1])
			{
                Projectile.penetrate = 1;
                if (Projectile.localAI[0] == 0f)
				{
					Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 7f;
                    Projectile.localAI[0] = 1f;
				}
				Projectile.velocity *= 1.035f;
			}
            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail7").Value;
            trailColor = colToUse;
            trailTime = Projectile.ai[1];

            // other things you can adjust
            trailPointLimit = 100;
            trailWidth = 40;
            trailMaxLength = 70;


            //MUST call TrailLogic AFTER assigning trailRot and trailPos
            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center;
            TrailLogic();

        }
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{

		}


        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num) * 0.5f; // 0.3f
        }
        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/GreyScaleStar").Value;

            //Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            //myEffect.Parameters["uColor"].SetValue(colToUse.ToVector3() * 2f);
            //myEffect.Parameters["uTime"].SetValue(2);
            //myEffect.Parameters["uOpacity"].SetValue(0.9f);
            //myEffect.Parameters["uSaturation"].SetValue(0f);
            TrailDrawing();


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
		}
	}
}