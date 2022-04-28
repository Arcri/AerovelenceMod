using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class MarkPlayer : ModPlayer
	{
		public int[] markedNPCs = { -1, -1, -1 };
		public int markShots = 0;
		///<summary>
		/// Returns true if all stored npcs are active and not dead.
		///</summary>
		public bool AllNPCsActive()
		{
			for (int i = 0; i < markedNPCs.Length; i++)
			{
				if (markedNPCs[i] == -1)
					return false;
				if (!Main.npc[markedNPCs[i]].active || Main.npc[markedNPCs[i]].life <= 0)
				{
					markedNPCs[i] = -1;
					return false;
				}
			}
			return true;
		}
		///<summary>
		/// Returns the next open slot in the markedNPCs array
		///</summary>
		public int GetNextSlot(int index)
		{
			for (int i = 0; i < markedNPCs.Length; i++)
				if (markedNPCs[i] == -1 || markedNPCs[i] == index)
					return i;
			return -1; //If they are all full, return -1
		}

		public void ExplodeNPCs(int damage)
		{
			for (int i = 0; i < markedNPCs.Length; i++)
			{
				Projectile p = Projectile.NewProjectileDirect(Main.npc[markedNPCs[i]].Center, Vector2.Zero, ProjectileID.Grenade, damage, 0, player.whoAmI);
				p.friendly = true;
				p.hostile = false;
				p.timeLeft = 3;
				p.netUpdate = true;
				markedNPCs[i] = -1;
			}
		}
	}
	public class TheMark : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Mark");
			Tooltip.SetDefault("Marks targets on hit\nEvery third enemy hit causes marks to explode, dealing damage");
		}
		public override void SetDefaults()
		{
			item.damage = 54;
			//item.damage affects explosion damage only

			item.width = 30; //Sprite width
			item.height = 30; //Sprite height
			item.ranged = true; //Ranged damage
			item.noMelee = true; //No melee hitbox
			item.knockBack = 3f;
			item.rare = 4; //Rarity value
			item.useTime = 23;
			item.useAnimation = 23;
			item.useStyle = 5;
			item.useAmmo = AmmoID.Bullet;
			item.shoot = ModContent.ProjectileType<MarkShot>();
			item.shootSpeed = 9f;
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			type = item.shoot;
			return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.IllegalGunParts);
			recipe.AddIngredient(ItemID.Minishark);
			recipe.AddIngredient(ItemID.Bomb, 10); //10 bombs
			recipe.AddTile(TileID.Hellforge);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	//This is a very generic beam projectile
	//If you want different AI, just change SetDefaults and AI
	//All of the logic for this is within ModifyHitNPC and OnHitNPC
	public class MarkShot : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_0"; //Comment this line out if projectile has a sprite.
		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;
			projectile.friendly = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 600;
			projectile.ranged = true;
			projectile.extraUpdates = 30;
		}
		public override void AI()
		{
			if (Main.rand.NextFloat() < .6f)
			{
				Dust d = Dust.NewDustDirect(projectile.position, 4, 4, 228); //Need to update to DustID
				d.velocity = Vector2.Zero;
				d.scale = .8f;
			}
		}
		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			crit = false;
			hitDirection = 0;
			knockback = 0;
			damage = 1; //Minimum damage while still doing collision.
						//I can't be bothered to do custom collision without Intellisense
		}
		public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit)
		{
			MarkPlayer modPlayer = Main.player[projectile.owner].GetModPlayer<MarkPlayer>();
			if (modPlayer.markShots < 3 || !modPlayer.AllNPCsActive())
			{
				//Main.NewText("1");
				int slot = modPlayer.GetNextSlot(target.whoAmI);
				if (modPlayer.markedNPCs[slot] != target.whoAmI)
				{
					//Main.NewText("2");
					modPlayer.markShots++;
					modPlayer.markedNPCs[slot] = target.whoAmI;
				}
				//Main.NewText("Added " + target.whoAmI + " at " + slot + ".");
			}
			if (modPlayer.AllNPCsActive())
			{
				//Main.NewText("3");
				//Main.NewText("Should be exploding.");
				modPlayer.ExplodeNPCs(projectile.damage);
			}
		}
	}
}