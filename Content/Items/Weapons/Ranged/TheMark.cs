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
				Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_None(), Main.npc[markedNPCs[i]].Center, Vector2.Zero, ProjectileID.Grenade, damage, 0, Player.whoAmI);
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
			Item.damage = 54;
			//item.damage affects explosion damage only

			Item.width = 30; //Sprite width
			Item.height = 30; //Sprite height
			Item.DamageType = DamageClass.Ranged; //Ranged damage
			Item.noMelee = true; //No melee hitbox
			Item.knockBack = 3f;
			Item.rare = 4; //Rarity value
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = 5;
			Item.useAmmo = AmmoID.Bullet;
			Item.shoot = ModContent.ProjectileType<MarkShot>();
			Item.shootSpeed = 9f;
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ItemID.IllegalGunParts)
				.AddIngredient(ItemID.Minishark)
				.AddIngredient(ItemID.Bomb, 10)
				.AddTile(TileID.Hellforge)
				.Register();
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
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 600;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.extraUpdates = 30;
		}
		public override void AI()
		{
			if (Main.rand.NextFloat() < .6f)
			{
				Dust d = Dust.NewDustDirect(Projectile.position, 4, 4, 228); //Need to update to DustID
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
			MarkPlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<MarkPlayer>();
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
				modPlayer.ExplodeNPCs(Projectile.damage);
			}
		}
	}
}