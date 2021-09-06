using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.damage = 12;
            item.ranged = true;
            item.width = 58;
            item.height = 18;
            item.useTime = item.useAnimation = 35;
           
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.holdStyle = ItemHoldStyleID.HarpHoldingOut;
            item.UseSound = SoundID.Item110;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 55, 40);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MarbleBullet>();
            item.shootSpeed = 16;
        }

        int itemShoot = 0; 
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 25f;
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
            
            return true;
        
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.MarbleBlock, 20);
            modRecipe.AddIngredient(ItemID.GoldBar, 5);
            modRecipe.AddIngredient(ItemID.Musket, 1);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this);
            modRecipe.AddRecipe();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }

        public override void UseStyle(Player player)
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
        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marble Bullet");
        }
        public override void SetDefaults()
        {

            projectile.width = projectile.height = 8;
            
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.light = 0.5f;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            int dust1 = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 159);
            int dust2 = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 159);
            Main.dust[dust1].noGravity = true;
            Main.dust[dust2].noGravity = true;
            Main.dust[dust1].velocity = Vector2.Zero;
            Main.dust[dust2].velocity = Vector2.Zero;
            Main.dust[dust2].scale = 0.9f;
            Main.dust[dust1].scale = 0.9f;

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.Kill();
            return true;
        }
        
        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item10, projectile.position);

            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 159);
        }
    }

    
}