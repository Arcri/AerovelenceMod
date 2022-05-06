using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Item.UseSound = SoundID.Item1;
            Item.crit = 8;
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.width = 60;
            Item.height = 32;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<CrystalKnifeProjectile>();
            Item.shootSpeed = 16f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.FallenStar, 10)
                .AddIngredient(ItemID.Diamond, 3)
                .AddIngredient(ItemID.IronBar, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class CrystalKnifeProjectile : ModProjectile
    {        
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = 1;
        }

        private bool rotChanged = false;
        public override void AI()
        {
            if (!rotChanged)
            {
                Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2;
                rotChanged = true;
            }
            Projectile.velocity.X *= 0.982f;
            Projectile.velocity.Y += 0.14f;

            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 160, 0, 0);

        }

        
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig);
            Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-10, 11) * .25f, Main.rand.Next(-10, -5) * .25f, ProjectileID.CrystalShard, 0, Projectile.owner);
            Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-5, 12) * .25f, Main.rand.Next(-6, 10) * .25f, ProjectileID.CrystalShard,  0, Projectile.owner);
            Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-2, 9) * .25f, Main.rand.Next(-3, 5) * .25f, ProjectileID.CrystalShard, 0, Projectile.owner);
        }
            
        
    }
}