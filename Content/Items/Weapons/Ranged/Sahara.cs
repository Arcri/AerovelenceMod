using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
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
            Item.UseSound = SoundID.Item5;
            Item.crit = 4;
            Item.damage = 17;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = AmmoID.Arrow;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 8.5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<HugeAntlionMandible>(), 3)
                .AddIngredient(ItemID.Sandstone, 30)
                .AddIngredient(ItemID.Cactus, 7)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<SaharaProj>();
        }
    }
    public class SaharaProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 7;
            Projectile.timeLeft = 200;
            Projectile.alpha = 100;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 vector = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 position = Projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.spriteBatch.Draw((Texture2D)TextureAssets.Projectile[Projectile.type], position, null, color, Projectile.rotation, vector, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        private int shootTimer;
        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation += 100;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 63, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.White, 1);
            Main.dust[dust].velocity /= 1.2f;
            Main.dust[dust].noGravity = true;
            shootTimer++;
            if (shootTimer >= Main.rand.Next(20, 21))
            {
                float speed = 2f;
                int type = ProjectileID.MolotovFire;
                Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, type, Projectile.damage, 5f, Projectile.owner);
                shootTimer = 0;
            }
            if(Math.Abs(Projectile.velocity.X) < 0.02f)
            {
                Projectile.Kill();
            }
        }
    
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; ++i)
            {
                float speed = 2f;
                int type = ProjectileID.MolotovFire;
                Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, velocity, type, Projectile.damage, 5f, Projectile.owner);
            }
        }
    }
}