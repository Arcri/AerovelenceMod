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
	public class CyverWhipProj : ModProjectile
	{
		Vector2 WhipEndPos;
		bool shouldDust = false;
		int DustCounter = 50;

		public override void SetStaticDefaults()
		{
			// This makes the projectile use whip collision detection and allows flasks to be applied to it.
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults()
		{
			// This method quickly sets the whip's properties.
			Projectile.DefaultToWhip();

			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true; // This prevents the projectile from hitting through solid tiles.
			Projectile.extraUpdates = 2; //1
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;

			Projectile.WhipSettings.Segments = 15;
            Projectile.WhipSettings.RangeMultiplier = 2f;
		}

		private float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

        public override bool PreAI()
        {
			//Main.NewText(WhipEndPos);
			return true;
        }

        public override void AI()
		{

			Player owner = Main.player[Projectile.owner];
			if (Timer > 10)
            {

				List<Vector2> points = Projectile.WhipPointsForCollision;
				Projectile.FillWhipControlPoints(Projectile, points);
				if (Main.player[Projectile.owner].Distance(points[points.Count - 1]) > 50)
				{

					/*
					if (Timer % 2 != 0)
                    {
						ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

						int d2 = GlowDustHelper.DrawGlowDust(points[points.Count - 1], Projectile.width / 2, Projectile.height / 2, ModContent.DustType<LineGlow>(),
							Color.DeepPink, 0.2f, 0.4f, 0, dustShader2);
						Main.dust[d2].velocity = (points[points.Count - 1] - Projectile.Center).SafeNormalize(Vector2.UnitX);
						Main.dust[d2].noGravity = true;
					}
					*/
					ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

					
					int d = GlowDustHelper.DrawGlowDust(points[points.Count - 1], Projectile.width / 2, Projectile.height / 2, ModContent.DustType<GlowCircleQuadStar>(),
						Color.DeepPink, 0.65f, 0.4f, 0, dustShader);
					Main.dust[d].velocity = (points[points.Count - 1] - Projectile.Center).SafeNormalize(Vector2.UnitX) * 2;
					Main.dust[d].noGravity = true;
					

					//int dust = Dust.NewDust(points[points.Count - 1], Projectile.width / 2, Projectile.height / 2, DustID.Firework_Blue);
					//Main.dust[dust].velocity = (points[points.Count - 1] - Projectile.Center).SafeNormalize(Vector2.UnitX) * 2;
					//Main.dust[dust].noGravity = true;
				}
				
			}

			// VANILLA DEFAULT BEHAVIOR
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
			Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

            Timer++;

			float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
			if (Timer >= swingTime || owner.itemAnimation <= 0)
			{
				Projectile.Kill();
				return;
			}

			owner.heldProj = Projectile.whoAmI;
			if (Timer == swingTime / 2)
			{
				// Plays a whipcrack sound at the tip of the whip.
				List<Vector2> points = Projectile.WhipPointsForCollision;
				Projectile.FillWhipControlPoints(Projectile, points);
				SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
			}

			//EXTRA BEHAVIOR
			//Projectile.NewProjectile(Projectile.GetSource_FromThis(), WhipEndPos, new Vector2((Projectile.position.X - owner.position.X) * .05f, (Projectile.position.Y - owner.position.Y) * .05f), ModContent.ProjectileType<ElectricityBolt>(), 32, 4);
			
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			//target.AddBuff(ModContent.BuffType<Content.Buffs.Electrified>(), 240);
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

			Projectile.damage = (int)(damage * 0.85f); // Multihit penalty. Decrease the damage the more enemies the whip hits.

			ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

			for (int j = 0; j < 3; j++)
			{
				Dust d = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 4),
					Color.DeepPink, 0.5f, 0.4f, 0f,
					dustShader);
				d.velocity *= 0.5f;

			}
		}

		// This method draws a line between all points of the whip, in case there's empty space between the sprites.
		private void DrawLine(List<Vector2> list)
		{
			Texture2D texture = TextureAssets.FishingLine.Value;
			Rectangle frame = texture.Frame();
			Vector2 origin = new Vector2(frame.Width / 2, 2);

			Vector2 pos = list[0];
			for (int i = 0; i < list.Count - 1; i++)
			{
				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;
				Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.Pink); //Color.White
				Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, Color.HotPink, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			List<Vector2> list = new List<Vector2>();
			Projectile.FillWhipControlPoints(Projectile, list);

			DrawLine(list);

			//Main.DrawWhip_WhipBland(Projectile, list);
			// The code below is for custom drawing.
			// If you don't want that, you can remove it all and instead call one of vanilla's DrawWhip methods, like above.
			// However, you must adhere to how they draw if you do.

			SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.instance.LoadProjectile(Type);
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++)
			{
				// These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
				// You can change them if they don't!
				Rectangle frame = new Rectangle(0, 0, 10, 26);
				Vector2 origin = new Vector2(5, 8);
				float scale = 1;

				// These statements determine what part of the spritesheet to draw for the current segment.
				// They can also be changed to suit your sprite.
				if (i == list.Count - 2)
				{
					frame.Y = 74;
					frame.Height = 18;

					// For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
					Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
					float t = Timer / timeToFlyOut;
					scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
				}
				else if (i > 10)
				{
					frame.Y = 58;
					frame.Height = 16;
				}
				else if (i > 5)
				{
					frame.Y = 42;
					frame.Height = 16;
				}
				else if (i > 0)
				{
					frame.Y = 26;
					frame.Height = 16;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				if (i == list.Count - 2 && Timer > 10)
                {
					var star = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;

					Main.spriteBatch.End();
					Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

					Main.EntitySpriteDraw(star, pos - Main.screenPosition, star.Frame(1,1,0,0), Color.HotPink, rotation, star.Size() / 2, scale * 0.075f, flip, 0);

					Main.spriteBatch.End();
					Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
				}

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);
				


				pos += diff;
			}
			return false;
		}
	}
}