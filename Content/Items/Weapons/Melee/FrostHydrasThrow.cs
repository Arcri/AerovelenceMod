using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class FrostHydrasThrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Hydra's Throw");
            Tooltip.SetDefault("Summons a blizzard of ice shards that do 75% damage");
        }
        public override void SetDefaults()
        {
            Item.channel = true;		
            Item.crit = 4;
            Item.damage = 18;
            Item.DamageType = DamageClass.Melee;
            Item.width = 36;
            Item.height = 48;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = Mod.Find<ModProjectile>("FrostHydrasThrowProjectile").Type;
            Item.shootSpeed = 2f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<FrostShard>(), 15)
                .AddRecipeGroup("IronBar", 7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class FrostHydrasThrowProjectile : ModProjectile
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 30;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 540f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 20f;
        }
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1f;
            Projectile.tileCollide = true;
            timer = 800;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        int counter = 0;
        public override void AI()
        {
            for(int k = 0; k < 4; k++)
            {
                counter++;
                Projectile.velocity = Collision.TileCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, true, true);
                Projectile.position += Projectile.velocity * 0.25f;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 outwards = new Vector2(0, 1 * (i * 2 - 1)).RotatedBy(MathHelper.ToRadians(counter * 1.5f));
                    Vector2 spawnAt = Projectile.Center;
                    Dust dust = Dust.NewDustDirect(spawnAt - new Vector2(5), 0, 0, ModContent.DustType<WispDust>());
                    dust.velocity = outwards * 6f;
                    dust.noGravity = true;
                    dust.scale *= 0.1f;
                    dust.scale += 1f;
                }
            }
            timer++;
            int spawnRate = 100;
            if (timer >= spawnRate)
            {
                for(int i = 0; i < (timer > 300 ? 2 : 1); i++)
                {
                    if(Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IcyShard>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Main.myPlayer, Projectile.identity);
                    }
                }
                timer = Main.rand.Next(30);
            }
        }
    }
}