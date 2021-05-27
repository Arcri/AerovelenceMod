using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class SearingScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
			Item.staff[item.type] = true;
            DisplayName.SetDefault("Searing Scepter");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 17;
            item.magic = true;
            item.mana = 8;
            item.width = 50;
            item.height = 50;
            item.useTime = 5;
            item.useAnimation = 15;
            item.reuseDelay = 15;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 0, 40, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SearingShard>();
            item.shootSpeed = 12f;
		}
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Sandstone, 35);
            recipe.AddRecipeGroup("Wood", 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
    public class SearingShard : ModProjectile
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
            Main.PlaySound(SoundID.Item73, projectile.position);


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
            Main.PlaySound(SoundID.Item10, projectile.position);
        }
    }
}