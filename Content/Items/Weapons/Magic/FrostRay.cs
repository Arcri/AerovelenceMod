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
			Item.staff[Item.type] = true;
		}
		public override void SetDefaults()
		{
			Item.UseSound = SoundID.Item43;
			Item.damage = 24;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 6;

			Item.width = Item.height = 44;
			
			Item.useTime = Item.useAnimation = 24;
			
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<ShadowbeamStaffCloneProjectile>();
			Item.shootSpeed = 6f;
		}
       
        public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<FrostShard>(), 8)
				.AddIngredient(ItemID.IceBlock, 30)
				.AddIngredient(ItemID.HellstoneBar, 8)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	
	class ShadowbeamStaffCloneProjectile : ModProjectile
	{
		int bounces;
		public override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.extraUpdates = 100;
			Projectile.timeLeft = 300;
			Projectile.penetrate = 300;
			bounces = 1;

		}
		public override string Texture { get { return "Terraria/Images/Projectile_" + ProjectileID.ShadowBeamFriendly; } }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X * 0.75f;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y * 0.75f;
			}
			return false;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.Frostburn, 120);

			Projectile.velocity.X = -Projectile.oldVelocity.X * 0.75f;
			if (Projectile.oldVelocity.Y > 0)
			{
				Projectile.velocity.Y = -Projectile.oldVelocity.Y * 0.5f;
			}
		}


        public override void AI()
		{
			Projectile.localAI[0] += 1f;
			if (Projectile.localAI[0] > 9f)
			{
				for (int i = 0; i < 2; i++)
				{
					Vector2 projectilePosition = Projectile.position;
					projectilePosition -= Projectile.velocity * ((float)i * 0.25f);
					Projectile.alpha = 255;
					int dust = Dust.NewDust(projectilePosition, 1, 1, 20, 0f, 0f, 0, default(Color), 1f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].position = projectilePosition;
					Main.dust[dust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
					Main.dust[dust].velocity *= 0.2f;
				}
			}
		}
	}
}