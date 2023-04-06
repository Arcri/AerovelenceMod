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
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.SlateSet
{
    public class DeerclopsRock : ModProjectile
    {
		Vector2 previousPos = Vector2.Zero;

		private int timer;
		private int timer2_electricboogalo;
		public float lerpage = 0.12f;
		private Vector2 mousePos;

		Vector2 home = Vector2.Zero;

		int whichRock = Main.rand.Next(3);//1;

		float scaleBonus = 0f;
		float rotationBonus = 0f;

		bool released = false;
		bool shouldHide = false;

		bool hasPlayedSound = false;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slate Boulder");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.penetrate = 2;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 120;

		}

        public override bool? CanDamage()
        {
			return !shouldHide;
        }

        public override void AI()
        {
			Player player = Main.player[Projectile.owner];


			if (player.inventory[player.selectedItem].type != ModContent.ItemType<SlateStaff>())
				Projectile.Kill();

			if (shouldHide)
            {
				Projectile.velocity = Vector2.Zero;
            } 
			else if (released == false)
            {
				previousPos = Projectile.Center;

				Vector2 center = player.MountedCenter; //-10, 5


				rotationBonus = (float)Math.Sin(timer * 0.02f) * 0.6f;
				Projectile.rotation = rotationBonus;

				if (Projectile.owner == Main.myPlayer)
				{
					mousePos = Main.MouseWorld;
				}
				Projectile.velocity = Vector2.Lerp(Vector2.Normalize(Projectile.velocity), Vector2.Normalize(mousePos - player.MountedCenter), lerpage); //slowly move towards direction of cursor
				Projectile.velocity.Normalize();


				Projectile.Center = player.Center + home.RotatedBy(Projectile.velocity.ToRotation());

				if (player.channel)
				{
					Projectile.timeLeft++;

				}
				else
				{
					released = true;
				}
				timer++;
			}
            else
            {
				Projectile.tileCollide = true;
				if (timer2_electricboogalo == 0)
                {
					Projectile.velocity = (Projectile.Center - previousPos);
                }
				Projectile.velocity.Y += 0.7f;

				timer2_electricboogalo++;
            }

			

		}

		public void setHome(int input)
        {
			if (input == 0) home = new Vector2(125, 0);
			if (input == 1) home = new Vector2(85, 0).RotatedBy(MathHelper.ToRadians(25));
			if (input == 2) home = new Vector2(85, 0).RotatedBy(MathHelper.ToRadians(-25));

		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {

			Kill(5);

			//onHit();
			return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Kill(5);
			
		}

        public override void Kill(int timeLeft)
        {
			
			if (shouldHide == false)
            {
				for (int i = 0; i < 20; i++)
				{

					ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

					int d = GlowDustHelper.DrawGlowDust(Projectile.Center, 0, 0, ModContent.DustType<GlowCircleRise>(), 0, Projectile.velocity.Y * Main.rand.NextFloat(0.1f, 0.3f), Color.Gray * 0.7f, 0.6f + Main.rand.NextFloat(-0.3f, 0.4f), 0.8f, 0, dustShader);
					//Main.dust[d].velocity.Y = Math.Abs(Main.dust[d].velocity.Y) * -1;
					Main.dust[d].velocity.X *= 1.2f;

				}
			}
			
			
			
			Projectile.tileCollide = false;
			shouldHide = true;
			Projectile.timeLeft = 5;

			//Big One
			Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(0, -6 + Main.rand.NextFloat(-3, 4)).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-25, 25))), ModContent.ProjectileType<SlateChunk>(), Projectile.damage / 3, 1, Main.myPlayer);
			//Little Ones
			Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(0,-6 + Main.rand.NextFloat(-3,4)).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-25, 25))), ModContent.ProjectileType<SlateChunk>(), Projectile.damage / 4, 1, Main.myPlayer, 0, 2);
			Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(0,-6 + Main.rand.NextFloat(-3,4)).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-25, 25))), ModContent.ProjectileType<SlateChunk>(), Projectile.damage / 4, 1, Main.myPlayer, 0, 2);



			onHit();

		}

		public void onHit()
        {
			if (!hasPlayedSound) SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, Projectile.Center);
			hasPlayedSound = true;
			Player myPlayer = Main.player[Projectile.owner];

			//myPlayer.GetModPlayer<ReduxPlayer>().ScreenShakePower = 20;


			for (int j = 0; j < 15; j++)
			{
				Dust dust2 = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Stone, Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f, Scale: 1.5f * Projectile.scale);
				//Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.Stone, Projectile.velocity);
				dust2.noGravity = true;
			}
		}

        public override bool PreDraw(ref Color lightColor)
		{
			
			Texture2D PinkAfterimage = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/SlateSet/DeerclopsRockPink").Value;
			Rectangle pinkSourceRect = new Rectangle(0, PinkAfterimage.Height * 0, PinkAfterimage.Width, PinkAfterimage.Width);

			scaleBonus = (float)Math.Sin(timer * 0.1) * 0.15f + 0.15f;

			//Effect myEffect = ModContent.Request<Effect>("Redux/Effects/SandAura", AssetRequestMode.ImmediateLoad).Value;
			//myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3());
			//myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redux/Effects/Swirl").Value);
			//myEffect.Parameters["uTime"].SetValue(timer);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			//myEffect.CurrentTechnique.Passes[0].Apply();
			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
			{
				float additionalAlpha = shouldHide ? 0.5f : 1f;

				float progress = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type] * i;
				Main.spriteBatch.Draw(PinkAfterimage, Projectile.oldPos[i] - Main.screenPosition + new Vector2(16, 16)
					+ (Projectile.velocity.SafeNormalize(Vector2.UnitX) * i * 0.75f * -1f), pinkSourceRect, Color.Pink * additionalAlpha /*(1f - progress)*/,
					Projectile.rotation, pinkSourceRect.Size() / 2f, Math.Max((Projectile.scale + scaleBonus + 0.05f) * (1f - progress), 0.1f), 
					Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


			//Rectangle sourceRectangle = new Rectangle(0, TextureAssets.Projectile[Projectile.type].Value.Height / 3 * whichRock, TextureAssets.Projectile[Projectile.type].Value.Width, TextureAssets.Projectile[Projectile.type].Value.Width);
			//Vector2 origin = sourceRectangle.Size() / 2f;

			//float offsetX = 0f;
			//origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);


			//Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value,
			//	Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(16f,0f),
			//	sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			if (!shouldHide)
            {
				Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/SlateSet/DeerclopsRock").Value;
				Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

			}
			return false;
			//return true;
		}
	}
}
