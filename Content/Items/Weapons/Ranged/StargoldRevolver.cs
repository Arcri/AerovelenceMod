/*using AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class StargoldRevolver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stargold Revolver");
            Tooltip.SetDefault("Each consecutive shot on an enemy creates stars around them. \nShooting an enemy with stars around them more than 5 times will cause the stars to move inwards!");
        }
        public override void SetDefaults()
        {
            item.damage = 60;
            item.ranged = true;
            item.width = 62;
            item.height = 32;
            item.useTime = 6;
            item.useAnimation = 6;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0.2f;
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 13f;
            item.useAmmo = AmmoID.Bullet;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.FallenStar, 10);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this);
            modRecipe.AddRecipe();
        }
        int timesHit;
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Main.NewText(timesHit);
            timesHit += 1;
            if(timesHit == 10)
            {
                Vector2 offset = new Vector2(0, -100);
                Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<PhanticSoul>(), 6, 1f, Terraria.Main.myPlayer);
                timesHit = 0;
            }
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, -2);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, velocity.Y).RotatedByRandom(MathHelper.ToRadians(3));
            speedX = perturbedSpeed.X;
            velocity.Y = perturbedSpeed.Y;
            Vector2 muzzleOffset = new Vector2(-7, -8);
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            return true;
        }
    }
    public class StargoldStar : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 20;
        }
        public override void AI()
        {
            Dust dust;


            dust = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.AmberBolt);
            dust.scale = 0.45f;
            dust.noGravity = true;

            projectile.rotation += 0.55f;
        }
        public override void Kill(int timeLeft)
        {

            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item73, projectile.position);
            
        }
    }
}*/