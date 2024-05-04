using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Weapons.Ember;
using AerovelenceMod.Content.Projectiles.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
{
	public class TitaniumRocketLauncher : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Titanium Rocket Launcher");
			// Tooltip.SetDefault("Right click to shoot a larger rocket that tries to follow the mouse\nDoes not require ammo");
		}

		public override void SetDefaults()
		{
			Item.damage = 60;
			Item.rare = ItemRarityID.Cyan;
			Item.width = 58;
			Item.height = 20;
			Item.useTime = 30; 
			Item.useAnimation = 30; 
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 7f;
			Item.knockBack = 6f;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.noUseGraphic = true;
			//Item.UseSound = SoundID.Item145;
			//Item.useAmmo = AmmoID.Rocket;
		}

		public override bool AltFunctionUse(Player player) => true;

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-20, 5);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			float multiplier = 0f;
			if (player.altFunctionUse == 2)
            {
				//SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { Volume = 0.4f, PitchVariance = 0.2f, Pitch = 0.5f }, player.Center);

				//player.velocity += velocity * -0.55f;
				multiplier = 2f;
				damage *= 4;
				//type = ModContent.ProjectileType<TitaniumRocket>();
				Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<TitaniumLauncherHeldLarge>(), damage, knockback, player.whoAmI);
				return false;
			}
			else
            {
				Vector2 oopsie = velocity * 0.25f;

				SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { Volume = 0.2f, PitchVariance = 0.2f, Pitch = 0.3f }, player.Center);
				multiplier = 1f;
				type = ModContent.ProjectileType<TitaniumMiniRocket>();
				damage *= 2;
				velocity *= 0.2f;
				//player.velocity += velocity * -0.1f;

				Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<TitaniumLauncherHeldSmall>(), 0, 0, player.whoAmI);

				for (int i = 0; i < 4 + Main.rand.Next(4); i++)
				{
					Dust d = Dust.NewDustPerfect(position + Vector2.Normalize(oopsie) * 50f, ModContent.DustType<MuraLineBasic>(), 
						Velocity: oopsie.RotatedByRandom(0.45f) * 4f * Main.rand.NextFloat(0.7f, 1.3f), Alpha: 100, Color.White, 0.4f);
				}

			}

			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}

			Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.07f, 0.07f)) * 2f, type, damage, knockback, player.whoAmI);

			/*
			ArmorShaderData dustShader1 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
			for (int i = 0; i < 2; i++)
            {
				Vector2 vel = velocity.SafeNormalize(Vector2.UnitX).RotatedBy(i == 0 ? -0.25f : 0.25f);
				Dust p = GlowDustHelper.DrawGlowDustPerfect(position, ModContent.DustType<LineGlow>(), vel * 4,
					Color.White, 0.2f * (multiplier * 0.5f), 0.4f, 0f, dustShader1);
			}

			for (int i = 0; i < 4 * multiplier; i++)
            {
				Vector2 offsetVel = velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
				Dust p = GlowDustHelper.DrawGlowDustPerfect(position, ModContent.DustType<GlowCircleFlare>(), offsetVel * (4f + Main.rand.NextFloat(-1f, 1f) + i * 0.5f),
					Color.White, Main.rand.NextFloat(0.3f + 0.2f * multiplier, 0.3f + 0.2f * multiplier), 0.4f, 0f, dustShader1);
				p.fadeIn = 40 + Main.rand.NextFloat(5,10);
			}
			*/

			return false;

			
		}

        public override float UseTimeMultiplier(Player player)
        {
			/*
			if (player.altFunctionUse == 2)
			{
				Item.useTime = 70;
				Item.useAnimation = 70;
			}
			else
			{
				Item.useTime = 24; //28
				Item.useAnimation = 24; //28
			}
			*/
			return base.UseTimeMultiplier(player);
        }

        public override void HoldItem(Player player)
		{
			
			if (player.altFunctionUse == 2)
			{
				Item.useTime = 70; 
				Item.useAnimation = 70;
			}
			else
			{
				Item.useTime = 26; //28
				Item.useAnimation = 26; //28
			}
			
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			/*
			int itemID = ModContent.ItemType<GaussShotgun>();
			TooltipLine line = new(Mod, "SS", "[i:" + ItemID.FallenStar  + "] Skill Strikes on the second shot [i:" + ItemID.FallenStar + "]")
			{
				OverrideColor = Color.Gold,
			};
			tooltips.Add(line);
			*/
		}
	}
	public class TitaniumRocket : ModProjectile
	{
        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2; 
        }
        public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.maxPenetrate = 1;
			Projectile.damage = 11;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 400;
		}
		int i;
		private readonly int oneHelixRevolutionInUpdateTicks = 30;

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			if (Main.myPlayer == Projectile.owner)
            {
				Projectile.velocity += (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 0.5f;

			}

			if (i == 60)
            {
                SkillStrikeUtil.setSkillStrike(Projectile, 1.3f);

                colVal = 1f;
			}
			if (i >= 60)
				colVal = Math.Clamp(MathHelper.Lerp(colVal, -0.25f, 0.07f), 0, 1);


			i++;
			++Projectile.localAI[0];

			ArmorShaderData dustShader1 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

			float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
			Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * Projectile.height;
			Dust newDust = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), ModContent.DustType<GlowCircleQuadStar>(),
				Vector2.Zero, Color.White, 0.35f, 0.4f, 0f, dustShader1);
			newDust.noGravity = true;
			newDustPosition.Y *= -1;
			newDust.noLight = true;

			Dust newDust2 = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), ModContent.DustType<GlowCircleQuadStar>(),
				Vector2.Zero, Color.White, 0.35f, 0.4f, 0f, dustShader1); newDust.noGravity = true;
			newDust2.velocity *= 0f;
			newDust2.noLight = true;
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (i % 2 == 0)
			{
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixel>(), Alpha: 100, newColor: Color.White, Scale: Main.rand.NextFloat(0.35f, 0.55f));

				Vector2 dustVel = (Projectile.velocity * Main.rand.NextFloat(0.85f, 1.15f) * -0.5f).RotateRandom(0.3f);
				d.velocity = dustVel;
			}
		}

		public override void OnKill(int timeLeft)
		{
			//SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_109") with { Volume = .31f, Pitch = .55f, PitchVariance = 0.2f, MaxInstances = -1 }; 
			//SoundEngine.PlaySound(stylea, Projectile.Center);

			//SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.63f, PitchVariance = 0.25f, MaxInstances = -1, Volume = 1f };
			//SoundEngine.PlaySound(style, Projectile.Center);

			SoundEngine.PlaySound(SoundID.Item70 with { Pitch = -0.5f, Volume = 0.67f, MaxInstances = -1, PitchVariance = 0.25f }, Projectile.Center);

			SoundStyle style3 = new SoundStyle("Terraria/Sounds/Item_45") with { Pitch = -.88f, Volume = 1f, MaxInstances = -1 };
			SoundEngine.PlaySound(style3, Projectile.Center);

			SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/hero_fury_charm_burst") with { Pitch = .35f, PitchVariance = 0.2f, MaxInstances = -1, Volume = 0.75f };
			SoundEngine.PlaySound(style2, Projectile.Center);

			SoundStyle style4 = new SoundStyle("Terraria/Sounds/Custom/dd2_explosive_trap_explode_1") with { PitchVariance = 0.15f, Pitch = -0.15f, Volume = 0.75f };
			SoundEngine.PlaySound(style4, Projectile.Center);

			float distanceToPlayer = (Projectile.Center - Main.player[Projectile.owner].Center).Length();

			if (distanceToPlayer < 1400)
				Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = (1f - (distanceToPlayer / 1500f)) * 10f;
			//float ssIntensity = (Projectile.Center - Main.player[Projectile.owner].Center).Length();
			//Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 7;

			for (int fg = 0; fg < 15; fg++)
			{
				Vector2 randomStart = Main.rand.NextVector2CircularEdge(4, 4);
				Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), randomStart * Main.rand.NextFloat(0.3f, 1.35f) * 1.5f, newColor: new Color(255, 145, 0), Scale: Main.rand.NextFloat(1f, 1.4f) * 0.6f);
				gd.alpha = 2;
			}

			for (int i = 0; i < 7; i++)
			{
				var v = Main.rand.NextVector2Unit();
				Dust sa = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, v * Main.rand.NextFloat(1f, 6f), 0,
					Color.Gold, Main.rand.NextFloat(0.2f, 0.5f) * 1.75f);
				sa.alpha = 50;

				if (sa.velocity.Y > 4)
					sa.velocity.Y *= -1f;

				if (Main.rand.NextBool())
					sa.velocity.Y = MathF.Abs(sa.velocity.Y) * -1;
			}

			int explosion = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FadeExplosionHandler>(), 0, 0, Main.myPlayer);

			if (Main.projectile[explosion].ModProjectile is FadeExplosionHandler feh)
			{
				feh.color = Color.OrangeRed;
				feh.colorIntensity = 1f;
				feh.fadeSpeed = 0.025f;
				for (int m = 0; m < 10; m++)
				{
					FadeExplosionClass newSmoke = new FadeExplosionClass(Main.projectile[explosion].Center, new Vector2(1f, 0).RotatedByRandom(6) * Main.rand.NextFloat(0.5f, 2f));

					newSmoke.size = 0.45f + Main.rand.NextFloat(-0.15f, 0.15f);
					feh.Smokes.Add(newSmoke);

				}
			}

			/*
			for (int j = 0; j < 10; j++)
            {
				int a = Projectile.NewProjectile(null, Projectile.Center, new Vector2(1f, 0).RotatedByRandom(6) * Main.rand.NextFloat(0.7f, 2f), ModContent.ProjectileType<FadeExplosion>(), 0, 0);
				Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
				if (Main.projectile[a].ModProjectile is FadeExplosion explo)
				{
					Color col = i >= 60 ? Color.Goldenrod : Color.OrangeRed;
					explo.color = col;
					explo.size = 0.65f;
					explo.colorIntensity = 0.7f; //0.5
					explo.rise = true;
				}
			}


			Projectile.position.X = Projectile.position.X + (Projectile.width / 2);
			Projectile.position.Y = Projectile.position.Y + (Projectile.width / 2);
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
			Projectile.position.Y = Projectile.position.Y - (Projectile.width / 2);

			for (int i = 0; i < 20; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, 0f, 0f, 100, default, 0.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 5f;
				Main.dust[dust].velocity.Y += -1;

				dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, 0f, 0f, 100, default, 0.5f);
				Main.dust[dust].velocity *= 2f;
				Main.dust[dust].velocity.Y += -1;
			}
			*/
		}

		float colVal = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			Color trailCol = Color.Lerp(Color.White, Color.Orange, colVal);

			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Starlight");
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			for (int k = 0; k < Projectile.oldPos.Length - 1; k++)
			{
				float floatScale = (Projectile.scale * 1.75f) - k / (float)Projectile.oldPos.Length;
				Vector2 scale = new Vector2(floatScale * (i == 0 ? 0.6f : 1f), floatScale * 0.75f);

				Vector2 drawPos = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
				Main.spriteBatch.Draw(texture, drawPos, null, trailCol, Projectile.oldRot[k], drawOrigin, scale, effects, 0f);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			Texture2D projTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/TitaniumRocket");

			Vector2 vec2Scale = new Vector2(1f, 1f - Math.Clamp(Projectile.velocity.Length() * 0.01f, 0, 0.3f)) * Projectile.scale * 0.75f;

			Color col = i >= 60 ? Color.Orange : Color.White;//Color.Lerp(Color.White, Color.Orange, colVal);

			Main.spriteBatch.Draw(projTex, Projectile.Center - Main.screenPosition, projTex.Frame(1, 1, 0, 0), lightColor, Projectile.rotation, projTex.Size() / 2, vec2Scale * 1.5f, effects, 0f);
			Main.spriteBatch.Draw(projTex, Projectile.Center - Main.screenPosition, projTex.Frame(1, 1, 0, 0), col with { A = 0 } * 0.75f, Projectile.rotation, projTex.Size() / 2, vec2Scale * 1.5f, effects, 0f);

			if (i >= 60)
				Main.spriteBatch.Draw(projTex, Projectile.Center - Main.screenPosition, projTex.Frame(1, 1, 0, 0), Color.Orange with { A = 0 }, Projectile.rotation, projTex.Size() / 2, vec2Scale * 2f * (1f + colVal), effects, 0f);

			return false;
        }


	}
	public class TitaniumMiniRocket : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20; //12
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		private Vector2 shotDir = Vector2.Zero;

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.maxPenetrate = 1;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 170;
			Projectile.extraUpdates = 1;
		}

        public override void AI()
        {
			//if (Projectile.ai[0] == 0) //first frame
				//shotDir = 

			//Projectile.velocity *= 1.01f;
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (Projectile.ai[0] < 30)
				Projectile.velocity *= 1.055f; //05

			if (Projectile.ai[0] % 2 == 0)
            {
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelFast>(), Alpha: 100, newColor: Color.White, Scale: Main.rand.NextFloat(0.25f, 0.35f));

				Vector2 dustVel = (Projectile.velocity * Main.rand.NextFloat(0.85f, 1.15f) * -0.5f).RotateRandom(0.3f);
				d.velocity = dustVel;
				d.fadeIn = 50;
            }

			//if (Projectile.ai[0] > 20 && Projectile.ai[0] < 105)
				//Projectile.velocity = Projectile.velocity.MoveTowards(Main.MouseWorld, 0.5f);

			Projectile.ai[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D projTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/TitaniumMiniRocket");
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Starlight");
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length - 5; k++) // 12 20
			{
				float floatScale = (Projectile.scale * 1f) - k / (float)Projectile.oldPos.Length;
				Vector2 scale = new Vector2(floatScale, floatScale * 0.85f);

				Vector2 drawPos = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
				Main.spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.oldRot[k], drawOrigin, scale, effects, 0f);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(projTex, Projectile.Center - Main.screenPosition, projTex.Frame(1, 1, 0, 0), lightColor, Projectile.rotation, projTex.Size() / 2, Projectile.scale, effects, 0f);

			Main.spriteBatch.Draw(projTex, Projectile.Center - Main.screenPosition, projTex.Frame(1, 1, 0, 0), Color.White with { A = 0 } * 0.75f, Projectile.rotation, projTex.Size() / 2, Projectile.scale * 1.5f, effects, 0f);

			return false;
		}

        public override void OnKill(int timeLeft)
        {
			//SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.55f, PitchVariance = 0.25f, MaxInstances = -1, Volume = 0.75f };
			//SoundEngine.PlaySound(style, Projectile.Center);

			SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/hero_fury_charm_burst") with { Pitch = .65f, PitchVariance = 0.2f, MaxInstances = -1, Volume = 0.75f }; 
			SoundEngine.PlaySound(style2, Projectile.Center);

			SoundEngine.PlaySound(SoundID.Item70 with { Pitch = -0.25f, Volume = 0.5f, MaxInstances = -1, PitchVariance = 0.25f }, Projectile.Center);

			SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_explosive_trap_explode_1") with { PitchVariance = 0.15f, Pitch = 0.1f, Volume = 0.75f };
			SoundEngine.PlaySound(style3, Projectile.Center);

			/*
			for (int i = 0; i < 10; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, 0f, 0f, 100, default, 0.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 5f;
				dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, 0f, 0f, 100, default, 0.5f);
				Main.dust[dust].velocity *= 2f;
			}
			*/

			for (int fg = 0; fg < 10; fg++)
			{
				Vector2 randomStart = Main.rand.NextVector2CircularEdge(3, 3);
				Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), randomStart * Main.rand.NextFloat(0.3f, 1.35f) * 1.5f, newColor: new Color(255, 130, 0), Scale: Main.rand.NextFloat(1f, 1.4f) * 0.5f);
				gd.alpha = 2;
				//gd.fadeIn = Main.rand.NextFloat(5f, 10f) + 30;
				//gd.alpha = 255;

			}

			for (int i = 0; i < 5; i++)
			{
				var v = Main.rand.NextVector2Unit();
				Dust sa = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, v * Main.rand.NextFloat(1f, 6f), 0,
					Color.Orange, Main.rand.NextFloat(0.4f, 0.7f));
			}

			int explosion = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FadeExplosionHandler>(), 0, 0, Main.myPlayer);

			if (Main.projectile[explosion].ModProjectile is FadeExplosionHandler feh)
			{
				feh.color = Color.OrangeRed;
				feh.colorIntensity = 1f;
				feh.fadeSpeed = 0.028f;

				for (int m = 0; m < 5; m++)
				{
					FadeExplosionClass newSmoke = new FadeExplosionClass(Main.projectile[explosion].Center, new Vector2(0.45f, 0).RotatedByRandom(6) * Main.rand.NextFloat(0.5f, 2f));

					newSmoke.size = 0.3f + Main.rand.NextFloat(-0.1f, 0.15f);
					feh.Smokes.Add(newSmoke);

				}
			}
		}
    }

	public class TitaniumLauncherHeldSmall : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		private bool firstFrame = false;

		private Vector2 currentDirection => Projectile.rotation.ToRotationVector2();

		Player owner => Main.player[Projectile.owner];

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Magic;

			Projectile.hostile = false;
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 999999;
			Projectile.ignoreWater = true;
		}
		public override bool? CanDamage() { return false; }
		public override void AI()
		{
			owner.heldProj = Projectile.whoAmI;

			if (owner.itemTime <= 1)
				Projectile.active = false;

			Projectile.Center = owner.MountedCenter;

			if (!firstFrame)
			{
				firstFrame = true;
				Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation();
			}

			if (Projectile.ai[0] == 2)
				offset = 7;

			if (Projectile.ai[0] > 2)
				offset = Math.Clamp(MathHelper.Lerp(offset, 21, 0.1f), 0, 22);
			/*
			if (Projectile.ai[0] < 9)
				offset = Math.Clamp(MathHelper.Lerp(offset, 0, 0.2f), -2, 23);
			else
				offset = Math.Clamp(MathHelper.Lerp(offset, 23, 0.05f), 0, 23);
			*/

			glowIntensity = Math.Clamp(MathHelper.Lerp(glowIntensity, -0.25f, 0.1f), 0, 1);

			Projectile.ai[0]++;
		}

		private float offset = 20;
		private float glowIntensity = 1f;
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D Texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/TitaniumRocketLauncher").Value;
			Texture2D Glow = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/TitaniumRocketLauncherGlow").Value;


			Vector2 position = (owner.MountedCenter + (currentDirection * offset)) - Main.screenPosition;
			position.Y += owner.gfxOffY;
			position += new Vector2(0, 2 * owner.direction).RotatedBy(Projectile.rotation); //Extra Offset

			float rotation = currentDirection.ToRotation() + (owner.direction == 1 ? 0 : -MathF.PI);
			SpriteEffects SE = (owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

			Vector2 origin = Texture.Size() / 2;

			Main.spriteBatch.Draw(Texture, position, null, lightColor, rotation, origin, 1f, SE, 0.0f);
			Main.spriteBatch.Draw(Glow, position, null, Color.White with { A = 0 } * glowIntensity * 1f, rotation, origin, 1f, SE, 0.0f);

			return false;
		}
	}

	public class TitaniumLauncherHeldLarge : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		private bool hasShot = false;

		private Vector2 currentDirection => Projectile.rotation.ToRotationVector2();

		Player owner => Main.player[Projectile.owner];

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Ranged;

			Projectile.friendly = true;
			Projectile.hostile = false;

			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 999999;
			Projectile.ignoreWater = true;
		}
		public override bool? CanDamage() { return false; }
		public override void AI()
		{
			owner.heldProj = Projectile.whoAmI;
			Projectile.Center = owner.Center;

			if (Projectile.owner == Main.myPlayer)
            {
				Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation();
			}

			owner.ChangeDir(Projectile.rotation.ToRotationVector2().X > 0 ? 1 : -1);

			owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
			owner.itemTime = 2; 
			owner.itemAnimation = 2;

			//Shoot rocket
			if (Projectile.ai[0] == 45)
			{
				Vector2 velocity = Projectile.rotation.ToRotationVector2() * 12f;
				Vector2 pos = owner.Center;

				Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;

				if (Collision.CanHit(pos, 0, 0, pos + muzzleOffset, 0, 0))
				{
					pos += muzzleOffset;
				}

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, velocity, ModContent.ProjectileType<TitaniumRocket>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
				owner.velocity += velocity * -0.55f;

				for (int i = 0; i < 8 + Main.rand.Next(4); i++)
				{
					Dust d = Dust.NewDustPerfect(owner.Center + muzzleOffset, ModContent.DustType<MuraLineBasic>(),
						Velocity: velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.7f, 1.3f), Alpha: 20, Color.White, 0.45f);
				}

				SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { Volume = 0.4f, PitchVariance = 0.2f, Pitch = 0.5f }, owner.Center);

				offset = 0;
				//if (Projectile.ai[0] > 2)
					//offset = Math.Clamp(MathHelper.Lerp(offset, 21, 0.1f), 0, 22);

				glowIntensity = 2;
				hasShot = true;
			}

			progress = Math.Clamp(Projectile.ai[0], 0f, 45f) / 45f;

			if (Projectile.ai[0] == 70)
				Projectile.active = false;

			if (Projectile.ai[0] < 65)
				gunAlpha = Math.Clamp(MathHelper.Lerp(gunAlpha, 1.5f, 0.15f), 0, 1);
			else
				gunAlpha = Math.Clamp(MathHelper.Lerp(gunAlpha, -0.5f, 0.15f), 0, 1);

			offset = Math.Clamp(MathHelper.Lerp(offset, 23, 0.15f), -20, 20);
			glowIntensity = Math.Clamp(MathHelper.Lerp(glowIntensity, -0.25f, 0.1f), 0, 1);

			Projectile.ai[0]++;
		}

		private float offset = 0;
		private float glowIntensity = 0f;
		private float gunAlpha = 0f;
		private float progress = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D Texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/TitaniumRocketLauncher").Value;
			Texture2D Glow = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/TitaniumRocketLauncherGlow").Value;


			Vector2 position = (owner.MountedCenter + (currentDirection * offset)) - Main.screenPosition;
			position.Y += owner.gfxOffY;
			position += new Vector2(0, 2 * owner.direction).RotatedBy(Projectile.rotation); //Extra Offset

			float rotation = currentDirection.ToRotation() + (owner.direction == 1 ? 0 : -MathF.PI);
			SpriteEffects SE = (owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

			Vector2 origin = (Texture.Size() / 2) + new Vector2(-2 * owner.direction, 0); //Origin more at the trigger

			Color col = Color.Lerp(Color.White, Color.Gold, 1 - glowIntensity);

			Vector2 vibration = Main.rand.NextVector2Circular(3f, 3f) * progress * (hasShot ? 0f : 1f);

			Main.spriteBatch.Draw(Texture, position + vibration, null, lightColor * gunAlpha, rotation, origin, 1f, SE, 0.0f);
			Main.spriteBatch.Draw(Texture, position + vibration, null, Color.White with { A = 0 } * gunAlpha * progress * 0.5f * (hasShot ? glowIntensity : 1f), rotation, origin, 1f, SE, 0.0f);
			Main.spriteBatch.Draw(Glow, position + vibration, null, col with { A = 0 } * glowIntensity * gunAlpha, rotation, origin, 1f, SE, 0.0f);

			return false;
		}
	}

}