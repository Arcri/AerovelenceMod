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
			Item.useTime = 30; //10
			Item.useAnimation = 30; //5
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 7f;
			Item.knockBack = 6f;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			//Item.UseSound = SoundID.Item145;
			//Item.useAmmo = AmmoID.Rocket;
		}

		public override bool AltFunctionUse(Player player) => true;

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-20, 5);
		}

		//public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
		//{
		/*
        var posArray = new Vector2[num];
        float spread = (float)(angle * 0.0555);
        float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + speedY * speedY);
        double baseAngle = System.Math.Atan2(speedX, speedY);
        double randomAngle;
        for (int i = 0; i < num; ++i)
        {
            randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
            posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
        }
        return posArray;
        */
		//}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			float multiplier = 0f;
			if (player.altFunctionUse == 2)
            {
				SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { Volume = 0.4f, PitchVariance = 0.2f, Pitch = 0.5f }, player.Center);

				player.velocity += velocity * -0.55f;
				multiplier = 2f;
				damage *= 4;
				type = ModContent.ProjectileType<TitaniumRocket>();
            } else
            {
				SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { Volume = 0.2f, PitchVariance = 0.2f, Pitch = 0.3f }, player.Center);
				multiplier = 1f;
				type = ModContent.ProjectileType<TitaniumMiniRocket>();
				damage *= 2;
				velocity *= 1.5f;
            }

			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}

			/*
			int a = Projectile.NewProjectile(null, position, velocity * 0.2f, ModContent.ProjectileType<FadeExplosion>(), 0, 0);
			Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
			if (Main.projectile[a].ModProjectile is FadeExplosion explo)
			{
				explo.color = Color.OrangeRed;
				explo.size = 0.2f;
				explo.colorIntensity = 1f;
				explo.rise = true;
			}
			*/

			Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 2f, type, damage, knockback, player.whoAmI);


			ArmorShaderData dustShader1 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
			ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

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

			//Dust p = GlowDustHelper.DrawGlowDustPerfect(position + muzzleOffset, ModContent.DustType<SolsearRiseDust>(), velocity.SafeNormalize(Vector2.UnitX) * (3f + Main.rand.NextFloat(-1f, 1f) ),
					//Color.Orange, Main.rand.NextFloat(0.2f, 0.3f), 0.4f, 0f, dustShader1);

			return false;

			
		}

		public override void HoldItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.useTime = 70; //10
				Item.useAnimation = 70; //5
			}
			else
			{
				Item.useTime = 30; //10
				Item.useAnimation = 30; //5
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
				Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
				Projectile.GetGlobalProjectile<SkillStrikeGProj>().travelDust = (int)SkillStrikeGProj.TravelDustType.glowProjCenter;
				Projectile.GetGlobalProjectile<SkillStrikeGProj>().critImpact = (int)SkillStrikeGProj.CritImpactType.None;
				Projectile.GetGlobalProjectile<SkillStrikeGProj>().impactScale = 1f;
			}

			//if (i < 50)
			//Projectile.velocity += (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 1f;

			//Projectile.velocity.Y += 0.4f;
			//Projectile.velocity *= 1.01f;
			i++;
			++Projectile.localAI[0];

			ArmorShaderData dustShader1 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

			float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
			Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * Projectile.height;
			Dust newDust = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), ModContent.DustType<GlowCircleQuadStar>(),
				Vector2.Zero, Color.White, 0.35f, 0.4f, 0f, dustShader1);
			//Dust newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), 91);
			newDust.noGravity = true;
			newDustPosition.Y *= -1;

			Dust newDust2 = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), ModContent.DustType<GlowCircleQuadStar>(),
				Vector2.Zero, Color.White, 0.35f, 0.4f, 0f, dustShader1); newDust.noGravity = true;
			newDust2.velocity *= 0f;
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f)
			{
				/*
				for (int i = 0; i < 2; i++)
				{
					int num255 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 91, 0f, 0f, 100);
					Dust dust = Main.dust[num255];

					Main.dust[num255].noGravity = true;
					num255 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width, Projectile.height, 91, 0f, 0f, 100, default(Color), 0.5f);
					Main.dust[num255].fadeIn = 1f + Main.rand.Next(5) * 0.1f;
					dust = Main.dust[num255];
					Main.dust[num255].noGravity = true;
					dust.scale *= 0.99f;
				}
				*/
			}
		}

		public override void Kill(int timeLeft)
		{
			SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_109") with { Volume = .31f, Pitch = .55f, PitchVariance = 0.2f }; 
			SoundEngine.PlaySound(stylea, Projectile.Center);

			//SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.position);
			SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.63f, PitchVariance = 0.25f, MaxInstances = -1, Volume = 1f };
			SoundEngine.PlaySound(style, Projectile.Center);

			SoundEngine.PlaySound(SoundID.Item70 with { Pitch = -0.8f, Volume = 0.67f, MaxInstances = -1, PitchVariance = 0.25f }, Projectile.Center);

			SoundStyle style3 = new SoundStyle("Terraria/Sounds/Item_45") with { Pitch = -.88f, Volume = 1f };
			SoundEngine.PlaySound(style3, Projectile.Center);

			Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 7;

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
				//Main.dust[dust].velocity.X *= 0.5f;

				dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, 0f, 0f, 100, default, 0.5f);
				Main.dust[dust].velocity *= 2f;
				Main.dust[dust].velocity.Y += -1;
				//Main.dust[dust].velocity.X *= 0.5f;

			}
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Starlight");
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			for (int k = 0; k < Projectile.oldPos.Length - 1; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
				Main.spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.oldRot[k], drawOrigin, (Projectile.scale * 1.75f) - k / (float)Projectile.oldPos.Length, effects, 0f);
				//Main.spriteBatch.Draw(texture, drawPos - Projectile.oldPos[k], null, Color.White * 2f, Projectile.oldRot[k], drawOrigin, (Projectile.scale  * 1f) - k / (float)Projectile.oldPos.Length, effects, 0f);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			Texture2D projTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/TitaniumRocket");
			Main.spriteBatch.Draw(projTex, Projectile.Center - Main.screenPosition, projTex.Frame(1, 1, 0, 0), i >= 60 ? Color.Gold : lightColor, Projectile.rotation, projTex.Size() / 2, Projectile.scale, effects, 0f);


			return false;
        }


	}
	public class TitaniumMiniRocket : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

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
		}

        public override void AI()
        {
			Projectile.velocity *= 1.01f;
			Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
		{

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Starlight");
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			for (int k = 0; k < Projectile.oldPos.Length - 1; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
				Main.spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.oldRot[k], drawOrigin, (Projectile.scale * 1f) - k / (float)Projectile.oldPos.Length, effects, 0f);
				//Main.spriteBatch.Draw(texture, drawPos - Projectile.oldPos[k], null, Color.White * 2f, Projectile.oldRot[k], drawOrigin, (Projectile.scale  * 1f) - k / (float)Projectile.oldPos.Length, effects, 0f);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			Texture2D projTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/TitaniumMiniRocket");
			Main.spriteBatch.Draw(projTex, Projectile.Center - Main.screenPosition, projTex.Frame(1, 1, 0, 0), lightColor, Projectile.rotation, projTex.Size() / 2, Projectile.scale, effects, 0f);

			return false;
		}

        public override void Kill(int timeLeft)
        {
			SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.53f, PitchVariance = 0.25f, MaxInstances = -1, Volume = 0.5f };
			SoundEngine.PlaySound(style, Projectile.Center);

			SoundEngine.PlaySound(SoundID.Item70 with { Pitch = -0.5f, Volume = 0.5f, MaxInstances = -1, PitchVariance = 0.25f }, Projectile.Center);

			for (int i = 0; i < 10; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, 0f, 0f, 100, default, 0.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 5f;
				dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, 0f, 0f, 100, default, 0.5f);
				Main.dust[dust].velocity *= 2f;
			}

			for (int i = 0; i < 5; i++)
			{
				int a = Projectile.NewProjectile(null, Projectile.Center, new Vector2(0.5f, 0).RotatedByRandom(6) * Main.rand.NextFloat(0.7f, 2f), ModContent.ProjectileType<FadeExplosionHighRes>(), 0, 0);
				Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
				if (Main.projectile[a].ModProjectile is FadeExplosionHighRes explo)
                {
					explo.color = Color.OrangeRed;
					explo.size = 0.4f;
					explo.colorIntensity = 0.75f; //0.5
                }
			}
		}
    }
}