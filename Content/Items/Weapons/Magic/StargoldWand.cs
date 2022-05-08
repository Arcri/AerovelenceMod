using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class StargoldWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Stargold Wand");
        }
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 17;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 5;
            Item.useAnimation = 15;
            Item.reuseDelay = 15;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 0, 40, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SearingShard>();
            Item.shootSpeed = 12f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.Sandstone, 35)
                .AddRecipeGroup("Wood", 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
  /*  public class StargoldWand : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 20;
        }
        public override void AI()
        {
            Dust dust;


            dust = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.AmberBolt);
            dust.scale = 0.45f;
            dust.noGravity = true;

            projectile.rotation += 0.55f;
        }
        public override void Kill(int timeLeft)
        {

            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            SoundEngine.PlaySound(SoundID.Item73, projectile.position);


            if (Main.myPlayer == projectile.owner)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<SandShard1>(), (int)(projectile.damage * 0.5f), 0, projectile.owner);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<SandShard1>(), (int)(projectile.damage * 0.5f), 0, projectile.owner);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<SandShard1>(), (int)(projectile.damage * 0.5f), 0, projectile.owner);

            }
        }
    }
    public class SandShard1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Shard");
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
            projectile.alpha = 0;
        }
        public override void AI()
        {

            Dust dust;
            projectile.velocity.Y += 0.09f;


            dust = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.AmberBolt);
            dust.scale = 0.45f;
            dust.noGravity = true;
        }
        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, projectile.position);
        }
    }
}*/