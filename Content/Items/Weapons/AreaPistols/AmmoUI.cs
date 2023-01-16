using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Weapons.AreaPistols.ErinGun;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.AreaPistols
{
	public class AmmoUI : ModProjectile
	{
        #region unimportant
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("AmmoUI");
		}

		public override void SetDefaults()
		{
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
		}

		public override bool? CanDamage()
		{
			return false;
		}
        #endregion

		//This one projectile is used for every time of weapon,
		//so we need to know which one in order to set things like
		//max ammo count correctly
		public enum whatWeaponEnum
        {
			ErinGun = 0,
        }
		public whatWeaponEnum whatWeapon;

		public float MaxAmmo = 0f;

		//old
		//List containing the number of bullets
		//It is a list instead of an array not because the length will dynamically change based on consumed ammo,
		//but because the max ammo will be different for each weapon
        public List<Bullet> Bullets = new List<Bullet>();

		public bool createdBullets = false;

		public float activeBulletsCount = 0f;


		//also old
		public float halfCurrentBullets = 0f;
		public float currentBullets = 0f;


		public List<bool> areActive = new List<bool>();

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			//Vector2 move = (owner.Center) - Projectile.Center;
			//float scalespeed = 10; //(timer < 20 ? 3f : 7); //5

			//Projectile.velocity.X = (Projectile.velocity.X + move.X) / 20f * scalespeed;
			//Projectile.velocity.Y = (Projectile.velocity.Y + move.Y) / 20f * scalespeed;

			Projectile.Center = owner.Center;

			#region old
			/*
			
			//Here we created the correct number of bullets based on the weapon
			if (!createdBullets)
			{
				switch (whatWeapon)
                {
					case whatWeaponEnum.ErinGun:
						MaxAmmo = 12;

						break;
					default:
						break;
                }

				for (int i = 0; i < MaxAmmo; i++)
				{
					Bullet newBullet = new Bullet(Projectile.Center, Vector2.Zero);
					Bullets.Add(newBullet);
				}

			}

			//Get the amount of current bullets from the player
			float currentAmmo = 0f;
			switch (whatWeapon)
            {
				case whatWeaponEnum.ErinGun:

					if (owner.inventory[owner.selectedItem].type != ModContent.ItemType<AntiquePistol>())
					{
						Projectile.active = false;
					}

					currentAmmo = owner.GetModPlayer<AmmoPlayer>().ErinAmmoCount;


					int currentAmmoTemp = (int)currentAmmo;
					foreach (Bullet b in Bullets)
					{
						if (currentAmmoTemp > 0)
                        {
							b.usedUp = false;
                        } else
                        {
							b.usedUp = true;
                        }
						currentAmmoTemp--;
					}

					break;
				default:
					Main.NewText("not set"); //Debug message
					break;
            }


            //Update bullet pos
            {
				//Get the active bullet count
				activeBulletsCount = 0f;
				foreach (Bullet b in Bullets)
                {
					if (!b.usedUp)
						activeBulletsCount++;
                }
				Main.NewText("Active Bullet Count --> " + activeBulletsCount);

				float halfCurrentBullets = activeBulletsCount / 2f;
				Main.NewText("half bullet count --> " + halfCurrentBullets); //Debug message


				//EXAMPLES OF WHAT HAPPENS BELOW
				// 6 active bullets --> 6/2 = 3
				// 3 * -1 + 0.5 = -2.5
				// | -2.5 | -1.5 | -0.5 | 0.5 | 1.5 | 2.5 | 
				// - - - - - - - - - - - - - - -
				// 7 active bullets --> 7/2 = 3.5
				// 3.5 * -1 + 0.5 = -3
				// | -3 | -2 | -1 | 0 | 1 | 2 | 3 | 

				int arrayIndex = 0;
				for (float j = (-1 * halfCurrentBullets) + 0.5f; j < halfCurrentBullets; j++)
                {
					Bullet b = Bullets[arrayIndex];

					Vector2 BulletPos = owner.Center + new Vector2(50f * j , -60);
					if (!createdBullets)
						Main.NewText("Bullet Pos at : " + new Vector2(50f * j, -60)); //Debug message
					b.Update(BulletPos);
                }
            }
			createdBullets = true;

			
			#endregion

			#region also old
			/*
			Player owner = Main.player[Projectile.owner];
			switch (whatWeapon)
			{
				case whatWeaponEnum.ErinGun:

					if (owner.inventory[owner.selectedItem].type != ModContent.ItemType<AntiquePistol>())
					{
						Projectile.active = false;
					}

					currentBullets = owner.GetModPlayer<AmmoPlayer>().ErinAmmoCount;
					halfCurrentBullets = currentBullets / 2;

					break;
				default:
					Main.NewText("not set"); //Debug message
					break;
			}
			*/
			#endregion
			
			if (!createdBullets)
			{
				switch (whatWeapon)
				{
					case whatWeaponEnum.ErinGun:
						MaxAmmo = 12;

						for (int i = 0; i < MaxAmmo; i++)
                        {
							areActive.Add(true);
                        }

						break;
					default:
						break;
				}

				createdBullets = true;
			}

			switch (whatWeapon)
			{
				case whatWeaponEnum.ErinGun:

					if (owner.inventory[owner.selectedItem].type != ModContent.ItemType<AntiquePistol>())
					{
						Projectile.active = false;
					}

					currentBullets = owner.GetModPlayer<AmmoPlayer>().ErinAmmoCount;
					halfCurrentBullets = currentBullets / 2;

					float tempCurrentBullets = currentBullets;

					for (int j = 0; j < areActive.Count; j++)
                    {
						if (tempCurrentBullets > 0)
						{
							areActive[j] = false;
						} else
                        {
							areActive[j] = true;
                        }
					} 

					break;
				default:
					Main.NewText("not set"); //Debug message
					break;
			}
			
		}

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D BulletTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/AreaPistols/AmmoUIBullet").Value;
			Texture2D BulletOpen = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/AreaPistols/AmmoOpen").Value;
			Texture2D BulletUsed = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/AreaPistols/AmmoUsed").Value;


			/*
			foreach (Bullet b in Bullets)
            {
				if (!b.usedUp)
					b.Draw(Main.spriteBatch, BulletTex, Main.player[Projectile.owner]);
            }
			*/

			
			for (int i = 0; i < areActive.Count; i++)
            {
				if (areActive[i])
                {
					Main.spriteBatch.Draw(BulletOpen, Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(15f * i, -40) - new Vector2(0, 10 - Main.player[Projectile.owner].gfxOffY), new Rectangle(0, 0, BulletOpen.Width, BulletOpen.Height), Color.White, Projectile.rotation, BulletOpen.Size() / 2, 1 * Projectile.scale, SpriteEffects.None, 0f);
				}
				else
                {
					Main.spriteBatch.Draw(BulletUsed, Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(15f * i, -40) - new Vector2(0, 10 - Main.player[Projectile.owner].gfxOffY), new Rectangle(0, 0, BulletUsed.Width, BulletUsed.Height), Color.White, Projectile.rotation, BulletUsed.Size() / 2, 1 * Projectile.scale, SpriteEffects.None, 0f);
				}
			}
			

			/*
			for (float j = (-1 * halfCurrentBullets) + 0.5f; j < halfCurrentBullets; j++)
			{
				Main.spriteBatch.Draw(BulletTex, Projectile.Center - Main.screenPosition + new Vector2(10f * j, -40), new Rectangle(0, 0, BulletTex.Width, BulletTex.Height), Color.White, Projectile.rotation, BulletTex.Size() / 2, 1 * Projectile.scale, SpriteEffects.None, 0f);

			}
			*/

			

			return false;


			/*
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, TextureAssets.Projectile[Projectile.type].Value.Height * 0.5f);

			Texture2D texture2 = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
			Main.spriteBatch.Draw(texture2, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle(0, 0, texture2.Width, texture2.Height), Color.CornflowerBlue * 0.8f, Projectile.rotation, texture2.Size() / 2, 2f * Projectile.scale, SpriteEffects.None, 0f);



			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
			*/
		}
	}

	//This class is for storing the placement of drawn bullets of the UI
	public class Bullet
	{
		//General variables 
		public Vector2 velocity;
		public Vector2 center;
		public float rotation;
		public Color color;
		public float scale;
		public int timer;

		//Whether an ammo slot has been consumed or not
		public bool usedUp;
		public Bullet(Vector2 pos, Vector2 vel)
		{
			center = pos;
			velocity = vel;
			scale = 1f;
			rotation = 0f;

			usedUp = false;
		}

		public void Update(Vector2 goal)
		{
			center = goal;
			timer++;
		}


		public void Draw(SpriteBatch sb, Texture2D tex, Player player)
		{
			if (!usedUp)
				sb.Draw(tex, center - Main.screenPosition - new Vector2(0, player.gfxOffY), null, Color.White, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);
		}
	}
}