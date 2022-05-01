using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class MutilatingEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutilating Edge");
        }
        public override void SetDefaults()
        {
            item.useTurn = true;
            item.crit = 20;
            item.damage = 17;
            item.melee = true;
            item.width = 28;
            item.height = 36;
            item.useTime = 10;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;
            item.shoot = ModContent.ProjectileType<MutilatingWave>();
            item.shootSpeed = 5f;
            item.value = Item.sellPrice(0, 0, 65, 20);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("IronBar", 10);
            recipe.AddIngredient(ItemID.Vertebrae, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (target.type == NPCID.Crimera)
            {
                damage *= 3;
            }
            {
                if (target.type == -22)
                {
                    damage *= 3;
                }
                {
                    if (target.type == -23)
                    {
                        damage *= 3;
                    }
                }
            }
        }
    }
    public class MutilatingWave : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutilating Wave");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.aiStyle = -1;
            projectile.scale = 0.8f;
            projectile.timeLeft = 500;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            Texture2D texture2D = mod.GetTexture("Assets/WaveGlow");
            Vector2 origin = new Vector2(texture2D.Width / 2, texture2D.Height / 2);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                float scale = projectile.scale * (projectile.oldPos.Length - k) / projectile.oldPos.Length * 1.0f;
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + Main.projectileTexture[projectile.type].Size() / 3f;
                Color color = projectile.GetAlpha(fetchRainbow()) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                for (int i = 0; i < 6; i++)
                {
                    if (i == 0)
                        spriteBatch.Draw(texture2D, drawPos, null, color, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture2D, drawPos + new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), null, Color.White.MultiplyRGBA(color * 0.5f), projectile.rotation, origin, scale * 0.6f, SpriteEffects.None, 0f);
                }
            }
            return false;
        }

        public Color fetchRainbow()
        {
            Color color = new Color(255, 0, 0);
            return color;
        }
        public override void AI()
        {
            projectile.rotation = projectile.AngleTo(projectile.Center + projectile.velocity);
            Lighting.AddLight(new Vector2(projectile.Center.X, projectile.Center.Y), fetchRainbow().R / 255f, fetchRainbow().G / 255f, fetchRainbow().B / 255f);
            if (Main.rand.NextBool(10))
            {
                int num2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 267);
                Dust dust = Main.dust[num2];
                Color color2 = new Color(110, 110, 110, 0).MultiplyRGBA(fetchRainbow());
                dust.color = color2;
                dust.noGravity = true;
                dust.fadeIn = 0.1f;
                dust.scale *= 2f;
                dust.alpha = 255 - (int)(255 * (projectile.timeLeft / 720f));
                dust.velocity *= 0.5f;
                dust.velocity += projectile.velocity * 0.4f;
            }
        }

    }
}