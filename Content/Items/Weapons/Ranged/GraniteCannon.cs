using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class GraniteCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Cannon");
            Tooltip.SetDefault("Fires a large moving chunk of granite that explodes into shards");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item92;
            item.crit = 6;
            item.damage = 29;
            item.ranged = true;
            item.width = 58;
            item.height = 18;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 55, 40);
            item.rare = ItemRarityID.Green;
            item.autoReuse = false;
            item.shoot = AmmoID.Bullet;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 3.2f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            if (type == ProjectileID.Bullet)
            {
                type = ModContent.ProjectileType<GraniteChunk>();
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.Granite, 45);
            modRecipe.AddIngredient(ItemID.IronBar, 5);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this);
            modRecipe.AddRecipe();
        }

    }

    public class GraniteChunk : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Chunk");
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
        

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - k) / projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        int Timer = 0;
        public override void AI()
        {
            projectile.rotation += 100;


            Timer++;
            if (Timer <= 12)
            {
                if (++projectile.localAI[1] > 10)
                {
                    float amountOfDust = 16f;
                    for (int i = 0; i < amountOfDust; ++i)
                    {
                        Vector2 spinningpoint5 = -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / amountOfDust)) * new Vector2(1f, 4f);
                        spinningpoint5 = spinningpoint5.RotatedBy(projectile.velocity.ToRotation());

                        Dust dust = Dust.NewDustPerfect(projectile.Center + spinningpoint5, 136, spinningpoint5, 0, Color.Blue, 1.3f);
                        dust.noGravity = true;
                    }

                    projectile.localAI[1] = 0;
                }
            }
        }
        public override void Kill(int timeLeft)
        {

            for (double i = 0; i < 6.28; i += 0.1)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, 226, new Vector2((float)Math.Sin(i) * 1.3f, (float)Math.Cos(i)) * 2.4f);
                dust.noGravity = true;
            }

            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item93, projectile.position);


            if (Main.myPlayer == projectile.owner)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<GraniteShard1>(), (int)(projectile.damage * 0.5f), 0, projectile.owner);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<GraniteShard1>(), (int)(projectile.damage * 0.5f), 0, projectile.owner);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<GraniteShard1>(), (int)(projectile.damage * 0.5f), 0, projectile.owner);

            }
        }
    }

    public class GraniteShard1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Shard");
        }
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
        }
        public override void AI()
        {

            Dust dust;
            projectile.velocity.Y += 0.09f;


            dust = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.Electric);
            dust.scale = 0.45f;
            dust.noGravity = true;
        }
        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item10, projectile.position);
        }
    }

    
}