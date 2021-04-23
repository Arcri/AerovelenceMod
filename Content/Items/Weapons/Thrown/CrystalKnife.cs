using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class CrystalKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Knife");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.crit = 8;
            item.damage = 12;
            item.melee = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<CrystalKnifeProjectile>();
            item.shootSpeed = 16f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FallenStar, 10);
            recipe.AddIngredient(ItemID.Diamond, 3);
            recipe.AddIngredient(ItemID.IronBar, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class CrystalKnifeProjectile : ModProjectile
    {        
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 180;
            projectile.aiStyle = 1;
        }

        private bool rotChanged = false;
        public override void AI()
        {
            if (!rotChanged)
            {
                projectile.rotation = projectile.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2;
                rotChanged = true;
            }
            projectile.velocity.X *= 0.982f;
            projectile.velocity.Y += 0.14f;

            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 160, 0, 0);

        }

        
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-10, 11) * .25f, Main.rand.Next(-10, -5) * .25f, ProjectileID.CrystalShard, 0, projectile.owner);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-5, 12) * .25f, Main.rand.Next(-6, 10) * .25f, ProjectileID.CrystalShard,  0, projectile.owner);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-2, 9) * .25f, Main.rand.Next(-3, 5) * .25f, ProjectileID.CrystalShard, 0, projectile.owner);
        }
            
        
    }
}