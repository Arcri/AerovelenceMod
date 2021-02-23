using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class HydraulicBlaster : ModItem
    {
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-6, 0);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hydraulic Blaster");
			Tooltip.SetDefault("Shoots out Bursts of water that bounces of enemies and tiles"); 
        }
        public override void SetDefaults()
        {
			item.crit = 20;
            item.damage = 38;
            item.ranged = true;
            item.width = 66;
            item.height = 22;
            item.useTime = 24;
            item.useAnimation = 24;
			item.UseSound = SoundID.Item36;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8;
            item.value = Item.sellPrice(0, 15, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("HydraulicBlasterProj");
			item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 28f;
        }

        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.HallowedBar, 10);
            modRecipe.AddIngredient(ItemID.Starfish, 5);
            modRecipe.AddIngredient(ItemID.SoulofSight, 3);
            modRecipe.AddIngredient(ItemID.WaterBucket, 1);
            modRecipe.AddTile(TileID.Hellforge);
            modRecipe.SetResult(this);
            modRecipe.AddRecipe();
        }


        public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.033);
            float baseSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            double baseAngle = Math.Atan2(speedX, speedY);
            double randomAngle;
            for (int i = 0; i < num; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)Math.Sin(randomAngle), baseSpeed * (float)Math.Cos(randomAngle));
            }
            return posArray;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2[] speeds = randomSpread(speedX, speedY, 3, 3);
            for (int i = 0; i < 3; ++i)
            {
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, mod.ProjectileType("HydraulicBlasterProj"), damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
	public class HydraulicBlasterProj : ModProjectile
    {
		int i;
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 1;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
			projectile.timeLeft = 120;
			projectile.alpha = 100;
        }
        public override void AI()
        {
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			i++;
			if (i % 3 == 0)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}
			projectile.velocity.Y += 0.1f;
			projectile.velocity *= 0.98f;
        }
		public override bool OnTileCollide(Vector2 oldVelocity) 
		{
			Projectile.NewProjectile(projectile.position.X, projectile.position.Y, Main.rand.Next(-10, 10), Main.rand.Next(-10, 10), mod.ProjectileType("HydraulicBlasterProjSmall"), 10, 2, Main.player[0].whoAmI);
			return true;
		}		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Projectile.NewProjectile(projectile.position.X, projectile.position.Y, Main.rand.Next(-10, 10), Main.rand.Next(-10, 10), mod.ProjectileType("HydraulicBlasterProjSmall"), damage / 2, knockback, Main.player[0].whoAmI);
		}
    }
		public class HydraulicBlasterProjSmall : ModProjectile
    {
		int i;
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = false;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
			projectile.timeLeft = 60;
			projectile.alpha = 100;
        }
        public override void AI()
        {
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			i++;
			if (i % 3 == 0)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}
			projectile.velocity.Y += 0.1f;
			projectile.velocity *= 0.98f;
        }
    }
}