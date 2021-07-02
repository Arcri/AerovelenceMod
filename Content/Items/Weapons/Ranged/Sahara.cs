using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Sahara : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sahara");
            Tooltip.SetDefault("Fires a fire vortex that spews out embers");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item5;
            item.crit = 4;
            item.damage = 17;
            item.ranged = true;
            item.width = 30;
            item.height = 54;
            item.useTime = 60;
            item.useAnimation = 60;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = AmmoID.Arrow;
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 8.5f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<HugeAntlionMandible>(), 3);
            recipe.AddIngredient(ItemID.Sandstone, 30);
            recipe.AddIngredient(ItemID.Cactus, 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = ModContent.ProjectileType<SaharaProj>();
            return true;
        }
    }
    public class SaharaProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 7;
            projectile.timeLeft = 200;
            projectile.alpha = 100;
        }
        public override bool PreDraw(SpriteBatch sb, Color lightColor)
        {
            Vector2 vector = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Vector2 position = projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / projectile.oldPos.Length);
                sb.Draw(Main.projectileTexture[projectile.type], position, null, color, projectile.rotation, vector, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        private int shootTimer;
        public override void AI()
        {
            projectile.velocity *= 0.95f;
            projectile.rotation += 100;
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 63, projectile.velocity.X, projectile.velocity.Y, 0, Color.White, 1);
            Main.dust[dust].velocity /= 1.2f;
            Main.dust[dust].noGravity = true;
            shootTimer++;
            if (shootTimer >= Main.rand.Next(20, 21))
            {
                float speed = 2f;
                int type = ProjectileID.MolotovFire;
                Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
                Projectile.NewProjectile(projectile.Center, velocity, type, projectile.damage, 5f, projectile.owner);
                shootTimer = 0;
            }
            if(Math.Abs(projectile.velocity.X) < 0.02f)
            {
                projectile.Kill();
            }
        }
    
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; ++i)
            {
                float speed = 2f;
                int type = ProjectileID.MolotovFire;
                Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
                Projectile.NewProjectile(projectile.Center, velocity, type, projectile.damage, 5f, projectile.owner);
            }
        }
    }
}