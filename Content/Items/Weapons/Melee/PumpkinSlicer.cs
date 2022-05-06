using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Item.crit = 4;
            Item.damage = 14;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Pumpkin>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 3.85f;
            Item.autoReuse = false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.Pumpkin, 25)
                .AddTile(TileID.WorkBenches)
                .Register();
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
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        int Timer = 0;
        public override void AI()
        {
            Projectile.rotation += 200;
        }
        public override void Kill(int timeLeft)
        {
            Dust dust = Dust.NewDustPerfect(Projectile.Center, 189, Projectile.velocity);
            dust.noGravity = true;
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
        }
    }
}