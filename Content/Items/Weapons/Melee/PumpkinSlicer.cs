using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class PumpkinSlicer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin Slicer");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 14;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useTurn = true;
            item.UseSound = SoundID.Item1;
            item.shoot = ModContent.ProjectileType<Pumpkin>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 20, 0);
            item.rare = ItemRarityID.Blue;
            item.shootSpeed = 3.85f;
            item.autoReuse = false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Pumpkin, 25);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
    public class Pumpkin : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin");
        }
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;
        }

        int Timer = 0;
        public override void AI()
        {
            projectile.rotation += 200;
        }
        public override void Kill(int timeLeft)
        {
            Dust dust = Dust.NewDustPerfect(projectile.Center, 189, projectile.velocity);
            dust.noGravity = true;
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item73, projectile.position);
        }
    }
}