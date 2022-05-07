using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;

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
            Item.UseSound = SoundID.Item92;
            Item.crit = 6;
            Item.damage = 29;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 58;
            Item.height = 18;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 55, 40);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = AmmoID.Bullet;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 3.2f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            CreateRecipe(1)
                .AddIngredient(ItemID.Granite, 45)
                .AddIngredient(ItemID.IronBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
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
        

		public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }

        int Timer = 0;
        public override void AI()
        {
            Timer++;
            if (Timer <= 12)
            {
                if (++Projectile.localAI[1] > 10)
                {
                    float amountOfDust = 16f;
                    for (int i = 0; i < amountOfDust; ++i)
                    {
                        Vector2 spinningpoint5 = -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / amountOfDust)) * new Vector2(1f, 4f);
                        spinningpoint5 = spinningpoint5.RotatedBy(Projectile.velocity.ToRotation());

                        Dust dust = Dust.NewDustPerfect(Projectile.Center + spinningpoint5, 136, spinningpoint5, 0, Color.Blue, 1.3f);
                        dust.noGravity = true;
                    }

                    Projectile.localAI[1] = 0;
                }
            }
        }
        public override void Kill(int timeLeft)
        {

            for (double i = 0; i < 6.28; i += 0.1)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 226, new Vector2((float)Math.Sin(i) * 1.3f, (float)Math.Cos(i)) * 2.4f);
                dust.noGravity = true;
            }

            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item93, Projectile.position);


            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<GraniteShard1>(), (int)(Projectile.damage * 0.5f), 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<GraniteShard1>(), (int)(Projectile.damage * 0.5f), 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center.X, Projectile.Center.Y - 16f, Main.rand.Next(-15, 15) * .25f, Main.rand.Next(-16, -7) * .25f, ModContent.ProjectileType<GraniteShard1>(), (int)(Projectile.damage * 0.5f), 0, Projectile.owner);

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
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
        }
        public override void AI()
        {

            Dust dust;
            Projectile.velocity.Y += 0.09f;


            dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Electric);
            dust.scale = 0.45f;
            dust.noGravity = true;
        }
        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    } 
}