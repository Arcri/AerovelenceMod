using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Ores.PreHM.Frost
{
    public class FrostRay : ModItem
    {
		public int i;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frost Ray");
			Item.staff[item.type] = true;
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item43;
			item.crit = 18;
            item.damage = 24;
            item.magic = true;
			item.mana = 6;
            item.width = 60;
            item.height = 32;
            item.useTime = 24;
			item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("ShadowbeamStaffCloneProjectile");
            item.shootSpeed = 16f;
        }

		public override void AddRecipes()  //How to craft this gun
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ModContent.ItemType<FrostShard>(), 8);
			modRecipe.AddIngredient(ItemID.IceBlock, 30);
			modRecipe.AddIngredient(ItemID.HellstoneBar, 8);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();
		}
	}
		class ShadowbeamStaffCloneProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;
			// NO! projectile.aiStyle = 48;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.extraUpdates = 100;
			projectile.timeLeft = 300; // lowered from 300
			projectile.penetrate = 300;
		}

		// Note, this Texture is actually just a blank texture, FYI.
		public override string Texture { get { return "Terraria/Projectile_" + ProjectileID.ShadowBeamFriendly; } }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.position.X = projectile.position.X + projectile.velocity.X;
				projectile.velocity.X = -oldVelocity.X;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.position.Y = projectile.position.Y + projectile.velocity.Y;
				projectile.velocity.Y = -oldVelocity.Y;
			}
			return false; // return false because we are handling collision
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			projectile.damage = (int)(projectile.damage * 0.8);
		}

		public override void AI()
		{
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 9f)
			{
				for (int i = 0; i < 2; i++)
				{
					Vector2 projectilePosition = projectile.position;
					projectilePosition -= projectile.velocity * ((float)i * 0.25f);
					projectile.alpha = 255;
					// Important, changed 173 to 172!
					int dust = Dust.NewDust(projectilePosition, 1, 1, 172, 0f, 0f, 0, default(Color), 1f);
					Main.dust[dust].noGravity = false;
					Main.dust[dust].position = projectilePosition;
					Main.dust[dust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
					Main.dust[dust].velocity *= 0.2f;
				}
			}
		}
	}
}