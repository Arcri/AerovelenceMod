using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class GraniteCannon : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Granite Cannon");
			Tooltip.SetDefault("Fires a large moving chunk of granite that explodes into shards");
		}
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item41;
            item.crit = 6;
            item.damage = 29;
            item.ranged = true;
            item.width = 58;
            item.height = 18;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 55, 40);
            item.rare = ItemRarityID.Green;
            item.autoReuse = false;
			item.shoot = AmmoID.Bullet;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 3.2f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			if (type == ProjectileID.Bullet)
			{
				type = ModContent.ProjectileType<GraniteChunk>();
			}
			return true;
		}

		public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.Granite, 45);
            modRecipe.AddRecipeGroup("IronBar", 5);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }

    }
}

namespace AerovelenceMod.Items.Weapons.Ranged
{
	public class GraniteChunk : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Granite Chunk");
		}
		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ranged = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 600;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.extraUpdates = 1;
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, 240, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
			int type = mod.ProjectileType("GraniteShard1");
			Vector2 velocity = new Vector2(projectile.velocity.X * -0.6f, projectile.velocity.Y * -0.6f).RotatedByRandom(MathHelper.ToRadians(40));
			Projectile.NewProjectile(projectile.Center, velocity, type, projectile.damage, 5f, projectile.owner);
			int type2 = mod.ProjectileType("GraniteShard2");
			Vector2 velocity2 = new Vector2(projectile.velocity.X * -0.6f, projectile.velocity.Y * -0.6f).RotatedByRandom(MathHelper.ToRadians(50));
			Projectile.NewProjectile(projectile.Center, velocity2, type2, projectile.damage, 5f, projectile.owner);
			int type3 = mod.ProjectileType("GraniteShard3");
			Vector2 velocity3 = new Vector2(projectile.velocity.X * -0.6f, projectile.velocity.Y * -0.6f).RotatedByRandom(MathHelper.ToRadians(-50));
			Projectile.NewProjectile(projectile.Center, velocity3, type3, projectile.damage, 5f, projectile.owner);
		}
        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, 240, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
			int type = mod.ProjectileType("GraniteShard1");
			Vector2 velocity = new Vector2(projectile.velocity.X * -0.6f, projectile.velocity.Y * -0.6f).RotatedByRandom(MathHelper.ToRadians(40));
			Projectile.NewProjectile(projectile.Center, velocity, type, projectile.damage, 5f, projectile.owner);
			int type2 = mod.ProjectileType("GraniteShard2");
			Vector2 velocity2 = new Vector2(projectile.velocity.X * -0.6f, projectile.velocity.Y * -0.6f).RotatedByRandom(MathHelper.ToRadians(50));
			Projectile.NewProjectile(projectile.Center, velocity2, type2, projectile.damage, 5f, projectile.owner);
			int type3 = mod.ProjectileType("GraniteShard3");
			Vector2 velocity3 = new Vector2(projectile.velocity.X * -0.6f, projectile.velocity.Y * -0.6f).RotatedByRandom(MathHelper.ToRadians(-50));
			Projectile.NewProjectile(projectile.Center, velocity3, type3, projectile.damage, 5f, projectile.owner);
			return true;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
        public override void AI()
        {
			projectile.rotation += 100;
        }
        public override void Kill(int timeLeft)
		{
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}
	}
}

namespace AerovelenceMod.Items.Weapons.Ranged
{
	public class GraniteShard1 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Granite Shard");
		}
		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ranged = true;
			projectile.penetrate = 5;
			projectile.timeLeft = 600;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.extraUpdates = 1;
		}
        public override void AI()
        {
			projectile.velocity.Y += 0.2f;
		}
        public override void Kill(int timeLeft)
		{
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}
	}
}
namespace AerovelenceMod.Items.Weapons.Ranged
{
	public class GraniteShard2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Granite Shard");
		}
		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ranged = true;
			projectile.penetrate = 5;
			projectile.timeLeft = 600;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.extraUpdates = 1;
		}
		public override void AI()
		{
			projectile.velocity.Y += 0.2f;
		}
		public override void Kill(int timeLeft)
		{
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}
	}
}
namespace AerovelenceMod.Items.Weapons.Ranged
{
	public class GraniteShard3 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Granite Shard");
		}
		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ranged = true;
			projectile.penetrate = 5;
			projectile.timeLeft = 600;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.extraUpdates = 1;
		}
		public override void AI()
		{
			projectile.velocity.Y += 0.2f;
		}
		public override void Kill(int timeLeft)
		{
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}
	}
}