using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
	public class DarknessDischarge : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}


		public override void SetDefaults()
		{
			Item.DamageType = DamageClass.Magic;
			Item.damage = 50;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Pink;

			Item.shoot = ModContent.ProjectileType<StarSpawnerProj>();
			Item.shootSpeed = 10; //2

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 80;
			Item.useAnimation = 80;
			Item.reuseDelay = 80;
			Item.UseSound = SoundID.DD2_BookStaffCast;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.mana = 20;


		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-8, 0);
		}

		public override void AddRecipes()
		{

		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            Projectile.NewProjectile(source, position, Vector2.Zero, Item.shoot, damage, knockback, player.whoAmI);
            //Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, Item.shoot, damage, knockback, player.whoAmI);




            //int a = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<PinkStar>(), 0, 0, Main.myPlayer);


            //fb + rs for ice cube spawn
            //Frosty Blast
            //SoundStyle stylefb = new SoundStyle("Terraria/Sounds/Item_101") with { Pitch = .54f, PitchVariance = .09f, MaxInstances = 0, };
            //SoundEngine.PlaySound(stylefb);

            //Crystal Serp
            //SoundStyle stylecs = new SoundStyle("Terraria/Sounds/Item_109") with { Pitch = .52f, PitchVariance = .11f, Volume = 0.5f };
            //SoundEngine.PlaySound(stylecs);


            //Energy Spiral
            //SoundStyle stylees = new SoundStyle("Terraria/Sounds/Item_117") with { Pitch = .52f, PitchVariance = .11f, };
            //SoundEngine.PlaySound(stylees);

            //Rime Voice
            //SoundStyle stylerv = new SoundStyle("Terraria/Sounds/NPC_Hit_52") with { Volume = .49f, Pitch = .89f, PitchVariance = .2f, MaxInstances = 1, };
            //SoundEngine.PlaySound(stylerv);

            //Ghastly Exhale
            //SoundStyle stylege = new SoundStyle("Terraria/Sounds/Item_103") with { Volume = .6f, Pitch = .52f, MaxInstances = 1, PitchVariance = 0.3f };
            //SoundEngine.PlaySound(stylege);


            //Low Exhale
            //SoundStyle stylele = new SoundStyle("Terraria/Sounds/Item_104") with { Volume = .4f, Pitch = -.48f, PitchVariance = .11f, MaxInstances = 1 };
            //SoundEngine.PlaySound(stylele);

            //Air Slash
            //SoundStyle styleas = new SoundStyle("Terraria/Sounds/Item_131") with { Pitch = .36f, PitchVariance = .33f, };
            //SoundEngine.PlaySound(styleas);

            //Fadey star
            //SoundStyle stylefs = new SoundStyle("Terraria/Sounds/Item_25") with { Pitch = .66f, };
            //SoundEngine.PlaySound(stylefs);

            //Crack Shot
            //SoundStyle stylev = new SoundStyle("Terraria/Sounds/Item_38") with { Pitch = .66f, };
            //SoundEngine.PlaySound(stylev);

            //RicoShot
            //SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_60") with { Pitch = .73f, PitchVariance = .45f, Volume = 0.3f};
            //SoundEngine.PlaySound(stylea);



            //int bomb = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<CyverLaserBomb>(), 10, 0, Main.myPlayer);
            //Main.projectile[bomb].rotation = randomAngle;


            /*
			for (int i = 0; i < 1; i++)
            {
				//int a = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<HollowPulse>(), 0, 0, Main.myPlayer);
				if (Main.projectile[a].ModProjectile is HollowPulse pulse)
				{
					pulse.color = Color.Gray * 0f;
					pulse.oval = false;
					pulse.size = 25f;
				}
			}
			*/

            //int b = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<RimeIceCube>(), 0, 0, Main.myPlayer);
            /*
			int c = Projectile.NewProjectile(null, player.Center + (player.direction * new Vector2(50,0)), Vector2.Zero, ModContent.ProjectileType<BigSoul>(), 10, 0, Main.myPlayer);
			if (Main.projectile[c].ModProjectile is BigSoul soul)
            {
				soul.leftOrRight = (player.direction == 1 ? true : false);
            }
			*/

            /*
			for (int i = -2; i < 3; i++) // < 
            {
				int a = Projectile.NewProjectile(null, position, velocity.RotatedBy(MathHelper.ToRadians(1.5f * i)) * 1.3f, ModContent.ProjectileType<DeepFreezeProj>(), 0, 0);
				Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
				if (Main.projectile[a].ModProjectile is DeepFreezeProj explo)
				{
					explo.size = 1.5f;
					explo.multiplier = 1.25f;
					//Color col = i >= 60 ? Color.Goldenrod : Color.OrangeRed;
					//explo.color = col;
					//explo.size = 0.65f;
					//explo.colorIntensity = 0.7f; //0.5
					//explo.rise = true;
				}
				Projectile.NewProjectile(null, position, velocity.RotatedBy(MathHelper.ToRadians(1.5f * i)) * 1.3f, ModContent.ProjectileType<AuroraBlast>(), damage, 0, Main.myPlayer);
			}
			*/



            //Star Strike
            /*
			for (int i = 0; i < 5; i++)
			{
				Vector2 spawnPos = new Vector2(400, 0).RotatedBy(MathHelper.ToRadians(360 / 5) * i);
				int index = NPC.NewNPC(source, (int)position.X, (int)position.Y, ModContent.NPCType<CyverBotOrbiter>(), player.whoAmI);
				NPC laser = Main.npc[index];
				laser.damage = 0;
				if (laser.ModNPC is CyverBotOrbiter bot)
				{
					bot.State = (int)(fastThenShort ? CyverBot.Behavior.PrimeLaser : CyverBot.Behavior.PrimeLaserLong);
					bot.GoalPoint = spawnPos;
				}
			}
			*/

            /*
			for (int i = 0; i < 6; i++)
            {
				int crossBombIndex = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<CyverExplosionBall>(), damage / 4, 2, Main.myPlayer);
				Projectile Bomb = Main.projectile[crossBombIndex];

				if (Bomb.ModProjectile is CyverExplosionBall BombadelaCross)
				{
					BombadelaCross.outVector = new Vector2(0, 450).RotatedBy(MathHelper.ToRadians(60 * i));
				}
			}
			*/
            return false;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/DarknessDischarge_Glow").Value;
			GlowmaskUtilities.DrawItemGlowmask(spriteBatch, texture, this.Item, rotation, scale);

		}
	}
	public class StarSpawnerProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";
		int projCycle;
		int projCount;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star Spawner"); // Name of the projectile. It can be appear in chat
		}


		public override void SetDefaults()
		{
			Projectile.width = 8; // The width of projectile hitbox
			Projectile.height = 8; // The height of projectile hitbox

			Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the projectile collide with tiles?
			Projectile.timeLeft = 101; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.penetrate = -1;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			//Projectile.Center = Main.MouseWorld;
			Projectile.Center = owner.Center;
            projCycle++;

			if (projCycle == 20)
			{
				SpawnStar(1, 0);
				projCount = 1;
			}
			if (projCycle == 40)
			{
				SpawnStar(2, 20);
                projCount = 2;
            }
			if (projCycle == 60)
			{
				SpawnStar(3, 40);
                projCount = 3;
            }
			if (projCycle == 80)
			{
				SpawnStar(4, 60);
                projCount = 4;
            }
			if (projCycle == 100)
			{
				SpawnStar(5, 80);
                projCount = 5;
            }

            for (int k = 0; k < Main.maxProjectiles; k++)
			{
				Projectile target = Main.projectile[k];
				if (target.type == ModContent.ProjectileType<NewDarknessDischargeStar>())
				{
					target.localAI[1] = projCount;
				}
			}
		}
		private void SpawnStar(int num, int delay)
		{
			Player owner = Main.player[Projectile.owner];
			int projType = ModContent.ProjectileType<NewDarknessDischargeStar>();
			var source = Projectile.InheritSource(Projectile);
			var pos = Projectile.Center;

			int spawn = Projectile.NewProjectile(source, pos, Vector2.Zero, projType, Projectile.damage, Projectile.knockBack, owner.whoAmI);
			Projectile proj = Main.projectile[spawn];
			proj.ai[0] = num;
			proj.ai[1] = delay;
		}
	}

	public class NewDarknessDischargeStar : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		int mainTimer = 0;
		private int orbitTimer;
		Color colToUse = Main.rand.NextBool() ? Color.HotPink : Color.SkyBlue;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Godstar");
		}

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 16;
			Projectile.DamageType = DamageClass.Magic;

			Projectile.ignoreWater = true;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.penetrate = 1;

			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;

			Projectile.timeLeft = 400;
		}

		BaseTrailInfo trail1 = new BaseTrailInfo();
		BaseTrailInfo trail2 = new BaseTrailInfo();

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			if (mainTimer == 0)
				Projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;

			if (mainTimer == 7)
				Projectile.ai[0] *= -1;

			#region trailInfo
			//Trail1 
			trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/gooeyLightningDim").Value;
			trail1.trailColor = colToUse;
			trail1.trailPointLimit = 300;
			trail1.trailWidth = 30;
			trail1.trailMaxLength = 300;
			trail1.timesToDraw = 2;
			//trail1.pinch = true;
			//trail1.pinchAmount = 0.1f;

			trail1.trailTime = mainTimer * 0.03f;
			trail1.trailRot = Projectile.velocity.ToRotation();
			
			trail1.trailPos = Projectile.Center + Projectile.velocity;
			trail1.TrailLogic();

			//Trail2
			trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/gooeyLightningDim").Value;
			trail2.trailColor = colToUse;
			trail2.trailPointLimit = 300;
			trail2.trailWidth = 30;
			trail2.trailMaxLength = 300;
			trail2.timesToDraw = 1;
			//trail1.pinch = true;
			//trail1.pinchAmount = 0.5f;

			trail2.trailTime = mainTimer * 0.06f;
			trail2.trailRot = Projectile.velocity.ToRotation();

			trail2.trailPos = Projectile.Center + Projectile.velocity;
			trail2.TrailLogic();

			#endregion

			if (Projectile.direction == 1)
			{
				Projectile.rotation += 0.1f;
			}
			else
			{
				Projectile.rotation -= 0.1f;
			}

			//Projectile.velocity.Y += 0.25f;

			if (mainTimer < 20)
				Projectile.velocity = Projectile.velocity.RotatedBy(0.04f * Projectile.ai[0]);

			if (mainTimer > 25)
				Projectile.velocity *= 0.9f;

			if (Projectile.timeLeft < 320)
				Projectile.active = false;

			mainTimer++;
			/*
			if (mainTimer <= 130 - Projectile.ai[1])
			{
				Projectile.penetrate = -1;
				orbitTimer += 4;
				//Vector2 orbitPos = Main.MouseWorld + new Vector2(50, 0).RotatedBy(MathHelper.ToRadians
					//(orbitTimer + (Projectile.ai[0] * (360 / Projectile.localAI[1])) + (4 * Projectile.ai[1])));
				Vector2 orbitPos = owner.Center + new Vector2(270, 0).RotatedBy(MathHelper.ToRadians(orbitTimer + (Projectile.ai[0] * (360 / Projectile.localAI[1])) + (4 * Projectile.ai[1])));
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
			*/

		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{

		}

		//TODO make it not additive blending 
		public override bool PreDraw(ref Color lightColor)
		{
			trail1.TrailDrawing(Main.spriteBatch);
			//trail2.TrailDrawing(Main.spriteBatch);

			Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/GreyScaleStar").Value;
			Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;

			//Main.spriteBatch.End();
			//Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			//Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, colToUse with { A = 0 }, Projectile.rotation, glow.Size() / 2, Projectile.scale * 0.75f * 1.5f, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, colToUse with { A = 0 }, Projectile.rotation, glow.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);


			//Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, colToUse with { A = 0 }, Projectile.rotation, star.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, colToUse with { A = 0 }, Projectile.rotation, star.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);


			//Main.spriteBatch.End();
			//Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
	}

	public class AnotherTrailTest : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		int mainTimer = 0;

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 16;
			Projectile.ignoreWater = true;
			Projectile.hostile = false;
			Projectile.friendly = true;

			Projectile.tileCollide = false;
			Projectile.timeLeft = 400;
		}

		BaseTrailInfo trail1 = new BaseTrailInfo();
		BaseTrailInfo trail2 = new BaseTrailInfo();

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			if (mainTimer == 0)
				Projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;

			if (mainTimer == 7)
				Projectile.ai[0] *= -1;

			#region trailInfo
			//Trail1 
			trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/gooeyLightningDim").Value;
			trail1.trailColor = Color.HotPink;
			trail1.trailPointLimit = 300;
			trail1.trailWidth = 40;
			trail1.trailMaxLength = 200;
			trail1.timesToDraw = 2;
			//trail1.pinch = true;
			//trail1.pinchAmount = 0.2f;

			//trail1.trailTime = mainTimer * 0.03f;
			trail1.trailRot = Projectile.velocity.ToRotation();

			trail1.trailPos = Projectile.Center + Projectile.velocity;
			trail1.TrailLogic();

			//Trail2
			trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail4").Value;
			trail2.trailColor = Color.HotPink;
			trail2.trailPointLimit = 300;
			trail2.trailWidth = 30;
			trail2.trailMaxLength = 300;
			trail2.timesToDraw = 1;
			//trail1.pinch = true;
			//trail1.pinchAmount = 0.5f;

			//trail2.trailTime = mainTimer * -0.06f;
			trail2.trailRot = Projectile.velocity.ToRotation();

			trail2.trailPos = Projectile.Center + Projectile.velocity;
			//trail2.TrailLogic();

			#endregion

			if (Projectile.direction == 1)
			{
				Projectile.rotation += 0.1f;
			}
			else
			{
				Projectile.rotation -= 0.1f;
			}

			if (mainTimer < 20)
				Projectile.velocity = Projectile.velocity.RotatedBy(0.08f * Projectile.ai[0]);

			if (mainTimer > 25)
				Projectile.velocity *= 0.9f;

			if (Projectile.timeLeft < 320)
				Projectile.active = false;

			mainTimer++;

		}

		public override bool PreDraw(ref Color lightColor)
		{
			trail1.TrailDrawing(Main.spriteBatch);
			trail2.TrailDrawing(Main.spriteBatch);

			Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/GreyScaleStar").Value;
			Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;

			return false;
		}
	}

	public class AnotherTrailTestTheSqueakquel : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		int mainTimer = 0;

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 16;
			Projectile.ignoreWater = true;
			Projectile.hostile = false;
			Projectile.friendly = true;

			Projectile.tileCollide = false;
			Projectile.timeLeft = 400;
		}

		BaseTrailInfo trail1 = new BaseTrailInfo();
		BaseTrailInfo trail2 = new BaseTrailInfo();

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			if (mainTimer == 0)
				Projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;

			if (mainTimer == 7)
				Projectile.ai[0] *= -1;

			#region trailInfo
			//Trail1 
			trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/FireEdge").Value;
			trail1.trailColor = Color.LightGreen;
			trail1.trailPointLimit = 300;
			trail1.trailWidth = 15;
			trail1.trailMaxLength = 300;
			trail1.timesToDraw = 2;
			trail1.pinch = true;
			trail1.pinchAmount = 0.4f;

			//trail1.trailTime = mainTimer * 0.03f;
			trail1.trailRot = Projectile.velocity.ToRotation();

			trail1.trailPos = Projectile.Center + Projectile.velocity;
			trail1.TrailLogic();

			//Trail2
			trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail4").Value;
			trail2.trailColor = Color.DarkGreen;
			trail2.trailPointLimit = 300;
			trail2.trailWidth = 10;
			trail2.trailMaxLength = 300;
			trail2.timesToDraw = 1;
			//trail1.pinch = true;
			//trail1.pinchAmount = 0.5f;

			//trail2.trailTime = mainTimer * 0.06f;
			trail2.trailRot = Projectile.velocity.ToRotation();

			trail2.trailPos = Projectile.Center + Projectile.velocity;
			trail2.TrailLogic();

			#endregion

			if (Projectile.direction == 1)
			{
				Projectile.rotation += 0.1f;
			}
			else
			{
				Projectile.rotation -= 0.1f;
			}

			if (mainTimer < 20)
				Projectile.velocity = Projectile.velocity.RotatedBy(0.04f * Projectile.ai[0]);

			if (mainTimer > 25)
				Projectile.velocity *= 0.9f;

			if (Projectile.timeLeft < 320)
				Projectile.active = false;

			mainTimer++;

		}

		public override bool PreDraw(ref Color lightColor)
		{
			trail1.TrailDrawing(Main.spriteBatch);
			trail2.TrailDrawing(Main.spriteBatch);

			Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/FireSpike").Value;
			float rot = (float)Main.timeForVisualEffects * 0.1f;


			Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.ForestGreen with { A = 0 }, Projectile.velocity.ToRotation() + rot, glow.Size() / 2, 0.2f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.ForestGreen with { A = 0 }, Projectile.velocity.ToRotation() + rot  + MathHelper.PiOver2, glow.Size() / 2, 0.2f, SpriteEffects.None, 0f);


			Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.velocity.ToRotation(), glow.Size() / 2, 0.1f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.velocity.ToRotation() + MathHelper.PiOver2, glow.Size() / 2, 0.1f, SpriteEffects.None, 0f);

			return false;
		}
	}

	public class AnothaOne : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		int mainTimer = 0;

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 16;
			Projectile.ignoreWater = true;
			Projectile.hostile = false;
			Projectile.friendly = true;

			Projectile.tileCollide = false;
			Projectile.timeLeft = 370;
		}

		BaseTrailInfo trail1 = new BaseTrailInfo();
		BaseTrailInfo trail2 = new BaseTrailInfo();

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			if (mainTimer == 0)
				Projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;

			if (mainTimer == 7)
				Projectile.ai[0] *= -1;

			#region trailInfo
			//Trail1 
			trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/FireEdge").Value;
			//trail1.trailColor = Color.DodgerBlue;
			trail1.trailPointLimit = 300;
			trail1.trailWidth = 30;
			trail1.trailMaxLength = 700;
			trail1.timesToDraw = 10;
			trail1.pinch = true;
			trail1.pinchAmount = 0.4f;

			//trail1.trailTime = mainTimer * 0.03f;
			trail1.trailRot = Projectile.velocity.ToRotation();

			trail1.trailPos = Projectile.Center + Projectile.velocity;
			trail1.TrailLogic();

			trail1.gradient = true;
			trail1.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad2").Value;
			trail1.shouldScrollColor = true;
			trail1.gradientTime = (float)Main.timeForVisualEffects * 0.03f;

			//Trail2
			trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5Loop").Value;
			trail2.trailColor = Color.White;
			trail2.trailPointLimit = 300;
			trail2.trailWidth = 40;
			trail2.trailMaxLength = 300;
			trail2.timesToDraw = 2;
			trail2.pinch = true;
			trail2.pinchAmount = 0.4f;

			trail2.trailTime = mainTimer * 0.06f;
			trail2.trailRot = Projectile.velocity.ToRotation();

			trail2.trailPos = Projectile.Center + Projectile.velocity;
			trail2.TrailLogic();

			#endregion

			if (Projectile.direction == 1)
			{
				Projectile.rotation += 0.1f;
			}
			else
			{
				Projectile.rotation -= 0.1f;
			}

			if (mainTimer < 35)
				Projectile.velocity = Projectile.velocity.RotatedBy(0.1f * Projectile.ai[0]);
			else if (mainTimer < 50)
				Projectile.velocity = Projectile.velocity.RotatedBy(-0.03f * Projectile.ai[0]);

			if (mainTimer > 25)
            {
				Projectile.velocity *= 0.99f;

			}

			if (Projectile.timeLeft < 320)
				Projectile.active = false;

			mainTimer++;

		}

		public override bool PreDraw(ref Color lightColor)
		{

			Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/FireSpike").Value;
			Texture2D glow2 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/bright_star").Value;

			float rot = (float)Main.timeForVisualEffects * 0.1f;

			Vector2 vecScale = new Vector2(1f, 1f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(glow2, Projectile.Center - Main.screenPosition - (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20), null, Color.DeepSkyBlue * 0.5f, Projectile.velocity.ToRotation(), glow2.Size() / 2, 0.8f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(glow2, Projectile.Center - Main.screenPosition - (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20), null, Color.SkyBlue * 0.5f, Projectile.velocity.ToRotation(), glow2.Size() / 2, 0.4f, SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


			//Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.ForestGreen with { A = 0 }, Projectile.velocity.ToRotation() + rot, glow.Size() / 2, 0.2f, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.ForestGreen with { A = 0 }, Projectile.velocity.ToRotation() + rot + MathHelper.PiOver2, glow.Size() / 2, 0.2f, SpriteEffects.None, 0f);


			//Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.velocity.ToRotation(), glow.Size() / 2, 0.1f, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.velocity.ToRotation() + MathHelper.PiOver2, glow.Size() / 2, 0.1f, SpriteEffects.None, 0f);

			trail1.TrailDrawing(Main.spriteBatch);
			trail2.TrailDrawing(Main.spriteBatch);

			return false;
		}
	}

}