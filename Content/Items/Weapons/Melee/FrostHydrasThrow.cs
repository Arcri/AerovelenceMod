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
            item.channel = true;		
            item.crit = 4;
            item.damage = 18;
            item.melee = true;
            item.width = 36;
            item.height = 48;
            item.useTime = 24;
            item.useAnimation = 24;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 8;
            item.value = Item.sellPrice(0, 8, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("FrostHydrasThrowProjectile");
            item.shootSpeed = 2f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 15);
            recipe.AddRecipeGroup("IronBar", 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class FrostHydrasThrowProjectile : ModProjectile
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 30;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 540f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 20f;
        }
        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.scale = 1f;
            projectile.tileCollide = true;
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
                projectile.velocity = Collision.TileCollision(projectile.position, projectile.velocity, projectile.width, projectile.height, true, true);
                projectile.position += projectile.velocity * 0.25f;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 outwards = new Vector2(0, 1 * (i * 2 - 1)).RotatedBy(MathHelper.ToRadians(counter * 1.5f));
                    Vector2 spawnAt = projectile.Center;
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
                    if(Main.myPlayer == projectile.owner)
                    {
                        Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<IcyShard>(), (int)(projectile.damage * 0.75f), projectile.knockBack, Main.myPlayer, projectile.identity);
                    }
                }
                timer = Main.rand.Next(30);
            }
        }
    }
}