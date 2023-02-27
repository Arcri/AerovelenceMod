using AerovelenceMod.Common.Utilities;
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
			DisplayName.SetDefault("Star Spawner"); // Name of the projectile. It can be appear in chat
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
				if (target.type == ModContent.ProjectileType<PinkStar>())
				{
					target.localAI[1] = projCount;
				}
			}
		}
		private void SpawnStar(int num, int delay)
		{
			Player owner = Main.player[Projectile.owner];
			int projType = ModContent.ProjectileType<PinkStar>();
			var source = Projectile.InheritSource(Projectile);
			var pos = Projectile.Center;

			int spawn = Projectile.NewProjectile(source, pos, Vector2.Zero, projType, Projectile.damage, Projectile.knockBack, owner.whoAmI);
			Projectile proj = Main.projectile[spawn];
			proj.ai[0] = num;
			proj.ai[1] = delay;
		}
	}
}