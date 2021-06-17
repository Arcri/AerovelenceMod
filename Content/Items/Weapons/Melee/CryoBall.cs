using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class CryoBall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryo Ball");
            Tooltip.SetDefault("Fires out snowflakes when near an enemy");
        }
        public override void SetDefaults()
        {
            item.channel = true;
            item.crit = 4;
            item.damage = 29;
            item.melee = true;
            item.width = 34;
            item.height = 40;
            item.useTime = 24;
            item.useAnimation = 24;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 5;
            item.value = Item.sellPrice(0, 3, 75, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("CryoBallProjectile");
            item.shootSpeed = 16f;
        }
    }

    public class CryoBallProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 13;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 245f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 16f;

            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;

            projectile.width = projectile.height = 16;

            projectile.aiStyle = 99;

            projectile.friendly = true;
            projectile.penetrate = -1;
        }

        int OnHit = 0;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHit++;
            if (OnHit == 9)
            {
                Main.PlaySound(SoundID.Item67, (int)projectile.Center.X, (int)projectile.Center.Y);

                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<CryoBallProj2>(), damage, 0, projectile.owner);
                OnHit = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Color color = Color.Lerp(Color.Blue, Color.Purple, 0.5f + (float)Math.Sin(MathHelper.ToRadians(projectile.frame)) / 2f) * 0.5f;
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + projectile.Size / 2f - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle), color, projectile.oldRot[i], rectangle.Size() / 2f, 1f, projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }

            return true;
        }

        public override void AI()
        {
            if (OnHit >= 3)
            {
                for (int j = 0; j < 10; j++)
                {
                    float t = (float)Main.time * 0.1f;
                    float x = projectile.position.X - projectile.velocity.X / 10f * (float)j;
                    float y = projectile.position.Y - projectile.velocity.Y / 10f * (float)j;
                    Dust dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 20, 0, 0, 0, Color.LightBlue, 0.9f);
                    dust.position.X = x;
                    dust.position.Y = y;
                    dust.velocity *= 0f;
                    dust.noGravity = true;
                    dust.scale = 0.9f;
                    dust.position = projectile.Center + new Vector2((float)Math.Sin(2 * t) / 2f, -(float)Math.Cos(3 * t) / 3f) * 100f;
                }
            }
            if (OnHit >= 6)
            {
                for (int j = 0; j < 10; j++)
                {
                    float t = (float)Main.time * 0.1f;
                    float x = projectile.position.X - projectile.velocity.X / 10f * (float)j;
                    float y = projectile.position.Y - projectile.velocity.Y / 10f * (float)j;
                    Dust dust2 = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 20, 0, 0, 0, Color.LightBlue, 0.9f);
                    dust2.position.X = x;
                    dust2.position.Y = y;
                    dust2.velocity *= 0f;
                    dust2.noGravity = true;
                    dust2.scale = 0.9f;
                    dust2.position = projectile.Center + new Vector2((float)Math.Sin(4 * t) / 2f, -(float)Math.Cos(2 * t) / 3f) * 100f;
                }
            }
        }

    }
    public class CryoBallProj2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 200;

            projectile.aiStyle = -1;
            projectile.friendly = projectile.melee = projectile.ignoreWater = true;

            projectile.penetrate = -1;
            projectile.timeLeft = 60;

            projectile.tileCollide = false;
            projectile.extraUpdates = 1;

            projectile.alpha = 255;
        }
        public override void AI()
        {
            for (int i = 0; i < 16; ++i)
            {
                float randomDust = Main.rand.NextFloat(-4, 4);
                float randomDust2 = Main.rand.NextFloat(4, -4);
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 20, randomDust, randomDust2);
            }

        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }
    }
}