using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class StormEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Edge");
        }
        public override void SetDefaults()
        {	
            Item.crit = 4;
            Item.damage = 52;
            Item.DamageType = DamageClass.Melee;
            Item.width = 38;
            Item.height = 38;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 2, 45, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StormEdgeProjectile>();
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = 13f;
        }


        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<BurnshockBar>(), 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class StormEdgeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Edge");
        }

        int i;

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.aiStyle = 3;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            // projectile.magic = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            i++;
            if (i % 2 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, 36, 30, 15, -0.2631578f, -2.631579f, 0, new Color(255, 255, 255));
            }
            Projectile.rotation += 0.1f;
            Vector2 origin = new Vector2(Projectile.Center.X, Projectile.Center.Y);
            float radius = 48;
            int numLocations = 30;
            if (i % 2 == 0)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Dust dust = Dust.NewDustPerfect(position, 36);
                dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
                dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Thrown/StormEdgeProjectile_GlowMask");
            Main.EntitySpriteDraw(
                texture,
                new Vector2
                (
                    Projectile.Center.Y - Main.screenPosition.X,
                    Projectile.Center.X - Main.screenPosition.Y
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                Projectile.rotation,
                texture.Size(),
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }
    }
}