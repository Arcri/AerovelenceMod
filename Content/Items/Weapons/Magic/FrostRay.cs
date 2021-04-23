using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class FrostRay : ModItem
	{
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frost Ray");
			Item.staff[item.type] = true;
		}
		public override void SetDefaults()
		{
			item.UseSound = SoundID.Item43;
			item.damage = 24;
			item.magic = true;
			item.mana = 6;

			item.width = item.height = 44;
			
			item.useTime = item.useAnimation = 24;
			
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<ShadowbeamStaffCloneProjectile>();
			item.shootSpeed = 6f;
		}
       
        public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ModContent.ItemType<FrostShard>(), 8);
			modRecipe.AddIngredient(ItemID.IceBlock, 30);
			modRecipe.AddIngredient(ItemID.HellstoneBar, 8);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.SetResult(this);
			modRecipe.AddRecipe();
		}
	}

	
	class ShadowbeamStaffCloneProjectile : ModProjectile
	{
		int bounces;
		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.extraUpdates = 100;
			projectile.timeLeft = 300;
			projectile.penetrate = 300;
			bounces = 1;

		}
		public override string Texture { get { return "Terraria/Projectile_" + ProjectileID.ShadowBeamFriendly; } }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X * 0.75f;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y * 0.75f;
			}
			return false;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.Frostburn, 120);

			projectile.velocity.X = -projectile.oldVelocity.X * 0.75f;
			if (projectile.oldVelocity.Y > 0)
			{
				projectile.velocity.Y = -projectile.oldVelocity.Y * 0.5f;
			}
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
					int dust = Dust.NewDust(projectilePosition, 1, 1, 15, 0f, 0f, 0, default(Color), 1f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].position = projectilePosition;
					Main.dust[dust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
					Main.dust[dust].velocity *= 0.2f;
				}
			}
		}
	}
}