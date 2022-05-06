using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class HailWarning : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Hail Warning");
            Tooltip.SetDefault("Casts an icicle that rains more icicles from the sky");
        }
        public override void SetDefaults()
        {
            Item.crit = 6;
            Item.damage = 11;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 34;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.UseSound = SoundID.Item101;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 1, 30, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("HailProjectile").Type;
            Item.shootSpeed = 40f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<FrostShard>(), 10)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }

    public class HailProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.aiStyle = -1;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }
        private int timer;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            timer++;
            if (timer % 5 == 0)
            {
                Vector2 offset = new Vector2(0, -500);
                Projectile.NewProjectile(Projectile.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<HailIcicle>(), 6, 1f, Main.myPlayer);
            }
        }
    }

    public class HailIcicle : ModProjectile
    {
        int i;
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.aiStyle = -1;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            i++;
            if (i > 100)
            {
                Projectile.tileCollide = true;
            }
            Projectile.velocity.Y += 3;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}