using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.Projectiles.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
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

		bool fastThenShort = true;
		public override void SetDefaults()
		{
			Item.DamageType = DamageClass.Magic;
			Item.damage = 30;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Pink;

			Item.shoot = ModContent.ProjectileType<CyverExplosionBall>();
			Item.shootSpeed = 10; //2

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 9; 
			Item.useAnimation = 9; 
			//Item.UseSound = SoundID.;
			Item.noMelee = true;
			
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


			//Crystal Serp
			//SoundStyle stylecs = new SoundStyle("Terraria/Sounds/Item_109") with { Pitch = .52f, PitchVariance = .11f, };
			//SoundEngine.PlaySound(stylecs);


			//Energy Spiral
			SoundStyle stylees = new SoundStyle("Terraria/Sounds/Item_117") with { Pitch = .52f, PitchVariance = .11f, };
			SoundEngine.PlaySound(stylees);

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
			//SoundStyle styleas = new SoundStyle("Terraria/Sounds/Item_131") with { Pitch = .66f, PitchVariance = .33f, };
			//SoundEngine.PlaySound(styleas);

			//Fadey star
			//SoundStyle stylefs = new SoundStyle("Terraria/Sounds/Item_25") with { Pitch = .66f, };
			//SoundEngine.PlaySound(stylefs);

			//Crack Shot
			//SoundStyle stylev = new SoundStyle("Terraria/Sounds/Item_38") with { Pitch = .66f, };
			//SoundEngine.PlaySound(stylev);

			//RicoShot
			//SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_60") with { Pitch = .73f, PitchVariance = .45f, };
			//SoundEngine.PlaySound(stylea);


			float randomAngle = Main.rand.NextFloat(6.28f);

			//int bomb = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<CyverLaserBomb>(), 10, 0, Main.myPlayer);
			//Main.projectile[bomb].rotation = randomAngle;

			bool randomInput = Main.rand.NextBool();

			for (int i = 0; i < 1; i++)
            {
				int a = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<HollowPulse>(), 0, 0, Main.myPlayer);
				if (Main.projectile[a].ModProjectile is HollowPulse pulse)
				{
					pulse.color = Color.Gray * 0.3f;
					pulse.oval = false;
					pulse.size = 25f;
				}
			}



			/*
			for (int i = 0; i < 4; i++)
			{
				int index = NPC.NewNPC(source, (int)position.X, (int)position.Y, ModContent.NPCType<CyverBot>(), player.whoAmI);
				NPC laser = Main.npc[index];
				laser.damage = 0;
				if (laser.ModNPC is CyverBot bot)
				{
					bot.State = (int)(fastThenShort ? CyverBot.Behavior.PrimeLaser : CyverBot.Behavior.PrimeLaserLong);
					bot.setTurn(randomInput);
					bot.setGoalLocation(new Vector2(-325, 0).RotatedBy(MathHelper.ToRadians(90 * i)));
					if (i == 0)
						bot.Leader = true;
				}
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
			fastThenShort = !fastThenShort;
			return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/DarknessDischarge_Glow").Value;
			GlowmaskUtilities.DrawItemGlowmask(spriteBatch, texture, this.Item, rotation, scale);

		}
	}
}