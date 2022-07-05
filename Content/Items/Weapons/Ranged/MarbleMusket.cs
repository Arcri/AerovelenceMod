using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class MarbleMusket : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marble Musket");
            Tooltip.SetDefault("Every third shot makes the weapon deal 3x the damage");
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 58;
            Item.height = 18;
            Item.useTime = Item.useAnimation = 35;
           
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.holdStyle = ItemHoldStyleID.HoldHeavy;
            Item.UseSound = SoundID.Item110;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 55, 40);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MarbleBullet>();
            Item.shootSpeed = 16;
        }

        int itemShoot = 0; 
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            
            itemShoot++;
            if (itemShoot >= 3)
            {
                damage *= 3;
                itemShoot = 0;
            }
        
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.MarbleBlock, 20)
                .AddIngredient(ItemID.GoldBar, 5)
                .AddIngredient(ItemID.Musket, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
        Vector2 targetPosition = Main.MouseWorld - Main.LocalPlayer.Center;
            int direction = Math.Sign(targetPosition.X);

            player.ChangeDir(direction);

            player.direction = direction;
            player.itemRotation = (targetPosition * direction).ToRotation();

        }
    }
    public class MarbleBullet : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.None;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marble Bullet");
        }
        public override void SetDefaults()
        {

            Projectile.width = Projectile.height = 8;
            
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 159);
            int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 159);
            Main.dust[dust1].noGravity = true;
            Main.dust[dust2].noGravity = true;
            Main.dust[dust1].velocity = Vector2.Zero;
            Main.dust[dust2].velocity = Vector2.Zero;
            Main.dust[dust2].scale = 0.9f;
            Main.dust[dust1].scale = 0.9f;

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return true;
        }
        
        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 159);
        }
    }

    
}