using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class Glacier : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glacier");
        }
        public override void SetDefaults()
        {	
            item.damage = 20;
            item.melee = true;
            item.width = item.height = 40;
            
            item.useTime = item.useAnimation = 24;
            
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<GlacierProjectile>();
            item.UseSound = SoundID.Item1;
            item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 8);
            recipe.AddRecipeGroup(ItemID.IronBar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<GlacierProjectile>()] < 1;
    }

    public class GlacierProjectile : ModProjectile
    {
        int i;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glacier");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 40;
            
            projectile.aiStyle = projectile.penetrate = 3;
           
            projectile.ranged =  projectile.friendly = true;

            projectile.magic = false;
            
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            i++;
            if (i % 10 == 0)
            {
                Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y, projectile.velocity.X - 2f, projectile.velocity.Y - 2, ModContent.ProjectileType<GlacierProjectile2>(), projectile.damage, projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
            }
            int num622 = Dust.NewDust(new Vector2(projectile.position.X - projectile.velocity.X, projectile.position.Y - projectile.velocity.Y), projectile.width, projectile.height, 191, 0f, 0f, 100, default, 2f);
            Main.dust[num622].noGravity = true;
            Main.dust[num622].scale = 0.5f;
            projectile.rotation += 0.1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(Color.LightPink) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
    public class GlacierProjectile2 : ModProjectile
    {
       

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 10;
            projectile.aiStyle = -1;
            projectile.friendly = projectile.melee = projectile.ignoreWater = true;
            projectile.hostile = projectile.tileCollide = false;
            
            projectile.penetrate = 3;
            projectile.timeLeft = 200;
            projectile.light = 0.5f;
             
            projectile.extraUpdates = 1;
        }
        public override void AI()  
        {
            projectile.velocity *= 0.90f;
            projectile.alpha = 255;
            projectile.scale *= 0.99f;
            if (projectile.alpha <= 0.4)
            {
                projectile.active = false;
            }
            if (Main.rand.NextFloat() < 0.5f)
            {
                int dustIndex = Dust.NewDust(projectile.position, projectile.width, projectile.height, 20, 0f, 0f, 0, default, 1.118421f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
            projectile.rotation = projectile.velocity.ToRotation();
        }
    }
}