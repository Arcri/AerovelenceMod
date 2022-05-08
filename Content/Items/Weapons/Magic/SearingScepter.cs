using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class SearingScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
			Item.staff[Item.type] = true;
            DisplayName.SetDefault("Searing Scepter");
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
                .AddIngredient(ModContent.ItemType<HugeAntlionMandible>(), 1)
                .AddIngredient(ItemID.Sandstone, 15)
                .AddIngredient(ItemID.Cactus, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class SearingShard : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
        }
        public override void AI()
        {
            Dust dust;


            dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.AmberBolt);
            dust.scale = 0.45f;
            dust.noGravity = true;

            Projectile.rotation += 0.55f;
        }
        public override void Kill(int timeLeft)
        {

            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item73, Projectile.position);


            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<SandShard1>(), (int)(Projectile.damage * 0.5f), 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<SandShard1>(), (int)(Projectile.damage * 0.5f), 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<SandShard1>(), (int)(Projectile.damage * 0.5f), 0, Projectile.owner);

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
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 0;
        }

        public override void AI()
        {

            Dust dust;
            Projectile.velocity.Y += 0.09f;


            dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.AmberBolt);
            dust.scale = 0.45f;
            dust.noGravity = true;
        }
        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}