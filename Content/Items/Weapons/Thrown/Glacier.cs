using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
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
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 40;
            
            Item.useTime = Item.useAnimation = 24;
            
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<GlacierProjectile>();
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<FrostShard>(), 8)
                .AddRecipeGroup("IronBar", 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<GlacierProjectile>()] < 1;
    }

    public class GlacierProjectile : ModProjectile
    {
        int i;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glacier");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            
            Projectile.aiStyle = 3;

            Projectile.penetrate = -1;

            Projectile.DamageType =  // projectile.friendly = true /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;

            Projectile.DamageType = DamageClass.Melee;
            
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            var entitySource = Projectile.GetSource_FromAI();
            i++;
            if (i % 10 == 0)
            {
                Projectile.NewProjectile(entitySource, Projectile.Center.X + Projectile.velocity.X, Projectile.Center.Y + Projectile.velocity.Y, Projectile.velocity.X - 2f, Projectile.velocity.Y - 2, ModContent.ProjectileType<GlacierProjectile2>(), Projectile.damage, Projectile.knockBack * 0.85f, Projectile.owner, 0f, 0f);
            }
            int num622 = Dust.NewDust(new Vector2(Projectile.position.X - Projectile.velocity.X, Projectile.position.Y - Projectile.velocity.Y), Projectile.width, Projectile.height, 191, 0f, 0f, 100, default, 2f);
            Main.dust[num622].noGravity = true;
            Main.dust[num622].scale = 0.5f;
            Projectile.rotation += 0.1f;
        }

		public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.LightPink) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
    public class GlacierProjectile2 : ModProjectile
    {
       

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.hostile = false;
            Projectile.tileCollide = false;
            
            Projectile.penetrate = 3;
            Projectile.timeLeft = 200;
            Projectile.light = 0.5f;
             
            Projectile.extraUpdates = 1;
        }
        public override void AI()  
        {
            Projectile.velocity *= 0.90f;
            Projectile.alpha = 255;
            Projectile.scale *= 0.99f;
            if (Projectile.alpha <= 0.4)
            {
                Projectile.active = false;
            }
            if (Main.rand.NextFloat() < 0.5f)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 20, 0f, 0f, 0, default, 1.118421f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}